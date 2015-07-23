
#define kAubioBufferSize 512

// Here we define a custom property so the view is able to retrieve the current frequency
// response curve.  The curve changes as the filter's cutoff frequency and resonance are
// changed...

// custom properties id's must be 64000 or greater
// see <AudioUnit/AudioUnitProperties.h> for a list of Apple-defined standard properties
//

#include <string>
#include <map>

enum
{
	kAudioUnitCustomProperty_ExtractorModuleConfigurations = 64000,
    kAudioUnitCustomProperty_AnalysisBufferSize = 64001,
    kAudioUnitCustomProperty_ExtractorCount = 64002
};

// We'll define our property data to be a size kNumberOfResponseFrequencies array of structs
// The UI will pass in the desired frequency in the mFrequency field, and the Extractor AU
// will provide the linear magnitude response of the filter in the mMagnitude field
// for each element in the array.

typedef struct CAExtractorModuleConfiguration
{
    int                 mExtractorType;
	CFStringRef         mHost;
	int                 mPort;
    CFStringRef         mPath;
    int                 mLayerID;
    int                 mGroupID;
    Float32             mScale;
    Float32             mOffset;
    CFDictionaryRef     mParameters;
} CAExtractorModuleConfiguration;





// extractor types

