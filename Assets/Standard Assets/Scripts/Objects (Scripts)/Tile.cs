using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace AmbitiousSnake
{
	public class Tile : MonoBehaviour
	{
		public Transform trs;
		public Tile[] directlyConnectedTo = new Tile[0];
		public Tile[] indirectlyConnectedTo = new Tile[0];
		public Tile[] supportingTiles = new Tile[0];
		public bool isSupportingTile;
		public Rigidbody rigid;

		public virtual void OnDestroy ()
		{
			if (_SceneManager.isLoading)
				return;
			for (int i = 0; i < directlyConnectedTo.Length; i ++)
			{
				Tile tile = directlyConnectedTo[i];
				if (tile != null)
					tile.directlyConnectedTo = tile.directlyConnectedTo.Remove(this);
			}
			List<List<Tile>> unsupportedTileGroups = new List<List<Tile>>();
			for (int i = 0; i < indirectlyConnectedTo.Length; i ++)
			{
				Tile tile = indirectlyConnectedTo[i];
				if (tile != null)
				{
					tile.indirectlyConnectedTo = tile.indirectlyConnectedTo.Remove(this);
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
									for (int i4 = 0; i4 < unsupportedTile.indirectlyConnectedTo.Length; i4 ++)
									{
										Tile tile2 = unsupportedTile.indirectlyConnectedTo[i4];
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
				Transform rigidTrs = rigid.GetComponent<Transform>();
				for (int i2 = 0; i2 < unsupportedTileGroup.Count; i2 ++)
				{
					Tile unsupportedTile = unsupportedTileGroup[i2];
					unsupportedTile.trs.SetParent(rigidTrs);
				}
			}
		}
	}
}