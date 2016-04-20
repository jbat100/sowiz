using UnityEngine;
using System.Collections;

public class MidiNotePositionModifier : MidiNoteModifier {

	public enum Foundation { Identity, Vector, Transform, Mesh, Spline };

	public Foundation foundation;

	public Vector3Mapping vectorMapping;
	public TransformMapping transformMapping;
	public MeshMapping meshMapping;
	// public SplineMapping splineMapping;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void NoteOn(GameObject instance, int channel, int pitch, int velocity) {

		float val = valueGenerator.Generate(channel, pitch, velocity);

		switch(foundation) {
		case Foundation.Identity:
			instance.transform.localPosition = Vector3.zero;
			break;
		case Foundation.Vector:
			instance.transform.localPosition = vectorMapping.Map(val);
			break;
		case Foundation.Transform:
			transformMapping.MapPosition(instance.transform, val);
			break;
		case Foundation.Mesh:
			meshMapping.MapPosition(instance.transform, val);
			break;
		case Foundation.Spline:
			break;
		}

	}
}
