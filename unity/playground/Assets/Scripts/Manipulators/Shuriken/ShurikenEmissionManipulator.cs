using UnityEngine;
using System.Collections;

public class ShurikenEmissionManipulator : ShurikenManipulator {


		public float zeroRate = 20.0f;
		public float unitRate = 100.0f;


		void Awake() {
				descriptors = new string[] {"rate"};	
				//descriptors = new string[] {"hue"};	
		}

		public override void ApplyMessageToTarget(GameObject target, SowizControlMessage message) {
				switch (message.descriptor) {
				case "rate":
						SetRate (target, (float) message.values[0]);
						break;
				default:
						break;	
				}
		}

		public void Update() {
				
				// use ParticleSystem.Emit directly in order to override colors
				// http://docs.unity3d.com/ScriptReference/ParticleSystem.Emit.html
				// http://docs.unity3d.com/ScriptReference/ParticleSystem.EmitParams.html

				// loop over the particles to alter their properties, for example based on lifetime
				// http://docs.unity3d.com/ScriptReference/ParticleSystem.Particle.html


		}

		public void SetRate(GameObject target, float r) {



				//ParticleSystem particleSystem = GetTargetParticleSystem(target);
				//var emission = particleSystem.emission;
				//emission.rate = new ParticleSystem.MinMaxCurve( (r * (unitRate - zeroRate)) + zeroRate );
		
		}
}
