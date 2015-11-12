using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class ConcurrentQueue<T> {

	private readonly object syncLock = new object();
	private Queue<T> queue;
	
	public int Count
	{
		get
		{
			lock(syncLock) 
			{
				return queue.Count;
			}
		}
	}
	
	public ConcurrentQueue()
	{
		this.queue = new Queue<T>();
	}
	
	public T Peek()
	{
		lock(syncLock)
		{
			return queue.Peek();
		}
	}	
	
	public void Enqueue(T obj)
	{
		lock(syncLock)
		{
			queue.Enqueue(obj);
		}
	}
	
	public T Dequeue()
	{
		lock(syncLock)
		{
			return queue.Dequeue();
		}
	}
	
	public void Clear()
	{
		lock(syncLock)
		{
			queue.Clear();
		}
	}
	
	public T[] CopyToArray()
	{
		lock(syncLock)
		{
			if(queue.Count == 0)
			{
				return new T[0];
			}
			
			T[] values = new T[queue.Count];
			queue.CopyTo(values, 0);	
			return values;
		}
	}
	
	public static ConcurrentQueue<T> InitFromArray(IEnumerable<T> initValues)
	{
		var queue = new ConcurrentQueue<T>();
		
		if(initValues == null)	
		{
			return queue;
		}
		
		foreach(T val in initValues)
		{
			queue.Enqueue(val);
		}
		
		return queue;
	}
}



public class SowizControlMessage
{
	public string analyser;
	public string descriptor;
	public string group;
	public string feature;

	public ArrayList values;

	public SowizControlMessage(string _analyser, string _descriptor, string _group, string _feature, ArrayList _values) {
		analyser = _analyser;
		descriptor = _descriptor;
		group = _group;
		feature = _feature;
		values = _values;
	}

	public override string ToString()
	{
		//Debug.Log("group is " + group);
		return "SowizRoutingParameters (analyser: " + analyser + ", descriptor: " + descriptor + ", group: " + group + ", feature: " + feature + ")";
	}

};

public class SowizOSCManager : MonoBehaviour {

	public string remoteIP = "127.0.0.1";
	public int remotePort = 9123;
	public int localPort = 3333;
	private Osc handler;

	private GameObject[] sowizObjects;
	private ConcurrentQueue<SowizControlMessage> messageQueue = new ConcurrentQueue<SowizControlMessage> ();
	private int updateCount = 0;

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

		sowizObjects = GameObject.FindGameObjectsWithTag("Sowiz");
	}
	
	// Update is called once per frame
	void Update () {

		updateCount++;

		// Debug.Log ("Update " + updateCount.ToString ());

		while (messageQueue.Count > 0) {
			try {
				SowizControlMessage message = messageQueue.Dequeue();
				//Debug.Log ("Dequeued control message " + message.ToString() );
				ApplyMessage(message);
			} catch(InvalidOperationException) {
				//Debug.Log ("Exception while dequeing message");
			}
		}

	}

	void DefaultMessageCallback (OscMessage oscMessage) {

		//Debug.Log("DefaultMessageCallback received message " + message.Address + ' ' + message.Values[0]);
		SowizControlMessage sowizMessage = SowizMessageFromOscMessage (oscMessage);
		if (sowizMessage != null) {
			Debug.Log("Received message with routing parameters : " + sowizMessage.ToString() );
			// work out how to call Apply from the main thread (doesn't like calling unity engine stuff on the OSC server's thread)
			messageQueue.Enqueue(sowizMessage);
		}

	}

	void ApplyMessage(SowizControlMessage message) {

		foreach (GameObject gameObject in sowizObjects) {
			
			SowizManipulator[] manipulators = gameObject.GetComponents<SowizManipulator>();
			
			foreach (SowizManipulator manipulator in manipulators) {
				if ( System.Array.IndexOf(manipulator.groups, message.group) != -1) {
					manipulator.ApplyMessage(message);
				}
			}
		}

	}

	SowizControlMessage SowizMessageFromOscMessage(OscMessage message) {
	
		string[] elements = message.Address.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

		//Debug.Log("RoutingParametersForMessage elements are " + elements.ToString());

		if (elements.Length < 4) {
			Debug.Log("DefaultMessageCallback unexpected elements length");
			return null;
		}
		
		return new SowizControlMessage (elements [0], elements [1], elements [2], elements [3], message.Values);
	
	}


};
