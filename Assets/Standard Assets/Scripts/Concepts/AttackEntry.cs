using UnityEngine;
using System;
using System.Collections;

namespace AmbitiousSnake
{
	[Serializable]
	public class AttackEntry
	{
		public BulletPattern3D BulletPattern3D;
		public int attackOnAnimationFrameIndex;
		public Bullet bulletPrefab;
		public Transform spawner;
		
		public virtual void Attack ()
		{
			BulletPattern3D.Shoot (spawner, bulletPrefab);
		}
	}
}