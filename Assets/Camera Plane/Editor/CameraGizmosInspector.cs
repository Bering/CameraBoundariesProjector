using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor (typeof(CameraGizmos))]
public class CameraGizmosInspector : Editor
{
	
	CameraGizmos cameraGizmos = null;


	override public void OnInspectorGUI ()
	{
		if (cameraGizmos == null) {
			cameraGizmos = (CameraGizmos)target;
		}

		CameraGizmos.GameViewSizeOptions gvs = null;
		Rect pivot, pos;
		int n, max;

		EditorGUILayout.BeginVertical ();
		EditorGUILayout.ObjectField (cameraGizmos.planeToRaycastAgainst, typeof(GameObject), true);

		foreach (GameViewSizeGroupType thisGroup in System.Enum.GetValues (typeof(GameViewSizeGroupType))) {

			cameraGizmos.unfoldSections [(int)thisGroup] = EditorGUILayout.Foldout (cameraGizmos.unfoldSections [(int)thisGroup], System.Enum.GetName (typeof(GameViewSizeGroupType), thisGroup));

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

			if (cameraGizmos.unfoldSections [(int)thisGroup]) {
				CameraGizmos.GameViewSizeOptions[] options = cameraGizmos.allAspects [(GameViewSizeGroupType)thisGroup];
				max = options.Length;

				for (n = 0; n < max; n++) {
					gvs = options [n];

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (gvs.name);
					gvs.showFrustrum = EditorGUILayout.Toggle (gvs.showFrustrum);
					gvs.showProjection = EditorGUILayout.Toggle (gvs.showProjection);
					gvs.onlyWhenSelected = EditorGUILayout.Toggle (gvs.onlyWhenSelected);
					EditorGUILayout.EndHorizontal ();
				}

			}

		}

		EditorGUILayout.EndVertical ();

		if (GUI.changed) {
			EditorUtility.SetDirty (target);
		}
	}

}
