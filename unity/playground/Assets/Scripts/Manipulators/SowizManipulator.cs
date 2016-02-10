using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;

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

	// TODO: consider using an automatic call mechanism as this is tedious
	// http://stackoverflow.com/questions/540066/calling-a-function-from-a-string-in-c-sharp

	public void ApplyMessageToTarget(GameObject target, SowizControlMessage message) {
		if (message.descriptor == null || message.descriptor.Length < 1) {
			return;
		}
		string methodStr = "Set" + char.ToUpper(message.descriptor[0]) + message.descriptor.Substring(1);
		Debug.Log ( this.GetType().Name + " calling " + methodStr );
		MethodInfo theMethod = this.GetType().GetMethod(methodStr);

		if (theMethod != null) {
			object [] parameters = new object [] {target, message.values};
			theMethod.Invoke(this, parameters);
		}
	}
		
}
