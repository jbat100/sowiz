using UnityEngine;
using System.Collections;

public class ShurikenColorHueManipulator : ShurikenColorManipulator {
	
	public float zeroHue = 0.0f;
	public float unitHue = 1.0f;

	void Awake() {
		descriptors = new string[] {"magnitude"};	
	}
	
	public override void ApplyMessage(SowizControlMessage message) {
		switch (message.descriptor) {
			case "magnitude":
				SetMagnitude ((float) message.values[0]);
				break;
			default:
				break;	
		}
	}
	
	private void SetMagnitude(float m) {
		Debug.Log ("ParticleColorHueManipulator setting magnitude to " + m.ToString ());
		Color color = GetColor();
		HSBColor hsbColor = HSBColor.FromColor (color);
		HSBColor newColor = hsbColor;
		newColor.h = (m * (unitHue - zeroHue)) + zeroHue;
		color = newColor.ToColor ();
		Debug.Log ("ParticleColorHueManipulator color is " + color.ToString() + ", old hsb " + hsbColor.ToString() + ", new hsb" + newColor.ToString() );
		SetColor(color);
	}
}
