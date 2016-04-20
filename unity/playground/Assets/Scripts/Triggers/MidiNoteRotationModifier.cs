using UnityEngine;
using System.Collections;

public class MidiNoteRotationModifier : MidiNoteModifier {

	public enum Foundation { Identity, Quaternion, Transform, Mesh, Spline };

	public Foundation foundation;

	public QuaternionMapping quaternionMapping;
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
			instance.transform.localRotation = Quaternion.identity;
			break;
		case Foundation.Quaternion:
			instance.transform.localRotation = quaternionMapping.Map(val);
			break;
		case Foundation.Transform:
			transformMapping.MapRotation(instance.transform, val);
			break;
		case Foundation.Mesh:
			meshMapping.MapRotation(instance.transform, val);
			break;
		case Foundation.Spline:
			break;
		}

	}
}
