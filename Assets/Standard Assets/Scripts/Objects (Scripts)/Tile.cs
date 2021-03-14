using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace AmbitiousSnake
{
	public class Tile : MonoBehaviour
	{
		public Transform trs;
		public Tile[] neighbors = new Tile[0];
		public Tile[] connectedTo = new Tile[0];
		public Tile[] supportingTiles = new Tile[0];
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
				if (tile == null)
					tile.neighbors = tile.neighbors.Remove(this);
			}
			List<List<Tile>> unsupportedTileGroups = new List<List<Tile>>();
			for (int i = 0; i < connectedTo.Length; i ++)
			{
				Tile tile = connectedTo[i];
				if (tile != null)
				{
					tile.connectedTo = tile.connectedTo.Remove(this);
					if (isSupportingTile)
					{
						tile.supportingTiles = tile.supportingTiles.Remove(this);
						if (tile.supportingTiles.Length == 0 && tile.rigid == null)
						{
							bool isInUnsupportedTileGroup = false;
							for (int i2 = 0; i2 < unsupportedTileGroups.Count; i2 ++)
							{
								List<Tile> unsupportedTileGroup = unsupportedTileGroups[i2];
								for (int i3 = 0; i3 < unsupportedTileGroup.Count; i3 ++)
								{
									Tile unsupportedTile = unsupportedTileGroup[i3];
									for (int i4 = 0; i4 < unsupportedTile.connectedTo.Length; i4 ++)
									{
										Tile tile2 = unsupportedTile.connectedTo[i4];
										if (tile == tile2)
										{
											unsupportedTileGroups[i2].Add(tile);
											isInUnsupportedTileGroup = true;
											break;
										}
									}
									if (isInUnsupportedTileGroup)
										break;
								}
								if (isInUnsupportedTileGroup)
									break;
							}
							if (!isInUnsupportedTileGroup)
								unsupportedTileGroups.Add(new List<Tile>() { tile });
						}
					}
				}
			}
			for (int i = 0; i < unsupportedTileGroups.Count; i ++)
			{
				List<Tile> unsupportedTileGroup = unsupportedTileGroups[i];
				rigid = new GameObject().AddComponent<Rigidbody>();
				rigid.mass = 0;
				Transform rigidTrs = rigid.GetComponent<Transform>();
				Vector3 worldCenterOfMass = new Vector3();
				for (int i2 = 0; i2 < unsupportedTileGroup.Count; i2 ++)
				{
					Tile unsupportedTile = unsupportedTileGroup[i2];
					unsupportedTile.trs.SetParent(rigidTrs);
					unsupportedTile.rigid = rigid;
					BoundsInt unsupportedTileBounds = unsupportedTile.trs.GetBounds().ToBoundsInt();
					rigid.mass += unsupportedTileBounds.GetVolume();
					foreach (Vector3 point in unsupportedTileBounds.allPositionsWithin)
					{
						worldCenterOfMass += point;
					}
				}
				worldCenterOfMass /= rigid.mass;
				rigid.centerOfMass = rigidTrs.InverseTransformPoint(worldCenterOfMass);
			}
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
			return (tile.trs.position - tile2.trs.position).sqrMagnitude == 1;
		}
	}
}