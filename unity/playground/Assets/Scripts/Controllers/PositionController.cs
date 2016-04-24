using UnityEngine;
using System.Collections;

public class PositionController : SonosthesiaController {



	public override void Start() {

		targetControlDelegates["scale"] = delegate(GameObject target, ArrayList values) {
			Vector3 newScale = scaleMapping.Map((float)(values[0]));
			Debug.Log ("TransformManipulator setting scale " + newScale.ToString() );
			target.transform.localScale = newScale;
		};

		targetControlDelegates["rotation"] = delegate(GameObject target, ArrayList values) {
			target.transform.rotation = Rotator.GetRotation ((float)(values[0]));
		};

		controlDelegates["spin"] = delegate(ArrayList values) {
			Spinner.Spin = (float)(values[0]);
		};

	}

	void Update() {


	}
}
