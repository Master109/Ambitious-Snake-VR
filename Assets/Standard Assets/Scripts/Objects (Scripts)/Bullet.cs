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
			rangedDespawn = ObjectPool.Instance.RangeDespawn(prefabIndex, gameObject, trs, range);
			rigid.velocity = trs.forward * moveSpeed;
		}

		void OnDisable ()
		{
			StopAllCoroutines();
			Physics.IgnoreCollision(collider, shooter.collider, false);
			dead = true;
		}

		void OnDestroy ()
		{
			ObjectPool.Instance.CancelRangedDespawn (rangedDespawn);
			Physics.IgnoreCollision(collider, shooter.collider, false);
			dead = true;
		}

		public override void OnCollisionEnter (Collision coll)
		{
			if (!dead)
			{
				base.OnCollisionEnter (coll);
				ObjectPool.Instance.Despawn (prefabIndex, gameObject, trs);
			}
		}
	}
}