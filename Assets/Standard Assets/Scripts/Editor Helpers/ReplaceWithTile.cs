#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace AmbitiousSnake
{
	public class ReplaceWithTile : EditorScript
	{
		public Transform trs;
		public Tile replaceWithTilePrefab;

		public override void Do ()
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			if (replaceWithTilePrefab != null)
			{
				Tile tile = (Tile) PrefabUtility.InstantiatePrefab(replaceWithTilePrefab);
				tile.trs.position = trs.position;
				tile.trs.rotation = trs.rotation;
				tile.trs.SetParent(trs.parent);
				tile.trs.localScale = trs.localScale;
				Destroy(gameObject);
			}
		}
	}
}
#else
using UnityEngine;

namespace AmbitiousSnake
{
	public class ReplaceWithTile : EditorScript
	{
	}
}
#endif