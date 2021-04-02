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

		public void OnCollisionEnter (Collision coll)
		{
			Rigidbody rigid = coll.gameObject.GetComponentInParent<Rigidbody>();
			if (stuckRigids.Contains(rigid))
				return;
			stuckRigids.Add(rigid);
			// rigid.isKinematic = true;
			rigid.useGravity = false;
			rigid.constraints = RigidbodyConstraints.FreezeAll;
		}

		void OnCollisionStay (Collision coll)
		{
			OnCollisionEnter (coll);
		}

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			for (int i = 0; i < stuckRigids.Count; i ++)
			{
				Rigidbody stuckRigid = stuckRigids[i];
				bool isHittingRigid = false;
				Collider[] hits = Physics.OverlapBox(trs.position, trs.lossyScale / 2 + Vector3.one * Physics.defaultContactOffset, trs.rotation);
				for (int i2 = 0; i2 < hits.Length; i2 ++)
				{
					Collider hit = hits[i2];
					if (hit.GetComponentInParent<Rigidbody>() == stuckRigid)
					{
						isHittingRigid = true;
						break;
					}
				}
				if (!isHittingRigid)
				{
					// stuckRigid.isKinematic = false;
					stuckRigid.constraints = RigidbodyConstraints.None;
					stuckRigid.useGravity = true;
					stuckRigids.RemoveAt(i);
					i --;
				}
			}
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}