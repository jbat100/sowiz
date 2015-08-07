//
//  ExtractorModule.cpp
//  ExtractorUnit
//
//  Created by Jonathan Thorpe on 21/07/15.
//
//

#include "ExtractorModule.h"

#include "aubio/aubio.h"
#include "oscpack/ip/UdpSocket.h"
#include "oscpack/osc/OscOutboundPacketStream.h"

ExtractorModule::ExtractorModule() : mTransmitSocket(0), mConfiguration(0), mResults(0), mProcessCount(0) {
    
}

void ExtractorModule::process(fvec_t* buffer)
{
    std::list<float> results;
    this->extract(buffer, results);

    // send the results
    
    osc::OutboundPacketStream p(mBuffer, OSC_BUFFER_SIZE);
    
    p << osc::BeginBundleImmediate
    << osc::BeginMessage( "/sowiz/processcount" ) << osc::int32(mProcessCount) << osc::EndMessage
    << osc::EndBundle;
   
    mTransmitSocket->Send( p.Data(), p.Size() );
    
    mProcessCount++;
}

ExtractorModuleConfiguration* ExtractorModule::getConfiguration(void) {
    return mConfiguration;
}

bool ExtractorModule::conformsToConfiguration(ExtractorModuleConfiguration* configuration) {
    return false;
}

bool ExtractorModule::applyConfiguration(ExtractorModuleConfiguration* configuration) {
    
    if (this->conformsToConfiguration(configuration)) {
        return true;
    }
    
    if (!configuration) {
        // destroy?
    }
    
    // convert CFString CFStringGetCStringPtr( configuration->mHost, kCFStringEncodingUTF8);
    
    // check that the UdpTransmitSocket is setup right, if not set it up
    
    IpEndpointName ipEndpointName = IpEndpointName(configuration->mHost.c_str(), configuration->mPort);
    
    if (ipEndpointName != mCurrentIpEndpointName) {
        if (mTransmitSocket) {
            delete mTransmitSocket;
        }
        mTransmitSocket = new UdpTransmitSocket(ipEndpointName);
        mCurrentIpEndpointName = ipEndpointName;
    }
    
    mConfiguration = configuration;
    
    return true;
}



void TestExtractorModule::extract(fvec_t* buffer, std::list<float>& results) {
    
}
    
bool TestExtractorModule::conformsToConfiguration(ExtractorModuleConfiguration* configuration) {
    return ExtractorModule::conformsToConfiguration(configuration);
}
    
bool TestExtractorModule::applyConfiguration(ExtractorModuleConfiguration* configuration) {
    return ExtractorModule::applyConfiguration(configuration);
};

void LevelExtractorModule::extract(fvec_t* buffer, std::list<float>& results) {
    
}

bool LevelExtractorModule::conformsToConfiguration(ExtractorModuleConfiguration* configuration) {
    return ExtractorModule::conformsToConfiguration(configuration);
}

bool LevelExtractorModule::applyConfiguration(ExtractorModuleConfiguration* configuration) {
    return ExtractorModule::applyConfiguration(configuration);
};




