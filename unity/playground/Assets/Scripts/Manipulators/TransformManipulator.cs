using UnityEngine;
using System.Collections;


public class TransformManipulator : SonosthesiaManipulator {

	public void SetPosition(GameObject target, Vector3 position) {
		target.transform.localPosition = position;
	}

	public void SetRotation(GameObject target, Quaternion rotation) {
		target.transform.localRotation = rotation;
	}

	public void SetScale(GameObject target, Vector3 scale) {
		target.transform.localScale = scale;
	}

}
