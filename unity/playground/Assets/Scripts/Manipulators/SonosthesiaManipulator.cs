using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SonosthesiaManipulator : SonosthesiaResponder {

	public GameObject[] targets;

	protected delegate void TargetControlDelegate(GameObject target, ArrayList values);

	protected Dictionary<string, TargetControlDelegate> targetControlDelegates;

	// Use this for initialization
	public override void Awake () {	
		base.Awake();
		targetControlDelegates = new Dictionary<string, TargetControlDelegate>();
	}

	public override void ApplyMessage(SonosthesiaControlMessage message) {
		TargetControlDelegate targetControlDelegate = null; 
		if (targetControlDelegates.TryGetValue(message.descriptor, out targetControlDelegate))
		{
			// call the target delegate for each target 
			foreach (GameObject target in targets) {
				targetControlDelegate(target, message.values);
			}
		}
	}
		
}
