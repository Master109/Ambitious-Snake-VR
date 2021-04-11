using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace AmbitiousSnake
{
	public class AcceleratorTile : Tile, ICollisionEnterHandler, IUpdatable
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		List<Rigidbody> touchingRigids = new List<Rigidbody>();
		public float changeMaterialOffsetRate;
		public Material material;
        public float forceAmount;

		public void OnCollisionEnter (Collision coll)
		{
			Rigidbody rigid = coll.gameObject.GetComponentInParent<Rigidbody>();
			if (touchingRigids.Contains(rigid))
				return;
			touchingRigids.Add(rigid);
		}

		void OnCollisionStay (Collision coll)
		{
			OnCollisionEnter (coll);
		}

		public override void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			if (material != null)
				material.mainTextureOffset += Vector2.down * changeMaterialOffsetRate * Time.deltaTime;
			for (int i = 0; i < touchingRigids.Count; i ++)
			{
				Rigidbody touchingRigid = touchingRigids[i];
				bool isHittingRigid = false;
				Collider[] hits = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents + Vector3.one * Physics.defaultContactOffset);
				for (int i2 = 0; i2 < hits.Length; i2 ++)
				{
					Collider hit = hits[i2];
					if (hit.GetComponentInParent<Rigidbody>() == touchingRigid)
					{
						isHittingRigid = true;
						break;
					}
				}
				if (isHittingRigid)
				{
					touchingRigid.AddForce(trs.forward * forceAmount * Time.deltaTime, ForceMode.Impulse);
				}
                else
                {
                    touchingRigids.RemoveAt(i);
					i --;
                }
			}
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
			if (material != null)
				material.mainTextureOffset = Vector2.zero;
		}
	}
}