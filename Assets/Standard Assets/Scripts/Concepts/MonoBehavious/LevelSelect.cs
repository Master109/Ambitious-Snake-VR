using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

namespace AmbitiousSnake
{
	public class LevelSelect : SingletonMonoBehaviour<LevelSelect>
	{
		public Transform trs;
        public Level[] levels = new Level[0];
		public Button startLevelButton;
		public TMP_Text infoText;
		public static int selectedLevelIndex;
		Coroutine startLevelPreviewCoroutine;
		Coroutine stopLevelPreviewCoroutine;

		void OnEnable ()
		{
			for (int i = 0; i < VR_UIPointer.instances.Length; i ++)
			{
				VR_UIPointer vR_UIPointer = VR_UIPointer.instances[i];
				vR_UIPointer.uiPlaneTrs = trs;
			}
			selectedLevelIndex = MathfExtensions.NULL_INT;
			int completedLevelCount = 0;
			int parTimeCount = 0;
			int starCount = 0;
			int levelCount = levels.Length;
			for (int i = 0; i < levelCount; i ++)
			{
				Level level = levels[i];
				if (level.HasWon)
					completedLevelCount ++;
				if (level.GotParTime)
					parTimeCount ++;
				if (level.CollectedStar)
					starCount ++;
			}
			infoText.text = "Current Level:" + "\n" + (SceneManager.GetActiveScene().buildIndex + 1) + "\n\n" + 
				"Levels Complete:" + "\n" + completedLevelCount + "/" + levelCount + "\n\n" + 
				"Par Times:" + "\n" + parTimeCount + "/" + levelCount + "\n\n" + 
				"Stars:" + "\n" + starCount + "/" + levelCount + "\n\n" + 
				"Score:" + "\n" + (AccountManager.CurrentAccount.score) + "/" + (levelCount * 3);
		}

		public void StartLevelPreview (int index)
		{
			if (startLevelPreviewCoroutine != null)
				LevelSelect.instance.StopCoroutine(startLevelPreviewCoroutine);
			if (stopLevelPreviewCoroutine != null)
				LevelSelect.instance.StopCoroutine(stopLevelPreviewCoroutine);
			stopLevelPreviewCoroutine = null;
			startLevelPreviewCoroutine = StartCoroutine(StartLevelPreviewRoutine (index));
		}

		IEnumerator StartLevelPreviewRoutine (int index)
		{
			yield return new WaitUntil(() => (SceneManager.sceneCount == 1 && stopLevelPreviewCoroutine == null));
			if (index == SceneManager.GetActiveScene().buildIndex)
				Level.instance.gameObject.SetActive(true);
			else
			{
				GameplayMenu.instance.trs.SetParent(null);
				Level.instance.gameObject.SetActive(false);
				yield return _SceneManager.instance.LoadSceneAsyncAdditiveWithoutTransition(index);
			}
			startLevelPreviewCoroutine = null;
		}

		public void StopLevelPreview (int index)
		{
			if (index != SceneManager.GetActiveScene().buildIndex && SceneManager.sceneCount == 2 && SceneManager.GetSceneByBuildIndex(index).isLoaded)
			{
				if (startLevelPreviewCoroutine != null)
					LevelSelect.instance.StopCoroutine(startLevelPreviewCoroutine);
				startLevelPreviewCoroutine = null;
				if (stopLevelPreviewCoroutine != null)
					LevelSelect.instance.StopCoroutine(stopLevelPreviewCoroutine);
				stopLevelPreviewCoroutine = StartCoroutine(StopLevelPreviewRoutine (index));
			}
		}

		IEnumerator StopLevelPreviewRoutine (int index)
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			yield return SceneManager.UnloadSceneAsync(index, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
			Level.instance.gameObject.SetActive(true);
			stopLevelPreviewCoroutine = null;
		}

		void OnSceneLoaded (Scene scene = new Scene(), LoadSceneMode loadSceneMode = LoadSceneMode.Single)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			Snake.instance = Snake.Instance;
			InputSystem.Update();
			VRCameraRig.instance.DoUpdate ();
			GameplayMenu.instance.SetOrientation ();
			GameplayMenu.instance.gameObject.SetActive(true);
		}
	}
}