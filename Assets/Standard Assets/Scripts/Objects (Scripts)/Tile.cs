using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using System;

namespace AmbitiousSnake
{
	public class Tile : MonoBehaviour
	{
		public Transform trs;
		public Tile[] neighbors = new Tile[0];
		public Tile[] supportingTiles = new Tile[0];
		public int tileGraphIndex;
		public int tileNodeIndex;
		public bool isSupportingTile;
		public Rigidbody rigid;

		public virtual void OnEnable ()
		{
		}

		public virtual void OnDestroy ()
		{
			if (_SceneManager.isLoading)
				return;
			for (int i = 0; i < neighbors.Length; i ++)
			{
				Tile tile = neighbors[i];
				tile.neighbors = tile.neighbors.Remove(this);
			}
			Graph<Tile> tileGraph = Level.instance.tileGraphs[tileGraphIndex];
			tileGraph.RemoveNode (tileGraph.nodes[tileNodeIndex], true);
			for (int i = tileNodeIndex; i < tileGraph.nodes.Count; i ++)
				tileGraph.nodes[i].value.tileNodeIndex ++;
			Level.instance.tileGraphs[tileGraphIndex] = tileGraph;
			for (int i = 0; i < neighbors.Length; i ++)
			{
				Tile neighbor = neighbors[i];
				List<Graph<Tile>.Node> connectedTileNodes = tileGraph.GetConnectedGroup(tileGraph.nodes[neighbor.tileNodeIndex]);
				Tile[] connectedTiles = new Tile[connectedTileNodes.Count];
				for (int i2 = 0; i2 < connectedTileNodes.Count; i2 ++)
					connectedTiles[i] = connectedTileNodes[i2].value;
				bool shouldSplit = true;
				for (int i2 = 0; i2 < neighbor.supportingTiles.Length; i2 ++)
				{
					Tile supportingTile = neighbor.supportingTiles[i2];
					if (connectedTiles.Contains(supportingTile))
					{
						shouldSplit = false;
						break;
					}
				}
				if (shouldSplit)
					SplitTiles (connectedTiles);
			}
		}

		public static void SplitTiles (Tile[] tiles)
		{
			Rigidbody rigid = new GameObject().AddComponent<Rigidbody>();
			rigid.mass = 0;
			Transform rigidTrs = rigid.GetComponent<Transform>();
			Vector3 worldCenterOfMass = new Vector3();
			for (int i = 0; i < tiles.Length; i ++)
			{
				Tile tile = tiles[i];
				tile.trs.SetParent(rigidTrs);
				tile.rigid = rigid;
				Bounds tileBounds = tile.trs.GetBounds();
				rigid.mass += tileBounds.GetVolume();
				for (float x = tileBounds.min.x; x < tileBounds.max.x; x ++)
				{
					for (float y = tileBounds.min.y; y < tileBounds.max.y; y ++)
					{
						for (float z = tileBounds.min.z; z < tileBounds.max.z; z ++)
							worldCenterOfMass += new Vector3(x, y, z) + Vector3.one / 2;
					}
				}
			}
			worldCenterOfMass /= rigid.mass;
			rigid.centerOfMass = rigidTrs.InverseTransformPoint(worldCenterOfMass);
		}

		public static bool AreNeighbors (Tile tile, Tile tile2)
		{
			// return tile.trs.GetBounds().Intersects(tile2.trs.GetBounds());
			Bounds tileBounds = tile.trs.GetBounds();
			Bounds tile2Bounds = tile2.trs.GetBounds();
			for (float x = tileBounds.min.x; x < tileBounds.max.x; x ++)
			{
				for (float y = tileBounds.min.y; y < tileBounds.max.y; y ++)
				{
					for (float z = tileBounds.min.z; z < tileBounds.max.z; z ++)
					{
						for (float x2 = tile2Bounds.min.x; x2 < tile2Bounds.max.x; x2 ++)
						{
							for (float y2 = tile2Bounds.min.y; y2 < tile2Bounds.max.y; y2 ++)
							{
								for (float z2 = tile2Bounds.min.z; z2 < tile2Bounds.max.z; z2 ++)
								{
									if ((new Vector3(x, y, z) - new Vector3(x2, y2, z2)).sqrMagnitude == 1)
										return true;
								}
							}
						}
					}
				}
			}
			return false;
			// BoundsInt tileBounds = tile.trs.GetBounds().ToBoundsInt();
			// BoundsInt tile2Bounds = tile2.trs.GetBounds().ToBoundsInt();
			// foreach (Vector3Int point in tileBounds.allPositionsWithin)
			// {
			// 	foreach (Vector3Int point2 in tile2Bounds.allPositionsWithin)
			// 	{
			// 		if ((point - point2).sqrMagnitude == 1)
			// 			return true;
			// 	}
			// }
			// return false;
		}

		public static Tile[] GetConnectedGroup (Tile tile)
		{
			List<Tile> output = new List<Tile>();
			List<Tile> checkedTiles = new List<Tile>() { tile };
			List<Tile> remainingTiles = new List<Tile>() { tile };
			while (remainingTiles.Count > 0)
			{
				Tile tile2 = remainingTiles[0];
				output.Add(tile2);
				for (int i = 0; i < tile2.neighbors.Length; i ++)
				{
					Tile connectedTile = tile2.neighbors[i];
					if (!checkedTiles.Contains(connectedTile))
					{
						checkedTiles.Add(connectedTile);
						remainingTiles.Add(connectedTile);
					}
				}
				remainingTiles.RemoveAt(0);
			}
			return output.ToArray();
		}
	}
}