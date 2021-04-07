using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Extensions;

namespace AmbitiousSnake
{
	public class RecorderTile : Tile, ICollisionEnterHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		[HideInInspector]
		public bool isRecording;
		public MeshRenderer meshRenderer;
		public static RecorderTile[] areRecording = new RecorderTile[0];
		public SnakeRecording currentRecording;
		Vector3 latestHeadPieceLocalPosition;
		Vector3 latestTailPieceLocalPosition;
		float timeStarted;
		bool hasBeenUsed;
		
		public void OnCollisionEnter (Collision coll)
		{
			if (hasBeenUsed)
				return;
			hasBeenUsed = true;
			timeStarted = Time.timeSinceLevelLoad;
			meshRenderer.material.color = meshRenderer.material.color.DivideAlpha(2);
			StartCoroutine(Record ());
		}

		public IEnumerator Record ()
		{
			areRecording = areRecording.Add(this);
			isRecording = true;
			currentRecording = new SnakeRecording();
			Vector3[] headPositions = new Vector3[Snake.instance.pieces.Count];
			for (int i = 0; i < Snake.instance.pieces.Count; i ++)
				headPositions[i] = Snake.instance.GetPieceLocalPosition(i);
			currentRecording.newHeadLocalPositions.Add(headPositions);
			currentRecording.translations.Add(Snake.instance.trs.position);
			currentRecording.rotations.Add(Snake.instance.trs.eulerAngles);
			currentRecording.timesSinceCreation.Add(0);
			while (isRecording)
			{
				yield return new WaitForEndOfFrame();
				// currentRecording.newHeadLocalPositions.Add(Snake.instance.GetPieceLocalPosition(Snake.instance.pieces.Count - 1));
				currentRecording.translations.Add(Snake.instance.trs.position);
				currentRecording.rotations.Add(Snake.instance.trs.eulerAngles);
				currentRecording.timesSinceCreation.Add(Time.timeSinceLevelLoad - timeStarted);
			}
			areRecording = areRecording.Remove(this);
		}
	}
}