
#include "AUEffectBase.h"
#include <AudioToolbox/AudioUnitUtilities.h>
#include "ExtractorVersion.h"
#include "Extractor.h"

#include <aubio/aubio.h>
#include <math.h>
#include <list>

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#pragma mark ____ExtractorKernel


class ExtractorKernel : public AUKernelBase		// the actual filter DSP happens here
{
public:
	ExtractorKernel(AUEffectBase *inAudioUnit );
	virtual ~ExtractorKernel();
			
	// processes one channel of non-interleaved samples
	virtual void 		Process(	const Float32 	*inSourceP,
									Float32		 	*inDestP,
									UInt32 			inFramesToProcess,
									UInt32			inNumChannels,
									bool &			ioSilence);

	// resets the filter state
	virtual void		Reset();
			
private:
    
    fvec_t* mAubioInputBuffer;
    
    int mCurrentBufferSize;
    
    ExtractorImplementation* mExtractorImplentation;
    
};


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#pragma mark ____Extractor

class Extractor : public AUEffectBase
{
public:
	Extractor(AudioUnit component);

	virtual OSStatus			Version() { return kExtractorVersion; }

	virtual OSStatus			Initialize();

	virtual AUKernelBase *		NewKernel() { return new ExtractorKernel(this); }

	// for custom property
	virtual OSStatus			GetPropertyInfo(	AudioUnitPropertyID		inID,
													AudioUnitScope			inScope,
													AudioUnitElement		inElement,
													UInt32 &				outDataSize,
													Boolean	&				outWritable );

	virtual OSStatus			GetProperty(		AudioUnitPropertyID 	inID,
													AudioUnitScope 			inScope,
													AudioUnitElement 		inElement,
													void 					* outData );


	virtual OSStatus			GetParameterInfo(	AudioUnitScope			inScope,
													AudioUnitParameterID	inParameterID,
													AudioUnitParameterInfo	&outParameterInfo );
	
    // handle presets:
    virtual OSStatus			GetPresets(	CFArrayRef	*outData	)	const;    
    virtual OSStatus			NewFactoryPresetSet (	const AUPreset & inNewFactoryPreset	);

	// we'll report a 1ms tail.   A reverb effect would have a much more substantial tail on
	// the order of several seconds....
	//
	virtual	bool				SupportsTail () { return false; }
    virtual Float64				GetTailTime() {return 0.0;}

	// we have no latency
	//
	// A lookahead compressor or FFT-based processor should report the true latency in seconds
    virtual Float64				GetLatency() {return 0.0;}


protected:
};

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Standard DSP AudioUnit implementation

AUDIOCOMPONENT_ENTRY(AUBaseProcessFactory, Extractor)


const float kDefaultScale = 1.0;

