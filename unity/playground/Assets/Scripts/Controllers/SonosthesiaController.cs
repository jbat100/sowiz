using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SonosthesiaController : SonosthesiaResponder {

	public List<GameObject> targets;

	public delegate List<GameObject> TargetProvider();

	public TargetProvider targetProvider;

	protected delegate void TargetControlDelegate(GameObject target, ArrayList values);

	protected Dictionary<string, TargetControlDelegate> targetControlDelegates = new Dictionary<string, TargetControlDelegate>();

	// a repository of fetched manipulators as performance booster (to avoid repeated GetComponent calls)
	private Dictionary<int, Dictionary<Type, Component>> manipulators = new Dictionary<int, Dictionary<Type, Component>>();

	public void AddTarget(GameObject target) {
		if (!targets.Contains(target)) {
			targets.Add(target);
		}
	}

	public void RemoveTarget(GameObject target) {
		if (targets.Contains(target)) {
			targets.Remove(target);
		}
	}

	public override void ApplyMessage(SonosthesiaControlMessage message) {
		TargetControlDelegate targetControlDelegate = null; 
		if (targetControlDelegates.TryGetValue(message.descriptor, out targetControlDelegate)) {
			// call the target delegate for each target 
			foreach (GameObject target in targets) {
				targetControlDelegate(target, message.values);
			}
		}
	}


		
}
