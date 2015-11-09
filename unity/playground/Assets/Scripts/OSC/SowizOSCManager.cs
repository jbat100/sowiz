using UnityEngine;
using System.Collections;
using System;

public class SowizRoutingParameters
{
	public string analyser;
	public string descriptor;
	public string group;

	public SowizRoutingParameters(string _analyser, string _descriptor, string _group) {
		analyser = _analyser;
		descriptor = _descriptor;
		group = _group;
	}

	public override string ToString()
	{
		Debug.Log("group is " + group);
		return "SowizRoutingParameters: " + analyser + " " + descriptor + " " + group;
	}

};

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


		SowizRoutingParameters parameters = RoutingParametersForMessage (message);

		if (parameters != null) {
			Debug.Log("Extracted routing parameters : " + parameters.ToString() );
		}

	}

	SowizRoutingParameters RoutingParametersForMessage(OscMessage message) {
	
		string[] elements = message.Address.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

		Debug.Log("RoutingParametersForMessage elements are " + elements.ToString());

		if (elements.Length < 3) {
			Debug.Log("DefaultMessageCallback unexpected elements length");
			return null;
		}
		
		return new SowizRoutingParameters (elements [0], elements [1], elements [2]);
	
	}


};
