using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
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
			Tile[] tiles = FindObjectsOfType<Tile>();
			for (int i = 0; i < tiles.Length; i ++)
			{
				Tile tile = tiles[i];
				tile.connectedTo = new Tile[0];
				tile.supportingTiles = new Tile[0];
			}
			for (int i = 0; i < tiles.Length; i ++)
			{
				Tile tile = tiles[i];
				for (int i2 = 0; i2 < tiles.Length; i2 ++)
				{
					Tile tile2 = tiles[i2];
					if (tile != tile2)
					{
						bool tileIsConnectedToTile2 = tile.connectedTo.Contains(tile2);
						bool tile2IsConnectedToTile = tile2.connectedTo.Contains(tile);
						bool areNeighbors = Tile.AreNeighbors(tile, tile2);
						if (tileIsConnectedToTile2 != tile2IsConnectedToTile)
						{
							if (tileIsConnectedToTile2)
								ConnectTile (tile, tile2, areNeighbors);
							else
								ConnectTile (tile2, tile, areNeighbors);
						}
						else if (!tileIsConnectedToTile2 && areNeighbors)
						{
							ConnectTile (tile, tile2, true);
							ConnectTile (tile2, tile, true);
						}
					}
				}
			}
		}

		static void ConnectTile (Tile connectFrom, Tile connectTo, bool areNeighbors)
		{
			if (areNeighbors)
				connectFrom.neighbors = connectFrom.neighbors.Add(connectTo);
			connectFrom.connectedTo = connectFrom.connectedTo.Add(connectTo);
			if (connectTo.isSupportingTile)
				connectFrom.supportingTiles = connectFrom.supportingTiles.Add(connectTo);
			for (int i = 0; i < connectTo.connectedTo.Length; i ++)
			{
				Tile tile = connectTo.connectedTo[i];
				if (tile != connectFrom && !connectFrom.connectedTo.Contains(tile))
				{
					// connectFrom.connectedTo = connectFrom.connectedTo.Add(tile);
					// if (tile.isSupportingTile)
					// 	connectFrom.supportingTiles = connectFrom.supportingTiles.Add(tile);
					ConnectTile (connectFrom, tile, Tile.AreNeighbors(connectFrom, tile));
				}
			}
			for (int i = 0; i < connectFrom.connectedTo.Length; i ++)
			{
				Tile tile = connectFrom.connectedTo[i];
				if (tile != connectTo && !connectTo.connectedTo.Contains(tile))
				{
					// connectTo.connectedTo = connectTo.connectedTo.Add(tile);
					// if (tile.isSupportingTile)
					// 	connectTo.supportingTiles = connectTo.supportingTiles.Add(tile);
					ConnectTile (connectTo, tile, Tile.AreNeighbors(connectTo, tile));
				}
			}
		}
#endif
	}
}