const float kDefaultOffset = 0.0;


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#pragma mark ____Construction_Initialization


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Extractor::Extractor
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Extractor::Extractor(AudioUnit component) : AUEffectBase(component)
{

}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Extractor::Initialize
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
OSStatus Extractor::Initialize()
{
	OSStatus result = AUEffectBase::Initialize();
	
	if(result == noErr )
	{
		// in case the AU was un-initialized and parameters were changed, the view can now
		// be made aware it needs to update the frequency response curve
        PropertyChanged(kAudioUnitCustomProperty_ExtractorCurrentValue, kAudioUnitScope_Global, 0 );
        PropertyChanged(kAudioUnitCustomProperty_ExtractorCount, kAudioUnitScope_Global, 0 );
	}
	
	return result;
}


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#pragma mark ____Parameters

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Extractor::GetParameterInfo
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
OSStatus Extractor::GetParameterInfo(   AudioUnitScope          inScope,
                                        AudioUnitParameterID	inParameterID,
                                        AudioUnitParameterInfo	&outParameterInfo )
{
	OSStatus result = noErr;

	outParameterInfo.flags = kAudioUnitParameterFlag_IsWritable + kAudioUnitParameterFlag_IsReadable;
		
	if (inScope == kAudioUnitScope_Global) {
		
		switch(inParameterID)
		{
            // current design has no parameters
				
			default:
				result = kAudioUnitErr_InvalidParameter;
				break;
		}
	} else {
		result = kAudioUnitErr_InvalidParameter;
	}
	
	return result;
}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#pragma mark ____Properties

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Extractor::GetPropertyInfo
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
OSStatus Extractor::GetPropertyInfo (   AudioUnitPropertyID				inID,
                                        AudioUnitScope					inScope,
                                        AudioUnitElement				inElement,
                                        UInt32 &						outDataSize,
                                        Boolean &						outWritable)
{
	if (inScope == kAudioUnitScope_Global)
	{
		switch (inID)
		{
			case kAudioUnitProperty_CocoaUI:
				outWritable = false;
				outDataSize = sizeof (AudioUnitCocoaViewInfo);
				return noErr;

			case kAudioUnitCustomProperty_ExtractorModuleConfigurations:	// our custom property
				if(inScope != kAudioUnitScope_Global ) return kAudioUnitErr_InvalidScope;
				outDataSize = sizeof(CFArrayRef);
				outWritable = true;
				return noErr;
                
            case kAudioUnitCustomProperty_AnalysisBufferSize:
                if(inScope != kAudioUnitScope_Global ) return kAudioUnitErr_InvalidScope;
                outDataSize = sizeof(int);
                outWritable = false;
                return noErr;
		}
	}
	
	return AUEffectBase::GetPropertyInfo (inID, inScope, inElement, outDataSize, outWritable);
}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Extractor::GetProperty
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
OSStatus Extractor::GetProperty (	AudioUnitPropertyID 		inID,
                                    AudioUnitScope 				inScope,
                                    AudioUnitElement			inElement,
                                    void *						outData)
{
	if (inScope == kAudioUnitScope_Global)
	{
		switch (inID)
		{
			// This property allows the host application to find the UI associated with this
			// AudioUnit
			//
			case kAudioUnitProperty_CocoaUI:
			{
				// Look for a resource in the main bundle by name and type.
				CFBundleRef bundle = CFBundleGetBundleWithIdentifier( CFSTR("sowiz.audiounit.Extractor") );
				
				if (bundle == NULL) return fnfErr;
                
				CFURLRef bundleURL = CFBundleCopyResourceURL( bundle, 
                    CFSTR("CocoaExtractorView"),	// this is the name of the cocoa bundle as specified in the CocoaViewFactory.plist
                    CFSTR("bundle"),			// this is the extension of the cocoa bundle
                    NULL);
                
                if (bundleURL == NULL) return fnfErr;
                
				CFStringRef className = CFSTR("Extractor_ViewFactory");	// name of the main class that implements the AUCocoaUIBase protocol
				AudioUnitCocoaViewInfo cocoaInfo = { bundleURL, { className } };
				*((AudioUnitCocoaViewInfo *)outData) = cocoaInfo;
				
				return noErr;
			}

			// This is our custom property which reports the current frequency response curve
			//
			case kAudioUnitCustomProperty_ExtractorFrequencyResponse:
			{
				if(inScope != kAudioUnitScope_Global) 	return kAudioUnitErr_InvalidScope;

				// the kernels are only created if we are initialized
				// since we're using the kernels to get the curve info, let
				// the caller know we can't do it if we're un-initialized
				// the UI should check for the error and not draw the curve in this case
				if(!IsInitialized() ) return kAudioUnitErr_Uninitialized;

				FrequencyResponse *freqResponseTable = ((FrequencyResponse*)outData);

				// each of our filter kernel objects (one per channel) will have an identical frequency response
				// so we arbitrarilly use the first one...
				//
				ExtractorKernel *filterKernel = dynamic_cast<ExtractorKernel*>(mKernelList[0]);


				double cutoff = GetParameter(kExtractorParam_CutoffFrequency);
				double resonance = GetParameter(kExtractorParam_Resonance );

				float srate = GetSampleRate();
				
				cutoff = 2.0 * cutoff / srate;
				if(cutoff > 0.99) cutoff = 0.99;		// clip cutoff to highest allowed by sample rate...

				filterKernel->CalculateLopassParams(cutoff, resonance);
				
				for(int i = 0; i < kNumberOfResponseFrequencies; i++ )
				{
					double frequency = freqResponseTable[i].mFrequency;
					
					freqResponseTable[i].mMagnitude = filterKernel->GetFrequencyResponse(frequency);
				}

				return noErr;
			}
		}
	}
	
	// if we've gotten this far, handles the standard properties
	return AUEffectBase::GetProperty (inID, inScope, inElement, outData);
}


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#pragma mark ____Presets

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Extractor::GetPresets
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
OSStatus			Extractor::GetPresets (		CFArrayRef * 		outData) const
{
		// this is used to determine if presets are supported 
		// which in this unit they are so we implement this method!
	if (outData == NULL) return noErr;
	
	CFMutableArrayRef theArray = CFArrayCreateMutable (NULL, kNumberPresets, NULL);
	for (int i = 0; i < kNumberPresets; ++i) {
		CFArrayAppendValue (theArray, &kPresets[i]);
    }
    
	*outData = (CFArrayRef)theArray;	// client is responsible for releasing the array
	return noErr;
}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Extractor::NewFactoryPresetSet
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
OSStatus	Extractor::NewFactoryPresetSet (const AUPreset & inNewFactoryPreset)
{
	SInt32 chosenPreset = inNewFactoryPreset.presetNumber;
	
	for(int i = 0; i < kNumberPresets; ++i)
	{
		if(chosenPreset == kPresets[i].presetNumber)
		{
			// set whatever state you need to based on this preset's selection
			//
			// Here we use a switch statement, but it would also be possible to
			// use chosenPreset as an index into an array (if you publish the preset
			// numbers as indices in the GetPresets() method)
			//			
			switch(chosenPreset)
			{
				case kPreset_One:
					SetParameter(kExtractorParam_CutoffFrequency, 200.0 );
					SetParameter(kExtractorParam_Resonance, -5.0 );
					break;
				case kPreset_Two:
					SetParameter(kExtractorParam_CutoffFrequency, 1000.0 );
					SetParameter(kExtractorParam_Resonance, 10.0 );
					break;
			}
            
            SetAFactoryPresetAsCurrent (kPresets[i]);
			return noErr;
		}
	}
	
	return kAudioUnitErr_InvalidPropertyValue;
}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#pragma mark ____ExtractorKernel


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	ExtractorKernel::ExtractorKernel()
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
ExtractorKernel::ExtractorKernel(AUEffectBase *inAudioUnit ) : AUKernelBase(inAudioUnit)
{
	Reset();
}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	ExtractorKernel::~ExtractorKernel()
//
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
ExtractorKernel::~ExtractorKernel( )
{
}


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	ExtractorKernel::Reset()
//
//		It's very important to fully reset all filter state variables to their
//		initial settings here.  For delay/reverb effects, the delay buffers must
//		also be cleared here.
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
void ExtractorKernel::Reset()
{
	mX1 = 0.0;
	mX2 = 0.0;
	mY1 = 0.0;
	mY2 = 0.0;
	
	// forces filter coefficient calculation
	mLastCutoff = -1.0;
	mLastResonance = -1.0;
}





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	ExtractorKernel::Process(int inFramesToProcess)
//
//		We process one non-interleaved stream at a time
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void ExtractorKernel::Process(	const Float32 	*inSourceP,
							Float32 		*inDestP,
							UInt32 			inFramesToProcess,
							UInt32			inNumChannels,	// for version 2 AudioUnits inNumChannels is always 1
							bool &			ioSilence)
{
	double cutoff = GetParameter(kExtractorParam_CutoffFrequency);
    double resonance = GetParameter(kExtractorParam_Resonance );
    
	// do bounds checking on parameters
	//
    if(cutoff < kMinCutoffHz) cutoff = kMinCutoffHz;

	if(resonance < kMinResonance ) resonance = kMinResonance;
	if(resonance > kMaxResonance ) resonance = kMaxResonance;

	
	// convert to 0->1 normalized frequency
	float srate = GetSampleRate();
	
	cutoff = 2.0 * cutoff / srate;
	if(cutoff > 0.99) cutoff = 0.99;		// clip cutoff to highest allowed by sample rate...
	

	// only calculate the filter coefficients if the parameters have changed from last time
	if(cutoff != mLastCutoff || resonance != mLastResonance )
	{
		CalculateLopassParams(cutoff, resonance);
		
		mLastCutoff = cutoff;
		mLastResonance = resonance;		
	}
	

	const Float32 *sourceP = inSourceP;
	Float32 *destP = inDestP;
	int n = inFramesToProcess;
	
	// Apply the filter on the input and write to the output
	// This code isn't optimized and is written for clarity...
	//
	while(n--)
	{
		float input = *sourceP++;
		
		float output = mA0*input + mA1*mX1 + mA2*mX2 - mB1*mY1 - mB2*mY2;

		mX2 = mX1;
		mX1 = input;
		mY2 = mY1;
		mY1 = output;
		
		*destP++ = output;
	}
}
