using UnityEngine;

namespace AmbitiousSnake
{
	public class LengthTutorial : Tutorial
	{
		public GameObject leftThumbstickIndicator;
		public GameObject rightThumbstickIndicator;
		bool hasGrown;
		bool hasShrunk;

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
			if (InputManager.ChangeLengthInput > 0)
			{
				hasGrown = true;
				if (hasShrunk)
					Finish ();
			}
			else if (InputManager.ChangeLengthInput < 0)
			{
				hasShrunk = true;
				if (hasGrown)
					Finish ();
			}
		}

		public override void Finish ()
		{
			base.Finish ();
			leftThumbstickIndicator.SetActive(false);
			rightThumbstickIndicator.SetActive(false);
		}
	}
}