using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AmbitiousSnake
{
	public class End : Tile, ICollisionEnterHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		
		public void OnCollisionEnter (Collision coll)
		{
			if (Snake.instance.rigid == coll.rigidbody)
				_SceneManager.instance.NextSceneWithoutTransition ();
		}
	}
}