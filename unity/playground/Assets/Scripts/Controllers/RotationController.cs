using UnityEngine;
using System.Collections;

public class RotationController : SonosthesiaController {

	public Rotator Rotator = new Rotator(new Vector3(1f, 0f, 0f), 180f);
	public Spinner Spinner = new Spinner(new Vector3(1f, 0f, 0f), 180f);

	private bool spun = false;

	// Use this for initialization
	public override void Start () {

		base.Start();
	
		targetControlDelegates["rotation"] = delegate(GameObject target, ArrayList values) {
			target.transform.rotation = Rotator.GetRotation ((float)(values[0]));
		};

		controlDelegates["spin"] = delegate(ArrayList values) {
			if ((float)(values[0]) > 1e-6) spun = true;
			else spun = false;
			Spinner.Spin = (float)(values[0]);
		};

	}
	
	// Update is called once per frame
	void Update () {
		if (spun) {
			foreach (GameObject target in targets) {
				target.transform.rotation *= Spinner.GetRotation();
			}	
		}
	}
}
