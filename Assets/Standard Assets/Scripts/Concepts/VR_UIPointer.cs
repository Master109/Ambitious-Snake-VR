using UnityEngine;
using System.Collections;
using Extensions;

namespace AmbitiousSnake
{
	public class VR_UIPointer : SingletonUpdateWhileEnabled<VR_UIPointer>
	{
		public Transform trs;
		public Transform pointerTrs;
		public GameObject graphicsGo;
		public Transform uiPlaneTrs;
		public Plane uiPlane;
		Vector3 previousPosition;
		
		public override void DoUpdate ()
		{
			uiPlane = new Plane(uiPlaneTrs.forward, uiPlaneTrs.position);
			Vector3 position;
			if (uiPlane.Raycast(new Ray(pointerTrs.position, pointerTrs.forward), out position))
			{
				if (trs.position != previousPosition)
				{
					trs.position = position;
					trs.rotation = Quaternion.LookRotation(pointerTrs.forward, trs.position - previousPosition);
					previousPosition = trs.position;
				}
				graphicsGo.SetActive(true);
			}
			else
				graphicsGo.SetActive(false);
		}
	}
}