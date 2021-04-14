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
		public ComplexTimer selectableColorMultiplier;
		public float minDistanceToRotateSqr;
		public string submitInputVariablePath;
		public static VR_UIPointer[] instances = new VR_UIPointer[0];
		_Selectable hoveredOver;
		_Selectable previousHoveredOver;
		Vector3 previousPosition;
		bool submitInput;
		bool previousSubmitInput;

		void Start ()
		{
			trs.SetParent(null);
			instances = instances.Add(this);
		}

		void OnDestroy ()
		{
			instances = instances.Remove(this);
		}
		
		public override void DoUpdate ()
		{
			if (uiPlaneTrs == null)
			{
				Destroy(gameObject);
				return;
			}
			if (!uiPlaneTrs.gameObject.activeSelf)
			{
				graphicsGo.SetActive(false);
				return;
			}
			submitInput = InputManager.instance.GetMember<bool>(submitInputVariablePath);
			uiPlane = new Plane(uiPlaneTrs.forward, uiPlaneTrs.position);
			Vector3 position = VectorExtensions.NULL3;
			if (uiPlane.Raycast(new Ray(pointerTrs.position, pointerTrs.forward), out position))
			{
				if (position != previousPosition)
				{
					trs.position = position;
					if ((position - previousPosition).sqrMagnitude >= minDistanceToRotateSqr)
						trs.rotation = Quaternion.LookRotation(pointerTrs.forward, position - previousPosition);
					previousPosition = position;
					hoveredOver = null;
					for (int i = 0; i < _Selectable.instances.Length; i ++)
					{
						_Selectable selectable = _Selectable.instances[i];
						if (selectable.rectTrs.rect.Contains(selectable.rectTrs.InverseTransformPoint(position)))
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
				if (previousHoveredOver != hoveredOver)
				{
					if (previousHoveredOver != null)
						SetSelectableColorMultiplier (previousHoveredOver.selectable, 1);
					selectableColorMultiplier.JumpToInitValue ();
				}
				SetSelectableColorMultiplier (hoveredOver.selectable, selectableColorMultiplier.GetValue());
				if (submitInput && !previousSubmitInput)
				{
					Button button = hoveredOver.selectable as Button;
					if (button != null)
						button.onClick.Invoke();
				}
			}
			else if (previousHoveredOver != null)
				SetSelectableColorMultiplier (previousHoveredOver.selectable, 1);
			previousHoveredOver = hoveredOver;
			previousSubmitInput = submitInput;
		}

		void SetSelectableColorMultiplier (Selectable selectable, float colorMultiplier)
		{
			ColorBlock colorBlock = selectable.colors;
			colorBlock.colorMultiplier = colorMultiplier;
			selectable.colors = colorBlock;
		}
	}
}