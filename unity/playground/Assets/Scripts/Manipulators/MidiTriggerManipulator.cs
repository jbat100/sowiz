using UnityEngine;
using System.Collections;

public class MidiTriggerManipulator : SowizManipulator {

	// Use this for initialization
	void Start () {

		targetControlDelegates["midi"] = delegate(GameObject target, ArrayList values) {

			MidiTrigger[] triggers = target.GetComponents<MidiTrigger>();

			string midiMessageType = (string)values[0];

			foreach(MidiTrigger trigger in triggers) {

				switch (midiMessageType) {
				case "note_on":

				}

				
			}

		}

	}

}
