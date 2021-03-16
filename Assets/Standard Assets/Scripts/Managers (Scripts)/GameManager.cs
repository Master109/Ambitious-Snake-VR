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
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		public int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}
		public TileParent tileParentPrefab;

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
			HandleRestart ();
			framesSinceLevelLoaded ++;
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