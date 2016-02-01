using UnityEngine;
using System.Collections;

public class RotationManipulator : SowizManipulator {

	public Vector3 axis = new Vector3(1.0f, 0.0f, 0.0f);
	public float scale = 180.0f;

	void Awake() {
		descriptors = new string[] {"magnitude"};	
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
		
		//Debug.Log ("Setting magnitude to " + m.ToString ());
		
		target.transform.rotation = Quaternion.AngleAxis ( scale * m, axis );
	}
}
