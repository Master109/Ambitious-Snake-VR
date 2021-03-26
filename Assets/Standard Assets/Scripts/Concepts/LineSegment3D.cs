using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;

[Serializable]
public class LineSegment3D
{
	public Vector3 start;
	public Vector3 end;
	public static LineSegment3D NULL = new LineSegment3D(VectorExtensions.NULL3, VectorExtensions.NULL3);

	public LineSegment3D ()
	{
	}

	public LineSegment3D (Vector3 start, Vector3 end)
	{
		this.start = start;
		this.end = end;
	}

	public override string ToString ()
	{
		return "[" + start + "], [" + end + "]";
	}

#if UNITY_EDITOR
	public virtual void DrawGizmos (Color color)
	{
		GizmosManager.GizmosEntry gizmosEntry = new GizmosManager.GizmosEntry();
		gizmosEntry.setColor = true;
		gizmosEntry.color = color;
		gizmosEntry.onDrawGizmos += DrawGizmos;
		GizmosManager.gizmosEntries.Add(gizmosEntry);
	}

	public virtual void DrawGizmos (params object[] args)
	{
		Gizmos.DrawLine(start, end);
	}
#endif
	
	public virtual bool ContainsPoint (Vector3 point)
	{
		return Vector3.Distance(point, start) + Vector3.Distance(point, end) == Vector3.Distance(start, end);
	}
	
	public virtual LineSegment3D Move (Vector3 movement)
	{
		return new LineSegment3D(start + movement, end + movement);
	}
	
	public virtual LineSegment3D Rotate (Vector3 pivotPoint, Quaternion rotation)
	{
		Vector3 outputStart = start.Rotate(pivotPoint, rotation);
		Vector3 outputEnd = end.Rotate(pivotPoint, rotation);
		return new LineSegment3D(outputStart, outputEnd);
	}

	public virtual Vector3 ClosestPoint (Vector3 point)
	{
		Vector3 output;
		float directedDistanceAlongParallel = GetDirectedDistanceAlongParallel(point);
		if (directedDistanceAlongParallel > 0 && directedDistanceAlongParallel < GetLength())
			output = GetPointWithDirectedDistance(directedDistanceAlongParallel);
		else if (directedDistanceAlongParallel >= GetLength())
			output = end;
		else
			output = start;
		return output;
	}

	public virtual Vector3 GetMidpoint ()
	{
		return (start + end) / 2;
	}

	public virtual float GetDirectedDistanceAlongParallel (Vector3 point)
	{
		Quaternion rotation = Quaternion.LookRotation(end - start);
		rotation = Quaternion.Inverse(rotation);
		LineSegment3D rotatedLine = Rotate(Vector3.zero, rotation);
		point = rotation * point;
		return point.z - rotatedLine.start.z;
	}

	public virtual Vector3 GetPointWithDirectedDistance (float directedDistance)
	{
		return start + (GetDirection() * directedDistance);
	}

	public virtual float GetLength ()
	{
		return Vector3.Distance(start, end);
	}

	public virtual float GetLengthSqr ()
	{
		return (start - end).sqrMagnitude;
	}

	public virtual Vector3 GetDirection ()
	{
		return (end - start).normalized;
	}
	
	public virtual LineSegment3D Flip ()
	{
		return new LineSegment3D(end, start);
	}

	public virtual bool DoIIntersectWithSphere (Sphere sphere)
	{
		return (ClosestPoint(sphere.center) - sphere.center).sqrMagnitude <= sphere.radius * sphere.radius;
	}

	public virtual bool DoIIntersectWithLineSegment (LineSegment3D other, bool shouldIncludeEndPoints)
	{
		Vector3 da = end - start; 
		Vector3 db = other.end - other.start;
		Vector3 dc = other.start - start;
		if (Vector3.Dot(dc, Vector3.Cross(da, db)) != 0) // lines are not coplanar
			return false;
		float s = Vector3.Dot(Vector3.Cross(dc, db), Vector3.Cross(da, db)) / Vector3.Dot(Vector3.Cross(da, db), Vector3.Cross(da, db));
		float t = Vector3.Dot(Vector3.Cross(dc, da), Vector3.Cross(da, db)) / Vector3.Dot(Vector3.Cross(da, db), Vector3.Cross(da, db));
		return s >= 0 && s <= 1 && t >= 0 && t <= 1;
	}
}