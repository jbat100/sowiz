
//You can set these variables in the scene because they are public 
public var RemoteIP : String = "127.0.0.1";
public var SendToPort : int = 9123;
public var ListenerPort : int = 3333;
private var handler : Osc;



public function Start ()
{
	//Initializes on start up to listen for messages
	//make sure this game object has both UDPPackIO and OSC script attached
	var udp : UDPPacketIO = GetComponent("UDPPacketIO");
	udp.init(RemoteIP, SendToPort, ListenerPort);
	handler = GetComponent("Osc");
	handler.init(udp);
			
	//handler.SetAddressHandler("/sowiz/scene/descriptor", SceneDescriptorMessageCallback);
	handler.SetAllMessageHandler(DefaultMessageCallback);
}

//these fucntions are called when messages are received
public function SceneDescriptorMessageCallback(oscMessage : OscMessage) : void
{	
	//How to access values: 
	//oscMessage.Values[0], oscMessage.Values[1], etc
	Debug.Log("SceneDescriptorMessageCallback layer " + oscMessage.Values[0]);
} 

public function DefaultMessageCallback(oscMessage : OscMessage) : void
{	
	//How to access values: 
	//oscMessage.Values[0], oscMessage.Values[1], etc
	Debug.Log("DefaultMessageCallback received message " + oscMessage.Address + ' ' + oscMessage.Values[0]);

	// var routingParameters = OscMessageRoutingParameters(oscMessage);
	// Debug.Log("DefaultMessageCallback routing parameters : " + routingParameters.ToString() );
} 

//public function OscMessageRoutingParameters(oscMessage : OscMessage) : object
//{
//	var elements = oscMessage.Address.Split('/');
//	
//	
//}
