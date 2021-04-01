using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class Turret : UpdateWhileEnabled
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Timer reloadTimer;
		public Bullet bulletPrefab;
		public float bulletWidth;
		public LineRenderer line;
		public Color lockedOnColor;
		public Color searchingColor;
		public Transform trs;
		public Laser laser;
		Vector3 shootDirection;

		public override void DoUpdate ()
		{
			if (GameManager.paused)
				return;
			shootDirection = VectorExtensions.NULL3;
			for (int i = Snake.instance.pieces.Count; i >= 0; i --)
			{
				Vector3 snakePiecePosition = Snake.instance.pieces[i].trs.position;
				Vector3 toSnakePiecePosition = snakePiecePosition - trs.position;
				trs.rotation = Quaternion.LookRotation(toSnakePiecePosition);
				laser.DoUpdate ();
				if (laser.hit.collider != null && laser.hit.transform.root == Snake.instance.trs)
				{
					shootDirection = toSnakePiecePosition;
					line.SetPosition(1, snakePiecePosition);
					break;
				}
			}
			if (shootDirection != VectorExtensions.NULL3)
			{
				trs.rotation = Quaternion.LookRotation(shootDirection);
				line.startColor = lockedOnColor;
				line.endColor = lockedOnColor;
				if (reloadTimer.timeRemaining <= 0)
				{
					reloadTimer.Reset ();
					reloadTimer.Start ();
					ObjectPool.Instance.SpawnComponent<Bullet> (bulletPrefab, trs.position, Quaternion.LookRotation(shootDirection));
				}
			}
			else
			{
				trs.rotation = Quaternion.LookRotation(Snake.instance.HeadPosition - trs.position);
				line.startColor = searchingColor;
				line.endColor = searchingColor;
			}
		}
	}
}