using UnityEngine;
using System.Collections;

public class ShurikenManipulator : SonosthesiaManipulator {

	/*
	 * 
	 * It seems we have very little access to the ParticleSystem modules via scripting which seriously sucks 
	 * http://forum.unity3d.com/threads/access-to-particlesystem-internals-shuriken-from-script.261061/
	 * 
	 * The base manipulator is used to access non-module attributes of the particle system
	 */

	public override void Start() {
			
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

	public void SetVelocity(GameObject target, float velocity) {
		ParticleSystem particleSystem = GetTargetParticleSystem(target);
		particleSystem.startSpeed = velocity;
	}

	public void SetSize(GameObject target, float size) {

		ParticleSystem particleSystem = GetTargetParticleSystem(target);
		particleSystem.startSize = size;

		// use ParticleSystem.Emit directly in order to override colors
		// http://docs.unity3d.com/ScriptReference/ParticleSystem.Emit.html
		// http://docs.unity3d.com/ScriptReference/ParticleSystem.EmitParams.html

		// loop over the particles to alter their properties, for example based on lifetime
		// http://docs.unity3d.com/ScriptReference/ParticleSystem.Particle.html

		//ParticleSystem particleSystem = GetTargetParticleSystem(target);
		//var emission = particleSystem.emission;
		//emission.rate = new ParticleSystem.MinMaxCurve( (r * (unitRate - zeroRate)) + zeroRate );
	}

	public ParticleSystem GetTargetParticleSystem(GameObject target) {
		return target.GetComponent<ParticleSystem>();
	}
			
};
