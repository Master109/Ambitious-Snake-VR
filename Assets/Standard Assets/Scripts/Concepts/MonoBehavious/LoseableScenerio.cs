using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AmbitiousSnake
{
	public class LoseableScenerio : SingletonMonoBehaviour<LoseableScenerio>
	{
		public static List<LoseableScenerio> activeScenarios = new List<LoseableScenerio>();
		
		public virtual void OnEnable ()
		{
			activeScenarios.Add(this);
		}
		
		public virtual void OnDisable ()
		{
			activeScenarios.Remove(this);
		}
		
		public virtual void Lose ()
		{
			_SceneManager.Instance.LoadSceneWithTransition ("Game Over");
		}
	}
}