#if UNITY_EDITOR
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class GridTransform : EditorScript
	{
		public Transform trs;

		void Start ()
		{
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				return;
			}
		}

		public override void Do ()
		{
			if (trs == null)
				return;
			BoundsInt bounds = trs.GetBounds().ToBoundsInt();
			trs.position = bounds.center - Vector3.one / 2;
			trs.SetWorldScale (bounds.size);
		}
	}
}
#else
namespace AmbitiousSnake
{
	public class GridTransform : EditorScript
	{
	}
}
#endif