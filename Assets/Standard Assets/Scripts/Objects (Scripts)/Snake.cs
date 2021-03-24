using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using TMPro;

namespace AmbitiousSnake
{
	public class Snake : SingletonMonoBehaviour<Snake>, IDestructable, IUpdatable
	{
#if UNITY_EDITOR
		public Transform followTrs;
#endif
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
		Vector3 move;
		public float maxDistanceBetweenPieces;
		List<SnakePiece> pieces = new List<SnakePiece>();
		public LayerMask whatICrashInto;
		public ClampedFloat length;
		public float currentLength;
		public SnakePiece piecePrefab;
		public float changeLengthRate;
		public delegate Vector3 OnMove(Vector3 move);
		public event OnMove onMove;
		public delegate float OnChangeLength(float amount);
		public event OnChangeLength onChangeLength;
		public float lengthTraveled;
		public SnakePiece HeadPiece
		{
			get
			{
				return pieces[pieces.Count - 1];
			}
		}
		public SnakePiece TailPiece
		{
			get
			{
				return pieces[0];
			}
		}
		public Vector3 HeadPosition
		{
			get
			{
				return HeadPiece.trs.position;
			}
		}
		public Vector3 TailPosition
		{
			get
			{
				return TailPiece.trs.position;
			}
		}

		public override void Awake ()
		{
			base.Awake ();
			AddHeadPiece (trs.position, 0);
			for (float distance = maxDistanceBetweenPieces; distance <= length.value; distance += maxDistanceBetweenPieces)
				AddHeadPiece (trs.position + trs.forward * distance, maxDistanceBetweenPieces);
		}
		
		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void Death ()
		{
			_SceneManager.instance.RestartSceneWithoutTransition ();
		}
		
		public void DoUpdate ()
		{
			if (dead)
				return;
			HandleMovement ();
			SetLength (Mathf.Clamp(length.value + InputManager.ChangeLengthInput * changeLengthRate * Time.deltaTime, length.valueRange.min, length.valueRange.max));
			rigid.ResetCenterOfMass();
			rigid.ResetInertiaTensor();
		}

		public void TakeDamage (float amount, Hazard source)
		{
			if (dead)
				return;
			hp = Mathf.Clamp(hp - amount, 0, MaxHp);
			if (hpText != null)
				hpText.text = "Health: " + hp;
			if (hp == 0)
			{
				dead = true;
				Death ();
			}
		}

		void HandleMovement ()
		{
			move = VRCameraRig.instance.eyesTrs.rotation * InputManager.MoveInput;
			if (move.sqrMagnitude == 0)
				return;
			Move (move);
		}
		
		void Move (Vector3 move)
		{
			if (onMove != null)
				move = onMove(move);
			RaycastHit hit;
			float totalMoveAmount = move.magnitude * moveSpeed * Time.deltaTime;
			Vector3 headPosition = HeadPosition;
			if (Physics.Raycast(headPosition, move, out hit, totalMoveAmount + SnakePiece.RADIUS + Physics.defaultContactOffset, whatICrashInto, QueryTriggerInteraction.Ignore))
				totalMoveAmount = hit.distance - SnakePiece.RADIUS - Physics.defaultContactOffset;
			while (totalMoveAmount > 0)
			{
				float moveAmount = Mathf.Min(maxDistanceBetweenPieces, totalMoveAmount);
				Vector3 position;
				if (Physics.Raycast(headPosition, move, out hit, moveAmount + SnakePiece.RADIUS + Physics.defaultContactOffset, whatICrashInto, QueryTriggerInteraction.Ignore))
				{
					if (hit.distance <= SnakePiece.RADIUS + Physics.defaultContactOffset)
						return;
					position = hit.point + (headPosition - hit.point).normalized * (SnakePiece.RADIUS + Physics.defaultContactOffset);
				}
				else
					position = headPosition + (move.normalized * moveAmount);
				AddHeadPiece (position, moveAmount);
				headPosition = position;
				totalMoveAmount -= moveAmount;
			}
		}

