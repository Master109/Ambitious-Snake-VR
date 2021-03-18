using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class SnakePiece : Spawnable
	{
        public float distanceToPreviousPiece;
		public float lengthTraveledAtSpawn;
		public MeshRenderer meshRenderer;
	}
}