using UnityEngine;
using System.Collections;

public class ScaleManipulator : SowizManipulator {

	public Vector3 zeroScale = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 unitScale = new Vector3(1.0f, 1.0f, 1.0f);

	//private Vector3 originalScale = new Vector3(1.0f, 1.0f, 1.0f);

	void Awake() {
		descriptors = new string[] {"scale"};	
	}

	void Start() {
		//originalScale = transform.localScale;
	}

	public override void ApplyMessageToTarget(GameObject target, SowizControlMessage message) {
		switch (message.descriptor) {
		case "scale":
			SetScale (target, (float) message.values[0]);
			break;
		default:
			break;
		}
	}

	private void SetScale(GameObject target, float m) {
		Debug.Log ("Setting magnitude to " + m.ToString ());
		Vector3 scaler = unitScale - zeroScale;
		target.transform.localScale = zeroScale + (m * scaler);
	}
}
