using UnityEngine;
using UnityEngine.Playables;
using Extensions;

namespace AmbitiousSnake
{
	public class SetDominantHandTutorial : Tutorial
	{
		public PlayableAsset playWhileLookingAt0Hands;
		public PlayableAsset playWhileLookingAt1Hand;
		public PlayableAsset playWhileLookingAt2Hands;
		public float shrinkCameraViewNormalized;
		public static bool LeftHandIsDominant
		{
			get
			{
				return PlayerPrefsExtensions.GetBool("Left-handed", false);
			}
			set
			{
				PlayerPrefsExtensions.SetBool("Left-handed", value);
			}
		}
		bool lookingAtLeftHand;
		bool lookingAtRightHand;
		bool previousLookingAtLeftHand;
		bool previousLookingAtRightHand;

		public override void OnEnable ()
		{
			base.OnEnable ();
			playableDirector.stopped += OnPlayableDirectorStopped;
		}

		void OnPlayableDirectorStopped (PlayableDirector playableDirector)
		{
			if (lookingAtLeftHand != lookingAtRightHand && lookingAtLeftHand == previousLookingAtLeftHand && lookingAtRightHand == previousLookingAtRightHand)
				Finish ();
		}

		public override void Finish ()
		{
			LeftHandIsDominant = lookingAtLeftHand;
			base.Finish ();
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			playableDirector.stopped -= OnPlayableDirectorStopped;
		}

		public override void DoUpdate ()
		{
			lookingAtLeftHand = IsLookingAtTransform(VRCameraRig.instance.leftHandTrs, shrinkCameraViewNormalized);
			lookingAtRightHand = IsLookingAtTransform(VRCameraRig.instance.rightHandTrs, shrinkCameraViewNormalized);
			if (lookingAtLeftHand != previousLookingAtLeftHand || lookingAtRightHand != previousLookingAtRightHand)
			{
				playableDirector.Stop();
				if (lookingAtLeftHand != lookingAtRightHand)
					playableDirector.Play(playWhileLookingAt1Hand);
				else if (lookingAtLeftHand && lookingAtRightHand)
					playableDirector.Play(playWhileLookingAt2Hands);
				else
					playableDirector.Play(playWhileLookingAt0Hands);
			}
			previousLookingAtLeftHand = lookingAtLeftHand;
			previousLookingAtRightHand = lookingAtRightHand;
		}
	}
}