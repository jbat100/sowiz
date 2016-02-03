using UnityEngine;
using System.Collections;

public class ShurikenColorHueManipulator : ShurikenColorManipulator {
	
	public float zeroHue = 0.0f;
	public float unitHue = 1.0f;

	void Awake() {
		descriptors = new string[] {"hue"};	
	}
	
	public override void ApplyMessageToTarget(GameObject target, SowizControlMessage message) {
		switch (message.descriptor) {
		case "hue":
			SetHue (target, (float) message.values[0]);
			break;
		default:
			break;	
		}
	}
	
	private void SetHue(GameObject target, float m) {
		Debug.Log ("ParticleColorHueManipulator setting magnitude to " + m.ToString ());
		Color color = GetTargetColor(target);
		HSBColor hsbColor = HSBColor.FromColor (color);
		HSBColor newColor = hsbColor;
		newColor.h = (m * (unitHue - zeroHue)) + zeroHue;
		color = newColor.ToColor ();
		Debug.Log ("ParticleColorHueManipulator color is " + color.ToString() + ", old hsb " + hsbColor.ToString() + ", new hsb" + newColor.ToString() );
		SetTargetColor(target, color);
	}
}
