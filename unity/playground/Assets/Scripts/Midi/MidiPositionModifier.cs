using UnityEngine;
using System.Collections;

public class MidiPositionModifier : MidiModifier {

	public enum Foundation { Identity, Vector, Transform, Mesh, Spline };

	public Foundation foundation;

	public Vector3Mapping vectorMapping;
	public TransformMapping transformMapping;
	public MeshMapping meshMapping;
	// public SplineMapping splineMapping;


	public override void NoteOn(GameObject instance, int channel, int pitch, int velocity) {

		float val = valueGenerator.GenerateNoteValue(channel, pitch, velocity);

		switch(foundation) {
		case Foundation.Identity:
			instance.transform.localPosition = Vector3.zero;
			break;
		case Foundation.Vector:
			instance.transform.localPosition = vectorMapping.Map(val);
			break;
		case Foundation.Transform:
			instance.transform.localPosition = transformMapping.MapPosition(val);
			break;
		case Foundation.Mesh:
			instance.transform.localPosition = meshMapping.MapPosition(val);
			break;
		case Foundation.Spline:
			break;
		}

	}
}
