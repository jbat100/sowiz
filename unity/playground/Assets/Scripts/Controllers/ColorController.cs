using UnityEngine;
using System.Collections;

public class ColorController : SonosthesiaController {

	public FloatMapping hueMapping = new FloatMapping(0f, 1f);
	public FloatMapping saturationMapping = new FloatMapping(0f, 1f);
	public FloatMapping brightnessMapping = new FloatMapping(0f, 1f);

	// Use this for initialization
	public override void Start () {

		base.Start();

		// TODO use interfaces and move the manipulator to the subclasses for more fine grained selection
		// using interfaces will definitely make things more flexible and extendible

		targetControlDelegates["hue"] = delegate(GameObject target, ArrayList values) {
			HSBColor hsbColor = HSBColor.FromColor(manipulator.GetTargetColor(target));
			hsbColor.h = hueMapping.Map((float)(values[0]));
			manipulator.SetTargetColor(target, hsbColor.ToColor ());
		};

		targetControlDelegates["saturation"] = delegate(GameObject target, ArrayList values) {
			HSBColor hsbColor = HSBColor.FromColor(manipulator.GetTargetColor(target));
			hsbColor.s = saturationMapping.Map((float)(values[0]));
			manipulator.SetTargetColor(target, hsbColor.ToColor ());
		};

		targetControlDelegates["brightness"] = delegate(GameObject target, ArrayList values) {
			HSBColor hsbColor = HSBColor.FromColor(manipulator.GetTargetColor(target));
			hsbColor.b = brightnessMapping.Map((float)(values[0]));
			manipulator.SetTargetColor(target, hsbColor.ToColor ());
		};
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
