using UnityEngine;
using Extensions;
using System.Collections;
using System.Collections.Generic;

namespace AmbitiousSnake
{
	public class DissolveTile : Tile, ICollisionEnterHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		public MeshRenderer meshRenderer;
		public float dissolveDuration;
		bool isDissolving;

		public void OnCollisionEnter (Collision coll)
		{
			if (isDissolving)
				return;
			isDissolving = true;
			StartCoroutine(DissolveRoutine ());
		}

		IEnumerator DissolveRoutine ()
		{
			do
			{
				yield return new WaitForEndOfFrame();
				meshRenderer.material.color = meshRenderer.material.color.AddAlpha(-Time.deltaTime / dissolveDuration);
				if (meshRenderer.material.color.a <= 0)
				{
					meshRenderer.material.color = meshRenderer.material.color.SetAlpha(0);
					break;
				}
			} while (true);
			Destroy(gameObject);
		}
	}
}