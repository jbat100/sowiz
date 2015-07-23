//
//  ExtractorModule.h
//  ExtractorUnit
//
//  Created by Jonathan Thorpe on 21/07/15.
//
//

#ifndef __ExtractorUnit__ExtractorModule__
#define __ExtractorUnit__ExtractorModule__

#include <list>

#include "aubio/aubio.h"
#include "oscpack/ip/UdpSocket.h"

#include "Extractor.h"

enum
{
    kExtractorType_None = 0,
    kExtractorType_Level = 1,
    kExtractorType_Pitch = 2
};


typedef std::map<std::string, std::string> parameter_map;

typedef struct ExtractorModuleConfiguration
{
    int                             mExtractorType;
    std::string                     mHost;
    int                             mPort;
    std::string                     mPath;
    int                             mLayerID;
    int                             mGroupID;
    Float32                         mScale;
    Float32                         mOffset;
    parameter_map                   mParameters;
} ExtractorModuleConfiguration;

class ExtractorModule {
    
public:
    
    static const std::string kDefaultHost;
    static const int kDefaultPort;
    
    void process(fvec_t* buffer);
    
    ExtractorModuleConfiguration* getConfiguration(void);
    
    /* virtual methods */
    
    virtual void extract(fvec_t* buffer, std::list<float>& results);
    
    virtual bool conformsToConfiguration(ExtractorModuleConfiguration* configuration);
    
    virtual bool applyConfiguration(ExtractorModuleConfiguration* configuration);
    
    
private:
    
    UdpTransmitSocket* mTransmitSocket;
    IpEndpointName mCurrentIpEndpointName;
    
    ExtractorModuleConfiguration* mConfiguration;
    CFArrayRef* mResults; // to store the results on each process, to avoid reallocating a results array
    
};


class LevelExtractorModule : public ExtractorModule {
    
    virtual void extract(fvec_t* buffer, std::list<float>& results);
    
    virtual bool conformsToConfiguration(ExtractorModuleConfiguration* configuration);
    
    virtual bool applyConfiguration(ExtractorModuleConfiguration* configuration);
    
};


#endif /* defined(__ExtractorUnit__ExtractorModule__) */

