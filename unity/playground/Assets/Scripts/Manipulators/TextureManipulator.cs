using UnityEngine;
using System.Collections;

public class TextureManipulator : SonosthesiaManipulator {

	public FloatMapping hueMapping = new FloatMapping(0f, 1f);
	public FloatMapping saturationMapping = new FloatMapping(0f, 1f);
	public FloatMapping brightnessMapping = new FloatMapping(0f, 1f);

	public override void Start() {

		targetControlDelegates["hue"] = delegate(GameObject target, ArrayList values) {
			HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
			hsbColor.h = hueMapping.Map((float)(values[0]));
			SetTargetColor(target, hsbColor.ToColor ());
		};

		targetControlDelegates["saturation"] = delegate(GameObject target, ArrayList values) {
			HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
			hsbColor.s = saturationMapping.Map((float)(values[0]));
			SetTargetColor(target, hsbColor.ToColor ());
		};

		targetControlDelegates["brightness"] = delegate(GameObject target, ArrayList values) {
			HSBColor hsbColor = HSBColor.FromColor(GetTargetColor(target));
			hsbColor.b = brightnessMapping.Map((float)(values[0]));
			SetTargetColor(target, hsbColor.ToColor ());
		};
			
	}

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
