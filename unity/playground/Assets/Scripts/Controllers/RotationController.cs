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
			IRotationManipulator manipulator = (IRotationManipulator)GetManipulator(target, typeof(IRotationManipulator));
			Quaternion rotation = Rotator.GetRotation((float)(values[0]));
			manipulator.SetRotation(target, rotation);
		};

		// spin does not apply to individual targets (hence controlDelegates and not targetControlDelegates)
		// the effect is applied to each target in the Update method
		controlDelegates["spin"] = delegate(ArrayList values) {
			if ((float)(values[0]) > 1e-6) spun = true;
			else spun = false;
			Spinner.Spin = (float)(values[0]);
		};

	}
	
	// Update is called once per frame
	void Update () {

		// spin involves the rotation being updated every frame
		if (spun) {
			foreach (GameObject target in targets) {
				IRotationManipulator manipulator = (IRotationManipulator)GetManipulator(target, typeof(IRotationManipulator));
				Quaternion old = manipulator.GetRotation(target);
				manipulator.SetRotation(target, old *= Spinner.GetRotation());
			}	
		}

	}
}
