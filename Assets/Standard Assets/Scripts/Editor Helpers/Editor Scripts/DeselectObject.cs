#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class DeselectObject : EditorScript
	{
		public override void Do ()
		{
			
		}
	}
}
#else
using UnityEngine;

namespace AmbitiousSnake
{
	public class DeselectObject : EditorScript
	{
	}
}
#endif