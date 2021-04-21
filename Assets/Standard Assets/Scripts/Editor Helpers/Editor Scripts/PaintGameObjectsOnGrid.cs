#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEditor;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class PaintGameObjectsOnGrid : EditorScript
	{
		public GameObject go;
		public Transform paintParent;
		List<Vector3> previousPaintPositions = new List<Vector3>();
		Bounds goBounds;
		Bounds[] gosBounds = new Bounds[0];

		[MenuItem("Tools/Start painting GameObjects _F1")]
		static void _StartPainting ()
		{
			FindObjectOfType<PaintGameObjectsOnGrid>().StartPainting ();
		}

		[MenuItem("Tools/Stop painting GameObjects #F1")]
		static void _StopPainting ()
		{
			FindObjectOfType<PaintGameObjectsOnGrid>().StopPainting ();
		}

		public void StartPainting ()
		{
			EditorApplication.update -= Paint;
			SceneView.duringSceneGui -= OnSceneGUI;
			previousPaintPositions.Clear();
			goBounds = go.GetComponentInChildren<Renderer>().bounds;
			Collider[] colliders = FindObjectsOfType<Collider>();
			List<Bounds> _gosBounds = new List<Bounds>();
			for (int i = 0; i < colliders.Length; i ++)
			{
				Collider collider = colliders[i];
				Renderer renderer = collider.GetComponent<Renderer>();
				if (renderer != null)
					_gosBounds.Add(renderer.bounds);
			}
			gosBounds = _gosBounds.ToArray();
			SceneView.duringSceneGui += OnSceneGUI;
			EditorApplication.update += Paint;
		}

		void Paint ()
		{
			Ray mouseRay = GetMouseRay();
			for (int i = 0; i < gosBounds.Length; i ++)
			{
				Bounds bounds = gosBounds[i];
				Vector3 hit;
				if (bounds.Raycast(mouseRay, out hit))
				{
					Vector3 spawnPosition = new Vector3();
					Quaternion spawnRotation = new Quaternion();
					if (hit.x == bounds.min.x)
					{
						spawnPosition = new Vector3(hit.x - goBounds.extents.x, Mathf.Round(hit.y), Mathf.Round(hit.z));
						spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.left);
					}
					else if (hit.x == bounds.max.x)
					{
						spawnPosition = new Vector3(hit.x + goBounds.extents.x, Mathf.Round(hit.y), Mathf.Round(hit.z));
						spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.right);
					}
					else if (hit.y == bounds.min.y)
					{
						spawnPosition = new Vector3(Mathf.Round(hit.x), hit.y - goBounds.extents.y, Mathf.Round(hit.z));
						spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
					}
					else if (hit.y == bounds.max.y)
					{
						spawnPosition = new Vector3(Mathf.Round(hit.x), hit.y + goBounds.extents.y, Mathf.Round(hit.z));
						spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
					}
					else if (hit.z == bounds.min.z)
					{
						spawnPosition = new Vector3(Mathf.Round(hit.x), Mathf.Round(hit.y), hit.z - goBounds.extents.z);
						spawnRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
					}
					else if (hit.z == bounds.max.z)
					{
						spawnPosition = new Vector3(Mathf.Round(hit.x), Mathf.Round(hit.y), hit.z + goBounds.extents.z);
						spawnRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
					}
					if (!previousPaintPositions.Contains(spawnPosition))
					{
						Instantiate(go, spawnPosition, spawnRotation, paintParent);
						previousPaintPositions.Add(spawnPosition);
					}
				}
			}
		}

		public void StopPainting ()
		{
			EditorApplication.update -= Paint;
			SceneView.duringSceneGui -= OnSceneGUI;
		}

		public override void OnEnable ()
		{
			base.OnEnable ();
			StopPainting ();
		}

		void OnSceneGUI (SceneView sceneView)
		{
			UpdateHotkeys ();
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			StopPainting ();
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			StopPainting ();
		}
	}

	[CustomEditor(typeof(PaintGameObjectsOnGrid))]
	public class PaintGameObjectsOnGridEditor : EditorScriptEditor
	{
	}
}
#else
namespace AmbitiousSnake
{
	public class PaintGameObjectsOnGrid : EditorScript
	{
	}
}
#endif