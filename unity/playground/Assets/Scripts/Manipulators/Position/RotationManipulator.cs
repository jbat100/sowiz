using UnityEngine;
using System.Collections;

public class RotationManipulator : SowizManipulator {

	public Vector3 axis = new Vector3(1.0f, 0.0f, 0.0f);
	public float scale = 180.0f;

	void Awake() {
		descriptors = new string[] {"rotation"};	
	}

	public override void ApplyMessageToTarget(GameObject target, SowizControlMessage message) {
		switch (message.descriptor) {
		case "rotation":
			SetRotation (target, (float) message.values[0]);
			break;
		default:
			break;
		}
	}
	
	private void SetRotation(GameObject target, float m) {
		//Debug.Log ("Setting magnitude to " + m.ToString ());
		target.transform.rotation = Quaternion.AngleAxis ( scale * m, axis );
	}
}
