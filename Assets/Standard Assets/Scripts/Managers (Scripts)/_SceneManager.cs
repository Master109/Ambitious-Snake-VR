using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AmbitiousSnake
{
	public class _SceneManager : SingletonMonoBehaviour<_SceneManager>, ISaveableAndLoadable
	{
		public static bool isLoading;
		public float transitionRate;
		[SaveAndLoadValue]
		public string mostRecentSceneName;
		public static Scene CurrentScene
		{
			get
			{
				return SceneManager.GetActiveScene();
			}
		}
		
		public void LoadSceneWithTransition (string levelName)
		{
			if (_SceneManager.Instance != this)
			{
				_SceneManager.Instance.LoadSceneWithTransition (levelName);
				return;
			}
			isLoading = true;
			Time.timeScale = 1;
			StartCoroutine (SceneTransition (levelName));
		}
		
		public void LoadSceneWithoutTransition (string levelName)
		{
			isLoading = true;
			Time.timeScale = 1;
			SceneManager.LoadScene(levelName);
		}
		
		public void LoadSceneWithTransition (int levelId)
		{
			if (_SceneManager.Instance != this)
			{
				_SceneManager.Instance.LoadSceneWithTransition (levelId);
				return;
			}
			isLoading = true;
			Time.timeScale = 1;
			StartCoroutine (SceneTransition (levelId));
		}
		
		public void LoadSceneWithoutTransition (int levelId)
		{
			isLoading = true;
			Time.timeScale = 1;
			SceneManager.LoadScene(levelId);
		}
		
		public void LoadSceneAdditiveWithTransition (string levelName)
		{
			if (_SceneManager.Instance != this)
			{
				_SceneManager.Instance.LoadSceneAdditiveWithTransition (levelName);
				return;
			}
			isLoading = true;
			Time.timeScale = 1;
			StartCoroutine (SceneTransition (levelName, LoadSceneMode.Additive));
		}
		
		public void LoadSceneAdditiveWithoutTransition (string levelName)
		{
			isLoading = true;
			Time.timeScale = 1;
			SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
		}
		
		public void RestartSceneWithTransition ()
		{
			isLoading = true;
			LoadSceneWithTransition (SceneManager.GetActiveScene().name);
		}
		
		public void RestartSceneWithoutTransition ()
		{
			isLoading = true;
			LoadSceneWithoutTransition (SceneManager.GetActiveScene().name);
		}
		
		public void NextSceneWithTransition ()
		{
			isLoading = true;
			LoadSceneWithTransition (SceneManager.GetActiveScene().buildIndex + 1);
		}
		
		public void NextSceneWithoutTransition ()
		{
			isLoading = true;
			LoadSceneWithoutTransition (SceneManager.GetActiveScene().buildIndex + 1);
		}
		
		public virtual void OnSceneLoaded (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			Camera.main.rect = new Rect(.5f, .5f, 0, 0);
			StartCoroutine(SceneTransition (null));
			SceneManager.sceneLoaded -= OnSceneLoaded;
			isLoading = false;
		}
		
		public virtual IEnumerator SceneTransition (string levelName = null, LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			bool transitioningIn = string.IsNullOrEmpty(levelName);
			float transitionRateMultiplier = 1;
			if (transitioningIn)
				transitionRateMultiplier *= -1;
			while ((Camera.main.rect.size.x > 0 && !transitioningIn) || (Camera.main.rect.size.x < 1 && transitioningIn))
			{
				Rect cameraRect = Camera.main.rect;
				cameraRect.size -= Vector2.one * transitionRate * transitionRateMultiplier * Time.unscaledDeltaTime;
				cameraRect.center += Vector2.one * transitionRate * transitionRateMultiplier * Time.unscaledDeltaTime / 2;
				Camera.main.rect = cameraRect;
				yield return new WaitForEndOfFrame();
			}
			if (transitioningIn)
				Camera.main.rect = new Rect(0, 0, 1, 1);
			else
			{
				Camera.main.rect = new Rect(.5f, .5f, 0, 0);
				SceneManager.sceneLoaded += OnSceneLoaded;
				if (!string.IsNullOrEmpty(levelName))
					SceneManager.LoadScene(levelName, loadMode);
			}
		}

		public virtual IEnumerator SceneTransition (int levelId = -1, LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			yield return StartCoroutine(SceneTransition (SceneManager.GetSceneByBuildIndex(levelId).name, loadMode));
		}
	}
}