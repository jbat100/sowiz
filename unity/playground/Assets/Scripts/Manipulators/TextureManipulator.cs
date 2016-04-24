using UnityEngine;
using System.Collections;

public class TextureManipulator : SonosthesiaManipulator {

	// Use this for initialization
	public Color GetTargetColor (GameObject target) {
		Renderer renderer = target.GetComponent<Renderer> ();
		//return renderer.material.color;
		return renderer.material.GetColor ("_MainTint"); 
	}
	
	// Update is called once per frame
	public void SetTargetColor (GameObject target, Color color) {
		Renderer renderer = target.GetComponent<Renderer> ();
		//renderer.material.color = color;
		renderer.material.SetColor ("_MainTint", color); 
	}
		
}
