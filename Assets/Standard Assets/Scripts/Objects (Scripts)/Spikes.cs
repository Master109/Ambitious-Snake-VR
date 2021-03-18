using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class Spikes : Hazard, ICollisionEnterHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}

		void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				if (collider == null)
					collider = GetComponent<Collider>();
				return;
			}
#endif
		}
	}
}