using UnityEngine;
using System.Collections;

public class ShurikenColorManipulator : ShurikenManipulator {

	public int gradientColorKeyIndex = 0;

	public Color GetTargetColor(GameObject target) {
		ParticleSystem particleSystem = GetTargetParticleSystem (target);
		//return particleSystem.colorOverLifetime.color.gradientMax.colorKeys [0].color;
		return particleSystem.startColor;
		//return new Color ();
	}

	public void SetTargetColor(GameObject target, Color color) {
		ParticleSystem particleSystem = GetTargetParticleSystem (target);
		//particleSystem.colorOverLifetime.color.gradientMax.colorKeys [0].color = color;
		particleSystem.startColor = color;
	}
}
