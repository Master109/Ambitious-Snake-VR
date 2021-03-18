using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AmbitiousSnake
{
	public class StickyTile : Tile, ICollisionEnterHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		public Dictionary<Rigidbody, StuckPoint> stuckPoints = new Dictionary<Rigidbody, StuckPoint>();

		public void OnCollisionEnter (Collision coll)
		{
			for (int i = 0; i < coll.contactCount; i ++)
			{
				StuckPoint stuckPoint = new StuckPoint();
				stuckPoint.fixedJoint = gameObject.AddComponent<FixedJoint>();
				stuckPoint.fixedJoint.connectedBody = coll.gameObject.GetComponentInParent<Rigidbody>();
				if (stuckPoint.fixedJoint.connectedBody)
				stuckPoints.Add(stuckPoint.fixedJoint.connectedBody, stuckPoint);
			}
		}

		void OnSnakeChangeLength ()
		{
			
		}

		void OnCollisionExit (Collision coll)
		{
			Rigidbody rigid = coll.gameObject.GetComponentInParent<Rigidbody>();
			StuckPoint stuckPoint;
			if (stuckPoints.TryGetValue(rigid, out stuckPoint))
			{
				stuckPoints.Remove(rigid);
				Destroy(stuckPoint.fixedJoint);
			}
		}

		public class StuckPoint
		{
			public FixedJoint fixedJoint;
		}

		public class SnakeStuckPoint : StuckPoint
		{
			public float stuckAtLengthTraveled;

			public SnakeStuckPoint (float stuckAtLengthTraveled)
			{
				this.stuckAtLengthTraveled = stuckAtLengthTraveled;
			}
		}
	}
}