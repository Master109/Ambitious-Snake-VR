using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class Star : SingletonMonoBehaviour<Star>
	{
		public Transform trs;
		public bool isCollected;
		public AudioClip[] onTriggerEnterAudioClips = new AudioClip[0];

		void OnTriggerEnter (Collider other)
		{
			isCollected = true;
			gameObject.SetActive(false);
			AudioManager.instance.MakeSoundEffect (onTriggerEnterAudioClips[Random.Range(0, onTriggerEnterAudioClips.Length)], trs.position);
		}
	}
}