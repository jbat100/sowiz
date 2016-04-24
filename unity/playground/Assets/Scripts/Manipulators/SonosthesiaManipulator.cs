using UnityEngine;
using System.Collections;


public class SonosthesiaManipulator : MonoBehaviour {

	static private string Tag = "SonosthesiaManipulator";

	public Color GetTargetColor(GameObject target) {
		return Color.white;
	}
		
	public void SetTargetColor(GameObject target, Color color) {
		Debug.Log(Tag + " unimplemented SetTargetColor");
	}

}
