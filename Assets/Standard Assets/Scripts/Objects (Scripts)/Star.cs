using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class Star : SingletonMonoBehaviour<Star>
	{
		public bool CollectedAndWonLevel
		{
			get
			{
				return PlayerPrefsExtensions.GetBool(Level.instance.name + " star collected", false);
			}
			set
			{
				PlayerPrefsExtensions.SetBool(Level.instance.name + " star collected", value);
			}
		}
		public bool isCollected;

		void OnTriggerEnter (Collider other)
		{
			isCollected = true;
			gameObject.SetActive(false);
		}
	}
}