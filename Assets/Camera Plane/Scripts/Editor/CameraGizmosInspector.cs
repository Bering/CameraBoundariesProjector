using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor (typeof(CameraGizmos))]
public class CameraGizmosInspector : Editor
{
	
	[SerializeField]
	CameraGizmos cameraGizmos;


	override public void OnInspectorGUI ()
	{
		if (cameraGizmos == null) {
			cameraGizmos = (CameraGizmos)target;
		}

		//Rect pivot, pos;
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.ObjectField (cameraGizmos.planeToRaycastAgainst, typeof(GameObject), true);

		foreach (GameViewSizeGroupType thisGroup in System.Enum.GetValues (typeof(GameViewSizeGroupType))) {

			cameraGizmos.unfoldSections [(int)thisGroup] = EditorGUILayout.Foldout (cameraGizmos.unfoldSections [(int)thisGroup], System.Enum.GetName (typeof(GameViewSizeGroupType), thisGroup));

			/*
			pivot = EditorGUILayout.GetControlRect ();
			EditorGUIUtility.RotateAroundPivot (-90, new Vector2 (pivot.x, pivot.y));

			pos = pivot;
			pos.y += 177f;
			pos.x -= 10f;
			EditorGUI.LabelField (pos, "Frustrum");
			pos.y += 23f;
			EditorGUI.LabelField (pos, "Bounds");
			pos.y += 23f;
			EditorGUI.LabelField (pos, "Selected");

			EditorGUIUtility.RotateAroundPivot (90, new Vector2 (pivot.x, pivot.y));
			*/

			if (cameraGizmos.unfoldSections [(int)thisGroup]) {
				
				foreach (CameraGizmos.GameViewSizeOptions gvs in cameraGizmos.allAspects) {

					// I know, I know, I should use a Dictionary instead of a List for that but Unity doesn't serialize Dictionaries and it's a huge pain to work around
					if (gvs.type != thisGroup) {
						continue;
					}

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (gvs.name);
					gvs.showFrustrum = EditorGUILayout.Toggle (gvs.showFrustrum);
					gvs.showBounds = EditorGUILayout.Toggle (gvs.showBounds);
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
