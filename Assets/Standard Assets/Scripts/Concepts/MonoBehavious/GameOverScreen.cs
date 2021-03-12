using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using AmbitiousSnake;

public class GameOverScreen : SingletonMonoBehaviour<GameOverScreen>
{
	public virtual void GameOver ()
	{
		_SceneManager.Instance.LoadSceneWithTransition (_SceneManager.Instance.mostRecentSceneName);
	}
}
