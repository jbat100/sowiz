using UnityEngine;
using System.Collections;

public class MidiScaleModifier : MidiModifier {

	public enum Foundation { Identity, Vector, Transform };

	public Foundation foundation;

	public Vector3Mapping vectorMapping;
	public TransformMapping transformMapping;

	public override void NoteOn(GameObject instance, int channel, int pitch, int velocity) {

		float val = valueGenerator.GenerateNoteValue(channel, pitch, velocity);

		switch(foundation) {
		case Foundation.Identity:
			instance.transform.localScale = Vector3.one;
			break;
		case Foundation.Vector:
			instance.transform.localScale = vectorMapping.Map(val);
			break;
		case Foundation.Transform:
			instance.transform.localScale = transformMapping.MapScale(val);
			break;
		}

	}
}
