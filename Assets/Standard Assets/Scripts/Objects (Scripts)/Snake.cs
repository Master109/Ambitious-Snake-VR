using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using TMPro;

namespace AmbitiousSnake
{
	public class Snake : SingletonMonoBehaviour<Snake>, IDestructable, IUpdatable
	{
		public Transform trs;
		public Rigidbody rigid;
		public float moveSpeed;
		[HideInInspector]
		public float hp;
		public float Hp
		{
			get
			{
				return hp;
			}
			set
			{
				hp = value;
			}
		}
		public int maxHp;
		public int MaxHp
		{
			get
			{
				return maxHp;
			}
			set
			{
				maxHp = value;
			}
		}
		public TMP_Text hpText;
		[HideInInspector]
		public bool dead;
		[HideInInspector]
		public Vector3 move;
		public float maxDistanceBetweenPieces;
		List<Vector3> vertices = new List<Vector3>();
		List<float> vertexDistances = new List<float>();
		public LayerMask whatICrashInto;
		public ClampedFloat length;
		public float pieceRadius;
		public Vector3 HeadPosition
		{
			get
			{
				return vertices[0];
			}
		}
		
		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void Death ()
		{
			LoseableScenerio.Instance.Lose ();
		}
		
		public void DoUpdate ()
		{
			if (dead)
				return;
			Move ();
		}

		public void TakeDamage (float amount, Hazard source)
		{
			hp = Mathf.Clamp(hp - amount, 0, MaxHp);
			hpText.text = "Health: " + hp;
			if (hp == 0)
			{
				dead = true;
				Death ();
			}
		}
		
		void Move ()
		{
			move = InputManager.MoveInput;
			RaycastHit hit;
			float totalMoveAmount = moveSpeed * Time.deltaTime;
			if (Physics.Raycast(HeadPosition, move, out hit, totalMoveAmount + pieceRadius, whatICrashInto, QueryTriggerInteraction.Ignore))
			{
				vertices.Insert(0, hit.point + (HeadPosition - hit.point).normalized * pieceRadius);
				vertexDistances.Insert(0, hit.distance - pieceRadius);
			}
			else
			{
				Vector3 position = HeadPosition + (move * totalMoveAmount);
				while (totalMoveAmount > 0)
				{
					float moveAmount = Mathf.Min(maxDistanceBetweenPieces, totalMoveAmount);
					totalMoveAmount -= moveAmount;
					vertexDistances.Insert(0, moveAmount);
					vertices.Insert(0, (position - HeadPosition).normalized * moveAmount);
				}
			}
		}

		void RemovePiece ()
		{
			vertices.RemoveAt(vertices.Count - 1);
			vertexDistances.RemoveAt(vertexDistances.Count - 1);
		}

		void SetLength (float newLength)
		{
			float lengthChange = length.value - newLength;
			length.SetValue (newLength);
			while (lengthChange > 0)
			{
				float vertexDistance = vertexDistances[vertexDistances.Count - 1];
				float lengthDifference = lengthChange - vertexDistance;
				if (lengthDifference >= 0)
				{
					RemovePiece ();
					lengthChange -= vertexDistance;
				}
				else
				{
					vertices[vertices.Count - 1] += (vertices[vertices.Count - 2] - vertices[vertices.Count - 1]).normalized * -lengthDifference;
					vertexDistances[vertexDistances.Count - 1] += lengthDifference;
					break;
				}
			}
		}

		void OnDestroy ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}