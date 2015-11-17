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

	public override void ApplyMessage(SowizControlMessage message) {

		switch (message.descriptor) {

		case "scale":
			SetScale ((float) message.values[0]);
			break;
		default:
			break;

		}

	}

	private void SetScale(float s) {

		Debug.Log ("Setting scale to " + s.ToString ());
	
		Vector3 scaler = unitScale - zeroScale;

		transform.localScale = zeroScale + (s * scaler);
	}
}
