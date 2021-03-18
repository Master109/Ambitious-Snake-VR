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
		[HideInInspector]
		public Vector3 move;
		public float maxDistanceBetweenPieces;
		List<SnakePiece> pieces = new List<SnakePiece>();
		public LayerMask whatICrashInto;
		public ClampedFloat length;
		public float currentLength;
		public float pieceRadius;
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
			AddPiece (trs.position, 0);
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
			Move ();
			SetLength (length.value + InputManager.ChangeLengthInput * changeLengthRate * Time.deltaTime);
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
		
		void Move ()
		{
			move = VRCameraRig.instance.eyesTrs.rotation * InputManager.MoveInput;
			if (move.sqrMagnitude == 0)
				return;
			if (onMove != null)
				move = onMove(move);
			RaycastHit hit;
			float totalMoveAmount = moveSpeed * Time.deltaTime;
			Vector3 position;
			Vector3 headPosition = HeadPosition;
			if (Physics.Raycast(headPosition, move, out hit, totalMoveAmount + pieceRadius, whatICrashInto, QueryTriggerInteraction.Ignore))
			{
				position = hit.point + (headPosition - hit.point).normalized * pieceRadius;
				totalMoveAmount = Mathf.Max(hit.distance - pieceRadius, 0);
			}
			else
				position = headPosition + (move * totalMoveAmount);
			while (totalMoveAmount > 0)
			{
				float moveAmount = Mathf.Min(maxDistanceBetweenPieces, totalMoveAmount);
				totalMoveAmount -= moveAmount;
				AddPiece (position, moveAmount);
			}
		}

		void AddPiece (Vector3 position)
		{
			AddPiece (position, (HeadPosition - position).magnitude);
		}

		void AddPiece (Vector3 position, float distanceToPreviousPiece)
		{
			SnakePiece headPiece = ObjectPool.Instance.SpawnComponent<SnakePiece>(piecePrefab.prefabIndex, position, piecePrefab.trs.rotation, trs);
			headPiece.distanceToPreviousPiece = distanceToPreviousPiece;
			lengthTraveled += distanceToPreviousPiece;
			headPiece.lengthTraveledAtSpawn = lengthTraveled;
			pieces.Add(headPiece);
			currentLength += distanceToPreviousPiece;
			Vector3 worldCenterOfMass = rigid.worldCenterOfMass;
			worldCenterOfMass *= pieces.Count - 1;
			worldCenterOfMass += position;
			worldCenterOfMass /= pieces.Count;
			rigid.centerOfMass = trs.InverseTransformPoint(worldCenterOfMass);
		}

		void RemovePiece ()
		{
			SnakePiece tailPiece = TailPiece;
			Transform tailPieceTrs = tailPiece.trs;
			ObjectPool.instance.Despawn (tailPiece.prefabIndex, tailPiece.gameObject, tailPieceTrs);
			pieces.RemoveAt(0);
			currentLength -= tailPiece.distanceToPreviousPiece;
			Vector3 worldCenterOfMass = rigid.worldCenterOfMass;
			worldCenterOfMass *= pieces.Count + 1;
			worldCenterOfMass -= tailPieceTrs.position;
			worldCenterOfMass /= pieces.Count;
			rigid.centerOfMass = trs.InverseTransformPoint(worldCenterOfMass);
		}

		void SetLength (float newLength)
		{
			float lengthChange = length.value - newLength;
			lengthChange -= newLength - currentLength;
			if (lengthChange != 0 && onChangeLength != null)
				lengthChange = onChangeLength(lengthChange);
			length.SetValue (newLength);
			while (lengthChange > 0)
			{
				float distanceToPreviousPiece = TailPiece.distanceToPreviousPiece;
				float lengthDifference = lengthChange - distanceToPreviousPiece;
				if (lengthDifference >= 0)
				{
					RemovePiece ();
					lengthChange -= distanceToPreviousPiece;
				}
				else
				{
					SnakePiece tailPiece = TailPiece;
					Transform tailPieceTrs = tailPiece.trs;
					Vector3 offset = (pieces[1].trs.position - tailPieceTrs.position).normalized * -lengthDifference;
					tailPieceTrs.position += offset;
					tailPiece.distanceToPreviousPiece += lengthDifference;
					currentLength += lengthDifference;
					Vector3 worldCenterOfMass = rigid.worldCenterOfMass;
					worldCenterOfMass *= pieces.Count;
					worldCenterOfMass += offset;
					worldCenterOfMass /= pieces.Count;
					rigid.centerOfMass = trs.InverseTransformPoint(worldCenterOfMass);
					break;
				}
			}
			for (int i = 0; i < pieces.Count; i ++)
			{
				SnakePiece piece = pieces[i];
				piece.meshRenderer.material.SetFloat("value", (float) (i + 1) / pieces.Count);
			}
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}