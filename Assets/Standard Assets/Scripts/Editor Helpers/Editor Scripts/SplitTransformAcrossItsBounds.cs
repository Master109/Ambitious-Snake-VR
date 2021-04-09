#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Extensions;
using UnityEditor;

namespace AmbitiousSnake
{
	public class SplitTransformAcrossItsBounds : EditorScript
	{
		public Vector3 splitInterval = Vector3.one;
		public Transform trs;

		public override void Do ()
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			_Do (trs, splitInterval);
		}

		static void _Do (Transform trs, Vector3 splitInterval)
		{
			Vector3[] pointsInside = trs.GetBounds().ToBoundsInt(MathfExtensions.RoundingMethod.RoundUpIfNotInteger, MathfExtensions.RoundingMethod.RoundDownIfNotInteger).ToBounds().GetPointsInside(splitInterval);
			trs.position = pointsInside[0];
			trs.SetWorldScale (splitInterval);
			for (int i = 1; i < pointsInside.Length; i ++)
			{
				Vector3 pointInside = pointsInside[i];
				Instantiate(trs, pointInside, trs.rotation, trs.parent);
			}
		}

		[MenuItem("Tools/Split selected objects across transform bounds")]
		static void _Do ()
		{
			Transform[] selectedTransforms = Selection.transforms;
			for (int i = 0; i < selectedTransforms.Length; i ++)
			{
				Transform selectedTrs = selectedTransforms[i];
				_Do (selectedTrs, Vector3.one);
			}
		}
	}
}
#else
namespace AmbitiousSnake
{
	public class SplitTransformAcrossItsBounds : EditorScript
	{
	}
}
#endif