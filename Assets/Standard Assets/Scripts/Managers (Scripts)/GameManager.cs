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
		bool leftGameplayMenuInput;
		bool previousLeftGameplayMenuInput;
		bool rightGameplayMenuInput;
		bool previousRightGameplayMenuInput;
		bool gameplayMenuInput;
		bool previousGameplayMenuInput;

		public override void Awake ()
		{
			base.Awake ();
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		void OnDestroy ()
		{
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
			foreach (IUpdatable updatable in updatables)
				updatable.DoUpdate ();
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
			if (GameplayMenu.instance.gameObject.activeSelf || !GameplayMenu.instance.interactive)
				return;
			if (leftGameplayMenuInput && !previousLeftGameplayMenuInput)
				GameplayMenu.instance.selectorTrs = VRCameraRig.instance.leftHandTrs;
			else if (rightGameplayMenuInput && !previousRightGameplayMenuInput)
				GameplayMenu.instance.selectorTrs = VRCameraRig.instance.rightHandTrs;
			else if (gameplayMenuInput && !previousGameplayMenuInput)
				GameplayMenu.instance.selectorTrs = VRCameraRig.instance.eyesTrs;
			else
				return;
			GameplayMenu.instance.trs.position = VRCameraRig.instance.eyesTrs.position + (VRCameraRig.instance.eyesTrs.forward * GameplayMenu.instance.distanceFromCamera);
			GameplayMenu.instance.trs.rotation = VRCameraRig.instance.eyesTrs.rotation;
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
	}
}