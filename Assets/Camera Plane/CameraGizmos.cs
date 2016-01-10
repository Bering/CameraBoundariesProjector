using UnityEngine;
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
		public bool showProjection;
		public bool onlyWhenSelected;
	}


	public GameObject planeToRaycastAgainst;
	public List<GameViewSizeOptions> allAspects;
	public bool[] showSections;

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
		this.showSections = new bool[this.allAspects.Count];
		this.cam = GetComponent<Camera> ();
	}


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

			if (o.showProjection && this.planeToRaycastAgainst != null) {

				if (o.showFrustrum) {
					cam.aspect = (float)o.width / (float)o.height;
					DrawFrustrum ();
				}

				this.DrawProjection ();
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
