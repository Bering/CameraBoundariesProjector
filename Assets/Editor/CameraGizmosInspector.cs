using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor (typeof(CameraGizmos))]
public class CameraGizmosInspector : Editor
{
	
	CameraGizmos cameraGizmos = null;


	override public void OnInspectorGUI ()
	{
		serializedObject.Update ();

		if (cameraGizmos == null) {
			//	cameraGizmos = (CameraGizmos)(serializedObject.targetObject);
			cameraGizmos = (CameraGizmos)target;
		}

		CameraGizmos.GameViewSizeEditorOptions gvs = null;
		Rect pivot, pos;
		int n, max;

		EditorGUILayout.BeginVertical ();
		EditorGUILayout.ObjectField (cameraGizmos.planeToRaycastAgainst, typeof(GameObject), true);

		foreach (GameViewSizeGroupType group in System.Enum.GetValues (typeof(GameViewSizeGroupType))) {

			cameraGizmos.showSections [(int)group] = EditorGUILayout.Foldout (cameraGizmos.showSections [(int)group], System.Enum.GetName (typeof(GameViewSizeGroupType), group));

			pivot = EditorGUILayout.GetControlRect ();
			EditorGUIUtility.RotateAroundPivot (-90, new Vector2 (pivot.x, pivot.y));

			pos = pivot;
			pos.y += 177f;
			pos.x -= 10f;
			EditorGUI.LabelField (pos, "Frustrum");
			pos.y += 23f;
			EditorGUI.LabelField (pos, "Projection");
			pos.y += 23f;
			EditorGUI.LabelField (pos, "Selected");

			EditorGUIUtility.RotateAroundPivot (90, new Vector2 (pivot.x, pivot.y));

			if (cameraGizmos.showSections [(int)group]) {
				
				CameraGizmos.GameViewSizeEditorOptions[] options = cameraGizmos.allAspects [(GameViewSizeGroupType)group];
				max = options.Length;

				for (n = 0; n < max; n++) {
					gvs = options [n];

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (gvs.displayText);
					gvs.showFrustrum = EditorGUILayout.Toggle (gvs.showFrustrum);
					gvs.showProjection = EditorGUILayout.Toggle (gvs.showProjection);
					gvs.onlyWhenSelected = EditorGUILayout.Toggle (gvs.onlyWhenSelected);
					EditorGUILayout.EndHorizontal ();
				}

			}

		}

		EditorGUILayout.EndVertical ();

		serializedObject.ApplyModifiedProperties ();
		DrawDefaultInspector ();
	}

}
