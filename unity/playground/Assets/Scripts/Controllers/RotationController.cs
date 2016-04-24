using UnityEngine;
using System.Collections;

public class RotationController : SonosthesiaController {

	public Rotator Rotator = new Rotator(new Vector3(1f, 0f, 0f), 180f);
	public Spinner Spinner = new Spinner(new Vector3(1f, 0f, 0f), 180f);

	// Use this for initialization
	void Start () {
	
		targetControlDelegates["rotation"] = delegate(GameObject target, ArrayList values) {
			target.transform.rotation = Rotator.GetRotation ((float)(values[0]));
		};

		controlDelegates["spin"] = delegate(ArrayList values) {
			Spinner.Spin = (float)(values[0]);
		};

	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject target in targets) {
			target.transform.rotation *= Spinner.GetRotation();
		}
	}
}
