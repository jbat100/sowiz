//=======================================================================
/** @file Osc.cpp
 *  @brief A class for constructing and send OSC messages
 *  @author Adam Stark
 *  @copyright Copyright (C) 2014  Adam Stark
 *
 * This file is part of Sound Analyser.
 *
 * Sound Analyser is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Sound Analyser is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Sound Analyser.  If not, see <http://www.gnu.org/licenses/>.
 */
//=======================================================================

#include "Osc.h"

#define LOCAL_PORT 0

//==============================================================================
Osc::Osc()
{
    datagramSocket = new DatagramSocket (false);
    
    currentIP = ADDRESS;
    currentPort = PORT;
    
    connect();
}

//==============================================================================
Osc::~Osc()
{
    disconnect();
}

//==============================================================================
void Osc::sendMessage(std::string address,float value)
{
    address += '\0';
    
    while ((address.size() % 4) != 0)
    {
        address += '\0';
    }
    
    address += ',';
    
    address += 'f';
    
    while ((address.size() % 4) != 0)
    {
        address += '\0';
    }
    
    char a[sizeof(float)];
    
    memcpy(a, &value, sizeof(float));
    
    address += a[3];
    address += a[2];
    address += a[1];
    address += a[0];
    
    datagramSocket->write(currentIP, currentPort, address.c_str(), address.size());
}

//==============================================================================
void Osc::sendMessage(std::string address,std::vector<float> values)
{
    address += '\0';
    
    while ((address.size() % 4) != 0)
    {
        address += '\0';
    }
    
    address += ',';
    
    // add all type tags
    for (int i = 0;i < values.size();i++)
    {
        address += 'f';
    }
    
    while ((address.size() % 4) != 0)
    {
        address += '\0';
    }
    
    char a[sizeof(float)];
    
    for (int i = 0;i < values.size();i++)
    {
        memcpy(a, &values[i], sizeof(float));
        
        address += a[3];
        address += a[2];
        address += a[1];
        address += a[0];
    }
    
    datagramSocket->write(currentIP, currentPort, address.c_str(), address.size());
}

//==============================================================================
void Osc::connect()
{
    /*
     
    if(datagramSocket->connect (currentIP.c_str(), currentPort))
    {
        // THEN WE CONNECTED SUCCESSFULLY
    }
    else
    {
        DBG("ERROR CONNECTING");
    }
     
     */
    
    datagramSocket->bindToPort(LOCAL_PORT);
    
}

//==============================================================================
void Osc::disconnect()
{
    /*
     
    if (datagramSocket != nullptr && datagramSocket->isConnected())
    {
        datagramSocket->close();
    }
     
     */
}

//==============================================================================
void Osc::setPort(int port)
{
    if (currentPort != port)
    {
        currentPort = port;
    
        connect();
    }
}

//==============================================================================
void Osc::setIpAddress(std::string ipAddress)
{
    if (ipAddress != currentIP)
    {
        currentIP = ipAddress;
    
        connect();
    }
}