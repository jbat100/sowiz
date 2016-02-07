using UnityEngine;
using System.Collections;

public class ShurikenColorHSBManipulator : ShurikenColorManipulator {


		void Awake() {
				descriptors = new string[] {"hue", "saturation", "brightness"};	
				//descriptors = new string[] {"hue"};	
		}

		public override void ApplyMessageToTarget(GameObject target, SowizControlMessage message) {
				switch (message.descriptor) {
				case "hue":
						SetHue (target, (float) message.values[0]);
						break;
				case "saturation":
						SetSaturation (target, (float) message.values[0]);
						break;
				case "brightness":
						SetBrightness (target, (float) message.values[0]);
						break;
				default:
						break;	
				}
		}

		private void SetHue(GameObject target, float m) {
				Debug.Log ("ParticleColorHueManipulator setting hue to " + m.ToString ());
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				hsbColor.h = (m * (unitHue - zeroHue)) + zeroHue;
				SetTargetColor(target, hsbColor.ToColor ());
		}

		private void SetSaturation(GameObject target, float m) {
				Debug.Log ("ParticleColorHueManipulator setting saturation to " + m.ToString ());
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				hsbColor.s = (m * (unitSaturation - zeroSaturation)) + zeroSaturation;
				SetTargetColor(target, hsbColor.ToColor ());
		}

		private void SetBrightness(GameObject target, float m) {
				Debug.Log ("ParticleColorHueManipulator setting brightness to " + m.ToString ());
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				hsbColor.b = (m * (unitBrightness - zeroBrightness)) + zeroBrightness;
				SetTargetColor(target, hsbColor.ToColor ());
		}
				
}
