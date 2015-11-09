using UnityEngine;
using System.Collections;

public class SowizOSCManager : MonoBehaviour {

	public string remoteIP = "127.0.0.1";
	public int remotePort = 9123;
	public int localPort = 3333;
	private Osc handler;

	// Use this for initialization
	void Start () {
		//Initializes on start up to listen for messages
		//make sure this game object has both UDPPackIO and OSC script attached
		UDPPacketIO udp = (UDPPacketIO)GetComponent("UDPPacketIO");
		udp.init(remoteIP, remotePort, localPort);
		handler = (Osc)GetComponent("Osc");
		handler.init(udp);
		
		//handler.SetAddressHandler("/sowiz/scene/descriptor", SceneDescriptorMessageCallback);
		handler.SetAllMessageHandler(DefaultMessageCallback);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DefaultMessageCallback (OscMessage message) {

		Debug.Log("DefaultMessageCallback received message " + message.Address + ' ' + message.Values[0]);

	}
}
