using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmbitiousSnake
{
	public class Bullet : Hazard
	{
		public float range;
		public Rigidbody rigid;
		public float moveSpeed;
		public Transform shooter;
		public ObjectPool.RangedDespawn rangedDespawn;
		public new Collider collider;
		[HideInInspector]
		public bool dead;
		public bool destroyOnContact;
		
		public virtual void OnEnable ()
		{
			dead = false;
			rangedDespawn = ObjectPool.Instance.RangeDespawn(prefabIndex, gameObject, trs, range);
			rigid.velocity = trs.forward * moveSpeed;
		}
		
		public override void ApplyDamage (IDestructable destructable, float amount)
		{
			if (destructable.Hp == 0)
				return;
			base.ApplyDamage (destructable, amount);
			if (destructable.Hp == 0 && shooter != null)
			{
				SoundEffect.Settings deathResponseSettings = new SoundEffect.Settings();
				deathResponseSettings.audioClip = AudioManager.Instance.deathResponses[Random.Range(0, AudioManager.Instance.deathResponses.Length)];
				deathResponseSettings.persistant = true;
				SoundEffect soundEffect = AudioManager.Instance.MakeSoundEffect(deathResponseSettings);
				soundEffect.trs.SetParent(shooter);
				soundEffect.trs.localPosition = Vector3.zero;
				soundEffect.trs.localEulerAngles = Vector3.zero;
			}
		}

		public virtual void OnDisable ()
		{
			StopAllCoroutines();
			dead = true;
		}

		public virtual void OnDestroy ()
		{
			ObjectPool.Instance.CancelRangedDespawn (rangedDespawn);
			dead = true;
		}

		public override void OnCollisionEnter (Collision coll)
		{
			if (!dead)
			{
				base.OnCollisionEnter (coll);
				if (destroyOnContact)
				{
					if (prefabIndex == -1)
						Destroy(gameObject);
					else
						ObjectPool.Instance.Despawn (prefabIndex, gameObject, trs);
				}
			}
		}
	}
}