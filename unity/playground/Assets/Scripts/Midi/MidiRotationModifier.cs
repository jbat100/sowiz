using UnityEngine;
using System.Collections;

public class MidiRotationModifier : MidiModifier {

	public enum Foundation { Identity, Quaternion, Transform, Mesh, Spline };

	public Foundation foundation;

	public QuaternionMapping quaternionMapping;
	public TransformMapping transformMapping;
	public MeshMapping meshMapping;
	// public SplineMapping splineMapping;

	public override void NoteOn(GameObject instance, int channel, int pitch, int velocity) {

		float val = valueGenerator.GenerateNoteValue(channel, pitch, velocity);

		switch(foundation) {
		case Foundation.Identity:
			instance.transform.localRotation = Quaternion.identity;
			break;
		case Foundation.Quaternion:
			instance.transform.localRotation = quaternionMapping.Map(val);
			break;
		case Foundation.Transform:
			instance.transform.localRotation = transformMapping.MapRotation(val);
			break;
		case Foundation.Mesh:
			instance.transform.localRotation = meshMapping.MapRotation(val);
			break;
		case Foundation.Spline:
			break;
		}

	}
}
