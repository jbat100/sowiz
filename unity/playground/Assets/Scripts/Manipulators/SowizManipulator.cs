using UnityEngine;
using System.Collections;
using System.Linq;

public class SowizManipulator : MonoBehaviour {

	public GameObject target;

	public string[] groups;

	protected string[] descriptors = new string[] {};

	// Use this for initialization
	void Start () {	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void ApplyMessage(SowizControlMessage message) {
		var result = string.Join(",", message.values.ToArray().Select(o => o.ToString()).ToArray());
		Debug.Log ("Applying " + message.ToString() + " with values : " + result );
	}
}