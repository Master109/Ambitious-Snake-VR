using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class Hazard : Spawnable, ICollisionEnterHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		public float damage;
		public bool destroyOnContact;
		
		public virtual void OnCollisionEnter (Collision coll)
		{
			IDestructable destructable = coll.rigidbody.GetComponent<IDestructable>();
			if (destructable != null)
				ApplyDamage (destructable, damage);
			if (destroyOnContact)
			{
				if (prefabIndex == -1)
					Destroy(gameObject);
				else
					ObjectPool.Instance.Despawn (prefabIndex, gameObject, trs);
			}
		}
		
		public virtual void ApplyDamage (IDestructable destructable, float amount)
		{
			destructable.TakeDamage (amount, this);
		}
	}
}