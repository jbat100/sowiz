using UnityEngine;
using System.Collections;

// todo have policies to decide wich prefab to use

public class MidiNoteObject : MonoBehaviour {

	GameObject[] Prefabs;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject GetPrefab(int channel, int pitch, int velocity) {

		// default policy: get the first prefab in the list

		if (Prefabs.Length > 0) {
			return Prefabs[0];
		}

		return null;
	}
}
