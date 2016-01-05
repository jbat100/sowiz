using UnityEngine;
using System.Collections;

public class ScaleManipulator : SowizManipulator {

	public Vector3 zeroScale = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 unitScale = new Vector3(1.0f, 1.0f, 1.0f);

	//private Vector3 originalScale = new Vector3(1.0f, 1.0f, 1.0f);

	void Awake() {
		descriptors = new string[] {"magnitude"};	
	}

	void Start() {
		//originalScale = transform.localScale;
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
	
		Vector3 scaler = unitScale - zeroScale;

		target.transform.localScale = zeroScale + (m * scaler);
	}
}
