
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialManipulator : SonosthesiaManipulator {

	public string colorParameter = "_MainTint";

	public int materialIndex = 0;

	// cache to avoid repeated GetComponent<Renderer> calls
	private Dictionary<int, Renderer> renderers; 

	public Color GetTargetColor (GameObject target) {
		return GetMaterial(target).GetColor (colorParameter); 
	}

	public void SetTargetColor (GameObject target, Color color) {
		GetMaterial(target).SetColor (colorParameter, color); 
	}

	private Material GetMaterial(GameObject target) {
		Renderer renderer = GetRenderer(target);
		if (renderer.materials.Length > materialIndex) {
			return renderer.materials[materialIndex];
		}
		return renderer.material;
	}

	private Renderer GetRenderer(GameObject target) {
		int id = target.GetInstanceID();
		if (renderers.ContainsKey(id)) {
			return renderers[id];
		} 
		Renderer renderer = target.GetComponent<Renderer> ();
		return renderers[id];
	}
		
}
