using UnityEngine;
using System.Collections;

public class ScaleController : SonosthesiaController {

	public Vector3Mapping scaleMapping = new Vector3Mapping(new Vector3(0.5f, 1f, 1f), new Vector3(5f, 1f, 1f));

	// Use this for initialization
	public override void Start () {

		base.Start();

		targetControlDelegates["scale"] = delegate(GameObject target, ArrayList values) {
			Vector3 newScale = scaleMapping.Map((float)(values[0]));
			Debug.Log ("TransformManipulator setting scale " + newScale.ToString() );
			IScaleManipulator manipulator = (IScaleManipulator)GetManipulator(target, typeof(IScaleManipulator));
			manipulator.SetScale(target, newScale);
		};
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
