using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AmbitiousSnake;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnityEngine.InputSystem;

namespace AmbitiousSnake
{
	public class GameManager : SingletonMonoBehaviour<GameManager>, ISaveableAndLoadable
	{
		public GameObject[] registeredGos = new GameObject[0];
		public static bool paused;
		public static IUpdatable[] updatables = new IUpdatable[0];
		public static int framesSinceLevelLoaded;
		public static bool isQuittingGame;
		[SaveAndLoadValue]
		static string enabledGosString = "";
		[SaveAndLoadValue]
		static string disabledGosString = "";
		public TileParent tileParentPrefab;
		public GameModifier[] gameModifiers = new GameModifier[0];
		public static Dictionary<string, GameModifier> gameModifierDict = new Dictionary<string, GameModifier>();
		bool leftGameplayMenuInput;
		bool previousLeftGameplayMenuInput;
		bool rightGameplayMenuInput;
		bool previousRightGameplayMenuInput;
		bool gameplayMenuInput;
		bool previousGameplayMenuInput;

		public override void Awake ()
		{
			base.Awake ();
			if (instance != this)
				return;
			gameModifierDict.Clear();
			foreach (GameModifier gameModifier in gameModifiers)
				gameModifierDict.Add(gameModifier.name, gameModifier);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		void OnDestroy ()
		{
			if (instance == this)
				SceneManager.sceneLoaded -= OnSceneLoaded;
		}
		
		void OnSceneLoaded (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			StopAllCoroutines();
			framesSinceLevelLoaded = 0;
		}

		void Update ()
		{
			Physics.Simulate(Time.deltaTime);
			for (int i = 0; i < updatables.Length; i ++)
			{
				IUpdatable updatable = updatables[i];
				updatable.DoUpdate ();
			}
			if (ObjectPool.Instance != null && ObjectPool.instance.enabled)
				ObjectPool.instance.DoUpdate ();
			InputSystem.Update ();
			leftGameplayMenuInput = InputManager.LeftGameplayMenuInput;
			rightGameplayMenuInput = InputManager.RightGameplayMenuInput;
			gameplayMenuInput = InputManager.GameplayMenuInput;
			HandleGameplayMenu ();
			HandleRestart ();
			framesSinceLevelLoaded ++;
			previousLeftGameplayMenuInput = leftGameplayMenuInput;
			previousRightGameplayMenuInput = rightGameplayMenuInput;
			previousGameplayMenuInput = gameplayMenuInput;
		}

		void HandleGameplayMenu ()
		{
			if (GameplayMenu.instance == null || GameplayMenu.instance.gameObject.activeSelf)
				return;
			if (leftGameplayMenuInput && !previousLeftGameplayMenuInput)
				GameplayMenu.selectorTrs = VRCameraRig.instance.leftHandTrs;
			else if (rightGameplayMenuInput && !previousRightGameplayMenuInput)
				GameplayMenu.selectorTrs = VRCameraRig.instance.rightHandTrs;
			else if (gameplayMenuInput && !previousGameplayMenuInput)
				GameplayMenu.selectorTrs = VRCameraRig.instance.eyesTrs;
			else
				return;
			GameplayMenu.instance.SetOrientation ();
			GameplayMenu.instance.gameObject.SetActive(true);
		}

		void HandleRestart ()
		{
			if (InputManager.RestartInput)
				_SceneManager.instance.RestartSceneWithoutTransition ();
		}
		
		public void Quit ()
		{
			Application.Quit();
		}

		public static void Log (object obj)
		{
			print(obj);
		}

		void OnApplicationQuit ()
		{
			// PlayerPrefs.DeleteAll();
			isQuittingGame = true;
		}
		
		public static bool ModifierExistsAndIsActive (string name)
		{
			GameModifier gameModifier;
			if (gameModifierDict.TryGetValue(name, out gameModifier))
				return gameModifier.isActive;
			else
				return false;
		}

		public static bool ModifierIsActive (string name)
		{
			return gameModifierDict[name].isActive;
		}

		public static bool ModifierExists (string name)
		{
			return gameModifierDict.ContainsKey(name);
		}

		[Serializable]
		public class GameModifier
		{
			public string name;
			public bool isActive;
		}
	}
}