		void AddHeadPiece (Vector3 position)
		{
			AddHeadPiece (position, (HeadPosition - position).magnitude);
		}

		void AddHeadPiece (Vector3 position, float distanceToPreviousPiece)
		{
			SnakePiece headPiece = ObjectPool.Instance.SpawnComponent<SnakePiece>(piecePrefab.prefabIndex, position, piecePrefab.trs.rotation, trs);
			headPiece.distanceToPreviousPiece = distanceToPreviousPiece;
			lengthTraveled += distanceToPreviousPiece;
			headPiece.lengthTraveledAtSpawn = lengthTraveled;
			pieces.Add(headPiece);
			currentLength += distanceToPreviousPiece;
		}

		void AddTailPiece (Vector3 position)
		{
			AddHeadPiece (position, (TailPosition - position).magnitude);
		}

		void AddTailPiece (Vector3 position, float distanceToPreviousPiece)
		{
			SnakePiece tailPiece = ObjectPool.Instance.SpawnComponent<SnakePiece>(piecePrefab.prefabIndex, position, piecePrefab.trs.rotation, trs);
			tailPiece.distanceToPreviousPiece = distanceToPreviousPiece;
			lengthTraveled += distanceToPreviousPiece;
			tailPiece.lengthTraveledAtSpawn = lengthTraveled;
			pieces.Insert(0, tailPiece);
			currentLength += distanceToPreviousPiece;
		}

		void RemoveTailPiece ()
		{
			SnakePiece tailPiece = TailPiece;
			Transform tailPieceTrs = tailPiece.trs;
			ObjectPool.instance.Despawn (tailPiece.prefabIndex, tailPiece.gameObject, tailPieceTrs);
			pieces.RemoveAt(0);
			currentLength -= tailPiece.distanceToPreviousPiece;
		}

		void SetLength (float newLength)
		{
			float lengthChange = newLength - length.value;
			if (lengthChange != 0 && onChangeLength != null)
			{
				float previousLengthChange = lengthChange;
				lengthChange = onChangeLength(lengthChange);
				newLength += lengthChange - previousLengthChange;
			}
			if (lengthChange > 0)
			{
				Vector3 tailPosition = TailPosition;
				move = (tailPosition - pieces[1].trs.position).normalized;
				float totalMoveAmount = newLength - currentLength;
				bool shouldBreak = false;
				LayerMask whatICrashIntoExcludingMe = whatICrashInto.RemoveFromMask("Snake");
				while (totalMoveAmount > 0)
				{
					float moveAmount = Mathf.Min(maxDistanceBetweenPieces, totalMoveAmount);
					Vector3 position;
					RaycastHit hit;
					if (Physics.Raycast(tailPosition, move, out hit, moveAmount + SnakePiece.RADIUS + Physics.defaultContactOffset, whatICrashIntoExcludingMe, QueryTriggerInteraction.Ignore))
					{
						for (int i = 0; i < pieces.Count; i ++)
						{
							SnakePiece piece = pieces[i];
							RaycastHit hit2;
							if (Physics.Raycast(piece.trs.position, -move, out hit2, moveAmount + SnakePiece.RADIUS + Physics.defaultContactOffset, whatICrashIntoExcludingMe, QueryTriggerInteraction.Ignore))
								return;
						}
					}
					position = tailPosition + (move * moveAmount);
					AddTailPiece (position, moveAmount);
					if (shouldBreak)
						break;
					tailPosition = position;
					totalMoveAmount -= moveAmount;
				}
			}
			while (currentLength > newLength)
				RemoveTailPiece ();
			length.SetValue (newLength);
			float distance = 0;
			for (int i = 0; i < pieces.Count; i ++)
			{
				SnakePiece piece = pieces[i];
				piece.meshRenderer.material.SetFloat("value", distance / currentLength);
				distance += piece.distanceToPreviousPiece;
			}
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}