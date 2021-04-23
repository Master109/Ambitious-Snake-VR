using UnityEngine;
using UnityEngine;

namespace AmbitiousSnake
{
	public class SetOrientationTutorial : Tutorial
	{
		public GameObject leftThumbstickIndicator;
		public GameObject rightThumbstickIndicator;
		public Vector2 maxCameraForwardToFinish;
		bool setOrientationInput;
		bool previousSetOrientationInput;

		public override void OnEnable ()
		{
			base.OnEnable ();
			if (!SetDominantHandTutorial.LeftHandIsDominant)
				leftThumbstickIndicator.SetActive(true);
			else
				rightThumbstickIndicator.SetActive(true);
		}
		
		public override void DoUpdate ()
		{
			setOrientationInput = InputManager.SetOrientationInput;
			if (setOrientationInput && VRCameraRig.instance.eyesTrs.forward.x <= maxCameraForwardToFinish.y && VRCameraRig.instance.eyesTrs.forward.y <= maxCameraForwardToFinish.y)
				Finish ();
			previousSetOrientationInput = setOrientationInput;
		}

		public override void Finish ()
		{
			base.Finish ();
			leftThumbstickIndicator.SetActive(false);
			rightThumbstickIndicator.SetActive(false);
		}
	}
}