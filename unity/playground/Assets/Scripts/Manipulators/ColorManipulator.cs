using UnityEngine;
using System.Collections;

public class ColorManipulator : SowizManipulator {

	// Use this for initialization
	public Color GetColor () {

		Renderer renderer = target.GetComponent<Renderer> ();

		return renderer.material.GetColor ("_MainTint"); 
	
	}
	
	// Update is called once per frame
	public void SetColor (Color color) {

		Renderer renderer = target.GetComponent<Renderer> ();

		renderer.material.SetColor ("_MainTint", color); 
	
	}
}
