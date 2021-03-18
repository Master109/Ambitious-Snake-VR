#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class CombineTiles : EditorScript
	{
		public bool checkForNeighbors;

		public override void Do ()
		{
			Tile[] tiles = SelectionExtensions.GetSelected<Tile>();
			if (!checkForNeighbors)
			{
				for (int i = 0; i < tiles.Length; i ++)
				{
					Tile tile = tiles[i];
					
				}
			}
			else
			{
				
			}
		}
	}
}
#else
using UnityEngine;

namespace AmbitiousSnake
{
	public class CombineTiles : EditorScript
	{
	}
}
#endif