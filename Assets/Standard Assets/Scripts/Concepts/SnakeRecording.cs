using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public struct SnakeRecording
	{
		public List<Frame> frames;

		public struct Frame
		{
			public Vector3[] newHeadPositions;
			public Vector3[] newTailPositions;
			public int removedTailPiecesCount;
			public Vector3 trsPosition;
			public Vector3 trsRotation;
			public float timeSinceCreated;
		}
	}
}