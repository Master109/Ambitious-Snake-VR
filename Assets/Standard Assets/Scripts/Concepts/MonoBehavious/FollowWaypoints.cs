using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class FollowWaypoints : UpdateWhileEnabled
	{
		public Transform trs;
		public MoveType moveType;
		public Transform waypointsParent;
		public List<Waypoint> waypoints = new List<Waypoint>();
		public bool autoSetWaypoints = true;
		public float moveSpeed;
		public float rotateSpeed;
		public int currentWaypoint;
		public bool backTracking;
		public WaypointPath path;
		public Transform rotationViewerPrefab;
		public List<LineRenderer> lineRenderers = new List<LineRenderer>(2);
		
#if UNITY_EDITOR
		void OnValidate ()
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			if (autoSetWaypoints)
			{
				if (waypointsParent != null)
				{
					Waypoint[] _waypoints = new Waypoint[waypointsParent.childCount];
					for (int i = 0; i < waypointsParent.childCount; i ++)
					{
						Transform child = waypointsParent.GetChild(i);
						_waypoints[i] = new Waypoint(child);
					}
					waypoints = new List<Waypoint>(_waypoints);
				}
				else
					waypointsParent = trs;
			}
			if (moveSpeed != 0)
			{
				Bounds bounds = GetBoundsOfChildren();
				lineRenderers = new List<LineRenderer>(GetComponentsInChildren<LineRenderer>());
				if (lineRenderers.Count == 0)
				{
					LineRenderer lineRenderer1 = AddLineRenderer();
					System.Threading.Thread.Sleep(0);
					LineRenderer lineRenderer2 = AddLineRenderer();
					SetLineRenderersToBoundsSides (bounds, lineRenderer1, lineRenderer2);
					// lineRenderer.positionCount = waypoints.Count + 1;
					// lineRenderer.SetPosition(0, trs.position);
					// if (moveType == MoveType.Loop)
					// {
					// 	int counter = 1;
					// 	int waypointIndex = currentWaypoint;
					// 	while (true)
					// 	{
					// 		lineRenderer.SetPosition(counter, waypoints[waypointIndex].trs.position);
					// 		if (backTracking)
					// 		{
					// 			waypointIndex --;
					// 			if (waypointIndex == -1)
					// 				waypointIndex = waypoints.Count - 1;
					// 		}
					// 		else
					// 		{
					// 			waypointIndex ++;
					// 			if (waypointIndex == waypoints.Count)
					// 				waypointIndex = 0;
					// 		}
					// 		if (waypointIndex == currentWaypoint)
					// 			break;
					// 		counter ++;
					// 	}
					// }
					// else
					// {
					// 	for (int i = 0; i < waypoints.Count; i ++)
					// 		lineRenderer.SetPosition(i + 1, waypoints[i].trs.position);
					// }
					// lineRenderer.material = path.material;
					// lineRenderer.startColor = path.color;
					// lineRenderer.endColor = path.color;
					// lineRenderer.startWidth = path.width;
					// lineRenderer.endWidth = path.width;
					// lineRenderer.sortingLayerName = path.sortingLayerName;
					// lineRenderer.sortingOrder = Mathf.Clamp(path.sortingOrder, -32768, 32767);
				}
			}
			if (rotateSpeed != 0)
			{
				for (int i = 0; i < waypoints.Count; i ++)
				{
					Waypoint waypoint = waypoints[i];
					Waypoint nextWaypoint = default(Waypoint);
					if ((i == waypoints.Count - 1 && !backTracking) || (i == 0 && backTracking))
					{
						switch (moveType)
						{
							case MoveType.Once:
								return;
							case MoveType.Loop:
								if (!backTracking)
									nextWaypoint = waypoints[0];
								else
									nextWaypoint = waypoints[waypoints.Count - 1];
								break;
							case MoveType.PingPong:
								if (!backTracking)
									nextWaypoint = waypoints[i - 1];
								else
									nextWaypoint = waypoints[i + 1];
								break;
						}
					}
					else
					{
						if (!backTracking)
							nextWaypoint = waypoints[i + 1];
						else
							nextWaypoint = waypoints[i - 1];
					}
					if (waypoint.trs.eulerAngles != nextWaypoint.trs.eulerAngles)
					{
						Transform rotationViewer = (Transform) Instantiate(rotationViewerPrefab);
						// List<Bounds> boundsInstances = new List<Bounds>();
						// foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
						// 	boundsInstances.Add(renderer.bounds);
						// Bounds rotationBounds = BoundsExtensions.Combine(boundsInstances.ToArray());
						// rotationViewer.position = waypoint.position;
						// float greatestRadius = Mathf.Max(rotationBounds.extents.x, rotationBounds.extents.y);
						// foreach (Bounds bounds in boundsInstances)
						// {
						// 	Vector2 furthestPoint = bounds.min;
						// 	float currentRadius = Vector2.Distance(rotationBounds.center, furthestPoint);
						// 	if (currentRadius > greatestRadius)
						// 		greatestRadius = currentRadius;
						// 	furthestPoint = bounds.max;
						// 	currentRadius = Vector2.Distance(rotationBounds.center, furthestPoint);
						// 	if (currentRadius > greatestRadius)
						// 		greatestRadius = currentRadius;
						// 	furthestPoint = new Vector2(bounds.min.x, bounds.max.y);
						// 	currentRadius = Vector2.Distance(rotationBounds.center, furthestPoint);
						// 	if (currentRadius > greatestRadius)
						// 		greatestRadius = currentRadius;
						// 	furthestPoint = new Vector2(bounds.max.x, bounds.min.y);
						// 	currentRadius = Vector2.Distance(rotationBounds.center, furthestPoint);
						// 	if (currentRadius > greatestRadius)
						// 		greatestRadius = currentRadius;
						// }
						// rotationViewer.localScale = Vector2.one * ((greatestRadius + Vector2.Distance(rotationBounds.center, transform.position)) * 2);
						// rotationViewer.eulerAngles = waypoint.eulerAngles;
						// rotationViewer.GetComponent<Image>().color = path.color;
						// float rotaAmount = Mathf.Abs(Mathf.DeltaAngle(waypoint.eulerAngles.z, nextWaypoint.eulerAngles.z));
						// if (MathfExtensions.RotationDirectionToAngle(waypoint.eulerAngles.z, nextWaypoint.eulerAngles.z) > 0 == backTracking)
						// 	rotationViewer.GetComponent<Image>().fillClockwise = false;
						// rotationViewer.GetComponent<Image>().fillAmount = 1f / (360f / rotaAmount);
					}
				}
			}
		}
