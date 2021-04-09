using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class Bullet : Hazard
	{
		public float range;
		public Rigidbody rigid;
		public float moveSpeed;
		public Turret shooter;
		public ObjectPool.RangedDespawn rangedDespawn;
		public new Collider collider;
		[HideInInspector]
		public bool dead;
		
		void OnEnable ()
		{
			dead = false;
			rangedDespawn = ObjectPool.instance.RangeDespawn(prefabIndex, gameObject, trs, range);
			rigid.velocity = trs.forward * moveSpeed;
		}

		void OnDisable ()
		{
			StopAllCoroutines();
			if (ObjectPool.instance != null)
				ObjectPool.instance.CancelRangedDespawn (rangedDespawn);
			if (shooter != null)
			{
				Physics.IgnoreCollision(collider, shooter.collider, false);
				Physics.IgnoreCollision(collider, shooter.suspensionRodCollider, false);
			}
			dead = true;
		}

		public override void OnCollisionEnter (Collision coll)
		{
			if (!dead)
			{
				base.OnCollisionEnter (coll);
				ObjectPool.instance.Despawn (prefabIndex, gameObject, trs);
			}
		}
	}
}