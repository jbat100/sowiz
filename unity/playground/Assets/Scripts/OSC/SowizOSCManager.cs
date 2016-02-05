using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class ControlMessageQueue {

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

		public ControlMessageQueue()
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

		public void Enqueue(SowizControlMessage message)
		{
				lock(syncLock)
				{
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

		public static ControlMessageQueue InitFromArray(IEnumerable<SowizControlMessage> initValues)
		{
				var queue = new ControlMessageQueue();

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
}



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

};

public class SowizOSCManager : MonoBehaviour {

		public string remoteIP = "127.0.0.1";
		public int remotePort = 9123;
		public int localPort = 3333;
		private Osc handler;

		private GameObject[] sowizObjects;
		private ControlMessageQueue messageQueue = new ControlMessageQueue ();
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

				/*

				while (messageQueue.Count > 0) 
				{
						try 
						{
								SowizControlMessage message = messageQueue.Dequeue();
								//Debug.Log ("Dequeued control message " + message.ToString() );
								ApplyMessage(message);
						} 
						catch(InvalidOperationException) 
						{
								//Debug.Log ("Exception while dequeing message");
						}
				}

*/

				List<SowizControlMessage> dequeued = messageQueue.DequeueAll();

				foreach (SowizControlMessage message in dequeued) {
						ApplyMessage(message);
				}

		}

		void OnApplicationQuit()
		{
				UDPPacketIO udp = (UDPPacketIO)GetComponent("UDPPacketIO");
				udp.Close ();
		}

		void DefaultMessageCallback (OscMessage oscMessage) {

				//Debug.Log("DefaultMessageCallback received message " + message.Address + ' ' + message.Values[0]);
				SowizControlMessage sowizMessage = SowizMessageFromOscMessage (oscMessage);
				if (sowizMessage != null) 
				{
						Debug.Log("Received message with routing parameters : " + sowizMessage.ToString() );
						// work out how to call Apply from the main thread (doesn't like calling unity engine stuff on the OSC server's thread)
						messageQueue.Enqueue(sowizMessage);
				}

		}

		void ApplyMessage(SowizControlMessage message) {

				foreach (GameObject gameObject in sowizObjects) 
				{
						SowizManipulator[] manipulators = gameObject.GetComponents<SowizManipulator>();
						//Debug.Log("Applying message to " + manipulators.Length.ToString() + " manipulators" );
						foreach (SowizManipulator manipulator in manipulators) 
						{
								if ( System.Array.IndexOf(manipulator.groups, message.group) != -1) 
								{
										manipulator.ApplyMessage(message);
								}
						}
				}

		}

		SowizControlMessage SowizMessageFromOscMessage(OscMessage message) {

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
