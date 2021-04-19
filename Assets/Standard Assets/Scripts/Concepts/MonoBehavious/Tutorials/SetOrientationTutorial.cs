using UnityEngine;
using UnityEngine;

namespace AmbitiousSnake
{
	public class SetOrientationTutorial : Tutorial
	{
		public Vector2 maxCameraForwardToFinish;
		bool setOrientationInput;
		bool previousSetOrientationInput;
		
		public override void DoUpdate ()
		{
			setOrientationInput = InputManager.SetOrientationInput;
			if (setOrientationInput && VRCameraRig.instance.eyesTrs.forward.x <= maxCameraForwardToFinish.y && VRCameraRig.instance.eyesTrs.forward.y <= maxCameraForwardToFinish.y)
				Finish ();
			previousSetOrientationInput = setOrientationInput;
		}
	}
}