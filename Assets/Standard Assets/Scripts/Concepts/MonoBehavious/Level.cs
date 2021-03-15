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
		public Graph<Tile>[] tileGraphs = new Graph<Tile>[0];
		
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
			Tile[] tiles = SelectionExtensions.GetSelected<Tile>();
			List<Graph<Tile>.Node> tileNodes = new List<Graph<Tile>.Node>();
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
					tileNodes.Add(new Graph<Tile>.Node(tile));
				}
			}
			for (int i = 0; i < tiles.Length; i ++)
			{
				Tile tile = tiles[i];
				for (int i2 = 0; i2 < tiles.Length; i2 ++)
				{
					Tile tile2 = tiles[i2];
					if (i != i2 && Tile.AreNeighbors(tile, tile2))
					{
						tile.neighbors = tile.neighbors.Add(tile2);
						tileNodes[i].connectedTo.Add(tileNodes[i2]);
					}
				}
			}
			List<Graph<Tile>> _tileGraphs = new List<Graph<Tile>>();
			Graph<Tile> tileGraph;
			List<Tile[]> supportingTileGroups = new List<Tile[]>();
			List<Tile> remainingTiles = new List<Tile>(tiles);
			while (remainingTiles.Count > 0)
			{
				tileGraph = new Graph<Tile>();
				Tile[] connectedTileGroup = Tile.GetConnectedGroup(remainingTiles[0]);
				List<Tile> supportingTiles = new List<Tile>();
				for (int i = 0; i < connectedTileGroup.Length; i ++)
				{
					Tile connectedTile = connectedTileGroup[i];
					if (connectedTile.isSupportingTile)
						supportingTiles.Add(connectedTile);
					for (int i2 = 0; i2 < tileNodes.Count; i2 ++)
					{
						Graph<Tile>.Node tileNode = tileNodes[i2];
						if (tileNode.value == connectedTile)
						{
							tileGraph.nodes.Add(tileNode);
							tileNodes.RemoveAt(i2);
							i2 --;
						}
					}
				}
				for (int i = 0; i < connectedTileGroup.Length; i ++)
				{
					Tile connectedTile = connectedTileGroup[i];
					remainingTiles.Remove(connectedTile);
				}
				supportingTileGroups.Add(supportingTiles.ToArray());
				_tileGraphs.Add(tileGraph);
			}
			Instance.tileGraphs = _tileGraphs.ToArray();
			for (int i = 0; i < _tileGraphs.Count; i ++)
			{
				tileGraph = _tileGraphs[i];
				Tile[] supportingTileGroup = supportingTileGroups[i];
				for (int i2 = 0; i2 < tileGraph.nodes.Count; i2 ++)
				{
					Tile tile = tileGraph.nodes[i2].value;
					tile.tileGraphIndex = i;
					tile.tileNodeIndex = i2;
					tile.supportingTiles = supportingTileGroup.Remove(tile);
				}
			}
		}
#endif
	}
}