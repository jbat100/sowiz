using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SowizControlMessage
{
	public string descriptor;
	public string group;

	public ArrayList values;

	public SowizControlMessage(string _group, string _descriptor, ArrayList _values) {
		descriptor = _descriptor;
		group = _group;
		values = _values;
	}

	public override string ToString()
	{
		//Debug.Log("group is " + group);
		return "SowizRoutingParameters (descriptor: " + descriptor + ", group: " + group + ")";
	}

	public bool sameTarget(SowizControlMessage message) {
		if (!message.descriptor.Equals (descriptor) || !message.group.Equals (group)) {
			return false;
		}
		return true;
	}

	public static SowizControlMessage FromOscMessage(OscMessage message) {

		string[] elements = message.Address.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

		//Debug.Log("RoutingParametersForMessage elements are " + elements.ToString());

		if (elements.Length < 2) 
		{
			Debug.Log("DefaultMessageCallback unexpected elements length");
			return null;
		}

		return new SowizControlMessage (elements [0], elements [1], message.Values);
	}
};

public class SowizControlMessageQueue {

	private readonly object syncLock = new object();
	private List<SowizControlMessage> queue;

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

	public SowizControlMessageQueue()
	{
		this.queue = new List<SowizControlMessage>();
	}

	public SowizControlMessage Peek()
	{
		lock(syncLock)
		{
			return queue[0];
		}
	}	

	private void UnprotectedEnqueue (SowizControlMessage message) {

		int index = -1;
		for (int i = 0; i < queue.Count; i++)
		{
			SowizControlMessage m = queue[i];
			if (m.sameTarget(message)) 
			{
				index = i;
				break;
			}
		}
		if (index == -1) 
		{
			queue.Add(message);
		} 
		else 
		{
			queue[index] = message;
		}
		
	}

	public void Enqueue(SowizControlMessage message)
	{
		lock(syncLock)
		{
			this.UnprotectedEnqueue(message);
		}
	}

	public void TransferFrom(SowizControlMessageQueue otherQueue)
	{
		lock(syncLock)
		{
			List<SowizControlMessage> transfered = otherQueue.DequeueAll();

			if (transfered.Count > 0) {
				Debug.Log("Transferring " + transfered.Count + " control messages");
			}

			foreach(SowizControlMessage message in transfered)
			{
				this.UnprotectedEnqueue(message);
			}
		}
	}

	public SowizControlMessage Dequeue()
	{
		lock(syncLock)
		{
			if (queue.Count > 0) {
				SowizControlMessage message = queue[0];
				queue.RemoveAt(0);
				return message;
			}
			return null;
		}
	}

	public List<SowizControlMessage> DequeueAll()
	{
		lock(syncLock)
		{
			List<SowizControlMessage> copied = new List<SowizControlMessage>(queue);			
			queue.Clear();
			return copied;
		}
	}

	public void Clear()
	{
		lock(syncLock)
		{
			queue.Clear();
		}
	}

	public SowizControlMessage[] CopyToArray()
	{
		lock(syncLock)
		{
			if(queue.Count == 0)
			{
				return new SowizControlMessage[0];
			}

			SowizControlMessage[] values = new SowizControlMessage[queue.Count];
			queue.CopyTo(values, 0);	
			return values;
		}
	}

	public static SowizControlMessageQueue InitFromArray(IEnumerable<SowizControlMessage> initValues)
	{
		var queue = new SowizControlMessageQueue();

		if(initValues == null)	
		{
			return queue;
		}

		foreach(SowizControlMessage val in initValues)
		{
			queue.Enqueue(val);
		}

		return queue;
	}
};
	

public class SowizControlMessageBuffer {

	public Osc handler;

	private SowizControlMessageQueue bufferQueue;

	public SowizControlMessageBuffer(Osc _handler) {
		
		bufferQueue = new SowizControlMessageQueue ();
		handler = _handler;
		handler.SetAllMessageHandler(MessageCallback);

	}

	public void TransferTo(SowizControlMessageQueue targetQueue) {
		
		targetQueue.TransferFrom(bufferQueue);

	}

	void MessageCallback (OscMessage oscMessage) {
		
		//Debug.Log("DefaultMessageCallback received message " + message.Address + ' ' + message.Values[0]);
		SowizControlMessage sowizMessage = SowizControlMessage.FromOscMessage (oscMessage);
		if (sowizMessage != null) 
		{
			Debug.Log("Received message : " + sowizMessage.ToString() );
			bufferQueue.Enqueue(sowizMessage);
		}

	}

};

public class SowizOSCManager : MonoBehaviour {

	public string remoteIP = "127.0.0.1";
	public int remotePort = 9123;
	public int localPort = 3333;

	private Osc handler;
	private SowizControlMessageBuffer messageBuffer;
	private SowizControlMessageQueue messageQueue;

	private SowizManipulator[] manipulators;
	private int updateCount = 0;
	private float period = 0.017f;

	// Use this for initialization
	void Start () {

		//Initializes on start up to listen for messages
		//make sure this game object has both UDPPackIO and OSC script attached
		UDPPacketIO udp = (UDPPacketIO)GetComponent("UDPPacketIO");
		udp.init(remoteIP, remotePort, localPort);
		handler = (Osc)GetComponent("Osc");
		handler.init(udp);

		messageQueue = new SowizControlMessageQueue ();
		messageBuffer = new SowizControlMessageBuffer(handler);

		// get all game objects with maniulator components
		//manipulators = gameObject.GetComponents<SowizManipulator>();

		//HingeJoint[] hinges = FindObjectsOfType(typeof(HingeJoint)) as HingeJoint[];

		manipulators = FindObjectsOfType(typeof(SowizManipulator)) as SowizManipulator[];

		//manipulators = (SowizManipulator[])FindObjectOfType<SowizManipulator>();

		Debug.Log("Found " + manipulators.Length + " manipulators");

		InvokeRepeating("ReadBuffer", period, period);

	}

	// Update is called once per frame
	void Update () {

		updateCount++;

		List<SowizControlMessage> dequeued = messageQueue.DequeueAll();

		foreach (SowizControlMessage message in dequeued) {
			ApplyMessage(message);
		}

	}

	void ReadBuffer() {
		
		messageBuffer.TransferTo(messageQueue);

	}

	void OnApplicationQuit() {
		
		UDPPacketIO udp = (UDPPacketIO)GetComponent("UDPPacketIO");
		udp.Close ();

	}

	void ApplyMessage(SowizControlMessage message) {
		
		foreach (SowizManipulator manipulator in manipulators) {
			if ( System.Array.IndexOf(manipulator.groups, message.group) != -1) {
				manipulator.ApplyMessage(message);
			}
		}

	}
};
