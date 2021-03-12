using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AmbitiousSnake
{
	public class End : SingletonMonoBehaviour<End>
	{
		void OnCollisionEnter (Collision coll)
		{
			if (Snake.instance.rigid == coll.rigidbody)
				_SceneManager.instance.NextSceneWithoutTransition ();
		}
	}
}