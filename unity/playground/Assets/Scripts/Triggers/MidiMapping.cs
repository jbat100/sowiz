using UnityEngine;
using System.Collections;
using System;

public enum MidiNoteParameter { Channel, Pitch, Velocity };

[System.Serializable]
public class MidiNoteMapping : System.Object {

	public ValueMapping valueMapping;

	public MidiNoteParameter parameter = MidiNoteParameter.Pitch;

	public float GetMapped(int channel, int pitch, int velocity) {

		int value = 0;

		switch(parameter) {
		case MidiNoteParameter.Channel:
			value = channel;
			break;
		case MidiNoteParameter.Pitch:
			value = pitch;
			break;
		case MidiNoteParameter.Velocity:
			value = velocity;
			break;
		default:
			break;
		}

		valueMapping.SetInput(((float)value) / 127.0f);

		return valueMapping.GetMapped();
		
	}

};
