
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SonosthesiaResponder : MonoBehaviour {


	protected delegate void ResponderDelegate(ArrayList values);

	protected Dictionary<string, ResponderDelegate> responderDelegates = new Dictionary<string, ResponderDelegate>();

	private static string Tag = "SonosthesiaResponder";


	public virtual void Awake() {

	}

	public virtual void Start() {

	}
		

	public virtual void ApplyMessage(SonosthesiaControlMessage message) {

		ResponderDelegate responderDelegate = null;
		Debug.Log (Tag + " control delegate keys: " + String.Join(", ", responderDelegates.Keys.ToArray()) );
		if (responderDelegates.TryGetValue(message.descriptor, out responderDelegate)) {
			Debug.Log (Tag + " found delegate for descriptor " + message.descriptor );
			// call the main delegate once
			responderDelegate(message.values);
		} else {
			Debug.Log (Tag + " no delegate for descriptor " + message.descriptor );
		}

	}

}
