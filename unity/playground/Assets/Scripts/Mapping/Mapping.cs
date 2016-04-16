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
	private float linear = 0.0f;
	private float input = 0.0f;

	public float SetInput(float value) {
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
	};

	public float GetMapped() {

		float value = 0.0f;

		switch(mappingMode) {
		case MappingMode.Drunk:
			value = drunk;
		case MappingMode.Random:
			value = random;
		case MappingMode.Linear:
			value = input;
		default:
			break;
		}

		return ApplyOverflow((value*scale)+offset);

	}

};