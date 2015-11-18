using UnityEngine;
using System.Collections;

public class SpinManipulator : SowizManipulator {

	public Vector3 axis = new Vector3(1.0f, 0.0f, 0.0f);
	public float scale = 180.0f;

	private float magnitude = 0.0f;
	
	void Awake() {
		descriptors = new string[] {"magnitude"};	
	}

	void Update() {
		target.transform.rotation *= Quaternion.AngleAxis ( (float) (scale * magnitude * Time.deltaTime * 60.0f), axis );
	}

	public override void ApplyMessage(SowizControlMessage message) {
		
		switch (message.descriptor) {
			
		case "magnitude":
			SetMagnitude ((float) message.values[0]);
			break;
		default:
			break;
			
		}
		
	}
	
	private void SetMagnitude(float m) {
		Debug.Log ("Setting magnitude to " + m.ToString ());
		magnitude = m;
	}
}
