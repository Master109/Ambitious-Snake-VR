using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using TMPro;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class Level : SingletonMonoBehaviour<Level>, IUpdatable
	{
		public bool HasWon
		{
			get
			{
				return PlayerPrefsExtensions.GetBool(name + " won");
			}
			set
			{
				PlayerPrefsExtensions.SetBool(name + " won", value);
			}
		}
		public float FastestTime
		{
			get
			{
				return PlayerPrefs.GetFloat(name + " best time", MathfExtensions.NULL_INT);
			}
			set
			{
				PlayerPrefs.SetFloat(name + " best time", value);
			}
		}
		public float parTime;
		public TMP_Text timerText;
		public Color pastParTimeTimerColor;
		
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
			float timeSinceLevelLoad = Time.timeSinceLevelLoad;
			timerText.text = string.Format("{0:0.#}", timeSinceLevelLoad);
			if (timeSinceLevelLoad > parTime)
				timerText.color = pastParTimeTimerColor;
		}

#if UNITY_EDITOR
		[MenuItem("Tools/Update Tiles")]
		static void UpdateTiles ()
		{
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
			for (int i = 0; i < tiles.Length; i ++)
			{
				Tile tile = tiles[i];
				for (int i2 = 0; i2 < tiles.Length; i2 ++)
				{
					Tile tile2 = tiles[i2];
					if (i != i2 && Tile.AreNeighbors(tile, tile2))
						tile.neighbors = tile.neighbors.Add(tile2);
				}
			}
			List<Tile> _tiles = new List<Tile>(tiles);
			List<Tile[]> connectedTileGroups = new List<Tile[]>();
			List<Tile[]> supportingTileGroups = new List<Tile[]>();
			List<Tile> remainingTiles = new List<Tile>(tiles);
			while (remainingTiles.Count > 0)
			{
				Tile[] connectedTileGroup = Tile.GetConnectedGroup(remainingTiles[0]);
				connectedTileGroups.Add(connectedTileGroup);
				List<Tile> supportingTiles = new List<Tile>();
				for (int i = 0; i < connectedTileGroup.Length; i ++)
				{
					Tile connectedTile = connectedTileGroup[i];
					if (connectedTile.isSupportingTile)
						supportingTiles.Add(connectedTile);
				}
				for (int i = 0; i < connectedTileGroup.Length; i ++)
				{
					Tile connectedTile = connectedTileGroup[i];
					remainingTiles.Remove(connectedTile);
				}
				supportingTileGroups.Add(supportingTiles.ToArray());
			}
			for (int i = 0; i < connectedTileGroups.Count; i ++)
			{
				Tile[] connectedTileGroup = connectedTileGroups[i];
				Tile[] supportingTileGroup = supportingTileGroups[i];
				for (int i2 = 0; i2 < connectedTileGroup.Length; i2 ++)
				{
					Tile tile = connectedTileGroup[i2];
					tile.supportingTiles = supportingTileGroup;
				}
			}
		}
#endif
	}
}