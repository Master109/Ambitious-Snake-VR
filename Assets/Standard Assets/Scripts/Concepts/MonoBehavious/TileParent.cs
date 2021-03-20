using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AmbitiousSnake
{
	public class TileParent : MonoBehaviour
	{
		public Transform trs;
		public Rigidbody rigid;
		public ICollisionEnterHandler[] collisionEnterHandlers = new ICollisionEnterHandler[0];
		public ICollisionExitHandler[] collisionExitHandlers = new ICollisionExitHandler[0];

		void OnCollisionEnter (Collision coll)
		{
			OnCollisionStay (coll);
		}

		void OnCollisionStay (Collision coll)
		{
			List<ICollisionEnterHandler> _collisionEnterHandlers = new List<ICollisionEnterHandler>(collisionEnterHandlers);
			for (int i = 0; i < coll.contactCount; i ++)
			{
				ContactPoint contactPoint = coll.GetContact(i);
				for (int i2 = 0; i2 < _collisionEnterHandlers.Count; i2 ++)
				{
					ICollisionEnterHandler collisionEnterHandler = _collisionEnterHandlers[i2];
					if (contactPoint.thisCollider == collisionEnterHandler.Collider)
					{
						collisionEnterHandler.OnCollisionEnter (coll);
						_collisionEnterHandlers.RemoveAt(i2);
						if (_collisionEnterHandlers.Count == 0)
							return;
						break;
					}
				}
			}
		}

		void OnCollisionExit (Collision coll)
		{
			List<ICollisionExitHandler> _collisionExitHandlers = new List<ICollisionExitHandler>(collisionExitHandlers);
			for (int i = 0; i < coll.contactCount; i ++)
			{
				ContactPoint contactPoint = coll.GetContact(i);
				for (int i2 = 0; i2 < _collisionExitHandlers.Count; i2 ++)
				{
					ICollisionExitHandler collisionExitHandler = _collisionExitHandlers[i2];
					if (contactPoint.thisCollider == collisionExitHandler.Collider)
					{
						collisionExitHandler.OnCollisionExit (coll);
						_collisionExitHandlers.RemoveAt(i2);
						if (_collisionExitHandlers.Count == 0)
							return;
						break;
					}
				}
			}
		}
	}
}