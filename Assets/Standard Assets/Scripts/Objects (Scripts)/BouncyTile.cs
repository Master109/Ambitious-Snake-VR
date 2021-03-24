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
		public float bounceCooldown;
		List<Rigidbody> bouncingRigids = new List<Rigidbody>();

		public void OnCollisionEnter (Collision coll)
		{
			Rigidbody rigid = coll.gameObject.GetComponentInParent<Rigidbody>();
			if (bouncingRigids.Contains(rigid))
				return;
			Vector3 forceDirection = new Vector3();
			ContactPoint[] contactPoints = new ContactPoint[coll.contactCount];
			coll.GetContacts(contactPoints);
			for (int i = 0; i < coll.contactCount; i ++)
				forceDirection -= contactPoints[i].normal;
			bouncingRigids.Add(rigid);
			StartCoroutine(BounceRoutine (rigid, forceDirection));
		}

		IEnumerator BounceRoutine (Rigidbody rigid, Vector3 forceDirection)
		{
			rigid.AddForce(forceDirection.normalized * forceAmount, ForceMode.Impulse);
			yield return new WaitForSeconds(bounceCooldown);
			bouncingRigids.Remove(rigid);
		}

		void OnCollisionStay (Collision coll)
		{
			OnCollisionEnter (coll);
		}
	}
}