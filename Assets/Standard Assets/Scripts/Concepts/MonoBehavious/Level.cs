using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class Level : SingletonMonoBehaviour<Level>, IUpdatable
	{
		public TMP_Text timerText;
		
		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public void DoUpdate ()
		{
			timerText.text = string.Format("{0:0.#}", Time.timeSinceLevelLoad);
		}

#if UNITY_EDITOR
		[MenuItem("Tools/Update Tiles")]
		static void UpdateTiles ()
		{
			// EditorCoroutineUtility.StartCoroutineOwnerless(UpdateTilesRoutine ());
			Tile[] tiles = SelectionExtensions.GetSelected<Tile>();
			for (int i = 0; i < tiles.Length; i ++)
			{
				Tile tile = tiles[i];
				if (!tile.enabled)
				{
					tiles = tiles.RemoveAt(i);
					i --;
				}
				else
					tile.neighbors = new Tile[0];
			}
			List<Tile> tilesRemaining = new List<Tile>(tiles);
			List<List<Tile>> connectedTileGroups = new List<List<Tile>>();
			List<List<Tile>> supportingTileGroups = new List<List<Tile>>();
			while (tilesRemaining.Count > 0)
			{
				Tile tile = tilesRemaining[0];
				for (int i = 0; i < tiles.Length; i ++)
				{
					Tile tile2 = tiles[i];
					if (tile != tile2 && Tile.AreNeighbors(tile, tile2))
					{
						if (!tile.neighbors.Contains(tile2))
							tile.neighbors = tile.neighbors.Add(tile2);
						if (!tile2.neighbors.Contains(tile))
							tile2.neighbors = tile2.neighbors.Add(tile);
						List<Tile> connectedTileGroup = new List<Tile>() { tile, tile2 };
						List<Tile> supportingTileGroup = new List<Tile>();
						if (tile.isSupportingTile)
							supportingTileGroup.Add(tile);
						if (tile2.isSupportingTile)
							supportingTileGroup.Add(tile2);
						for (int i2 = 0; i2 < connectedTileGroup.Count; i2 ++)
						{
							Tile tile3 = tiles[i2];
							for (int i3 = 0; i3 < tiles.Length; i3 ++)
							{
								Tile tile4 = tiles[i3];
								if (Tile.AreNeighbors(tile3, tile4))
								{
									if (!connectedTileGroup.Contains(tile4))
									{
										connectedTileGroup.Add(tile4);
										if (tile4.isSupportingTile)
										{
											if (!connectedTileGroup.Contains(tile4))
												supportingTileGroup.Add(tile4);
										}
									}
								}
							}
						}
						connectedTileGroups.Add(connectedTileGroup);
						supportingTileGroups.Add(supportingTileGroup);
					}
				}
				tilesRemaining.RemoveAt(0);
			}
			for (int i = 0; i < connectedTileGroups.Count; i ++)
			{
				List<Tile> connectedTileGroup = connectedTileGroups[i];
				List<Tile> supportingTileGroup = supportingTileGroups[i];
				for (int i2 = 0; i2 < connectedTileGroup.Count; i2 ++)
				{
					Tile tile = connectedTileGroup[i2];
					tile.connectedTo = connectedTileGroup.ToArray().Remove(tile);
					tile.supportingTiles = supportingTileGroup.ToArray().Remove(tile);
				}
			}
		}

		static IEnumerator UpdateTilesRoutine ()
		{
			print("Began");
			Tile[] tiles = SelectionExtensions.GetSelected<Tile>();
			for (int i = 0; i < tiles.Length; i ++)
			{
				Tile tile = tiles[i];
				if (!tile.enabled)
				{
					tiles = tiles.RemoveAt(i);
					i --;
				}
				else
				{
					tile.neighbors = new Tile[0];
					tile.supportingTiles = new Tile[0];
				}
			}
			List<Tile> tilesRemaining = new List<Tile>(tiles);
			List<List<Tile>> connectedTileGroups = new List<List<Tile>>();
			List<List<Tile>> supportingTileGroups = new List<List<Tile>>();
			while (tilesRemaining.Count > 0)
			{
				Tile tile = tilesRemaining[0];
				for (int i = 0; i < tiles.Length; i ++)
				{
					Tile tile2 = tiles[i];
					if (tile != tile2 && Tile.AreNeighbors(tile, tile2))
					{
						if (!tile.neighbors.Contains(tile2))
							tile.neighbors = tile.neighbors.Add(tile2);
						if (!tile2.neighbors.Contains(tile))
							tile2.neighbors = tile2.neighbors.Add(tile);
						List<Tile> connectedTileGroup = new List<Tile>() { tile, tile2 };
						for (int i2 = 0; i2 < connectedTileGroup.Count; i2 ++)
						{
							Tile tile3 = tiles[i2];
							for (int i3 = 0; i3 < tiles.Length; i3 ++)
							{
								Tile tile4 = tiles[i3];
								if (Tile.AreNeighbors(tile3, tile4))
								{
									if (!connectedTileGroup.Contains(tile4))
										connectedTileGroup.Add(tile4);
								}
								yield return null;
							}
						}
						connectedTileGroups.Add(connectedTileGroup);
					}
				}
				tilesRemaining.RemoveAt(0);
				yield return null;
			}
			for (int i = 0; i < connectedTileGroups.Count; i ++)
			{
				List<Tile> connectedTileGroup = connectedTileGroups[i];
				for (int i2 = 0; i2 < connectedTileGroup.Count; i2 ++)
				{
					Tile tile = connectedTileGroup[i2];
					tile.connectedTo = connectedTileGroup.ToArray().Remove(tile);
				}
			}
			print("Done");
			yield break;
		}
#endif
	}
}