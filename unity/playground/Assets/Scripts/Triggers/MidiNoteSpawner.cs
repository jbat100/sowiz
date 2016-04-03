using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MidiNoteSpawnerElement : System.Object
{
	// TODO: have an option to spawn on spline or within volume instead of fixed StartTransform

	public Transform StartTransform;
	public GameObject Prefab;
	public int Pitch; // -1 means default

	public void Spawn(int channel, int pitch, int velocity) {
		
	}

	public void Unspawn(int channel, int pitch, int velocity) {

	}
}

public class MidiNoteSpawner : MidiTrigger {

	public MidiNoteSpawnerElement[] Elements;

	public int[] channels;

	// apparently having a list (for inspector and serialization purposes), then building a dict for fast lookup 
	// is sensible: http://answers.unity3d.com/questions/642431/dictionary-in-inspector.html

	private Dictionary<int, MidiNoteSpawnerElement> ElementsByPitch;

	// Use this for initialization
	void Start () {

		ElementsByPitch = new Dictionary<int, MidiNoteSpawnerElement>();

		foreach (MidiNoteSpawnerElement element in Elements) {
			
		}

		noteOnDelegate = delegate(int channel, int pitch, int velocity) {
			MidiNoteSpawnerElement element = null;
			if (ElementsByPitch.TryGetValue(pitch, out element))
			{
				element.Spawn(channel, pitch, velocity);
			}
		};
	
	}

}
