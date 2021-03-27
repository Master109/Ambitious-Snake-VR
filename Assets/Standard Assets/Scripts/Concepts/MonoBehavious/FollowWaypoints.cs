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
		public bool autoSetLineRenderers = true;
		public bool autoSetWaypoints = true;
		public Transform waypointsParent;
#endif
		public Transform trs;
		public FollowType followType;
		public List<Waypoint> waypoints = new List<Waypoint>();
		public float moveSpeed;
		public float rotateSpeed;
		public int currentWaypoint;
		public bool isBacktracking;
		public WaypointPath path;
		public Transform rotationViewerPrefab;
		public List<LineRenderer> lineRenderers = new List<LineRenderer>();
		public Vector3 pivotPoint;
		
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
			if (autoSetLineRenderers)
			{
				lineRenderers = new List<LineRenderer>(GetComponentsInChildren<LineRenderer>());
				for (int i = 0; i < lineRenderers.Count; i ++)
				{
					LineRenderer lineRenderer = lineRenderers[i];
					RemoveLineRenderer (lineRenderer);
				}
				if (followType == FollowType.Loop)
				{
					int waypointIndex = currentWaypoint;
					Waypoint nextWaypoint = waypoints[waypointIndex];
					Waypoint previousWaypoint = waypoints[GetPreviousWaypointIndex(waypointIndex)];
					while (true)
					{
						Waypoint waypoint = nextWaypoint;
						waypointIndex = GetNextWaypointIndex(waypointIndex);
						if (waypointIndex == currentWaypoint)
							break;
						nextWaypoint = waypoints[waypointIndex];
						lineRenderers.AddRange(MakeLineRenderersForWaypoints(waypoint, previousWaypoint, nextWaypoint));
						previousWaypoint = waypoint;
					}
				}
				else
				{
					Waypoint nextWaypoint = waypoints[0];
					Waypoint previousWaypoint = default(Waypoint);
					for (int i = 1; i < waypoints.Count; i ++)
					{
						Waypoint waypoint = nextWaypoint;
						nextWaypoint = waypoints[i];
						lineRenderers.AddRange(MakeLineRenderersForWaypoints(waypoint, previousWaypoint, nextWaypoint));
						previousWaypoint = waypoint;
					}
				}
			}
		}
