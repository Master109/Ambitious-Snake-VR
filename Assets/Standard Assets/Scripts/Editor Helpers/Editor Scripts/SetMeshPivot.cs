#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmbitiousSnake;

[ExecuteInEditMode]
public class SetMeshPivot : EditorScript
{
	public Transform trs;
	public MeshFilter[] meshFilters;
	
	public override void Do ()
	{
		for (int i = 0; i < meshFilters.Length; i ++)
		{
			MeshFilter meshFilter = meshFilters[i];
			meshFilter.GetComponent<Transform>().position += meshFilter.GetComponent<Transform>().rotation * (trs.position - meshFilter.sharedMesh.bounds.center);
		}
	}
}
#endif