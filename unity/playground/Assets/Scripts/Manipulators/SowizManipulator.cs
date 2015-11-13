using UnityEngine;
using System.Collections;
using System.Linq;

public class SowizManipulator : MonoBehaviour {

	public string[] groups;

	protected string[] descriptor;

	// Use this for initialization
	void Start () {
		descriptor = [];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string GetDescriptor() {
		return descriptor;
	}

	public void ApplyMessage(SowizControlMessage message) {
		var result = string.Join(",", message.values.ToArray().Select(o => o.ToString()).ToArray());
		Debug.Log ("Applying " + message.ToString() + " with values : " + result );
	}
}