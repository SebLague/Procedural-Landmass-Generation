using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		UpdatableData data = (UpdatableData)target;

		if (GUILayout.Button ("Update")) {
			data.NotifyOfUpdatedValues ();
			EditorUtility.SetDirty (target);
		}
	}
	
}
