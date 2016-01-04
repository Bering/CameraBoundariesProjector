﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class CameraGizmos : MonoBehaviour
{

	[System.Serializable]
	public class GameViewSizeEditorOptions : GameViewUtils.GameViewSize
	{
		public bool showFrustrum;
		public bool showProjection;
		public bool onlyWhenSelected;

		public GameViewSizeEditorOptions (GameViewUtils.GameViewSize gvs)
		{
			this.baseText = gvs.baseText;
			this.displayText = gvs.displayText;
			this.sizeType = gvs.sizeType;
			this.width = gvs.width;
			this.height = gvs.height;
		}
	}


	public GameObject planeToRaycastAgainst;
	public Dictionary<GameViewSizeGroupType,GameViewSizeEditorOptions[]> allAspects;
	public bool[] showSections;

	Camera c;
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
		if (c == null) {
			c = GetComponent<Camera> ();
		}

		if (allAspects == null) {
			this.BuildDictionaryOfGameViewSizes ();
			showSections = new bool[allAspects.Count];
		}
	}


	protected void BuildDictionaryOfGameViewSizes ()
	{
		allAspects = new Dictionary<GameViewSizeGroupType, GameViewSizeEditorOptions[]> ();

		foreach (GameViewSizeGroupType t in System.Enum.GetValues (typeof(GameViewSizeGroupType))) {
			GameViewUtils.GameViewSize[] sizes = GameViewUtils.GetGroupSizes (t);
			int count = sizes.Length;

			GameViewSizeEditorOptions[] options = new GameViewSizeEditorOptions [count];
			for (int n = 0; n < count; n++) {
				options [n] = new GameViewSizeEditorOptions (sizes [n]);
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
			GameViewSizeEditorOptions[] options = allAspects [t];
			int count = options.Length;

			for (int n = 0; n < count; n++) {
				GameViewSizeEditorOptions o = options [n];

				if (o.onlyWhenSelected && !currentlySelected) {
					continue;
				}

				if (o.showFrustrum) {
					c.aspect = (float)o.width / (float)o.height;
					DrawFrustrum ();
				}

				if (o.showProjection && planeToRaycastAgainst != null) {
					c.aspect = (float)o.width / (float)o.height;
					DrawProjection ();
				}
			}

		}

		c.ResetAspect ();
	}


	protected void DrawFrustrum ()
	{
		// DrawFrustrum is bugged, these shennanigans make it work as expected
		Matrix4x4 temp = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS (c.transform.position, c.transform.rotation, Vector3.one);
		Gizmos.DrawFrustum (Vector3.zero, c.fieldOfView, c.farClipPlane, c.nearClipPlane, c.aspect);
		Gizmos.matrix = temp;
	}


	protected void DrawProjection ()
	{
		Color prevColor = Gizmos.color;

		topLeft = GetPlaneIntersection (c.ViewportPointToRay (topLeftViewportPoint));
		topRight = GetPlaneIntersection (c.ViewportPointToRay (topRightViewportPoint));
		bottomRight = GetPlaneIntersection (c.ViewportPointToRay (bottomRightViewportPoint));
		bottomLeft = GetPlaneIntersection (c.ViewportPointToRay (bottomLeftViewportPoint));

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

		return Vector3.zero;
	}

}