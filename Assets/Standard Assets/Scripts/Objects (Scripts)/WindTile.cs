using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class WindTile : MonoBehaviour
	{
		public Plane pushStartPlane;
		public Transform pushDirectionTrs;
		public float forceAmount;

		void OnTriggerStay (Collider other)
		{
			Rigidbody rigid = other.GetComponentInParent<Rigidbody>();
			if (rigid != null)
				rigid.AddForce(pushDirectionTrs.forward * forceAmount * GetAffectedArea(other), ForceMode.Impulse);
		}

		float GetAffectedArea (Collider collider)
		{
			SphereCollider sphereCollider = collider as SphereCollider;
			if (sphereCollider != null)
			{
				Vector3 sphereCastStart = pushStartPlane.ClosestPointOnPlane(sphereCollider.bounds.center);
				float sphereCastDistance = 0;
				if (pushStartPlane.GetSide(sphereCollider.bounds.center))
					sphereCastDistance = Vector3.Distance(sphereCastStart, sphereCollider.bounds.center);
				RaycastHit[] hits = Physics.SphereCastAll(sphereCastStart, sphereCollider.bounds.extents.x, pushDirectionTrs.forward, sphereCastDistance);
				for (int i = 0; i < hits.Length; i ++)
				{
					RaycastHit hit = hits[i];
					
				}
				return 0;
			}
			else
			{
				return 0;
			}
		}
	}
}