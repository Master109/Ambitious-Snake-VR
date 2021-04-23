using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class Lava : Hazard
	{
		public AudioClip[] onTriggerEnterAudioClips = new AudioClip[0];
		public AnimationCurve onTriggerEnterSpeedToVolumeCurve;

		void OnTriggerEnter (Collider other)
		{
			float volume = onTriggerEnterSpeedToVolumeCurve.Evaluate(other.GetComponentInParent<Rigidbody>().velocity.magnitude);
			AudioManager.instance.MakeSoundEffect (onTriggerEnterAudioClips[Random.Range(0, onTriggerEnterAudioClips.Length)], other.GetComponent<Transform>().position, volume);
			IDestructable destructable = other.GetComponentInParent<IDestructable>();
			if (destructable != null)
				ApplyDamage (destructable, damage);
		}
	}
}