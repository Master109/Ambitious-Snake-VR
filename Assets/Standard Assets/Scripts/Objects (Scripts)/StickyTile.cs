using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AmbitiousSnake
{
	public class StickyTile : Tile, ICollisionEnterHandler, ICollisionExitHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		Dictionary<string, StuckPoint> stuckPoints = new Dictionary<string, StuckPoint>();
		List<SnakeStuckPoint> snakeStuckPoints = new List<SnakeStuckPoint>();

		public void OnCollisionEnter (Collision coll)
		{
			Rigidbody rigid = coll.gameObject.GetComponentInParent<Rigidbody>();
			if (rigid == null || stuckPoints.ContainsKey(rigid.name))
				return;
			StuckPoint stuckPoint = new StuckPoint();
			stuckPoint.fixedJoint = gameObject.AddComponent<FixedJoint>();
			stuckPoint.fixedJoint.enableCollision = true;
			stuckPoint.fixedJoint.connectedBody = rigid;
			if (rigid == Snake.instance.rigid)
			{
				if (coll.collider == Snake.instance.HeadPiece.collider)
					Snake.instance.enabled = false;
				else
				{
					SnakePiece snakePiece = coll.collider.GetComponent<SnakePiece>();
					SnakeStuckPoint snakeStuckPoint = new SnakeStuckPoint(snakePiece.lengthTraveledAtSpawn, Snake.instance.lengthTraveled - snakePiece.lengthTraveledAtSpawn);
					snakeStuckPoints.Add(snakeStuckPoint);
					if (snakeStuckPoints.Count == 1)
					{
						Snake.instance.onMove += OnSnakeMove;
						Snake.instance.onChangeLength += OnSnakeChangeLength;
					}
				}
			}
			stuckPoints.Add(rigid.name, stuckPoint);
		}

		Vector3 OnSnakeMove (Vector3 move)
		{
			float moveDistance = Snake.instance.moveSpeed * Time.deltaTime;
			float lengthTraveledAfterMove = Snake.instance.lengthTraveled + moveDistance;
			for (int i = 0; i < snakeStuckPoints.Count; i ++)
			{
				SnakeStuckPoint snakeStuckPoint = snakeStuckPoints[i];
				float lengthTraveledPastSnakeStuckPointAfterMove = lengthTraveledAfterMove - snakeStuckPoint.lengthTraveled;
				float lengthTraveledAfterMoveOvershoot = lengthTraveledPastSnakeStuckPointAfterMove - snakeStuckPoint.lengthToHead;
				if (lengthTraveledAfterMoveOvershoot >= 0)
				{
					move = Vector3.ClampMagnitude(move, 1f - lengthTraveledAfterMoveOvershoot / moveDistance);
					break;
				}
			}
			return move;
		}

		float OnSnakeChangeLength (float amount)
		{
			for (int i = 0; i < snakeStuckPoints.Count; i ++)
			{
				SnakeStuckPoint snakeStuckPoint = snakeStuckPoints[i];
				float lengthTraveledPastSnakeStuckPoint = Snake.instance.lengthTraveled - snakeStuckPoint.lengthTraveled;
				float lengthTraveledOvershoot = lengthTraveledPastSnakeStuckPoint - snakeStuckPoint.lengthToHead;
				if (amount > lengthTraveledOvershoot)
				{
					snakeStuckPoints.RemoveAt(i);
					if (snakeStuckPoints.Count == 0)
					{
						Snake.instance.onMove -= OnSnakeMove;
						Snake.instance.onChangeLength -= OnSnakeChangeLength;
					}
					StuckPoint stuckPoint;
					if (stuckPoints.TryGetValue(Snake.instance.rigid.name, out stuckPoint))
					{
						Destroy(stuckPoint.fixedJoint);
						stuckPoints.Remove(Snake.instance.rigid.name);
					}
					break;
				}
			}
			return amount;
		}

		public void OnCollisionExit (Collision coll)
		{
			Rigidbody rigid = coll.gameObject.GetComponentInParent<Rigidbody>();
			if (rigid == null)
				return;
			StuckPoint stuckPoint;
			if (stuckPoints.TryGetValue(rigid.name, out stuckPoint))
			{
				// SnakeStuckPoint snakeStuckPoint = stuckPoint as SnakeStuckPoint;
				// if (snakeStuckPoint != null)
				// {
				// 	snakeStuckPoints.Remove(snakeStuckPoint);
				// 	if (snakeStuckPoints.Count == 0)
				// 	{
				// 		Snake.instance.onMove -= OnSnakeMove;
				// 		Snake.instance.onChangeLength -= OnSnakeChangeLength;
				// 	}
				// }
				stuckPoints.Remove(rigid.name);
				Destroy(stuckPoint.fixedJoint);
			}
		}

		public class StuckPoint
		{
			public FixedJoint fixedJoint;
		}

		public class SnakeStuckPoint : StuckPoint
		{
			public float lengthTraveled;
			public float lengthToHead;

			public SnakeStuckPoint (float lengthTraveled, float lengthToHead)
			{
				this.lengthTraveled = lengthTraveled;
				this.lengthToHead = lengthToHead;
			}
		}
	}
}