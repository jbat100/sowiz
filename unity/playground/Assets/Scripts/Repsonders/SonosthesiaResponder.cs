using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SonosthesiaResponder : MonoBehaviour {

	public string[] groups;

	protected delegate void ControlDelegate(ArrayList values);

	protected Dictionary<string, ControlDelegate> controlDelegates;

	// Use this for initialization
	public virtual void Awake () {	
		controlDelegates = new Dictionary<string, ControlDelegate>();
	}

	public virtual void Start() {

	}

	public void ProcessMessage(SonosthesiaControlMessage message) {
		// should we apply messages for this group?
		if (Array.IndexOf(groups, message.group) == -1) {
			return;
		} else {
			var result = string.Join(",", message.values.ToArray().Select(o => o.ToString()).ToArray());
			Debug.Log ("Applying " + message.ToString() + " with values : " + result );
			ApplyMessage(message);
		}
	}

	public virtual void ApplyMessage(SonosthesiaControlMessage message) {
		ControlDelegate mainControlDelegate = null;
		if (controlDelegates.TryGetValue(message.descriptor, out mainControlDelegate))
		{
			// call the main delegate once
			mainControlDelegate(message.values);
		}
	}

}