#endif

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			base.OnEnable ();
			for (int i = 0; i < waypoints.Count; i ++)
			{
				Waypoint waypoint = waypoints[i];
				waypoint.trs.SetParent(null);
			}
		}

		int GetNextWaypointIndex (int waypointIndex)
		{
			return GetNextWaypointIndex(waypointIndex, isBacktracking);
		}

		int GetPreviousWaypointIndex (int waypointIndex)
		{
			return GetNextWaypointIndex(waypointIndex, !isBacktracking);
		}

		int GetNextWaypointIndex (int waypointIndex, bool isBacktracking)
		{
			if ((waypointIndex == waypoints.Count - 1 && !isBacktracking) || (waypointIndex == 0 && isBacktracking))
			{
				switch (followType)
				{
					case FollowType.Once:
						return waypointIndex;
					case FollowType.Loop:
						if (!isBacktracking)
							return 0;
						else
							return waypoints.Count - 1;
					case FollowType.PingPong:
						if (!isBacktracking)
							return waypointIndex - 1;
						else
							return waypointIndex + 1;
					default:
						return -1;
				}
			}
			else
			{
				if (!isBacktracking)
					return waypointIndex + 1;
				else
					return waypointIndex - 1;
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

		void RemoveLineRenderer (LineRenderer lineRenderer)
		{
			if (lineRenderer.gameObject == gameObject)
				Destroy(lineRenderer);
			else
				Destroy(lineRenderer.gameObject);
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

		LineRenderer[] MakeLineRenderersForWaypoints (Waypoint waypoint, Waypoint previousWaypoint, Waypoint nextWaypoint)
		{
			List<LineRenderer> output = new List<LineRenderer>();
			if (waypoint.trs.eulerAngles == nextWaypoint.trs.eulerAngles)
			{
				LineRenderer[] lineRenderers = new LineRenderer[12];
				for (int i = 0; i < 12; i ++)
					lineRenderers[i] = AddLineRenderer();
				if (!previousWaypoint.Equals(default(Waypoint)))
				{
					HashSet<LineRenderer> extraLineRenderers = new HashSet<LineRenderer>();
					if (waypoint.trs.position.x > previousWaypoint.trs.position.x)
					{
						extraLineRenderers.Add(lineRenderers[1]);
						extraLineRenderers.Add(lineRenderers[6]);
						extraLineRenderers.Add(lineRenderers[10]);
						extraLineRenderers.Add(lineRenderers[11]);
					}
					else if (waypoint.trs.position.x < previousWaypoint.trs.position.x)
					{
						extraLineRenderers.Add(lineRenderers[1]);
						extraLineRenderers.Add(lineRenderers[4]);
						extraLineRenderers.Add(lineRenderers[7]);
						extraLineRenderers.Add(lineRenderers[8]);
					}
					if (waypoint.trs.position.y > previousWaypoint.trs.position.y)
					{
						extraLineRenderers.Add(lineRenderers[1]);
						extraLineRenderers.Add(lineRenderers[5]);
						extraLineRenderers.Add(lineRenderers[8]);
						extraLineRenderers.Add(lineRenderers[9]);
					}
					else if (waypoint.trs.position.y < previousWaypoint.trs.position.y)
					{
						extraLineRenderers.Add(lineRenderers[0]);
						extraLineRenderers.Add(lineRenderers[2]);
						extraLineRenderers.Add(lineRenderers[3]);
						extraLineRenderers.Add(lineRenderers[10]);
					}
					if (waypoint.trs.position.z > previousWaypoint.trs.position.z)
					{
						extraLineRenderers.Add(lineRenderers[3]);
						extraLineRenderers.Add(lineRenderers[4]);
						extraLineRenderers.Add(lineRenderers[5]);
						extraLineRenderers.Add(lineRenderers[6]);
					}
					else if (waypoint.trs.position.z < previousWaypoint.trs.position.z)
					{
						extraLineRenderers.Add(lineRenderers[0]);
						extraLineRenderers.Add(lineRenderers[7]);
						extraLineRenderers.Add(lineRenderers[9]);
						extraLineRenderers.Add(lineRenderers[11]);
					}
					foreach (LineRenderer extraLineRenderer in extraLineRenderers)
						RemoveLineRenderer (extraLineRenderer);
				}
				Bounds bounds = new Bounds(waypoint.trs.position, GetBoundsOfChildren().size);
				SetLineRenderersToBoundsSidesAndRotate (bounds, lineRenderers, waypoint.trs.position + waypoint.pivotOffset, waypoint.trs.rotation);
				output.AddRange(lineRenderers);
				// for (int i = 0; i < 12; i ++)
				// 	lineRenderers[i] = AddLineRenderer();
				// bounds = new Bounds(nextWaypoint.trs.position, GetBoundsOfChildren().size);
				// SetLineRenderersToBoundsSidesAndRotate (bounds, lineRenderers, nextWaypoint.trs.position + nextWaypoint.pivotOffset, nextWaypoint.trs.rotation);
				// output.AddRange(lineRenderers);
			}
			else
			{
			}
			return output.ToArray();
		}
		
		public override void DoUpdate ()
		{
			if (GameManager.paused || _SceneManager.isLoading)
				return;
			if (moveSpeed != 0)
				trs.position = Vector3.Lerp(trs.position + pivotPoint, waypoints[currentWaypoint].trs.position, moveSpeed * Time.deltaTime * (1f / Vector2.Distance(trs.position, waypoints[currentWaypoint].trs.position)));
			if (rotateSpeed != 0)
				trs.rotation = Quaternion.Slerp(trs.rotation, waypoints[currentWaypoint].trs.rotation, rotateSpeed * Time.deltaTime * (1f / Quaternion.Angle(trs.rotation, waypoints[currentWaypoint].trs.rotation)));
			if ((trs.position == waypoints[currentWaypoint].trs.position || moveSpeed == 0) && (trs.eulerAngles == waypoints[currentWaypoint].trs.eulerAngles || rotateSpeed == 0))
				OnReachedWaypoint ();
		}
		
		void OnReachedWaypoint ()
		{
			if (isBacktracking)
				currentWaypoint --;
			else
				currentWaypoint ++;
			switch (followType)
			{
				case FollowType.Once:
					if (currentWaypoint == waypoints.Count)
						currentWaypoint = waypoints.Count - 1;
					else if (currentWaypoint == -1)
						currentWaypoint = 0;
					return;
				case FollowType.Loop:
					if (currentWaypoint == waypoints.Count)
						currentWaypoint = 0;
					else if (currentWaypoint == -1)
						currentWaypoint = waypoints.Count - 1;
					return;
				case FollowType.PingPong:
					if (currentWaypoint == waypoints.Count)
					{
						currentWaypoint -= 2;
						isBacktracking = !isBacktracking;
					}
					else if (currentWaypoint == -1)
					{
						currentWaypoint += 2;
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