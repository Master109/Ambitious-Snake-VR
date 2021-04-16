#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEditor;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class CenterParentOnChildrenColliderBounds : EditorScript
	{
		public Transform trs;

		public override void Do ()
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			_Do (trs);
		}

		public static void _Do (Transform trs)
		{
			Collider[] colliders = trs.GetComponentsInChildren<Collider>();
			Bounds[] collidersBounds = new Bounds[colliders.Length];
			for (int i = 0; i < colliders.Length; i ++)
			{
				Collider collider = colliders[i];
				collidersBounds[i] = collider.bounds;
			}
			Bounds bounds = collidersBounds.Combine();
			Vector3 previousPosition = trs.position;
			trs.position = bounds.center;
			Vector3 toPreviousPosition = previousPosition - trs.position;
			for (int i = 0; i < trs.childCount; i ++)
			{
				Transform child = trs.GetChild(i);
				child.position += toPreviousPosition;
			}
		}

		[MenuItem("Tools/Center selected parents on children colliders' bounds")]
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
using UnityEngine;

namespace AmbitiousSnake
{
	public class CenterParentOnChildrenColliderBounds : EditorScript
	{
	}
}
#endif