using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class Hazard : Spawnable
	{
		public float damage;
		
		public virtual void OnCollisionEnter (Collision coll)
		{
			IDestructable destructable = coll.collider.GetComponentInParent<IDestructable>();
			if (destructable != null)
				ApplyDamage (destructable, damage);
		}
		
		public virtual void ApplyDamage (IDestructable destructable, float amount)
		{
			destructable.TakeDamage (amount, this);
		}
	}
}