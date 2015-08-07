
#include "AUEffectBase.h"
#include <AudioToolbox/AudioUnitUtilities.h>
#include "ExtractorVersion.h"
#include "Extractor.h"
#include "ExtractorModule.h"

#include <aubio/aubio.h>
#include <math.h>
#include <list>

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#pragma mark ____ExtractorKernel


class ExtractorKernel : public AUKernelBase		// the actual filter DSP happens here
{
    
public:
    
	ExtractorKernel(AUEffectBase *inAudioUnit);
	virtual ~ExtractorKernel();
			
	virtual void Process(   const Float32 	*inSourceP,
                            Float32		 	*inDestP,
                            UInt32 			inFramesToProcess,
                            UInt32			inNumChannels,
                            bool &			ioSilence);

	virtual void Reset();
    
    void SetAubioBufferSize(UInt32 size);
			
private:
    
    fvec_t* mAubioInputBuffer;
    
    int mCurrentBufferSize;
    
    std::list<ExtractorModule*> mExtractorModules;
    
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
    
    CAExtractorModuleConfiguration mDefaultConfiguration;
    
    std::list<CAExtractorModuleConfiguration> mConfigurations;
    
    
};

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	Standard DSP AudioUnit implementation

AUDIOCOMPONENT_ENTRY(AUBaseProcessFactory, Extractor)


const int kDefaultAnalysisBufferSize = 1024;
const int kDefaultExtractorCount = 0;

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
        PropertyChanged(kAudioUnitCustomProperty_AnalysisBufferSize, kAudioUnitScope_Global, 0 );
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

#pragma mark ____Properties

//------------------------------------------------------------------------------------------

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
				outDataSize = sizeof(std::list<CAExtractorModuleConfiguration>);
				outWritable = true;
				return noErr;
                
            case kAudioUnitCustomProperty_ExtractorModuleConfiguration:	// our custom property
                if(inScope != kAudioUnitScope_Global ) return kAudioUnitErr_InvalidScope;
                outDataSize = sizeof(CAExtractorModuleConfiguration);
                outWritable = true;
                return noErr;
                
            case kAudioUnitCustomProperty_AnalysisBufferSize:
                if(inScope != kAudioUnitScope_Global ) return kAudioUnitErr_InvalidScope;
                outDataSize = sizeof(int);
                outWritable = false;
                return noErr;
                
            case kAudioUnitCustomProperty_ExtractorCount:
                if(inScope != kAudioUnitScope_Global ) return kAudioUnitErr_InvalidScope;
                outDataSize = sizeof(int);
                outWritable = false;
                return noErr;
		}
	}
	
	return AUEffectBase::GetPropertyInfo (inID, inScope, inElement, outDataSize, outWritable);
}

//------------------------------------------------------------------------------------------

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
                    CFSTR("bundle"),                // this is the extension of the cocoa bundle
                    NULL);
                
                if (bundleURL == NULL) return fnfErr;
                
				CFStringRef className = CFSTR("Extractor_ViewFactory");	// name of the main class that implements the AUCocoaUIBase protocol
				AudioUnitCocoaViewInfo cocoaInfo = { bundleURL, { className } };
				*((AudioUnitCocoaViewInfo *)outData) = cocoaInfo;
				
				return noErr;
			}

			// This is our custom property which reports the current frequency response curve
			//
			case kAudioUnitCustomProperty_ExtractorCount:
			{
                *((int*)outData) = 0;
			}
 
            case kAudioUnitCustomProperty_AnalysisBufferSize:
            {
                *((int*)outData) = kDefaultAnalysisBufferSize;
            }
                
            case kAudioUnitCustomProperty_ExtractorModuleConfigurations:
            {
                *((std::list<CAExtractorModuleConfiguration>*)outData) = mConfigurations;
            }
                
            case kAudioUnitCustomProperty_ExtractorModuleConfiguration:
            {
                *((CAExtractorModuleConfiguration*)outData) = mDefaultConfiguration;
            }
            
		}
	}
	
	// if we've gotten this far, handles the standard properties
	return AUEffectBase::GetProperty (inID, inScope, inElement, outData);
}



#pragma mark ____Presets

//------------------------------------------------------------------------------------------

OSStatus Extractor::GetPresets (CFArrayRef* outData) const
{
    // this is used to determine if presets are supported 
    // which in this unit they are so we implement this method!
	if (outData == NULL) return noErr;
	CFMutableArrayRef theArray = CFArrayCreateMutable(NULL, 0, NULL);
	*outData = (CFArrayRef)theArray;	// client is responsible for releasing the array
	return noErr;
}

//------------------------------------------------------------------------------------------

OSStatus Extractor::NewFactoryPresetSet (const AUPreset & inNewFactoryPreset)
{
	return noErr;
}

#pragma mark ____ExtractorKernel

//------------------------------------------------------------------------------------------

ExtractorKernel::ExtractorKernel(AUEffectBase *inAudioUnit ) : AUKernelBase(inAudioUnit), mAubioInputBuffer(0), mCurrentBufferSize(0)
{
    mAubioInputBuffer = new_fvec ((uint_t) kDefaultAnalysisBufferSize);
    
	Reset();
}

//------------------------------------------------------------------------------------------

ExtractorKernel::~ExtractorKernel( )
{
}

//------------------------------------------------------------------------------------------

void ExtractorKernel::Reset()
{
    for ( ExtractorModule* module : mExtractorModules) {
        module->reset();
    }
    
    mCurrentBufferSize = 0;
}

//------------------------------------------------------------------------------------------

void ExtractorKernel::Process(	const Float32 	*inSourceP,
                                Float32 		*inDestP,
                                UInt32 			inFramesToProcess,
                                UInt32			inNumChannels,	// for version 2 AudioUnits inNumChannels is always 1
                                bool &			ioSilence)
{

	const Float32 *sourceP = inSourceP;
	Float32 *destP = inDestP;
	int n = inFramesToProcess;
	
	// Apply the filter on the input and write to the output
	// This code isn't optimized and is written for clarity...
	//
	while(n--)
	{
        // simply copy to the output (memcopy or something like that might be a lot faster...)
		float input = *sourceP++;
		*destP++ = input;
        
        mAubioInputBuffer->data[mCurrentBufferSize] = input;
        
        mCurrentBufferSize++;
        
        if (mCurrentBufferSize >= mAubioInputBuffer->length) {
            mCurrentBufferSize = 0;
            for (ExtractorModule* module : mExtractorModules) {
                module->process(mAubioInputBuffer);
            }
            
        }
	}
}




