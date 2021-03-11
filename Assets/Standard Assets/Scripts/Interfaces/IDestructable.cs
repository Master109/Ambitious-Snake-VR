using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public interface IDestructable
	{
		float Hp { get; set; }
		int MaxHp { get; set; }
		
		void TakeDamage (float amount, Hazard source);
		void Death ();
	}
}