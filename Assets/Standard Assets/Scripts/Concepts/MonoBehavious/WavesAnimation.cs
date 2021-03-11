using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class WavesAnimation : MonoBehaviour, IUpdatable
	{
		public float waveMovementMultiplier;
		public Vector2 waveRate;
		public Vector2 waveOffset;
		public Material material;
		public Transform wavesAudioTrs;
		public float audioMovementMultiplier;
		
		void Start ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			material.mainTextureOffset = new Vector2(Mathf.Sin(Time.time * waveRate.x) + waveOffset.x, Mathf.Sin(Time.time * waveRate.y) + waveOffset.y) * waveMovementMultiplier;
			if (wavesAudioTrs != null)
				wavesAudioTrs.position = VRCameraRig.Instance.eyesTrs.position + (new Vector2(material.mainTexture.width, material.mainTexture.height).Multiply(material.mainTextureOffset) * audioMovementMultiplier).XYToXZ();
		}
		
		void OnDestroy ()
		{
			material.mainTextureOffset = Vector2.zero;
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}