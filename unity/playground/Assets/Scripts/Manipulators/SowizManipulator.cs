using UnityEngine;
using System.Collections;
using System.Linq;

[System.Serializable]
public class SowizFloatMapping : System.Object
{
		public float Zero = 0.0f;
		public float Unit = 1.0f;

		public SowizFloatMapping(float _zero, float _unit) {
				Zero = _zero;
				Unit = _unit;
		}
				
		public float Map(float val) {
				return (val * (Unit - Zero)) + Zero;
		}
}

public class SowizManipulator : MonoBehaviour {

	public GameObject[] targets;

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
		foreach (GameObject target in targets) {
			ApplyMessageToTarget (target, message);
		}
	}

	public virtual void ApplyMessageToTarget(GameObject target, SowizControlMessage message) {
		// override this method to do processing
	}
}
