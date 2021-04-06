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

		public override void Awake ()
		{
			base.Awake ();
			isLoading = false;
		}
		
		public void LoadSceneWithTransition (string levelName)
		{
			if (Instance != this)
			{
				instance.LoadSceneWithTransition (levelName);
				return;
			}
			isLoading = true;
			StartCoroutine (SceneTransition (levelName));
		}
		
		public void LoadSceneWithoutTransition (string levelName)
		{
			isLoading = true;
			SceneManager.LoadScene(levelName);
		}
		
		public void LoadSceneWithTransition (int levelId)
		{
			if (Instance != this)
			{
				instance.LoadSceneWithTransition (levelId);
				return;
			}
			isLoading = true;
			StartCoroutine (SceneTransition (levelId));
		}
		
		public void LoadSceneWithoutTransition (int levelId)
		{
			isLoading = true;
			SceneManager.LoadScene(levelId);
		}
		
		public void LoadSceneAdditiveWithTransition (string levelName)
		{
			if (Instance != this)
			{
				Instance.LoadSceneAdditiveWithTransition (levelName);
				return;
			}
			isLoading = true;
			StartCoroutine (SceneTransition (levelName, LoadSceneMode.Additive));
		}
		
		public void LoadSceneAdditiveWithoutTransition (string levelName)
		{
			isLoading = true;
			SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
		}

		public void LoadSceneAdditiveWithTransition (int levelId)
		{
			if (Instance != this)
			{
				Instance.LoadSceneAdditiveWithTransition (levelId);
				return;
			}
			isLoading = true;
			StartCoroutine (SceneTransition (levelId, LoadSceneMode.Additive));
		}
		
		public void LoadSceneAdditiveWithoutTransition (int levelId)
		{
			isLoading = true;
			SceneManager.LoadScene(levelId, LoadSceneMode.Additive);
		}
		
		public IEnumerator LoadSceneAsyncAdditiveWithoutTransition (string levelName)
		{
			isLoading = true;
			yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
		}
		
		public IEnumerator LoadSceneAsyncAdditiveWithoutTransition (int levelId)
		{
			isLoading = true;
			yield return SceneManager.LoadSceneAsync(levelId, LoadSceneMode.Additive);
		}
		
		public void RestartSceneWithTransition ()
		{
			LoadSceneWithTransition (SceneManager.GetActiveScene().name);
		}
		
		public void RestartSceneWithoutTransition ()
		{
			LoadSceneWithoutTransition (SceneManager.GetActiveScene().name);
		}
		
		public void NextSceneWithTransition ()
		{
			LoadSceneWithTransition (SceneManager.GetActiveScene().buildIndex + 1);
		}
		
		public void NextSceneWithoutTransition ()
		{
			LoadSceneWithoutTransition (SceneManager.GetActiveScene().buildIndex + 1);
		}
		
		public void OnSceneLoaded (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			Camera.main.rect = new Rect(.5f, .5f, 0, 0);
			StartCoroutine(SceneTransition (null));
			SceneManager.sceneLoaded -= OnSceneLoaded;
			isLoading = false;
			mostRecentSceneName = scene.name;
		}
		
		public IEnumerator SceneTransition (string levelName = null, LoadSceneMode loadMode = LoadSceneMode.Single)
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

		public IEnumerator SceneTransition (int levelId = -1, LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			yield return StartCoroutine(SceneTransition (SceneManager.GetSceneByBuildIndex(levelId).name, loadMode));
		}
	}
}