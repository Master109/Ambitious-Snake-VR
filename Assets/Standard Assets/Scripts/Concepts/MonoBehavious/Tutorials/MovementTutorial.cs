using UnityEngine;
using UnityEngine.Playables;
using Extensions;

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

		}
	}
}