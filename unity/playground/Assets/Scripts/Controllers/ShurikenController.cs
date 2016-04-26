﻿using UnityEngine;
using System.Collections;

public class ShurikenController : SonosthesiaController {

	public FloatMapping velocityMapping = new FloatMapping(0.1f, 5.0f);
	public FloatMapping sizeMapping = new FloatMapping(0.1f, 1.9f);

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
