using UnityEngine;
using System.Collections;

public class ColorHueManipulator : SowizManipulator {

	public int zeroHue = 0;
	public int unitHue = 128;
	
	void Awake() {
		descriptors = new string[] {"magnitude"};	
	}
	
	public override void ApplyMessage(SowizControlMessage message) {
		
		switch (message.descriptor) {
			
		case "magnitude":
			SetMagnitude ((float) message.values[0]);
			break;
		default:
			break;
			
		}
		
	}
	
	private void SetMagnitude(float m) {
		
		Debug.Log ("Setting magnitude to " + m.ToString ());

		Renderer renderer = target.GetComponent<Renderer> ();

		Color currentColor = renderer.material.GetColor ("_MainTint");

		Debug.Log ("Current color is " + currentColor.ToString ());
	}

}
