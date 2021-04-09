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
		SnakeRecording.Frame currentFrame;
		int currentFrameIndex;
		float playbackTime;
		
		public override void Awake ()
		{
			currentFrameIndex = 0;
			playbackTime = 0;
			StartCoroutine(Init ());
		}
		
		public override void DoUpdate ()
		{
			if (GameManager.paused)
				return;
			currentFrame = playing.frames[currentFrameIndex];
			while (playbackTime <= currentFrame.timeSinceCreated)
			{
				NextFrame ();
				if (currentFrameIndex == playing.frames.Count - 1)
					return;
				if (WillCollide())
					break;
				Translate ();
				Rotate ();
				AddHeadPieces ();
				AddTailPieces ();
				RemoveTailPieces ();
				OnSetLength ();
			}
			playbackTime += Time.deltaTime;
		}

		public override void OnSetLength ()
		{
			float distance = 0;
			for (int i = 0; i < pieces.Count; i ++)
			{
				SnakePiece piece = pieces[i];
				piece.meshRenderer.material.SetFloat("value", distance / currentLength);
				distance += piece.distanceToPreviousPiece;
			}
		}
		
		public override void TakeDamage (float amount, Hazard source)
		{
			return;
		}
		
		bool WillCollide ()
		{
			Vector3 previousPiecePosition = HeadPosition;
			for (int i = 0; i < currentFrame.newHeadPositions.Length; i ++)
			{
				Vector3 piecePosition = currentFrame.newHeadPositions[i];
				Vector3 previousToCurrentPiecePosition = piecePosition - previousPiecePosition;
				if (Physics.SphereCast(new Ray(piecePosition, previousToCurrentPiecePosition), SnakePiece.RADIUS, previousToCurrentPiecePosition.magnitude, whatICrashInto))
					return true;
				previousPiecePosition = piecePosition;
			}
			previousPiecePosition = Quaternion.Euler(currentFrame.trsRotation) * TailLocalPosition + currentFrame.trsPosition;
			for (int i = 0; i < currentFrame.newTailPositions.Length - currentFrame.removedTailPiecesCount; i ++)
			{
				Vector3 piecePosition = currentFrame.newTailPositions[i];
				Vector3 previousToCurrentPiecePosition = piecePosition - previousPiecePosition;
				if (Physics.SphereCast(new Ray(piecePosition, previousToCurrentPiecePosition), SnakePiece.RADIUS, previousToCurrentPiecePosition.magnitude, whatICrashInto))
					return true;
				previousPiecePosition = piecePosition;
			}
			return false;
		}
		
		IEnumerator Init ()
		{
			yield return new WaitUntil(() => (!playing.Equals(null)));
			currentFrame = playing.frames[currentFrameIndex];
			Translate ();
			Rotate ();
			for (int i = 0; i < currentFrame.newHeadPositions.Length; i ++)
			{
				Vector3 headPosition = currentFrame.newHeadPositions[i];
				AddHeadPiece (headPosition);
			}
			yield break;
		}
		
		void Translate ()
		{
			trs.position = currentFrame.trsPosition;
		}

		void Rotate ()
		{
			trs.eulerAngles = currentFrame.trsRotation;
		}

		void AddHeadPieces ()
		{
			for (int i = 0; i < currentFrame.newHeadPositions.Length; i ++)
			{
				Vector3 newHeadPosition = currentFrame.newHeadPositions[i];
				AddHeadPiece (newHeadPosition);
			}
		}

		void AddTailPieces ()
		{
			for (int i = 0; i < currentFrame.newTailPositions.Length; i ++)
			{
				Vector3 newTailPosition = currentFrame.newTailPositions[i];
				AddTailPiece (newTailPosition);
			}
		}

		void RemoveTailPieces ()
		{
			for (int i = 0; i < currentFrame.removedTailPiecesCount; i ++)
				RemoveTailPiece ();
		}
		
		void NextFrame ()
		{
			if (currentFrameIndex < playing.frames.Count - 1)
			{
				currentFrameIndex ++;
				currentFrame = playing.frames[currentFrameIndex];
			}
			else if (currentFrameIndex == playing.frames.Count - 1)
				ObjectPool.instance.Despawn (prefabIndex, gameObject, trs);
		}
	}
}