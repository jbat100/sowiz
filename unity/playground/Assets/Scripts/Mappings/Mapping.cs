using UnityEngine;
using System.Collections;


[System.Serializable]
public class ValueMapping : System.Object {

	public enum MappingMode { Linear, Random, Drunk };
	public enum OverflowMode { Unbound, Clamp, Repeat };

	public MappingMode mappingMode;
	public OverflowMode overflowMode;

	public float offset = 0.0f;
	public float scale = 1.0f;

	public float drunkStep = 0.01f;

	private float random = 0.0f;
	private float drunk = 0.5f;
	private float input = 0.0f;

	public void SetInput(float value) {
		input = value;
	}

	public float ApplyOverflow(float value) {

		switch(overflowMode) {
		case OverflowMode.Clamp:
			value = Mathf.Clamp(value, 0.0f, 1.0f);
			break;
		case OverflowMode.Repeat:
			value = Mathf.Repeat(value, 1.0f);
			break;
		}
		return value;

	}

	// ususally called from a mono behaviour's update, with the time delta as argument
	// this is because some mapping generators may be time dependent (like drunk for example)

	public void Update(float time) {

		switch (mappingMode) {
		case MappingMode.Drunk:
			drunk += (drunkStep * (UnityEngine.Random.value > 0.5f ? 1.0f : -1.0f) ) * time;
			drunk = ApplyOverflow(drunk);
			break;
		case MappingMode.Random:
			random = UnityEngine.Random.value;
			break;
		default:
			break;
		}

	}

	// if we are using a random / drunk modes (for example) and want the corresponding randomly generated
	// values to be locked accross multiple mappings then call Update on one of them, then call SyncTo 
	// on the others giving the first as argument

	public void SyncTo(ValueMapping other) {
		drunk = other.drunk;
		random = other.random;
	}

	public float GetMapped() {

		float value = 0.0f;

		switch(mappingMode) {
		case MappingMode.Drunk:
			value = drunk;
			break;
		case MappingMode.Random:
			value = random;
			break;
		case MappingMode.Linear:
			value = input;
			break;
		default:
			break;
		}

		return ApplyOverflow((value*scale)+offset);

	}

};



[System.Serializable]
public class SonosthesiaFloatMapping : System.Object
{
	public float Zero = 0f;
	public float Unit = 1f;

	public SonosthesiaFloatMapping(float _zero, float _unit) {
		Zero = _zero;
		Unit = _unit;
	}

	public float Map(float val) {
		return (val * (Unit - Zero)) + Zero;
	}
}

[System.Serializable]
public class SonosthesiaVector3Mapping : System.Object
{
	public Vector3 Zero = new Vector3(0f, 0f, 0f);
	public Vector3 Unit = new Vector3(0f, 0f, 1f);

	public SonosthesiaVector3Mapping(Vector3 _zero, Vector3 _unit) {
		Zero = _zero;
		Unit = _unit;
	}

	public Vector3 Map(float val) {
		return ((1f - val) * Zero) + (val * Unit);
	}
}

[System.Serializable]
public class SonosthesiaRotator : System.Object
{
	public Vector3 Axis = new Vector3(1f, 0f, 0f);
	public float Scale = 180f;

	public SonosthesiaRotator(Vector3 _axis, float _scale) {
		Axis = _axis;
		Scale = _scale;
	}

	public Quaternion GetRotation(float val) {
		return Quaternion.AngleAxis ( Scale * val, Axis );
	}
}

[System.Serializable]
public class SonosthesiaSpinner : System.Object
{
	public Vector3 Axis = new Vector3(1f, 0f, 0f);
	public float Scale = 180f;
	public float Spin = 0f;

	public SonosthesiaSpinner(Vector3 _axis, float _scale) {
		Axis = _axis;
		Scale = _scale;
	}

	public Quaternion GetRotation() {
		return Quaternion.AngleAxis ((float)(Scale * Spin * Time.deltaTime * 60f), Axis);
	}
}