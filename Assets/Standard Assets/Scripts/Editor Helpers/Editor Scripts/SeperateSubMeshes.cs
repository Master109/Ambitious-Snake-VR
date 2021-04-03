#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace AmbitiousSnake
{
	public class SeperateSubMeshes : EditorScript
	{
		public MeshFilter meshFilter;

		public override void Do ()
		{
			if (meshFilter == null)
				meshFilter = GetComponent<MeshFilter>();
			_Do (meshFilter.sharedMesh);
		}

		static void _Do (Mesh mesh)
		{
			print(mesh.subMeshCount);
		}

		[MenuItem("Tools/Seperate sub meshes of selected meshes")]
		static void _Do ()
		{
			Transform[] selectedTransforms = Selection.transforms;
			for (int i = 0; i < selectedTransforms.Length; i ++)
			{
				Transform selectedTrs = selectedTransforms[i];
				MeshFilter meshFilter = selectedTrs.GetComponent<MeshFilter>();
				if (meshFilter != null)
					_Do (meshFilter.sharedMesh);
			}
		}
	}
}
#else
namespace AmbitiousSnake
{
	public class SeperateSubMeshes : EditorScript
	{
	}
}
#endif