using UnityEngine;

namespace AmbitiousSnake
{
	public class Tutorial : UpdateWhileEnabled
	{
		public static bool IsLookingAtTransform (Transform trs, float shrinkCameraViewNormalized)
		{
			Vector3 viewportPoint = Camera.main.WorldToViewportPoint(trs.position);
			shrinkCameraViewNormalized /= 2;
			return viewportPoint.x >= shrinkCameraViewNormalized && viewportPoint.x <= 1f - shrinkCameraViewNormalized && viewportPoint.y >= shrinkCameraViewNormalized && viewportPoint.y <= 1f - shrinkCameraViewNormalized;
		}
	}
}