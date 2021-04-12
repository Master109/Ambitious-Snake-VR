using UnityEngine;
using System.Collections;
using Extensions;
using UnityEngine.UI;

namespace AmbitiousSnake
{
	public class VR_UIPointer : SingletonUpdateWhileEnabled<VR_UIPointer>
	{
		public Transform trs;
		public Transform pointerTrs;
		public GameObject graphicsGo;
		public Transform uiPlaneTrs;
		public Plane uiPlane;
		public string submitInputVariablePath;
		_Selectable hoveredOver;
		Vector3 previousPosition;
		bool submitInput;
		bool previousSubmitInput;
		
		public override void DoUpdate ()
		{
			submitInput = InputManager.instance.GetMember<bool>(submitInputVariablePath);
			uiPlane = new Plane(uiPlaneTrs.forward, uiPlaneTrs.position);
			Vector3 position = VectorExtensions.NULL3;
			if (uiPlane.Raycast(new Ray(pointerTrs.position, pointerTrs.forward), out position))
			{
				if (trs.position != previousPosition)
				{
					trs.position = position;
					trs.rotation = Quaternion.LookRotation(pointerTrs.forward, trs.position - previousPosition);
					previousPosition = trs.position;
					hoveredOver = null;
					for (int i = 0; i < _Selectable.instances.Length; i ++)
					{
						_Selectable selectable = _Selectable.instances[i];
						if (selectable.rectTrs.GetWorldBounds().Contains(position))
						{
							hoveredOver = selectable;
							break;
						}
					}
				}
				graphicsGo.SetActive(true);
			}
			else
			{
				hoveredOver = null;
				graphicsGo.SetActive(false);
			}
			if (hoveredOver != null)
			{
				if (submitInput && !previousSubmitInput)
				{
					Button button = hoveredOver.selectable as Button;
					if (button != null)
						button.onClick.Invoke();
				}
			}
			previousSubmitInput = submitInput;
		}
	}
}