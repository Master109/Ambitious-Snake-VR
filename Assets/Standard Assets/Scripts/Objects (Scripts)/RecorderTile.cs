using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Extensions;

namespace AmbitiousSnake
{
	public class RecorderTile : Tile, ICollisionEnterHandler, IUpdatable
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
		public static RecorderTile[] areRecording = new RecorderTile[0];
		public SnakeRecording currentRecording;
		List<Vector3> newHeadPositions = new List<Vector3>();
		List<Vector3> newTailPositions = new List<Vector3>();
		int removedTailPiecesCount;
		float timeStarted;
		bool hasBeenUsed;
		
		public void OnCollisionEnter (Collision coll)
		{
			if (hasBeenUsed)
				return;
			hasBeenUsed = true;
			meshRenderer.material.color = meshRenderer.material.color.Divide(2);
			StartRecording ();
		}

		void StartRecording ()
		{
			areRecording = areRecording.Add(this);
			timeStarted = Time.timeSinceLevelLoad;
			currentRecording = new SnakeRecording();
			SnakeRecording.Frame frame = new SnakeRecording.Frame();
			frame.newHeadPositions = new Vector3[Snake.instance.pieces.Count];
			for (int i = 0; i < Snake.instance.pieces.Count; i ++)
				frame.newHeadPositions[i] = Snake.instance.GetPiecePosition(i);
			frame.trsPosition = Snake.instance.trs.position;
			frame.trsRotation = Snake.instance.trs.eulerAngles;
			currentRecording.frames.Add(frame);
			Snake.instance.onAddHeadPiece += OnAddHeadPiece;
			Snake.instance.onAddTailPiece += OnAddTailPiece;
			Snake.instance.onRemoveTailPiece += OnRemoveTailPiece;
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			SnakeRecording.Frame frame = new SnakeRecording.Frame();
			frame.newHeadPositions = newHeadPositions.ToArray();
			newHeadPositions.Clear();
			frame.newTailPositions = newTailPositions.ToArray();
			newTailPositions.Clear();
			frame.removedTailPiecesCount = removedTailPiecesCount;
			removedTailPiecesCount = 0;
			frame.trsPosition = Snake.instance.trs.position;
			frame.trsRotation = Snake.instance.trs.eulerAngles;
			frame.timeSinceCreated = Time.timeSinceLevelLoad - timeStarted;
			currentRecording.frames.Add(frame);
		}

		void OnAddHeadPiece (Vector3 position)
		{
			newHeadPositions.Add(position);
		}

		void OnAddTailPiece (Vector3 position)
		{
			newTailPositions.Add(position);
		}

		void OnRemoveTailPiece ()
		{
			removedTailPiecesCount ++;
		}

		public void StopRecording ()
		{
			areRecording = areRecording.Remove(this);
			Snake.instance.onAddHeadPiece -= OnAddHeadPiece;
			Snake.instance.onAddTailPiece -= OnAddTailPiece;
			Snake.instance.onRemoveTailPiece -= OnRemoveTailPiece;
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}