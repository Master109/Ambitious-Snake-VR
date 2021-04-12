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
			{
				if (Star.instance.isCollected && !Star.instance.CollectedAndWonLevel)
				{
					Star.instance.CollectedAndWonLevel = true;
					AccountManager.CurrentAccount.score ++;
				}
				if (!Level.instance.HasWon)
				{
					Level.instance.HasWon = true;
					AccountManager.CurrentAccount.score ++;
				}
				float timeSinceLevelLoad = Time.timeSinceLevelLoad;
				if (Level.instance.FastestTime > timeSinceLevelLoad)
				{
					if (timeSinceLevelLoad <= Level.instance.parTime && Level.instance.FastestTime > Level.instance.parTime)
						AccountManager.CurrentAccount.score ++;
					Level.instance.FastestTime = timeSinceLevelLoad;
				}
				_SceneManager.instance.NextSceneWithoutTransition ();
			}
		}
	}
}