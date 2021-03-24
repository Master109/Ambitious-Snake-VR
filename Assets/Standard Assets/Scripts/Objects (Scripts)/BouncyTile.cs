using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AmbitiousSnake
{
	public class BouncyTile : Tile, ICollisionEnterHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		public float forceAmount;

		public void OnCollisionEnter (Collision coll)
		{
			Rigidbody rigid = coll.gameObject.GetComponentInParent<Rigidbody>();
			Vector3 forceDirection = new Vector3();
			ContactPoint[] contactPoints = new ContactPoint[coll.contactCount];
			coll.GetContacts(contactPoints);
			for (int i = 0; i < coll.contactCount; i ++)
				forceDirection -= contactPoints[i].normal;
			print(forceDirection + " " + coll.contactCount);
			rigid.AddForce(forceDirection.normalized * forceAmount, ForceMode.Impulse);
		}

		void OnCollisionStay (Collision coll)
		{
			OnCollisionEnter (coll);
		}
	}
}