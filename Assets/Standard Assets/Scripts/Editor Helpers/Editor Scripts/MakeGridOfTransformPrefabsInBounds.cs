#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Extensions;
using UnityEditor;

namespace AmbitiousSnake
{
	public class MakeGridOfTransformPrefabsInBounds : EditorScript
	{
		public Vector3 gridCellSize = Vector3.one;
		public Transform trs;
		public Bounds bounds;

		public override void Do ()
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			_Do (PrefabUtility.GetCorrespondingObjectFromSource(trs), bounds, gridCellSize, trs.parent);
		}

		static void _Do (Transform trsPrefab, Bounds bounds, Vector3 gridCellSize, Transform makeUnderParent = null)
		{
			Vector3[] pointsInside = bounds.GetPointsInside(gridCellSize);
			for (int i = 0; i < pointsInside.Length; i ++)
			{
				Vector3 pointInside = pointsInside[i];
				Transform trs = (Transform) PrefabUtility.InstantiatePrefab(trsPrefab);
				trs.SetParent(makeUnderParent);
				trs.position = pointInside;
				trs.SetWorldScale (gridCellSize);
			}
		}

		[MenuItem("Tools/Make grid of corresponding prefabs in selected mesh bounds")]
		static void _Do ()
		{
			Transform[] selectedTransforms = Selection.transforms;
			for (int i = 0; i < selectedTransforms.Length; i ++)
			{
				Transform selectedTrs = selectedTransforms[i];
				MeshFilter meshFilter = selectedTrs.GetComponent<MeshFilter>();
				if (meshFilter != null)
				{
					Bounds bounds = meshFilter.sharedMesh.bounds;
					bounds.center += selectedTrs.position;
					bounds = bounds.ToBoundsInt(MathfExtensions.RoundingMethod.RoundUpIfNotInteger, MathfExtensions.RoundingMethod.RoundDownIfNotInteger).ToBounds();
					_Do (PrefabUtility.GetCorrespondingObjectFromSource(selectedTrs), bounds, Vector3.one, selectedTrs.parent);
				}
			}
		}
	}
}
#else
namespace AmbitiousSnake
{
	public class MakeGridOfObjectsInBounds : EditorScript
	{
	}
}
#endif