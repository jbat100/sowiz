using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class SowizFloatMapping : System.Object
{
		public float Zero = 0f;
		public float Unit = 1f;

		public SowizFloatMapping(float _zero, float _unit) {
				Zero = _zero;
				Unit = _unit;
		}
				
		public float Map(float val) {
				return (val * (Unit - Zero)) + Zero;
		}
}

[System.Serializable]
public class SowizVector3Mapping : System.Object
{
	public Vector3 Zero = new Vector3(0f, 0f, 0f);
	public Vector3 Unit = new Vector3(0f, 0f, 1f);

	public SowizVector3Mapping(Vector3 _zero, Vector3 _unit) {
		Zero = _zero;
		Unit = _unit;
	}

	public Vector3 Map(float val) {
		return ((1f - val) * Zero) + (val * Unit);
	}
}

[System.Serializable]
public class SowizRotator : System.Object
{
	public Vector3 Axis = new Vector3(1f, 0f, 0f);
	public float Scale = 180f;

	public SowizRotator(Vector3 _axis, float _scale) {
		Axis = _axis;
		Scale = _scale;
	}

	public Quaternion GetRotation(float val) {
		return Quaternion.AngleAxis ( Scale * val, Axis );
	}
}

[System.Serializable]
public class SowizSpinner : System.Object
{
	public Vector3 Axis = new Vector3(1f, 0f, 0f);
	public float Scale = 180f;
	public float Spin = 0f;

	public SowizSpinner(Vector3 _axis, float _scale) {
		Axis = _axis;
		Scale = _scale;
	}

	public Quaternion GetRotation() {
		return Quaternion.AngleAxis ((float)(Scale * Spin * Time.deltaTime * 60f), Axis);
	}
}

public class SowizManipulator : MonoBehaviour {

	public GameObject[] targets;

	public string[] groups;

	protected delegate void SowizMainControlDelegate(ArrayList values);
	protected delegate void SowizTargetControlDelegate(GameObject target, ArrayList values);

	protected Dictionary<string, SowizMainControlDelegate> mainControlDelegates;
	protected Dictionary<string, SowizTargetControlDelegate> targetControlDelegates;

	// Use this for initialization
	void Awake () {	
		mainControlDelegates = new Dictionary<string, SowizMainControlDelegate>();
		targetControlDelegates = new Dictionary<string, SowizTargetControlDelegate>();
	}

	void Start() {
	
	}

	public virtual void ApplyMessage(SowizControlMessage message) {

		// should we apply messages for this group?
		if (Array.IndexOf(groups, message.group) == -1) {
			return;
		} 

		var result = string.Join(",", message.values.ToArray().Select(o => o.ToString()).ToArray());
		Debug.Log ("Applying " + message.ToString() + " with values : " + result );

		SowizMainControlDelegate mainControlDelegate = null;
		if (mainControlDelegates.TryGetValue(message.descriptor, out mainControlDelegate))
		{
			// call the main delegate once
			mainControlDelegate(message.values);
		}

		SowizTargetControlDelegate targetControlDelegate = null; 
		if (targetControlDelegates.TryGetValue(message.descriptor, out targetControlDelegate))
		{
			// call the target delegate for each target 
			foreach (GameObject target in targets) {
				targetControlDelegate(target, message.values);
			}
		}
	}
		
}
