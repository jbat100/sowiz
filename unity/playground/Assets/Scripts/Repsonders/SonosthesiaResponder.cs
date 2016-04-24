
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SonosthesiaResponder : MonoBehaviour {

	static private string Tag = "SonosthesiaResponder";

	public string[] groups;

	protected delegate void ControlDelegate(ArrayList values);

	protected Dictionary<string, ControlDelegate> controlDelegates = new Dictionary<string, ControlDelegate>();

	public virtual void Start() {

	}

	public void ProcessMessage(SonosthesiaControlMessage message) {
		// should we apply messages for this group?
		var result = string.Join(", ", message.values.ToArray().Select(o => o.ToString()).ToArray());
		if (Array.IndexOf(groups, message.group) == -1) {
			Debug.Log (Tag + " not applying " + message.ToString() + " with values : " + result );
			return;
		} else {
			Debug.Log (Tag + " applying " + message.ToString() + " with values : " + result );
			ApplyMessage(message);
		}
	}

	public virtual void ApplyMessage(SonosthesiaControlMessage message) {
		ControlDelegate mainControlDelegate = null;
		Debug.Log (Tag + " control delegate keys: " + String.Join(", ", controlDelegates.Keys.ToArray()) );
		if (controlDelegates.TryGetValue(message.descriptor, out mainControlDelegate)) {
			Debug.Log (Tag + " found delegate for descriptor " + message.descriptor );
			// call the main delegate once
			mainControlDelegate(message.values);
		} else {
			Debug.Log (Tag + " no delegate for descriptor " + message.descriptor );
		}
	}

}
