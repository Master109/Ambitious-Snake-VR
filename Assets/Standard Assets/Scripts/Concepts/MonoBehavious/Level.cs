using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using TMPro;

namespace AmbitiousSnake
{
	public class Level : SingletonMonoBehaviour<Level>, IUpdatable
	{
		public TMP_Text timerText;
		
		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public void DoUpdate ()
		{
			timerText.text = string.Format("{0:0.#}", Time.timeSinceLevelLoad);
		}
	}
}