using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class ParticleManipulator : SowizManipulator {

	protected PlaygroundParticlesC particles;

	// Use this for initialization
	void Start () {

		particles = GetComponent<PlaygroundParticlesC>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
