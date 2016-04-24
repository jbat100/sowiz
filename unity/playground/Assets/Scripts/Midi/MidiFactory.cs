using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MidiNoteInstance : System.Object {

	GameObject instance;

	int channel;
	int pitch;
	int velocity;

	public MidiNoteInstance(GameObject _instance, int _channel, int _pitch, int _velocity) {
		instance = _instance;
		channel = _channel;
		pitch = _pitch;
		velocity = _velocity;
	}

	public GameObject Instance { 
		get { return instance; }
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


public class MidiFactory : MidiResponder {

	private static string Tag = "MidiFactory";

	public MidiNoteDomain NoteDomain;

	public GameObject prefab;

	private List<MidiNoteInstance> instances;
	private List<MidiModifier> modifiers;

	private MidiNoteInstance GetInstance (int channel, int pitch) {
		// make this faster with ling
		return instances.Find(i => i.Channel == channel && i.Pitch == pitch);
	}

	private void DestroyInstance (int channel, int pitch) {
		MidiNoteInstance midiNoteInstance = GetInstance(channel, pitch);
		instances.Remove(midiNoteInstance);
		if (midiNoteInstance != null) {
			Destroy(midiNoteInstance.Instance);
		};
	}

	// Use this for initialization
	public override void Start () {

		base.Start();

		Debug.Log(Tag + " Start, setting midi on and off delegates");

		instances = new List<MidiNoteInstance>();

		modifiers = GetComponents<MidiModifier>().ToList();

		Debug.Log(Tag + " Start, got " + modifiers.Count + " modifiers");

		// TODO : get all modifiers

		noteOnDelegate = delegate(int channel, int pitch, int velocity) {

			Debug.Log(Tag + " note_on : " + channel + " " + pitch + " " + velocity);

			if (NoteDomain.ContainsNote(channel, pitch, velocity) == false) {
				Debug.Log(Tag + " midi note is not in the relevant midi domain");
				return;
			}

			if (prefab == null) {
				Debug.Log(Tag + " no prefab");
				return; 
			}

			// set the instance as a child of the spawner so that the overall spawning structure can be moved/rotated
			GameObject instance = Instantiate(prefab);
			instance.transform.parent = transform;

			foreach(MidiModifier modifier in modifiers) {
				modifier.NoteOn(instance, channel, pitch, velocity);
			}

			instances.Add(new MidiNoteInstance(instance, channel, pitch, velocity));

		};

		noteOffDelegate = delegate(int channel, int pitch, int velocity) {

			Debug.Log(Tag + " note_off : " + channel + " " + pitch + " " + velocity);

			MidiNoteInstance midiNoteInstance = GetInstance(channel, pitch);
			if (midiNoteInstance != null) {
				foreach(MidiModifier modifier in modifiers) {
					modifier.NoteOff(midiNoteInstance.Instance, channel, pitch, velocity);
				}
			}

			DestroyInstance(channel, pitch);

		};
	
	}

}
