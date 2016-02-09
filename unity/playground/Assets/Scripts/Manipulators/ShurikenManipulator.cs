using UnityEngine;
using System.Collections;

public class ShurikenManipulator : SowizManipulator {

		public SowizFloatMapping hueMapping = new SowizFloatMapping(0f, 1f);
		public SowizFloatMapping saturationMapping = new SowizFloatMapping(0f, 1f);
		public SowizFloatMapping brightnessMapping = new SowizFloatMapping(0f, 1f);
		public SowizFloatMapping scaleMapping = new SowizFloatMapping(0.1f, 1.9f);
		public SowizFloatMapping velocityMapping = new SowizFloatMapping(0.1f, 5.0f);

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

		public void SetScale(GameObject target, ArrayList values) {
				ParticleSystem particleSystem = GetTargetParticleSystem(target);
				float val = (float)values[0];
				particleSystem.startSize = scaleMapping.Map(val);
		}

		public void SetVelocity(GameObject target, ArrayList values) {
				ParticleSystem particleSystem = GetTargetParticleSystem(target);
				float val = (float)values[0];
				particleSystem.startSpeed = velocityMapping.Map(val);
		}

		public void SetHue(GameObject target, ArrayList values) {
				
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				float val = (float)values[0];
				// Debug.Log ( this.GetType().Name + " in SetHue with values " + values.ToString() + " val : " + val.ToString());
				hsbColor.h = hueMapping.Map(val);
				SetTargetColor(target, hsbColor.ToColor ());
		}

		public void SetSaturation(GameObject target, ArrayList values) {
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				float val = (float)values[0];
				hsbColor.s = saturationMapping.Map(val);
				SetTargetColor(target, hsbColor.ToColor ());
		}

		public void SetBrightness(GameObject target, ArrayList values) {
				HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
				float val = (float)values[0];
				hsbColor.b = brightnessMapping.Map(val);
				SetTargetColor(target, hsbColor.ToColor ());
		}

	public void SetRate(GameObject target, float r) {

		// use ParticleSystem.Emit directly in order to override colors
		// http://docs.unity3d.com/ScriptReference/ParticleSystem.Emit.html
		// http://docs.unity3d.com/ScriptReference/ParticleSystem.EmitParams.html

		// loop over the particles to alter their properties, for example based on lifetime
		// http://docs.unity3d.com/ScriptReference/ParticleSystem.Particle.html


		//ParticleSystem particleSystem = GetTargetParticleSystem(target);
		//var emission = particleSystem.emission;
		//emission.rate = new ParticleSystem.MinMaxCurve( (r * (unitRate - zeroRate)) + zeroRate );

	}
}
