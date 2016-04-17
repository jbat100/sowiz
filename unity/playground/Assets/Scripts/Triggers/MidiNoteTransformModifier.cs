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

	public enum Foundation { Identity, Endpoints, Mesh, Spline };

	public Foundation foundation;

	public MidiNoteMapping positionMapping;
	public MidiNoteMapping rotationMapping;
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

	public RelativeTransform GetLocalTransform(int channel, int pitch, int velocity) {

		Vector3 position = GetLocalPosition(channel, pitch, velocity);
		Quaternion rotation = GetLocalRotation(channel, pitch, velocity);
		Vector3 scale = GetLocalScale(channel, pitch, velocity);

		return new RelativeTransform(position, rotation, scale);
	}

	public Vector3 GetLocalScale(int channel, int pitch, int velocity) {

		float mapped = scaleMapping.GetMapped(channel, pitch, velocity);

		Vector3 scale = Vector3.one;

		switch(foundation) {
		case Foundation.Endpoints:
			scale = Vector3.Lerp(startpoint.localScale, endpoint.localScale, mapped);
			break;
		case Foundation.Identity:
			break;
		default:
			Debug.Log("Unimplemented foundation");
			break;
		}

		return scale;
		
	}

	public Vector3 GetLocalPosition(int channel, int pitch, int velocity) {

		float mapped = positionMapping.GetMapped(channel, pitch, velocity);

		Vector3 position = Vector3.zero;

		switch(foundation) {
		case Foundation.Endpoints:
			position = Vector3.Lerp(startpoint.localPosition, endpoint.localPosition, mapped);
			break;
		case Foundation.Identity:
			break;
		default:
			Debug.Log("Unimplemented foundation");
			break;
		}

		return position;

	}

	public Quaternion GetLocalRotation(int channel, int pitch, int velocity) {

		float mapped = rotationMapping.GetMapped(channel, pitch, velocity);

		Quaternion rotation = Quaternion.identity;

		switch(foundation) {
		case Foundation.Endpoints:
			rotation = Quaternion.Slerp(startpoint.localRotation, endpoint.localRotation, mapped);
			break;
		case Foundation.Identity:
			break;
		default:
			Debug.Log("Unimplemented foundation");
			break;
		}

		return rotation;

	}
}
