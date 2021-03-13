using UnityEngine;
using System;
using Extensions;
using System.Collections;
using System.Collections.Generic;

namespace AmbitiousSnake
{
	//[ExecuteInEditMode]
	public class DissolveTile : MonoBehaviour
	{
		public MeshRenderer meshRenderer;
		public float dissolveDuration;
		bool isDissolving;

		void OnCollisionEnter2D (Collision2D coll)
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