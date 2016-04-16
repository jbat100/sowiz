using UnityEngine;
using System;
using System.Collections;

public class RelativeTransform : System.Object {

	public Vector3 Position;
	public Quaternion Rotation;
	public Vector3 Scale;

	public RelativeTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale) {
		this.Position = _position;
		this.Rotation = _rotation;
		this.Scale = _scale;
	}

}
	
	
public class MidiNoteTransformModifier : MonoBehaviour {

	public enum TransformFoundation { Identity, Endpoints, Mesh, Spline };

	public TransformFoundation Foundation;

	public MidiNoteMapping positionMapping;
	public MidiNoteMapping orientationMapping;
	public MidiNoteMapping scaleMapping;

	public Transform startpoint;
	public Transform endpoint;

	public Mesh mesh;
	// public Spline spline;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {

	}
		

	// switched from multiple methods for getting the position, orientation and scale 
	// this is for example if we randomly select a point on a spline and want the values
	// to be consistent with each other

	public RelativeTransform GetRelativeTransform(int channel, int pitch, int velocity) {



		return new RelativeTransform(Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public Vector3 GetRelativePosition(int channel, int pitch, int velocity) {
		
	}
}
