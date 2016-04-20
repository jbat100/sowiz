using UnityEngine;
using System.Collections;

public class MidiNoteModifier : MonoBehaviour {

	public MidiNoteValueGenerator valueGenerator;

	// perhaps have a sync (source/target) switch here for the value generator

	private MidiNoteModifier syncSource;

	// Use this for initialization
	public virtual void Start () {

	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

	public virtual void NoteOn(GameObject instance, int channel, int pitch, int velocity) {
		
	}

	public virtual void NoteOff(GameObject instance, int channel, int pitch, int velocity) {
		
	}

	// add methods for control, pressure, aftertouch midi messages
}
