#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace AmbitiousSnake
{
	public class MeasureDistanceBetweenTransforms : EditorScript
	{
		public override void Do ()
		{
			Do ();
		}

		[MenuItem("Tools/Print distance between selected objects")]
		static void _Do ()
		{
			Transform[] selectedTransforms = Selection.transforms;
			for (int i = 0; i < selectedTransforms.Length; i ++)
			{
				Transform selectedTrs = selectedTransforms[i];
				for (int i2 = i + 1; i2 < selectedTransforms.Length; i2 ++)
				{
					Transform selectedTrs2 = selectedTransforms[i2];
					print(Vector3.Distance(selectedTrs.position, selectedTrs2.position) + " is the distance between " + selectedTrs.name + " and " + selectedTrs2.name);
				}
			}
		}
	}
}
#else
namespace AmbitiousSnake
{
	public class MeasureDistanceBetweenTransforms : EditorScript
	{
	}
}
#endif