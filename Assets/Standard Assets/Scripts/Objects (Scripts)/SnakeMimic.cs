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
		public float shrinkSphereChecks;
		SnakeRecording.Frame currentFrame;
		int currentFrameIndex;
		bool isPlaying;
		float playbackTime;
		
		public override void Awake ()
		{
			currentFrameIndex = 0;
			playbackTime = 0;
			StartCoroutine(Init ());
		}

		public override void OnEnable ()
		{
			isPlaying = true;
		}
		
		public override void DoUpdate ()
		{
			if (GameManager.paused)
				return;
			currentFrame = playing.frames[currentFrameIndex];
			if (!isPlaying)
			{
				if (!WillCollide())
					isPlaying = true;
			}
			else
			{
				playbackTime += Time.deltaTime;
				while (playbackTime > currentFrame.timeSinceCreated)
				{
					NextFrame ();
					if (currentFrameIndex == playing.frames.Count - 1)
						return;
					if (WillCollide())
					{
						isPlaying = false;
						break;
					}
					Translate ();
					Rotate ();
					AddHeadPieces ();
					AddTailPieces ();
					RemoveTailPieces ();
					OnSetLength ();
				}
			}
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
			Vector3 previousPiecePosition;
			if (currentFrame.newHeadPositions.Length > 0)
			{
				previousPiecePosition = currentFrame.newHeadPositions[0];
				for (int i = 1; i < currentFrame.newHeadPositions.Length; i ++)
				{
					Vector3 piecePosition = currentFrame.newHeadPositions[i];
					Vector3 previousToCurrentPiecePosition = piecePosition - previousPiecePosition;
					RaycastHit[] hits = Physics.SphereCastAll(new Ray(piecePosition, previousToCurrentPiecePosition), SnakePiece.RADIUS - shrinkSphereChecks, previousToCurrentPiecePosition.magnitude + SnakePiece.RADIUS, whatICrashInto);
					for (int i2 = 0; i2 < hits.Length; i2 ++)
					{
						RaycastHit hit = hits[i2];
						if (hit.rigidbody != rigid)
							return true;
					}
					previousPiecePosition = piecePosition;
				}
			}
			if (currentFrame.newTailPositions.Length > 0)
			{
				previousPiecePosition = currentFrame.newTailPositions[0];
				for (int i = 1; i < currentFrame.newTailPositions.Length - currentFrame.removedTailPiecesCount; i ++)
				{
					Vector3 piecePosition = currentFrame.newTailPositions[i];
					Vector3 previousToCurrentPiecePosition = piecePosition - previousPiecePosition;
					RaycastHit[] hits = Physics.SphereCastAll(new Ray(piecePosition, previousToCurrentPiecePosition), SnakePiece.RADIUS - shrinkSphereChecks, previousToCurrentPiecePosition.magnitude + SnakePiece.RADIUS, whatICrashInto);
					for (int i2 = 0; i2 < hits.Length; i2 ++)
					{
						RaycastHit hit = hits[i2];
						if (hit.rigidbody != rigid)
							return true;
					}
					previousPiecePosition = piecePosition;
				}
			}
			return false;
		}
		
		IEnumerator Init ()
		{
			yield return new WaitUntil(() => (!playing.Equals(null)));
			currentFrame = playing.frames[currentFrameIndex];
			Translate ();
			Rotate ();
			while (true)
			{
				bool willCollide = false;
				for (int i = 0; i < currentFrame.newHeadPositions.Length; i ++)
				{
					Vector3 headPosition = currentFrame.newHeadPositions[i];
					if (Physics.CheckSphere(headPosition, SnakePiece.RADIUS - shrinkSphereChecks, whatICrashInto))
					{
						willCollide = true;
						break;
					}
				}
				if (!willCollide)
					break;
				yield return new WaitForEndOfFrame();
			}
			AddHeadPiece (currentFrame.newHeadPositions[0], 0);
			for (int i = 1; i < currentFrame.newHeadPositions.Length; i ++)
			{
				Vector3 newHeadPosition = currentFrame.newHeadPositions[i];
				AddHeadPiece (newHeadPosition);
			}
			GameManager.updatables = GameManager.updatables.Add(this);
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