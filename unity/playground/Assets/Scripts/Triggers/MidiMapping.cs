using UnityEngine;
using System.Collections;
using System;

public enum MidiNoteParameter { Channel, Pitch, Velocity };



[System.Serializable]
public class MidiNoteMapping : System.Object {

	public ValueMapping valueMapping;

	public MidiNoteParameter parameter = MidiNoteParameter.Pitch;

	void float GetMapped() {
		
	}

};
