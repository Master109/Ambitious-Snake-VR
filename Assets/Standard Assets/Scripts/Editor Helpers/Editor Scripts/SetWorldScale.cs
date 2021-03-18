#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class SetWorldScale : EditorScript
	{
		public Transform trs;
		public Vector3 scale;
		
		public override void Do ()
		{
			trs.SetWorldScale(scale);
		}
	}
}
#else
using UnityEngine;

namespace AmbitiousSnake
{
	public class SetWorldScale : EditorScript
	{
	}
}
#endif