using UnityEngine;
using System.Collections;

public class ColorController : SonosthesiaController {

	public FloatMapping hueMapping = new FloatMapping(0f, 1f);
	public FloatMapping saturationMapping = new FloatMapping(0f, 1f);
	public FloatMapping brightnessMapping = new FloatMapping(0f, 1f);

	// Use this for initialization
	public override void Start () {

		base.Start();

		controlDelegates["hue"] = delegate(ControlTarget target, ArrayList values) {
			ColorManipulator manipulator = target.GetManipulator<ColorManipulator>();
			HSBColor hsbColor = HSBColor.FromColor(manipulator.GetColor(target));
			hsbColor.h = hueMapping.Map((float)(values[0]));
			manipulator.SetColor(hsbColor.ToColor ());
		};

		controlDelegates["saturation"] = delegate(ControlTarget target, ArrayList values) {
			ColorManipulator manipulator = target.GetManipulator<ColorManipulator>();
			HSBColor hsbColor = HSBColor.FromColor(manipulator.GetColor(target));
			hsbColor.s = saturationMapping.Map((float)(values[0]));
			manipulator.SetColor(target, hsbColor.ToColor ());
		};

		controlDelegates["brightness"] = delegate(ControlTarget target, ArrayList values) {
			ColorManipulator manipulator = target.GetManipulator<ColorManipulator>();
			HSBColor hsbColor = HSBColor.FromColor(manipulator.GetColor(target));
			hsbColor.b = brightnessMapping.Map((float)(values[0]));
			manipulator.SetColor(target, hsbColor.ToColor ());
		};
	
	}

}
