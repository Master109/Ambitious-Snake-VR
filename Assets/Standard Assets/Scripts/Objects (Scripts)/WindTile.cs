using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class WindTile : MonoBehaviour
	{
		public Transform pushDirectionTrs;
		public LayerMask whatIPush;
		public float forceAmountPerHit;
		[Range(2, 20)]
		public int raycastsPerUnit;

		void OnTriggerStay (Collider other)
		{
			Rigidbody rigid = other.GetComponentInParent<Rigidbody>();
			if (rigid != null)
				rigid.AddForce(pushDirectionTrs.forward * GetForce(rigid), ForceMode.Impulse);
		}

		float GetForce (Rigidbody rigid)
		{
			float output = 0;
			Bounds bounds = pushDirectionTrs.GetBounds();
			bounds.center = Vector3.zero;
			for (float x = bounds.min.x; x <= bounds.max.x; x += 1f / raycastsPerUnit)
			{
				for (float y = bounds.min.y; y <= bounds.max.y; y += 1f / raycastsPerUnit)
				{
					RaycastHit hit;
					Vector3 raycastStart = pushDirectionTrs.TransformPoint(new Vector3(x, y));
					if (Physics.Raycast(raycastStart, pushDirectionTrs.forward, out hit, pushDirectionTrs.lossyScale.z, whatIPush) && hit.rigidbody == rigid)
						output += forceAmountPerHit;
				}
			}
			return output;
		}
	}
}