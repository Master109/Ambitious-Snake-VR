using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class Star : SingletonMonoBehaviour<Star>
	{
		public bool isCollected;

		void OnTriggerEnter (Collider other)
		{
			isCollected = true;
			gameObject.SetActive(false);
		}
	}
}