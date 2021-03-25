using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
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
		[HideInInspector]
		public bool hasTraveledFullCycle;
		
		void Start ()
		{
			foreach (SnapPosition snapPosition in GetComponentsInChildren<SnapPosition>())
				snapPosition.enabled = false;
			if (autoSetWaypoints)
				waypointsParent = transform;
			if (waypointsParent != null)
				waypoints.AddRange(waypointsParent.GetComponentsInChildren<Waypoint>());
			foreach (Waypoint waypoint in waypoints)
				waypoint.trs.SetParent(null);
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
		
		public override void DoUpdate ()
		{
			if (GameManager.paused || _SceneManager.isLoading)
				return;
			if (moveSpeed != 0)
				transform.position = Vector2.Lerp(transform.position, waypoints[currentWaypoint].trs.position, moveSpeed * (1f / Vector2.Distance(transform.position, waypoints[currentWaypoint].trs.position)));
			if (rotateSpeed != 0)
				trs.rotation = Quaternion.Slerp(trs.rotation, waypoints[currentWaypoint].trs.rotation, rotateSpeed * (1f / Quaternion.Angle(transform.rotation, waypoints[currentWaypoint].trs.rotation)));
			if ((trs.position == waypoints[currentWaypoint].trs.position || moveSpeed == 0) && (transform.up == waypoints[currentWaypoint].trs.up || rotateSpeed == 0))
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
					{
						hasTraveledFullCycle = true;
						currentWaypoint = waypoints.Count - 1;
					}
					else if (currentWaypoint == -1)
					{
						hasTraveledFullCycle = true;
						currentWaypoint = 0;
					}
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
						hasTraveledFullCycle = true;
					}
					else if (currentWaypoint == -1)
					{
						currentWaypoint += 2;
						backTracking = !backTracking;
						hasTraveledFullCycle = true;
					}
					return;
			}
		}
	}

	[Serializable]
	public struct Waypoint
	{
		public Transform trs;
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