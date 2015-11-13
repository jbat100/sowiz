using UnityEngine;
using System.Collections;

public class ScaleManipulator : SowizManipulator {

	public Vector3 zeroScale = new Vector3(0.0, 0.0, 0.0);
	public Vector3 unitScale = new Vector3(1.0, 1.0, 1.0);

	protected string[] descriptor = ["scale"];

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
