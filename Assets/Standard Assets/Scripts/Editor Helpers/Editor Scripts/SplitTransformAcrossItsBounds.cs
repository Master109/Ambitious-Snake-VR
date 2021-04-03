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
		public Transform trs;

		public override void Do ()
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			_Do (trs);
		}

		static void _Do (Transform trs)
		{
			Vector3[] pointsInside = trs.GetBounds().ToBoundsInt(MathfExtensions.RoundingMethod.RoundUpIfNotInteger, MathfExtensions.RoundingMethod.RoundDownIfNotInteger).ToBounds().GetPointsInside(Vector3.one);
			for (int i = 0; i < pointsInside.Length; i ++)
			{
				Vector3 pointInside = pointsInside[i];
			}
		}

		[MenuItem("Tools/Split selected objects across transform bounds")]
		static void _Do ()
		{
			Transform[] selectedTransforms = Selection.transforms;
			for (int i = 0; i < selectedTransforms.Length; i ++)
			{
				Transform selectedTrs = selectedTransforms[i];
				_Do (selectedTrs);
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