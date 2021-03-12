using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using AmbitiousSnake;
using UnityEngine.InputSystem;
using Extensions;
using Unity.XR.Oculus.Input;

namespace AmbitiousSnake
{
	public class Cinematic : SingletonMonoBehaviour<Cinematic>, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public VideoPlayer video;
		public string LoadSceneOnDone;
		public Animation[] skipNotifyAnims;
		public float skipAfterTime;
		public float playSkipNotifyAnimsInterval;
		float skipTimer;
		InputAction anyInputAction;

		void OnEnable ()
		{
			anyInputAction = new InputAction(binding: "/*/<button>");
			anyInputAction.performed += ShowSkipNotification;
			anyInputAction.Enable();
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void DoUpdate ()
		{
			if (InputManager.SkipCinematicInput)
				skipTimer += Time.deltaTime;
			else
				skipTimer = 0;
			if (Time.timeSinceLevelLoad > video.frameCount * (1f / video.frameRate) / video.playbackSpeed || skipTimer > skipAfterTime)
			{
				enabled = false;
				_SceneManager.Instance.mostRecentSceneName = LoadSceneOnDone;
				_SceneManager.Instance.LoadSceneWithTransition (LoadSceneOnDone);
			}
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
			anyInputAction.Disable();
			anyInputAction.performed -= ShowSkipNotification;
		}

		void ShowSkipNotification (InputAction.CallbackContext context)
		{
			if (context.control.device != InputManager.Hmd)
			{
				bool isPlayingAnim = false;
				foreach (Animation anim in skipNotifyAnims)
				{
					if (anim.isPlaying)
					{
						isPlayingAnim = true;
						break;
					}
				}
				if (!isPlayingAnim)
				{
					foreach (Animation anim in skipNotifyAnims)
					{
						if (anim.gameObject.activeInHierarchy)
							anim.Play();
					}
				}
			}
			anyInputAction.Disable();
			anyInputAction.performed -= ShowSkipNotification;
			StartCoroutine(DelaySetAnyInputActionRoutine ());
		}

		IEnumerator DelaySetAnyInputActionRoutine ()
		{
			yield return new WaitForSecondsRealtime(playSkipNotifyAnimsInterval);
			anyInputAction = new InputAction(binding: "/*/<button>");
			anyInputAction.performed += ShowSkipNotification;
			anyInputAction.Enable();
		}
	}
}