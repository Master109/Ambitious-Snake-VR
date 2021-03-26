using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
		LineRenderer line;
		public WaypointPath path;
		public Transform rotationViewerPrefab;
		
#if UNITY_EDITOR
		void OnValidate ()
		{
			foreach (SnapPosition snapPosition in GetComponentsInChildren<SnapPosition>())
				snapPosition.enabled = false;
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
				if (GetComponent<LineRenderer>() == null)
					line = gameObject.AddComponent<LineRenderer>();
				else
					line = GetComponent<LineRenderer>();
				line.positionCount = waypoints.Count + 1;
				line.SetPosition(0, transform.position);
				if (moveType == MoveType.Loop)
				{
					int counter = 1;
					int waypointIndex = currentWaypoint;
					while (true)
					{
						line.SetPosition(counter, waypoints[waypointIndex].trs.position);
						if (backTracking)
						{
							waypointIndex --;
							if (waypointIndex == -1)
								waypointIndex = waypoints.Count - 1;
						}
						else
						{
							waypointIndex ++;
							if (waypointIndex == waypoints.Count)
								waypointIndex = 0;
						}
						if (waypointIndex == currentWaypoint)
							break;
						counter ++;
					}
				}
				else
				{
					for (int i = 0; i < waypoints.Count; i ++)
						line.SetPosition(i + 1, waypoints[i].trs.position);
				}
				line.material = path.material;
				line.startColor = path.color;
				line.endColor = path.color;
				line.startWidth = path.width;
				line.endWidth = path.width;
				line.sortingLayerName = path.sortingLayerName;
				line.sortingOrder = Mathf.Clamp(path.sortingOrder, -32768, 32767);
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
				return;
#endif
			base.OnEnable ();
			for (int i = 0; i < waypoints.Count; i ++)
			{
				Waypoint waypoint = waypoints[i];
				waypoint.trs.SetParent(null);
			}
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