using UnityEngine;
using System.Collections;

public class MidiResponder : SonosthesiaResponder {

	public delegate void MidiNoteOnDelegate(int channel, int pitch, int velocity);
	public delegate void MidiNoteOffDelegate(int channel, int pitch, int velocity);

	[HideInInspector]
	public MidiNoteOnDelegate noteOnDelegate;

	[HideInInspector]
	public MidiNoteOffDelegate noteOffDelegate;

	// Use this for initialization
	public override void Start () {


		controlDelegates["midi"] = delegate(ArrayList values) {

			string midiMessageType = (string)values[0];

			switch (midiMessageType) {

			case "note_on":
				if (values.Count == 4) {
					noteOnDelegate((int)values[1], (int)values[2], (int)values[3]);
				}
				break;

			case "note_off":
				if (values.Count == 4) {
					noteOffDelegate((int)values[1], (int)values[2], (int)values[3]);
				}
				break;

			default:
				Debug.Log("Unsupported midi message type " + midiMessageType);
				break;

			}

		};

	}

}
