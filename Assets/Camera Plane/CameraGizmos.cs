using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class CameraGizmos : MonoBehaviour
{

	[System.Serializable]
	public class GameViewSizeOptions
	{
		public string name;
		public int width;
		public int height;
		public bool showFrustrum;
		public bool showProjection;
		public bool onlyWhenSelected;
	}


	public GameObject planeToRaycastAgainst;
	public Dictionary<GameViewSizeGroupType,GameViewSizeOptions[]> allAspects;
	public bool[] showSections;

	Camera cam;
	Vector3 topLeft;
	Vector3 topRight;
	Vector3 bottomRight;
	Vector3 bottomLeft;

	Vector3 topLeftViewportPoint = new Vector3 (0, 1);
	Vector3 topRightViewportPoint = new Vector3 (1, 1);
	Vector3 bottomRightViewportPoint = new Vector3 (1, 0);
	Vector3 bottomLeftViewportPoint = new Vector3 (0, 0);


	void Awake ()
	{
		InitIfRequired ();
	}


	protected void InitIfRequired ()
	{
		if (cam == null) {
			cam = GetComponent<Camera> ();
		}

		if (allAspects == null) {
			this.BuildDictionaryOfGameViewSizes ();
			showSections = new bool[allAspects.Count];
		}
	}


	protected void BuildDictionaryOfGameViewSizes ()
	{
		allAspects = new Dictionary<GameViewSizeGroupType, GameViewSizeOptions[]> ();

		foreach (GameViewSizeGroupType t in System.Enum.GetValues (typeof(GameViewSizeGroupType))) {
			GameViewUtils.GameViewSize[] sizes = GameViewUtils.GetGroupSizes (t);
			int count = sizes.Length;

			GameViewSizeOptions[] options = new GameViewSizeOptions [count];
			GameViewSizeOptions option = null;

			for (int n = 0; n < count; n++) {
				option = new GameViewSizeOptions ();

				option.name = sizes [n].displayText;
				option.width = sizes [n].width;
				option.height = sizes [n].height;

				options [n] = option;

			}

			allAspects.Add (t, options);
		}
	}


	void OnDrawGizmos ()
	{
		InitIfRequired ();
		DrawAllGizmos (false);
	}


	void OnDrawGizmosSelected ()
	{
		InitIfRequired ();
		DrawAllGizmos (true);
	}


	protected void DrawAllGizmos (bool currentlySelected)
	{
		foreach (GameViewSizeGroupType t in System.Enum.GetValues (typeof(GameViewSizeGroupType))) {
			GameViewSizeOptions[] options = allAspects [t];
			int count = options.Length;

			for (int n = 0; n < count; n++) {
				GameViewSizeOptions o = options [n];

				if (o.onlyWhenSelected && !currentlySelected) {
					continue;
				}

				if (o.showFrustrum) {
					cam.aspect = (float)o.width / (float)o.height;
					DrawFrustrum ();
				}

				if (o.showProjection && planeToRaycastAgainst != null) {
					cam.aspect = (float)o.width / (float)o.height;
					DrawProjection ();
				}
			}

		}

		cam.ResetAspect ();
	}


	protected void DrawFrustrum ()
	{
		// DrawFrustrum is bugged, these shennanigans make it work as expected
		Matrix4x4 temp = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS (cam.transform.position, cam.transform.rotation, Vector3.one);
		Gizmos.DrawFrustum (Vector3.zero, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
		Gizmos.matrix = temp;
	}


	protected void DrawProjection ()
	{
		Color prevColor = Gizmos.color;

		topLeft = GetPlaneIntersection (cam.ViewportPointToRay (topLeftViewportPoint));
		topRight = GetPlaneIntersection (cam.ViewportPointToRay (topRightViewportPoint));
		bottomRight = GetPlaneIntersection (cam.ViewportPointToRay (bottomRightViewportPoint));
		bottomLeft = GetPlaneIntersection (cam.ViewportPointToRay (bottomLeftViewportPoint));

		Gizmos.color = Color.white;
		Gizmos.DrawLine (topLeft, topRight);
		Gizmos.DrawLine (topRight, bottomRight);
		Gizmos.color = Color.red;
		Gizmos.DrawLine (bottomRight, bottomLeft);
		Gizmos.color = Color.green;
		Gizmos.DrawLine (bottomLeft, topLeft);

		Gizmos.color = prevColor;
	}


	protected Vector3 GetPlaneIntersection (Ray r)
	{
		//Gizmos.color = Color.blue;
		//Gizmos.DrawRay (r);

		foreach (var hit in Physics.RaycastAll (r)) {
			if (hit.collider.gameObject == this.planeToRaycastAgainst) {
				return hit.point;
			}
		}
			
		return cam.transform.position;
	}

}
