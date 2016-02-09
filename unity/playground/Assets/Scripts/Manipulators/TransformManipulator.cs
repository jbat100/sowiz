using UnityEngine;
using System.Collections;


public class TransformManipulator : SowizManipulator {

	public SowizVector3Mapping scaleMapping = new SowizVector3Mapping(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f));
	public SowizRotator Rotator = SowizRotator(new Vector3(1f, 0f, 0f), 180f);
	public SowizSpinner Spinner = SowizSpinner(new Vector3(1f, 0f, 0f), 180f);

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
	private void SetSpin(GameObject target, float m) {
		Spinner.Spin = m;
	}
		
	private void SetScale(GameObject target, float m) {
		target.transform.localScale = scaleMapping.Map(m);
	}

	public void SetRotation(GameObject target, float m) {
		target.transform.rotation = Rotator.GetRotation (m);
	}
}
