using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace AmbitiousSnake
{
	public class StickyTile : Tile, ICollisionEnterHandler, IUpdatable
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		List<Rigidbody> stuckRigids = new List<Rigidbody>();
		List<Rigidbody> previousStuckRigids = new List<Rigidbody>();

		public void OnCollisionEnter (Collision coll)
		{
			Rigidbody rigid = coll.gameObject.GetComponentInParent<Rigidbody>();
			stuckRigids.Add(rigid);
			rigid.isKinematic = true;
		}

		void OnCollisionStay (Collision coll)
		{
			OnCollisionEnter (coll);
		}

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void DoUpdate ()
		{
			for (int i = 0; i < previousStuckRigids.Count; i ++)
			{
				Rigidbody previousStuckRigid = previousStuckRigids[i];
				if (!stuckRigids.Contains(previousStuckRigid))
					previousStuckRigid.isKinematic = false;
			}
			previousStuckRigids = new List<Rigidbody>(stuckRigids);
			stuckRigids.Clear();
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}