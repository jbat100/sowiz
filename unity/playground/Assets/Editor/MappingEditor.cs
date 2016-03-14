using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(SowizManipulator))]
public class MappingEditor : Editor {
		
		public override void OnInspectorGUI() {
				SowizManipulator manipulator = (SowizManipulator)target;
		}
	
}
