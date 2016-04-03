using UnityEngine;
using System.Collections;

public class MidiTrigger : MonoBehaviour {

	public delegate void MidiNoteOnDelegate(int channel, int pich, int velocity);
	public delegate void MidiNoteOffDelegate(int channel, int pich, int velocity);

	public MidiNoteOnDelegate noteOnDelegate;
	public MidiNoteOffDelegate noteOffDelegate;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
}
