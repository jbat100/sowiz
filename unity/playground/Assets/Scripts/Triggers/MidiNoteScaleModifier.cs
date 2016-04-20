using UnityEngine;
using System.Collections;

public class MidiNoteScaleModifier : MidiNoteModifier {

	public enum Foundation { Identity, Vector, Transform };

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
			instance.transform.localScale = Vector3.one;
			break;
		case Foundation.Vector:
			instance.transform.localScale = vectorMapping.Map(val);
			break;
		case Foundation.Transform:
			transformMapping.MapScale(instance.transform, val);
			break;
		}

	}
}
