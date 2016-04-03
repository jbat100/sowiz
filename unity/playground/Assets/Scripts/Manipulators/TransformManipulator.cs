using UnityEngine;
using System.Collections;


public class TransformManipulator : SowizManipulator {

	public SowizVector3Mapping scaleMapping = new SowizVector3Mapping(new Vector3(0.5f, 1f, 1f), new Vector3(5f, 1f, 1f));
	public SowizRotator Rotator = new SowizRotator(new Vector3(1f, 0f, 0f), 180f);
	public SowizSpinner Spinner = new SowizSpinner(new Vector3(1f, 0f, 0f), 180f);

	void Start() {

		targetControlDelegates["scale"] = delegate(GameObject target, ArrayList values) {
			Vector3 newScale = scaleMapping.Map((float)(values[0]));
			Debug.Log ("TransformManipulator setting scale " + newScale.ToString() );
			target.transform.localScale = newScale;
		};

		targetControlDelegates["rotation"] = delegate(GameObject target, ArrayList values) {
			target.transform.rotation = Rotator.GetRotation ((float)(values[0]));
		};

		mainControlDelegates["spin"] = delegate(ArrayList values) {
			Spinner.Spin = (float)(values[0]);
		};

	}

	void Update() {

		foreach (GameObject target in targets) {
			target.transform.rotation *= Spinner.GetRotation();
		}

	}

}
