using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Assertions;

[System.Serializable]
[RequireComponent(typeof(Camera))]
public class CameraGizmos : MonoBehaviour
{
	public string displayText;
	public int width;
	public int height;

	[Tooltip("How many points to project along each borders. Between 9 and 99 recommended.")]
	public int projectionQuality;
	public bool drawPoints;
	public bool drawRays;
	public bool drawLines;
	public bool drawFrustrum;
	public bool onlyWhenSelected;
	public Color customColor;

	protected Camera cam;
	protected Vector3[] projectedPoints;


	void Reset ()
	{
		this.displayText = "Camera Gizmos (16:9)";
		this.width = 16;
		this.height = 9;
		this.projectionQuality = 9;
		this.drawPoints = true;
		this.drawRays = false;
		this.drawLines = true;
		this.drawFrustrum = true;
		this.onlyWhenSelected = false;
		this.customColor = Color.white;
	}


	protected Vector3[] computeViewpointPoints(int definition)
	{
		int n = 0;
		Vector3 p = Vector3.zero;
		Vector3[] points = new Vector3[definition * 4];

		if (definition < 1) {
			return points;
		}

		float step = 1f / definition;

		for(n = 0; n < definition; n++) {
			
			// Top
			p.x = n * step;
			p.y = 1;
			points[n] = projectPoint (this.cam.ViewportPointToRay(p));

			// Right
			p.x = 1;
			p.y = 1 - (n * step);
			points[definition + n] = projectPoint (this.cam.ViewportPointToRay(p));

			// Bottom
			p.x = 1 - (n * step);
			p.y = 0;
			points[(definition*2) + n] = projectPoint (this.cam.ViewportPointToRay(p));

			// Left
			p.x = 0;
			p.y = n * step;
			points[(definition*3) + n] = projectPoint (this.cam.ViewportPointToRay(p));


		}

		return points;
	}


	protected Vector3 projectPoint (Ray r)
	{
		RaycastHit closestHit = new RaycastHit();
		closestHit.distance = float.PositiveInfinity;
		closestHit.point = this.cam.transform.position;

		foreach (var hit in Physics.RaycastAll (r)) {

			if (hit.distance < closestHit.distance) {
				closestHit = hit;
			}

		}

		return closestHit.point;
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
		if (this.onlyWhenSelected && !currentlySelected) {
			return;
		}

		if (!this.cam) {
			this.cam = GetComponent<Camera> ();
		}

		if  (!this.cam.gameObject.activeSelf || !this.cam.isActiveAndEnabled) {
			return;
		}

		if (this.cam.aspect != (float)width / (float)height) {
			this.cam.aspect = (float)width / (float)height;
		}

		if (this.drawFrustrum) {
			this.DrawFrustrum ();
		}

		if (currentlySelected || projectedPoints == null || projectedPoints.Length != (projectionQuality*4)) {
			projectedPoints = computeViewpointPoints (projectionQuality);
		}

		this.DrawProjection ();

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


	protected void DrawProjection ()
	{
		int currentPoint, nextPoint, lastPoint = Mathf.CeilToInt(this.projectionQuality * 4) - 1;
		Color prevColor = Gizmos.color;
		Ray r = new Ray();

		for (currentPoint = 0; currentPoint <= lastPoint; currentPoint++) {

			if (this.projectedPoints[currentPoint] == cam.transform.position) {
				continue;
			}

			Gizmos.color = GetColorForPoint(currentPoint);

			if (this.drawPoints) {
				Gizmos.DrawCube (this.projectedPoints[currentPoint], Vector3.one * 0.05f);
			}

			nextPoint = (currentPoint == lastPoint) ? 0 : currentPoint + 1;

			if (this.projectedPoints[nextPoint] == cam.transform.position) {
				continue;
			}

			if (this.drawRays) {
				r.origin = this.projectedPoints[currentPoint];
				r.direction = this.projectedPoints[nextPoint] - r.origin;
				Gizmos.DrawRay (r);
			}

			if (this.drawLines) {
				Gizmos.DrawLine (this.projectedPoints[currentPoint], this.projectedPoints[nextPoint]);
			}

		}

		Gizmos.color = prevColor;
	}


	protected Color GetColorForPoint(int pointIndex)
	{
		if (pointIndex < (projectionQuality*2)) {
			return this.customColor;
		}

		if (pointIndex < (projectionQuality*3)) {
			return Color.red;
		}

		return Color.green;
	}


}
