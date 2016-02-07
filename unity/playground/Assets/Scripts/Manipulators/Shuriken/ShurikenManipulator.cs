using UnityEngine;
using System.Collections;

public class ShurikenManipulator : SowizManipulator {

		public float zeroHue = 0.0f;
		public float unitHue = 1.0f;

		public float zeroSaturation = 0.0f;
		public float unitSaturation = 1.0f;

		public float zeroBrightness = 0.0f;
		public float unitBrightness = 1.0f;

		public float zeroScale = 0.1f;
		public float unitScale = 1.9f;

		public float zeroVelocity = 1.0f;
		public float unitVelocity = 5.0f;

		/*
	 * 
	 * It seems we have very little access to the ParticleSystem modules via scripting which seriously sucks 
	 * http://forum.unity3d.com/threads/access-to-particlesystem-internals-shuriken-from-script.261061/
	 * 
	 * The base manipulator is used to access non-module attributes of the particle system
	 */

		void Awake() {
				descriptors = new string[] {"scale", "velocity", "hue", "saturation", "brightness"};	
				//descriptors = new string[] {"hue"};	
		}

		// Use this for initialization
		void Start () {
				//particleSystem = target.GetComponent<ParticleSystem>();
		}

		// Update is called once per frame
		void Update () {

		}

		public override void ApplyMessageToTarget(GameObject target, SowizControlMessage message) {
				switch (message.descriptor) {
				case "scale":
						SetScale (target, (float) message.values[0]);
						break;
				case "velocity":
						SetVelocity (target, (float) message.values[0]);
						break;
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

		public Color GetTargetColor(GameObject target) {
				ParticleSystem particleSystem = GetTargetParticleSystem (target);
				return particleSystem.startColor;
				//return new Color ();
		}

		public void SetTargetColor(GameObject target, Color color) {
				ParticleSystem particleSystem = GetTargetParticleSystem (target);
				particleSystem.startColor = color;
		}

		public ParticleSystem GetTargetParticleSystem(GameObject target) {
				return target.GetComponent<ParticleSystem>();
		}

		private void SetScale(GameObject target, float m) {
				Debug.Log ("ShurikenManipulator setting scale to " + m.ToString ());
				ParticleSystem particleSystem = GetTargetParticleSystem(target);
				particleSystem.startSize = (m * (unitScale - zeroScale)) + zeroScale;
		}

		private void SetVelocity(GameObject target, float m) {
				Debug.Log ("ShurikenManipulator setting velocity to " + m.ToString ());
				ParticleSystem particleSystem = GetTargetParticleSystem(target);
				particleSystem.startSpeed = (m * (unitVelocity - zeroVelocity)) + zeroVelocity;
		}

		private void SetHue(GameObject target, float m) {
				Debug.Log ("ShurikenManipulator setting hue to " + m.ToString ());
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				hsbColor.h = (m * (unitHue - zeroHue)) + zeroHue;
				SetTargetColor(target, hsbColor.ToColor ());
		}

		private void SetSaturation(GameObject target, float m) {
				Debug.Log ("ShurikenManipulator setting saturation to " + m.ToString ());
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				hsbColor.s = (m * (unitSaturation - zeroSaturation)) + zeroSaturation;
				SetTargetColor(target, hsbColor.ToColor ());
		}

		private void SetBrightness(GameObject target, float m) {
				Debug.Log ("ShurikenManipulator setting brightness to " + m.ToString ());
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				hsbColor.b = (m * (unitBrightness - zeroBrightness)) + zeroBrightness;
				SetTargetColor(target, hsbColor.ToColor ());
		}
}
