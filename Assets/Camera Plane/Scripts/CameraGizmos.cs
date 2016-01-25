﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class CameraGizmos : MonoBehaviour
{

	[System.Serializable]
	public class GameViewSizeOptions
	{
		public GameViewSizeGroupType type;
		public string name;
		public int width;
		public int height;
		public bool showFrustrum;
		public bool showBounds;
		public bool onlyWhenSelected;
	}


	public GameObject planeToRaycastAgainst;
	public List<GameViewSizeOptions> allAspects;
	public bool[] unfoldSections;

	[SerializeField]
	Camera cam;

	Vector3 topLeft;
	Vector3 topRight;
	Vector3 bottomRight;
	Vector3 bottomLeft;

	Vector3 topLeftViewportPoint = new Vector3 (0, 1);
	Vector3 topRightViewportPoint = new Vector3 (1, 1);
	Vector3 bottomRightViewportPoint = new Vector3 (1, 0);
	Vector3 bottomLeftViewportPoint = new Vector3 (0, 0);


	void Reset ()
	{
		this.BuildListOfGameViewSizes ();
		this.unfoldSections = new bool[System.Enum.GetValues (typeof(GameViewSizeGroupType)).Length];
		this.cam = GetComponent<Camera> ();
	}


	[ContextMenu ("Refresh list of aspect ratios")]
	protected void BuildListOfGameViewSizes ()
	{
		this.allAspects = new List<GameViewSizeOptions> ();
		GameViewSizeOptions option = null;

		foreach (GameViewSizeGroupType thisGroup in System.Enum.GetValues (typeof(GameViewSizeGroupType))) {
			foreach (GameViewUtils.GameViewSize thisSize in GameViewUtils.GetGroupSizes (thisGroup)) {

				option = new GameViewSizeOptions ();

				option.type = thisGroup;
				option.name = thisSize.displayText;
				option.width = thisSize.width;
				option.height = thisSize.height;

				this.allAspects.Add (option);
			}
		}

	}


	void OnDrawGizmos ()
	{
		DrawAllGizmos (false);
	}


	void OnDrawGizmosSelected ()
	{
		DrawAllGizmos (true);
	}


	protected void DrawAllGizmos (bool currentlySelected)
	{
		foreach (GameViewSizeOptions o in this.allAspects) {
			
			if (o.onlyWhenSelected && !currentlySelected) {
				continue;
			}

			if (o.showFrustrum) {
				this.cam.aspect = (float)o.width / (float)o.height;
				this.DrawFrustrum ();
			}

			if (o.showBounds && this.planeToRaycastAgainst != null) {

				// special "Free Aspect" option has height = 0. So use the current aspect instead
				if (o.height == 0) {
					this.cam.ResetAspect ();
				} else {
					this.cam.aspect = (float)o.width / (float)o.height;
				}

				this.DrawBounds ();
			}

		}

		this.cam.ResetAspect ();
	}


	protected void DrawFrustrum ()
	{
		// DrawFrustrum is bugged, these shennanigans make it work as expected
		Matrix4x4 temp = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS (this.cam.transform.position, this.cam.transform.rotation, Vector3.one);
		Gizmos.DrawFrustum (Vector3.zero, this.cam.fieldOfView, this.cam.farClipPlane, this.cam.nearClipPlane, this.cam.aspect);
		Gizmos.matrix = temp;
	}


	protected void DrawBounds ()
	{
		Color prevColor = Gizmos.color;

		this.topLeft = GetPlaneIntersection (this.cam.ViewportPointToRay (this.topLeftViewportPoint));
		this.topRight = GetPlaneIntersection (this.cam.ViewportPointToRay (this.topRightViewportPoint));
		this.bottomRight = GetPlaneIntersection (this.cam.ViewportPointToRay (this.bottomRightViewportPoint));
		this.bottomLeft = GetPlaneIntersection (this.cam.ViewportPointToRay (this.bottomLeftViewportPoint));

		Gizmos.color = Color.white;
		Gizmos.DrawLine (this.topLeft, this.topRight);
		Gizmos.DrawLine (this.topRight, this.bottomRight);
		Gizmos.color = Color.red;
		Gizmos.DrawLine (this.bottomRight, this.bottomLeft);
		Gizmos.color = Color.green;
		Gizmos.DrawLine (this.bottomLeft, this.topLeft);

		Gizmos.color = prevColor;
	}


	protected Vector3 GetPlaneIntersection (Ray r)
	{
		RaycastHit closestHit = new RaycastHit();
		closestHit.distance = float.PositiveInfinity;
		closestHit.point = this.cam.transform.position;
		foreach (var hit in Physics.RaycastAll (r)) {
			
			if (hit.distance < closestHit.distance) {
			}
		}

		return closestHit.point;
	}

}
