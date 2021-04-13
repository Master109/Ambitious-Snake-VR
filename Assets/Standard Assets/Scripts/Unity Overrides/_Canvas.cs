using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	[RequireComponent(typeof(Canvas))]
	[ExecuteInEditMode]
	public class _Canvas : UpdateWhileEnabled
	{
		public Canvas canvas;
		public float planeDistance;
		
		void Start ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				canvas = GetComponent<Canvas>();
				return;
			}
#endif
			canvas.worldCamera = Camera.main;
			canvas.planeDistance = planeDistance;
		}
		
		public override void DoUpdate ()
		{
			Canvas.ForceUpdateCanvases();
		}
	}
}