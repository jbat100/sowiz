using UnityEngine;
using System.Collections;

public class ShurikenManipulator : SowizManipulator {

	/*
	 * It seems we have very little access to the ParticleSystem modules via scripting which seriously sucks 
	 * http://forum.unity3d.com/threads/access-to-particlesystem-internals-shuriken-from-script.261061/
	 */

	// Use this for initialization
	void Start () {
		//particleSystem = target.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public ParticleSystem GetTargetParticleSystem(GameObject target) {
		return target.GetComponent<ParticleSystem>();
	}
}
