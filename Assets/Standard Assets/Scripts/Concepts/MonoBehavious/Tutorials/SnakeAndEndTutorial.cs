using UnityEngine;
using Extensions;
using UnityEngine.Playables;

namespace AmbitiousSnake
{
	public class SnakeAndEndTutorial : Tutorial
	{
		public RectTransform canvasRectTrs;
		public RectTransform snakeIndicatorRectTrs;
		public RectTransform endIndicatorRectTrs;
		public float shrinkCameraViewNormalized;

		public override void OnEnable ()
		{
			if (!EnableTutorials)
				Destroy(gameObject);
			playableDirector.stopped += OnPlayableDirectorStopped;
		}

		void OnPlayableDirectorStopped (PlayableDirector playableDirector)
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			playableDirector.stopped -= OnPlayableDirectorStopped;
		}
		
		public override void DoUpdate ()
		{
			if (snakeIndicatorRectTrs.gameObject.activeSelf)
			{
				snakeIndicatorRectTrs.rotation = Quaternion.LookRotation(VRCameraRig.instance.eyesTrs.forward, Snake.instance.HeadPosition - snakeIndicatorRectTrs.position);
				Vector2 snakeViewportOffset = (Vector2) Camera.main.WorldToViewportPoint(Snake.instance.HeadPosition) - (Vector2.one / 2);
				Vector2 snakeCanvasOffset = snakeViewportOffset.Multiply(canvasRectTrs.sizeDelta);
				snakeIndicatorRectTrs.sizeDelta = snakeIndicatorRectTrs.sizeDelta.SetY(snakeCanvasOffset.magnitude);
				if (IsLookingAtTransform(Snake.instance.HeadPiece.trs, shrinkCameraViewNormalized))
				{
					snakeIndicatorRectTrs.gameObject.SetActive(false);
					if (!endIndicatorRectTrs.gameObject.activeSelf)
						Finish ();
				}
			}
			if (endIndicatorRectTrs.gameObject.activeSelf)
			{
				endIndicatorRectTrs.rotation = Quaternion.LookRotation(VRCameraRig.instance.eyesTrs.forward, End.instance.trs.position - endIndicatorRectTrs.position);
				Vector2 endViewportOffset = (Vector2) Camera.main.WorldToViewportPoint(End.instance.trs.position) - (Vector2.one / 2);
				Vector2 endCanvasOffset = endViewportOffset.Multiply(canvasRectTrs.sizeDelta);
				endIndicatorRectTrs.sizeDelta = endIndicatorRectTrs.sizeDelta.SetY(endCanvasOffset.magnitude);
				if (IsLookingAtTransform(End.instance.trs, shrinkCameraViewNormalized))
				{
					endIndicatorRectTrs.gameObject.SetActive(false);
					if (!snakeIndicatorRectTrs.gameObject.activeSelf)
						Finish ();
				}
			}
		}
	}
}