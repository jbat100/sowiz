using UnityEngine;
using System.Collections;


public class TransformManipulator : SowizManipulator {

	public SowizVector3Mapping scaleMapping = new SowizVector3Mapping(new Vector3(0.5f, 1f, 1f), new Vector3(5f, 1f, 1f));
	public SowizRotator Rotator = new SowizRotator(new Vector3(1f, 0f, 0f), 180f);
	public SowizSpinner Spinner = new SowizSpinner(new Vector3(1f, 0f, 0f), 180f);

	void Awake() {
		descriptors = new string[] {"scale", "rotation", "spin"};	
	}

	void Update() {
		foreach (GameObject target in targets) {
			target.transform.rotation *= Spinner.GetRotation();
		}
	}

	// TODO: find a better way to handle dispatch when per target application is not needed, 
	// maybe have an dict of delegate methods (this would also be faster...) 

	public void SetSpin(GameObject target, ArrayList values) {
		Spinner.Spin = (float)(values[0]);
	}
		
	public void SetScale(GameObject target, ArrayList values) {
		Vector3 newScale = scaleMapping.Map((float)(values[0]));
		Debug.Log ("TransformManipulator setting scale " + newScale.ToString() );
		target.transform.localScale = newScale;
	}

	public void SetRotation(GameObject target, ArrayList values) {
		target.transform.rotation = Rotator.GetRotation ((float)(values[0]));
	}
}