#endif

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				// Bounds bounds = new Bounds(Vector3.one / 2, Vector3.one);
				// Vector3[] corners = bounds.GetCorners();
				// for (int i = 0; i < corners.Length; i ++)
				// {
				// 	Vector3 corner = corners[i];
				// 	GameObject newGo = new GameObject();
				// 	newGo.name = "corner" + i;
				// 	Transform newTrs = newGo.GetComponent<Transform>();
				// 	newTrs.position = corner;
				// }
				// LineSegment3D[] sides = bounds.GetSides();
				// foreach (IList permutation in sides.GetPermutations())
				// {
				// 	bool isWindingSequence = true;
				// 	LineSegment3D previousSide = (LineSegment3D) permutation[permutation.Count - 1];
				// 	for (int i = 0; i < permutation.Count; i ++)
				// 	{
				// 		LineSegment3D side = (LineSegment3D) permutation[i];
				// 		if (side.start == previousSide.end)
				// 		{
				// 		}
				// 		else
				// 		{
				// 			side = side.Flip();
				// 			if (side.start == previousSide.end)
				// 			{
				// 			}
				// 			else
				// 			{
				// 				isWindingSequence = false;
				// 				break;
				// 			}
				// 		}
				// 		previousSide = side;
				// 	}
				// 	if (isWindingSequence)
				// 	{
				// 		for (int i = 0; i < permutation.Count; i ++)
				// 		{
				// 			LineSegment3D side = (LineSegment3D) permutation[i];
				// 			print(side);
				// 		}
				// 		return;
				// 	}
				// }
				return;
			}
