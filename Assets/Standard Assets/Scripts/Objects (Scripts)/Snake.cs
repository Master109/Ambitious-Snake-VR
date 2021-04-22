﻿using System.Collections;
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
		public float maxDistanceBetweenPieces;
		public List<SnakePiece> pieces = new List<SnakePiece>();
		public LayerMask whatICrashInto;
		public ClampedFloat length;
		public float currentLength;
		public SnakePiece piecePrefab;
		public float changeLengthRate;
		public AudioClip[] collisionWithWallAudioClips = new AudioClip[0];
		public LineRenderer lengthIndicator;
		public PhysicMaterial physicsMaterial;
		public delegate void OnAddHeadPiece(Vector3 position);
		public event OnAddHeadPiece onAddHeadPiece;
		public delegate void OnAddTailPiece(Vector3 position);
		public event OnAddTailPiece onAddTailPiece;
		public delegate void OnRemoveTailPiece();
		public event OnRemoveTailPiece onRemoveTailPiece;
		public float maxAbsContactNormalYToHaveNoFriction;
		public AnimationCurve wallCollisionVelocityToVolumeCurve;
		public AudioSource headMovementAudioSource;
		public Transform headMovementAudioSourceTrs;
		public AudioSource tailMovementAudioSource;
		public Transform tailMovementAudioSourceTrs;
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
		public Vector3 HeadLocalPosition
		{
			get
			{
				return HeadPiece.trs.localPosition;
			}
		}
		public Vector3 TailLocalPosition
		{
			get
			{
				return TailPiece.trs.localPosition;
			}
		}
		Vector3 move;
		float changeLengthInput;
		float friction;
		Dictionary<Collider, ContactPoint[]> contactPointsWithCollidersDict = new Dictionary<Collider, ContactPoint[]>();

		public override void Awake ()
		{
			base.Awake ();
			friction = physicsMaterial.dynamicFriction;
			AddHeadPiece (trs.position, 0);
			for (float distance = maxDistanceBetweenPieces; distance <= length.value; distance += maxDistanceBetweenPieces)
				AddHeadPiece (trs.position + trs.forward * distance, maxDistanceBetweenPieces);
		}
		
		public virtual void OnEnable ()
		{
			instance = this;
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void Death ()
		{
			_SceneManager.instance.RestartSceneWithoutTransition ();
		}
		
		public virtual void DoUpdate ()
		{
			if (GameManager.paused || dead)
				return;
			HandleMovement ();
			changeLengthInput = InputManager.ChangeLengthInput;
			SetLength (Mathf.Clamp(length.value + changeLengthInput * changeLengthRate * Time.deltaTime, length.valueRange.min, length.valueRange.max));
			rigid.ResetCenterOfMass();
			rigid.ResetInertiaTensor();
			if (move.sqrMagnitude == 0)
				headMovementAudioSource.Pause();
			else
				headMovementAudioSourceTrs.position = HeadPosition;
			if (changeLengthInput == 0)
				tailMovementAudioSource.Pause();
			else
				tailMovementAudioSourceTrs.position = TailPosition;
		}

		public virtual void TakeDamage (float amount, Hazard source)
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
		
		public virtual void Move (Vector3 move)
		{
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
					moveAmount = hit.distance - SnakePiece.RADIUS - Physics.defaultContactOffset;
					MakeCollisionSoundEffect (hit.collider.gameObject, hit.point, move.magnitude * moveSpeed);
				}
				else
					position = headPosition + (move.normalized * moveAmount);
				AddHeadPiece (position, moveAmount);
				headPosition = position;
				totalMoveAmount -= moveAmount;
			}
		}

		public void AddHeadPiece (Vector3 position)
		{
			AddHeadPiece (position, (HeadPosition - position).magnitude);
		}

		public void AddHeadPiece (Vector3 position, float distanceToPreviousPiece)
		{
			SnakePiece headPiece = ObjectPool.Instance.SpawnComponent<SnakePiece>(piecePrefab.prefabIndex, position, piecePrefab.trs.rotation, trs);
			headPiece.distanceToPreviousPiece = distanceToPreviousPiece;
			pieces.Add(headPiece);
			currentLength += distanceToPreviousPiece;
			headMovementAudioSource.UnPause();
			if (onAddHeadPiece != null)
				onAddHeadPiece (position);
		}

		public void AddTailPiece (Vector3 position)
		{
			AddTailPiece (position, (TailPosition - position).magnitude);
		}

		void AddTailPiece (Vector3 position, float distanceToPreviousPiece)
		{
			SnakePiece tailPiece = ObjectPool.Instance.SpawnComponent<SnakePiece>(piecePrefab.prefabIndex, position, piecePrefab.trs.rotation, trs);
			tailPiece.distanceToPreviousPiece = distanceToPreviousPiece;
			pieces.Insert(0, tailPiece);
			currentLength += distanceToPreviousPiece;
			tailMovementAudioSource.UnPause();
			if (onAddTailPiece != null)
				onAddTailPiece (position);
		}

		public void RemoveTailPiece ()
		{
			SnakePiece tailPiece = TailPiece;
			Transform tailPieceTrs = tailPiece.trs;
			ObjectPool.instance.Despawn (tailPiece.prefabIndex, tailPiece.gameObject, tailPieceTrs);
			pieces.RemoveAt(0);
			currentLength -= tailPiece.distanceToPreviousPiece;
			if (onRemoveTailPiece != null)
				onRemoveTailPiece ();
		}

		void SetLength (float newLength)
		{
			float lengthChange = newLength - length.value;
			if (lengthChange > 0)
			{
				Vector3 tailPosition = TailPosition;
				move = (tailPosition - pieces[1].trs.position).normalized;
				float totalMoveAmount = newLength - currentLength;
				LayerMask whatICrashIntoExcludingMe = whatICrashInto.RemoveFromMask("Snake");
				while (totalMoveAmount > 0)
				{
					float moveAmount = Mathf.Min(maxDistanceBetweenPieces, totalMoveAmount);
					RaycastHit hit;
					if (Physics.Raycast(tailPosition, move, out hit, moveAmount + SnakePiece.RADIUS + Physics.defaultContactOffset, whatICrashIntoExcludingMe, QueryTriggerInteraction.Ignore))
					{
						MakeCollisionSoundEffect (hit.collider.gameObject, hit.point, changeLengthInput * changeLengthRate);
						for (int i = 0; i < pieces.Count; i ++)
						{
							SnakePiece piece = pieces[i];
							RaycastHit hit2;
							if (Physics.Raycast(piece.trs.position, -move, out hit2, moveAmount + SnakePiece.RADIUS + Physics.defaultContactOffset, whatICrashIntoExcludingMe, QueryTriggerInteraction.Ignore))
							{
								MakeCollisionSoundEffect (hit2.collider.gameObject, hit2.point, changeLengthInput * changeLengthRate);
								OnSetLength ();
								return;
							}
						}
					}
					Vector3 position = tailPosition + (move * moveAmount);
					AddTailPiece (position, moveAmount);
					tailPosition = position;
					totalMoveAmount -= moveAmount;
				}
			}
			while (currentLength > newLength)
				RemoveTailPiece ();
			length.SetValue (newLength);
			OnSetLength ();
		}

		public virtual void OnSetLength ()
		{
			float distance = 0;
			for (int i = 0; i < pieces.Count; i ++)
			{
				SnakePiece piece = pieces[i];
				piece.meshRenderer.material.SetFloat("value", distance / currentLength);
				distance += piece.distanceToPreviousPiece;
			}
			lengthIndicator.SetPosition(0, Vector3.up * currentLength / length.valueRange.max);
		}

		void OnDisable ()
		{
			physicsMaterial.dynamicFriction = friction;
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		void OnDestroy ()
		{
			onAddHeadPiece = null;
			onAddTailPiece = null;
			onRemoveTailPiece = null;
		}

		public Vector3 GetPiecePosition (int index)
		{
			return pieces[index].trs.position;
		}

		public Vector3 GetPieceLocalPosition (int index)
		{
			return pieces[index].trs.localPosition;
		}

		void OnCollisionEnter (Collision coll)
		{
			ContactPoint[] contactPoints = new ContactPoint[coll.contactCount];
			coll.GetContacts(contactPoints);
			if (!contactPointsWithCollidersDict.ContainsKey(coll.collider))
			{
				if (contactPointsWithCollidersDict.Count == 0)
				{
					for (int i = 0; i < coll.contactCount; i ++)
					{
						ContactPoint contactPoint = contactPoints[i];
						if (move.sqrMagnitude == 0 || contactPoint.thisCollider != HeadPiece.collider)
						{
							if (coll.gameObject.layer == LayerMask.NameToLayer("Wall"))
								MakeCollisionSoundEffect (CollisionSoundEffectType.Wall, contactPoint.point);
						}
					}
				}
				contactPointsWithCollidersDict.Add(coll.collider, contactPoints);
			}
			else
				contactPointsWithCollidersDict[coll.collider] = contactPoints;
			List<ContactPoint> allContactPoints = new List<ContactPoint>();
			foreach (ContactPoint[] _contactPoints in contactPointsWithCollidersDict.Values)
				allContactPoints.AddRange(_contactPoints);
			int horiozntalHitCount = 0;
			for (int i = 0; i < allContactPoints.Count; i ++)
			{
				ContactPoint contactPoint = allContactPoints[i];
				if (Mathf.Abs(contactPoint.normal.y) <= maxAbsContactNormalYToHaveNoFriction)
				{
					horiozntalHitCount ++;
					if (horiozntalHitCount > 1)
					{
						physicsMaterial.dynamicFriction = 0;
						return;
					}
				}
			}
			physicsMaterial.dynamicFriction = friction;
		}

		void OnCollisionStay (Collision coll)
		{
			OnCollisionEnter (coll);
		}

		void OnCollisionExit (Collision coll)
		{
			contactPointsWithCollidersDict.Remove(coll.collider);
		}

		SoundEffect MakeCollisionSoundEffect (GameObject hitGo, Vector3 hitPoint)
		{
			if (hitGo.layer == LayerMask.NameToLayer("Wall"))
				return MakeCollisionSoundEffect(CollisionSoundEffectType.Wall, hitPoint);
			else
				return null;
		}

		SoundEffect MakeCollisionSoundEffect (GameObject hitGo, Vector3 hitPoint, float hitSpeed)
		{
			if (hitGo.layer == LayerMask.NameToLayer("Wall"))
				return MakeCollisionSoundEffect(CollisionSoundEffectType.Wall, hitPoint, hitSpeed);
			else
				return null;
		}

		SoundEffect MakeCollisionSoundEffect (CollisionSoundEffectType collisionSoundEffectType, Vector3 hitPoint)
		{
			return MakeCollisionSoundEffect(collisionSoundEffectType, hitPoint, rigid.GetPointVelocity(hitPoint).magnitude);
		}

		SoundEffect MakeCollisionSoundEffect (CollisionSoundEffectType collisionSoundEffectType, Vector3 hitPoint, float hitSpeed)
		{
			if (collisionSoundEffectType == CollisionSoundEffectType.Wall)
			{
				float volume = wallCollisionVelocityToVolumeCurve.Evaluate(hitSpeed);
				return AudioManager.instance.MakeSoundEffect(collisionWithWallAudioClips[Random.Range(0, collisionWithWallAudioClips.Length)], hitPoint, volume);
			}
			else
				return null;
		}

		public enum CollisionSoundEffectType
		{
			Wall
		}
	}
}