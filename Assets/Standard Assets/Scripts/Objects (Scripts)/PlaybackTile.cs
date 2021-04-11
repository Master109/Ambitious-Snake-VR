using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	public class PlaybackTile : Tile, ICollisionEnterHandler
	{
		public Collider collider;
		public Collider Collider
		{
			get
			{
				return collider;
			}
		}
		public SnakeMimic snakeMimicPrefab;
		public MeshRenderer meshRenderer;
		public static SnakeRecording[] recordings = new SnakeRecording[0];
		bool hasBeenUsed;

		void Awake ()
		{
			recordings = new SnakeRecording[0];
		}
		
		public void OnCollisionEnter (Collision coll)
		{
			if (hasBeenUsed)
				return;
			hasBeenUsed = true;
            meshRenderer.material.color = meshRenderer.material.color.Divide(2);
			for (int i = 0; i < RecorderTile.areRecording.Length; i ++)
			{
				RecorderTile recorder = RecorderTile.areRecording[i];
				recorder.StopRecording ();
				recordings = recordings.Add(recorder.currentRecording);
			}
			for (int i = 0; i < recordings.Length; i ++)
			{
				SnakeRecording recording = recordings[i];
				SnakeMimic mimic = ObjectPool.instance.SpawnComponent<SnakeMimic>(snakeMimicPrefab.prefabIndex);
				mimic.playing = recording;
			}
		}
	}
}