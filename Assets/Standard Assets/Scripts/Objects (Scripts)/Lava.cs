using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class Lava : Hazard
	{
		void OnTriggerEnter (Collider other)
		{
			IDestructable destructable = other.GetComponentInParent<IDestructable>();
			if (destructable != null)
				ApplyDamage (destructable, damage);
		}
	}
}