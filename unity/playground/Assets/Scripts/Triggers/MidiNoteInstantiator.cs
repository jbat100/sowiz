using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MidiNoteInstance : System.Object {

	GameObject prefabInstance;

	int channel;
	int pitch;
	int velocity;

	public MidiNoteInstance(GameObject _prefabInstance, int _channel, int _pitch, int _velocity) {
		prefabInstance = _prefabInstance;
		channel = _channel;
		pitch = _pitch;
		velocity = _velocity;
	}

	public GameObject PrefabInstance { 
		get { return prefabInstance; }
	}

	public int Channel { 
		get { return channel; }
	}

	public int Pitch { 
		get { return pitch; }
	}

	public int Velocity { 
		get { return velocity; }
	}

}


public class MidiNoteInstantiator : MidiTrigger {

	private static string Tag = "MidiNoteInstantiator";

	public MidiNoteDomain NoteDomain;
	public MidiNoteTransform NoteTransform;

	// I think there should be only one prefab so as to have instantiator level modifiers which fit
	// If you want multiple prefabs, then create mutiple instantiators
	public GameObject prefab;

	private List<MidiNoteInstance> instances;

	private MidiNoteInstance GetInstance (int channel, int pitch) {
		// make this faster with ling
		return instances.Find(i => i.Channel == channel && i.Pitch == pitch);
	}

	private void DestroyInstance (int channel, int pitch) {
		MidiNoteInstance instance = GetInstance(channel, pitch);
		instances.Remove(instance);
		if (instance) {
			Destroy(instance);
		};
	}

	// Use this for initialization
	void Start () {

		noteOnDelegate = delegate(int channel, int pitch, int velocity) {

			Debug.Log(Tag + " note on : " + channel + " " + pitch + " " + velocity);

			if (NoteDomain.ContainsNote(channel, pitch, velocity) == false) {
				Debug.Log(Tag + " midi note is not in the relevant midi domain");
				return;
			}

			if (prefab == null) {
				Debug.Log(Tag + " NoteObject did not specify prefab");
				return; 
			}

			// set the instance as a child of the spawner so that the overall spawning structure can be moved/rotated

			GameObject instance = Instantiate(prefab);

			instance.transform.parent = transform;

			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			Vector3 scale = Vector3.one;

			if (NoteTransform) {
				position = NoteTransform.GetPosition(channel, pitch, velocity);
				rotation = NoteTransform.GetRotation(channel, pitch, velocity);
				scale = NoteTransform.GetScale(channel, pitch, velocity);
			} 

			instance.transform.localPosition = position;
			instance.transform.localRotation = rotation;
			instance.transform.localScale = scale;

			Debug.Log(Tag + " relative transform : " + position + " " + rotation + " " + scale);

			// color ? material ? depends on prefab so it's difficult to do something generic

		};
	
	}

}
