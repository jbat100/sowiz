using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SonosthesiaController : SonosthesiaResponder {

	public delegate void ControlDelegate(ControlTarget target, ArrayList values);

	public ControlTargetProvider TargetProvider { get { return targetProvider;} }

	protected Dictionary<string, ControlDelegate> controlDelegates = new Dictionary<string, ControlDelegate>();

	private ControlTargetProvider targetProvider;

	public override void Awake() {

		base.Awake();

		targetProvider = GetComponent<ControlTargetProvider>();

	}

	public override void ApplyMessage(SonosthesiaControlMessage message) {

		// apply normal responder delegate through base (target independant)
		base.ApplyMessage(message);

		// then call the controlDelegate for each target provided
		List<ControlTarget> targets = targetProvider.GetTargets();
		ControlDelegate controlDelegate = null; 
		if (controlDelegates.TryGetValue(message.descriptor, out controlDelegate)) {
			// call the target delegate for each target 
			foreach (ControlTarget target in targets) {
				controlDelegate(target, message.values);
			}
		}
	}


		
}
