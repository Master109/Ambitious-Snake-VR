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
#if UNITY_EDITOR
		public bool autoSetWaypoints = true;
		public Transform waypointsParent;
#endif
		public Transform trs;
		public FollowType followType;
		public List<Waypoint> waypoints = new List<Waypoint>();
		public float moveSpeed;
		public float rotateSpeed;
		public int currentWaypointIndex;
		public bool isBacktracking;
		public WaypointPath path;
		public Transform rotationViewerPrefab;
		public Vector3 pivotOffset;
		
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
						_waypoints[i] = new Waypoint(child, Vector3.zero);
					}
					waypoints = new List<Waypoint>(_waypoints);
				}
				else
					waypointsParent = trs;
			}
		}
#endif

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				AutoSetLineRenderers ();
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

		void AutoSetLineRenderers ()
		{
			List<LineRenderer> lineRenderers = new List<LineRenderer>(GetComponentsInChildren<LineRenderer>());
			for (int i = 0; i < lineRenderers.Count; i ++)
			{
				LineRenderer lineRenderer = lineRenderers[i];
				RemoveLineRenderer (lineRenderer);
			}
			lineRenderers.Clear();
			int previousCurrentWaypointIndex = currentWaypointIndex;
			bool previousIsBacktracking = isBacktracking;
			Waypoint nextWaypoint = waypoints[currentWaypointIndex];
			isBacktracking = !isBacktracking;
			OnReachedWaypoint ();
			int previousWaypointIndex = currentWaypointIndex;
			Waypoint previousWaypoint = waypoints[previousWaypointIndex];
			currentWaypointIndex = previousCurrentWaypointIndex;
			isBacktracking = previousIsBacktracking;
			int passedPreviousCurrentWaypointCount = 0;
			while (true)
			{
				Waypoint waypoint = nextWaypoint;
				if (currentWaypointIndex == previousCurrentWaypointIndex)
					passedPreviousCurrentWaypointCount ++;
				OnReachedWaypoint ();
				nextWaypoint = waypoints[currentWaypointIndex];
				if (followType == FollowType.Loop)
				{
					if (currentWaypointIndex == previousCurrentWaypointIndex)
						break;
				}
				else if (followType == FollowType.PingPong)
				{
					if ((passedPreviousCurrentWaypointCount == 2 && (previousCurrentWaypointIndex == 0 || previousCurrentWaypointIndex == waypoints.Count - 1)) || passedPreviousCurrentWaypointCount == 3)
						break;
				}
				else// if (followType == FollowType.Once)
				{
					if (currentWaypointIndex == waypoints.Count - 1 || currentWaypointIndex == 0)
						break;
				}
				lineRenderers.AddRange(MakeLineRenderersForWaypoint(waypoint, previousWaypoint, nextWaypoint));
				previousWaypoint = waypoint;
			}
			currentWaypointIndex = previousCurrentWaypointIndex;
			isBacktracking = previousIsBacktracking;
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
			lineRenderer.material = path.material;
			lineRenderer.startColor = path.color;
			lineRenderer.endColor = path.color;
			lineRenderer.startWidth = path.width;
			lineRenderer.endWidth = path.width;
			lineRenderer.sortingLayerName = path.sortingLayerName;
			lineRenderer.sortingOrder = Mathf.Clamp(path.sortingOrder, -32768, 32767);
			return lineRenderer;
		}

		void RemoveLineRenderer (LineRenderer lineRenderer)
		{
			if (lineRenderer.gameObject == gameObject)
				DestroyImmediate(lineRenderer);
			else
				DestroyImmediate(lineRenderer.gameObject);
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

		void SetLineRenderersToBoundsSides (Bounds bounds, LineRenderer[] lineRenderers)
		{
			LineSegment3D[] sides = bounds.GetSides();
			for (int i = 0; i < 12; i ++)
			{
				LineSegment3D side = sides[i];
				lineRenderers[i].SetPositions(new Vector3[2] { side.start, side.end });
			}
		}

		void SetLineRenderersToBoundsSidesAndRotate (Bounds bounds, LineRenderer[] lineRenderers, Vector3 pivotPoint, Quaternion rotation)
		{
			LineSegment3D[] sides = bounds.GetSides();
			for (int i = 0; i < 12; i ++)
			{
				LineSegment3D side = sides[i];
				lineRenderers[i].SetPositions(new Vector3[2] { side.start.Rotate(pivotPoint, rotation), side.end.Rotate(pivotPoint, rotation) });
			}
		}

		LineRenderer[] MakeLineRenderersForWaypoint (Waypoint waypoint, Waypoint previousWaypoint, Waypoint nextWaypoint)
		{
			List<LineRenderer> output = new List<LineRenderer>();
			if (waypoint.trs.eulerAngles == nextWaypoint.trs.eulerAngles)
			{
				LineRenderer[] lineRenderers = new LineRenderer[12];
				for (int i = 0; i < 12; i ++)
					lineRenderers[i] = AddLineRenderer();
				HashSet<LineRenderer> extraLineRenderers = new HashSet<LineRenderer>();
				if (!previousWaypoint.Equals(default(Waypoint)) && !nextWaypoint.Equals(previousWaypoint))
				{
					if (previousWaypoint.trs.position.x > waypoint.trs.position.x)
					{
						extraLineRenderers.Add(lineRenderers[1]);
						extraLineRenderers.Add(lineRenderers[6]);
						extraLineRenderers.Add(lineRenderers[10]);
						extraLineRenderers.Add(lineRenderers[11]);
					}
					else if (previousWaypoint.trs.position.x < waypoint.trs.position.x)
					{
						extraLineRenderers.Add(lineRenderers[1]);
						extraLineRenderers.Add(lineRenderers[4]);
						extraLineRenderers.Add(lineRenderers[7]);
						extraLineRenderers.Add(lineRenderers[8]);
					}
					if (previousWaypoint.trs.position.y > waypoint.trs.position.y)
					{
						extraLineRenderers.Add(lineRenderers[1]);
						extraLineRenderers.Add(lineRenderers[5]);
						extraLineRenderers.Add(lineRenderers[8]);
						extraLineRenderers.Add(lineRenderers[9]);
					}
					else if (previousWaypoint.trs.position.y < waypoint.trs.position.y)
					{
						extraLineRenderers.Add(lineRenderers[0]);
						extraLineRenderers.Add(lineRenderers[2]);
						extraLineRenderers.Add(lineRenderers[3]);
						extraLineRenderers.Add(lineRenderers[10]);
					}
					if (previousWaypoint.trs.position.z > waypoint.trs.position.z)
					{
						extraLineRenderers.Add(lineRenderers[3]);
						extraLineRenderers.Add(lineRenderers[4]);
						extraLineRenderers.Add(lineRenderers[5]);
						extraLineRenderers.Add(lineRenderers[6]);
					}
					else if (previousWaypoint.trs.position.z < waypoint.trs.position.z)
					{
						extraLineRenderers.Add(lineRenderers[0]);
						extraLineRenderers.Add(lineRenderers[7]);
						extraLineRenderers.Add(lineRenderers[9]);
						extraLineRenderers.Add(lineRenderers[11]);
					}
				}
				Bounds bounds = new Bounds(waypoint.trs.position, GetBoundsOfChildren().size);
				SetLineRenderersToBoundsSidesAndRotate (bounds, lineRenderers, waypoint.trs.position + waypoint.pivotOffset, waypoint.trs.rotation);
				if (!nextWaypoint.Equals(waypoint))
				{
					Bounds bounds2 = new Bounds(nextWaypoint.trs.position, GetBoundsOfChildren().size);
					HashSet<LineRenderer> changedLineRenderers = new HashSet<LineRenderer>();
					Vector3[] corners = bounds.GetCorners();
					Vector3 corner0 = corners[0];
					Vector3 corner1 = corners[1];
					Vector3 corner2 = corners[2];
					Vector3 corner3 = corners[3];
					Vector3 corner4 = corners[4];
					Vector3 corner5 = corners[5];
					Vector3 corner6 = corners[6];
					Vector3 corner7 = corners[7];
					if (nextWaypoint.trs.position.x > waypoint.trs.position.x)
					{
						ChangeLineRenderer (lineRenderers[1], bounds2, corner1, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[6], bounds2, corner2, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[10], bounds2, corner3, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[11], bounds2, corner7, ref changedLineRenderers);
					}
					else if (nextWaypoint.trs.position.x < waypoint.trs.position.x)
					{
						ChangeLineRenderer (lineRenderers[1], bounds2, corner0, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[4], bounds2, corner4, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[7], bounds2, corner5, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[8], bounds2, corner6, ref changedLineRenderers);
					}
					if (nextWaypoint.trs.position.y > waypoint.trs.position.y)
					{
						ChangeLineRenderer (lineRenderers[1], bounds2, corner2, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[5], bounds2, corner3, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[8], bounds2, corner4, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[9], bounds2, corner5, ref changedLineRenderers);
					}
					else if (nextWaypoint.trs.position.y < waypoint.trs.position.y)
					{
						ChangeLineRenderer (lineRenderers[0], bounds2, corner0, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[2], bounds2, corner1, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[3], bounds2, corner6, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[10], bounds2, corner7, ref changedLineRenderers);
					}
					if (nextWaypoint.trs.position.z > waypoint.trs.position.z)
					{
						ChangeLineRenderer (lineRenderers[3], bounds2, corner3, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[4], bounds2, corner5, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[5], bounds2, corner6, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[6], bounds2, corner7, ref changedLineRenderers);
					}
					else if (nextWaypoint.trs.position.z < waypoint.trs.position.z)
					{
						ChangeLineRenderer (lineRenderers[0], bounds2, corner0, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[7], bounds2, corner1, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[9], bounds2, corner2, ref changedLineRenderers);
						ChangeLineRenderer (lineRenderers[11], bounds2, corner4, ref changedLineRenderers);
					}
					foreach (LineRenderer changedLineRenderer in changedLineRenderers)
						extraLineRenderers.Remove(changedLineRenderer);
				}
				foreach (LineRenderer extraLineRenderer in extraLineRenderers)
					RemoveLineRenderer (extraLineRenderer);
				output.AddRange(lineRenderers);
			}
			else
			{
			}
			return output.ToArray();
		}

		void ChangeLineRenderer (LineRenderer lineRenderer, Bounds otherBounds, Vector3 point, ref HashSet<LineRenderer> changedLineRenderers)
		{
			lineRenderer.SetPositions(new Vector3[2] { otherBounds.ClosestPoint(point), point });
			changedLineRenderers.Add(lineRenderer);
		}

		public override void DoUpdate ()
		{
			if (GameManager.paused || _SceneManager.isLoading)
				return;
			if (moveSpeed != 0)
				trs.position = Vector3.Lerp(trs.position, waypoints[currentWaypointIndex].trs.position - pivotOffset, moveSpeed * Time.deltaTime * (1f / Vector2.Distance(trs.position, waypoints[currentWaypointIndex].trs.position - pivotOffset)));
			if (rotateSpeed != 0)
				trs.rotation = Quaternion.Slerp(trs.rotation, waypoints[currentWaypointIndex].trs.rotation, rotateSpeed * Time.deltaTime * (1f / Quaternion.Angle(trs.rotation, waypoints[currentWaypointIndex].trs.rotation)));
			if ((trs.position == waypoints[currentWaypointIndex].trs.position - pivotOffset || moveSpeed == 0) && (trs.eulerAngles == waypoints[currentWaypointIndex].trs.eulerAngles || rotateSpeed == 0))
				OnReachedWaypoint ();
		}
		
		void OnReachedWaypoint ()
		{
			if (isBacktracking)
				currentWaypointIndex --;
			else
				currentWaypointIndex ++;
			switch (followType)
			{
				case FollowType.Once:
					if (currentWaypointIndex == waypoints.Count)
						currentWaypointIndex = waypoints.Count - 1;
					else if (currentWaypointIndex == -1)
						currentWaypointIndex = 0;
					return;
				case FollowType.Loop:
					if (currentWaypointIndex == waypoints.Count)
						currentWaypointIndex = 0;
					else if (currentWaypointIndex == -1)
						currentWaypointIndex = waypoints.Count - 1;
					return;
				case FollowType.PingPong:
					if (currentWaypointIndex == waypoints.Count)
					{
						currentWaypointIndex -= 2;
						isBacktracking = !isBacktracking;
					}
					else if (currentWaypointIndex == -1)
					{
						currentWaypointIndex += 2;
						isBacktracking = !isBacktracking;
					}
					return;
			}
		}
	}

	[Serializable]
	public struct Waypoint
	{
		public Transform trs;
		public Vector3 pivotOffset;

		public Waypoint (Transform trs, Vector3 pivotOffset)
		{
			this.trs = trs;
			this.pivotOffset = pivotOffset;
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

	public enum FollowType
	{
		Once = 0,
		Loop = 1,
		PingPong = 2
	}
}