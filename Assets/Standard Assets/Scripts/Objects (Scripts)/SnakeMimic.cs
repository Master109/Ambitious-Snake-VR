using System.Collections;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class SnakeMimic : Snake, ISpawnable
	{
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
		[HideInInspector]
		public SnakeRecording playing;
		int currentFrame;
		float playbackTime;
		
		public override void Awake ()
		{
			currentFrame = 0;
			playbackTime = 0;
			StartCoroutine(Init ());
		}
		
		public override void DoUpdate ()
		{
			if (currentFrame >= playing.newHeadLocalPositions.Count - 1)
			{
				StartCoroutine(Init ());
				return;
			}
			if (GameManager.paused)
				return;
			while (playbackTime <= playing.timesSinceCreation[currentFrame])
			{

				Translate ();
				Rotate ();
				NextFrame ();
			}
			playbackTime += Time.deltaTime;
		}
		
		public override void TakeDamage (float amount, Hazard source)
		{
			return;
		}
		
		bool WillCollide ()
		{
			Vector3 positionOffset = playing.translations[currentFrame + 1] - trs.position;
			Quaternion rotationOffset = Quaternion.Euler(QuaternionExtensions.GetDeltaAngles(trs.eulerAngles, playing.rotations[currentFrame + 1]));
			Vector3 previousPiecePosition = HeadPosition;
			Vector3[] headLocalPositions = playing.newHeadLocalPositions[currentFrame]; 
			for (int i = 0; i < headLocalPositions.Length; i ++)
			{
				Vector3 piecePosition = GetPiecePosition(i);
				Vector3 startPosition = piecePosition + positionOffset;
				Vector3 endPosition = piecePosition + positionOffset;
				Vector3 startToEndPosition = endPosition - startPosition;
				if (Physics.SphereCast(new Ray(startPosition, startToEndPosition), SnakePiece.RADIUS, startToEndPosition.magnitude, whatICrashInto))
					return true;
				previousPiecePosition = piecePosition;
			}
			return false;
		}
		
		IEnumerator Init ()
		{
			Translate ();
			Rotate ();
			Vector3[] headLocalPositions = playing.newHeadLocalPositions[currentFrame]; 
			for (int i = 0; i < headLocalPositions.Length; i ++)
			{
				
			}
			yield break;
		}
		
		void Translate ()
		{
			trs.position = playing.translations[currentFrame];
		}

		void Rotate ()
		{
			trs.eulerAngles = playing.rotations[currentFrame];
		}
		
		void NextFrame ()
		{
			if (currentFrame < playing.newHeadLocalPositions.Count - 1)
				currentFrame ++;
			else if (currentFrame == playing.newHeadLocalPositions.Count - 1)
			{
				ObjectPool.instance.Despawn(prefabIndex, gameObject, trs);
			}
		}
	}
}