using UnityEngine;
using System.Collections;

public class SpinManipulator : SowizManipulator {

	public Vector3 axis = new Vector3(1.0f, 0.0f, 0.0f);
	public float scale = 180.0f;

	private float spin = 0.0f;
	
	void Awake() {
		descriptors = new string[] {"spin"};	
	}

	void Update() {
		foreach (GameObject target in targets) {
			target.transform.rotation *= Quaternion.AngleAxis ((float)(scale * spin * Time.deltaTime * 60.0f), axis);
		}
	}

	public override void ApplyMessage(SowizControlMessage message) {
		switch (message.descriptor) {
		case "spin":
			SetSpin ((float) message.values[0]);
			break;
		default:
			break;
		}
	}
	
	private void SetSpin(float m) {
		//Debug.Log ("Setting magnitude to " + m.ToString ());
		spin = m;
	}
}
