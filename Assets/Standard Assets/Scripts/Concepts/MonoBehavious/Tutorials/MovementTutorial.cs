using UnityEngine;

namespace AmbitiousSnake
{
	public class MovementTutorial : Tutorial
	{
		public GameObject leftGripIndicator;
		public GameObject rightGripIndicator;

		public override void OnEnable ()
		{
			base.OnEnable ();
			if (SetDominantHandTutorial.LeftHandIsDominant)
				leftGripIndicator.SetActive(true);
			else
				rightGripIndicator.SetActive(true);
		}
		
		public override void DoUpdate ()
		{
			if (InputManager.MoveInput != Vector3.zero)
				Finish ();
		}

		public override void Finish ()
		{
			base.Finish ();
			leftGripIndicator.SetActive(false);
			rightGripIndicator.SetActive(false);
		}
	}
}