#endif
			base.OnEnable ();
			for (int i = 0; i < waypoints.Count; i ++)
			{
				Waypoint waypoint = waypoints[i];
				waypoint.trs.SetParent(null);
			}
		}

		LineRenderer AddLineRenderer ()
		{
			GameObject go = gameObject;
			if (GetComponent<LineRenderer>() != null)
			{
				go = new GameObject();
				Transform goTrs = go.GetComponent<Transform>();
				goTrs.SetParent(trs);
			}
			return AddLineRenderer(go);
		}

		LineRenderer AddLineRenderer (GameObject go)
		{
			LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
			lineRenderers.Add(lineRenderer);
			lineRenderer.material = path.material;
			lineRenderer.startColor = path.color;
			lineRenderer.endColor = path.color;
			lineRenderer.startWidth = path.width;
			lineRenderer.endWidth = path.width;
			lineRenderer.sortingLayerName = path.sortingLayerName;
			lineRenderer.sortingOrder = Mathf.Clamp(path.sortingOrder, -32768, 32767);
			return lineRenderer;
		}
		
		Bounds GetBoundsOfChildren ()
		{
			// Renderer[] renderers = GetComponentsInChildren<Renderer>();
			// Bounds[] childBoundsArray = new Bounds[renderers.Length];
			// for (int i = 0; i < renderers.Length; i ++)
			// {
			// 	Renderer renderer = renderers[i];
			// 	childBoundsArray[i] = renderer.GetComponent<Transform>().GetBounds();
			// }
			Collider[] colliders = GetComponentsInChildren<Collider>();
			Bounds[] childBoundsArray = new Bounds[colliders.Length];
			for (int i = 0; i < colliders.Length; i ++)
			{
				Collider collider = colliders[i];
				childBoundsArray[i] = collider.GetComponent<Transform>().GetBounds();
			}
			return childBoundsArray.Combine();
		}

		void SetLineRenderersToBoundsSides (Bounds bounds, LineRenderer lineRenderer1, LineRenderer lineRenderer2)
		{
			Vector3[] corners = bounds.GetCorners();
			Vector3 corner0 = corners[0];
			Vector3 corner1 = corners[1];
			Vector3 corner2 = corners[2];
			Vector3 corner3 = corners[3];
			Vector3 corner4 = corners[4];
			Vector3 corner5 = corners[5];
			Vector3 corner6 = corners[6];
			Vector3 corner7 = corners[7];
			lineRenderer1.positionCount = 7;
			lineRenderer1.SetPositions(new Vector3[7] { corner0, corner6, corner5, corner4, corner0, corner1, corner2 });
			lineRenderer2.positionCount = 9;
			lineRenderer2.SetPositions(new Vector3[9] { corner4, corner2, corner3, corner7, corner6, corner5, corner3, corner7, corner1 });
		}
		
		public override void DoUpdate ()
		{
			if (GameManager.paused || _SceneManager.isLoading)
				return;
			if (moveSpeed != 0)
				trs.position = Vector3.Lerp(trs.position, waypoints[currentWaypoint].trs.position, moveSpeed * Time.deltaTime * (1f / Vector2.Distance(trs.position, waypoints[currentWaypoint].trs.position)));
			if (rotateSpeed != 0)
				trs.rotation = Quaternion.Slerp(trs.rotation, waypoints[currentWaypoint].trs.rotation, rotateSpeed * Time.deltaTime * (1f / Quaternion.Angle(trs.rotation, waypoints[currentWaypoint].trs.rotation)));
			if ((trs.position == waypoints[currentWaypoint].trs.position || moveSpeed == 0) && (trs.eulerAngles == waypoints[currentWaypoint].trs.eulerAngles || rotateSpeed == 0))
				OnReachedWaypoint ();
		}
		
		void OnReachedWaypoint ()
		{
			if (backTracking)
				currentWaypoint --;
			else
				currentWaypoint ++;
			switch (moveType)
			{
				case MoveType.Once:
					if (currentWaypoint == waypoints.Count)
						currentWaypoint = waypoints.Count - 1;
					else if (currentWaypoint == -1)
						currentWaypoint = 0;
					return;
				case MoveType.Loop:
					if (currentWaypoint == waypoints.Count)
						currentWaypoint = 0;
					else if (currentWaypoint == -1)
						currentWaypoint = waypoints.Count - 1;
					return;
				case MoveType.PingPong:
					if (currentWaypoint == waypoints.Count)
					{
						currentWaypoint -= 2;
						backTracking = !backTracking;
					}
					else if (currentWaypoint == -1)
					{
						currentWaypoint += 2;
						backTracking = !backTracking;
					}
					return;
			}
		}
	}

	[Serializable]
	public struct Waypoint
	{
		public Transform trs;

		public Waypoint (Transform trs)
		{
			this.trs = trs;
		}
	}

	[Serializable]
	public struct WaypointPath
	{
		public float width;
		public Color color;
		public Material material;
		public string sortingLayerName;
		[Range(-32768, 32767)]
		public int sortingOrder;
	}

	public enum MoveType
	{
		Once = 0,
		Loop = 1,
		PingPong = 2
	}
}