using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public struct SnakeRecording
	{
		public List<Vector3[]> newHeadLocalPositions;
		public List<Vector3[]> newTailLocalPositions;
		public List<int> removedTailPieces;
		public List<Vector3> translations;
		public List<Vector3> rotations;
		public List<float> timesSinceCreation;
	}
}