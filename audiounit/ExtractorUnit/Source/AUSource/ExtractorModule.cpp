//
//  ExtractorModule.cpp
//  ExtractorUnit
//
//  Created by Jonathan Thorpe on 21/07/15.
//
//

#include "ExtractorModule.h"


    
void ExtractorModule::process(fvec_t* buffer) {
    
    std::list<float> result = extract(buffer);
    
    
    
}
    
ExtractorModuleConfiguration* ExtractorModule::getConfiguration(void) {
    return mConfiguration;
}

bool ExtractorModule::applyConfiguration(ExtractorModuleConfiguration* configuration) {
    
    if (this->conformsToConfiguration(configuration)) {
        return true;
    }
    
    if (!configuration) {
        // destroy?
    }
    
    // check that the UdpTransmitSocket is setup right, if not set it up
    
    const char * address = CFStringGetCStringPtr( configuration->mHost, kCFStringEncodingUTF8);
    int port = configuration->mPort;
    
    IpEndpointName ipEndpointName = IpEndpointName(address, port);
    
    if (ipEndpointName != mCurrentIpEndpointName) {
        if (mTransmitSocket) {
            delete mTransmitSocket;
        }
        mTransmitSocket = new UdpTransmitSocket(ipEndpointName);
        mCurrentIpEndpointName = ipEndpointName;
    }
    
    mConfiguration = configuration;
    
}
    
