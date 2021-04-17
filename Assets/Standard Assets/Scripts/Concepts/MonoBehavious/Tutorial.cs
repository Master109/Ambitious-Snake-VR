using UnityEngine;
using UnityEngine.Playables;

namespace AmbitiousSnake
{
	public class Tutorial : SingletonUpdateWhileEnabled<Tutorial>
	{
		public PlayableDirector playableDirector;
		public GameObject[] activateOnFinish;

		public static bool IsLookingAtTransform (Transform trs, float shrinkCameraViewNormalized)
		{
			Vector3 viewportPoint = Camera.main.WorldToViewportPoint(trs.position);
			shrinkCameraViewNormalized /= 2;
			return viewportPoint.x >= shrinkCameraViewNormalized && viewportPoint.x <= 1f - shrinkCameraViewNormalized && viewportPoint.y >= shrinkCameraViewNormalized && viewportPoint.y <= 1f - shrinkCameraViewNormalized;
		}

		public virtual void Finish ()
		{
			gameObject.SetActive(false);
			for (int i = 0; i < activateOnFinish.Length; i ++)
			{
				GameObject go = activateOnFinish[i];
				go.SetActive(true);
			}
		}
	}
}