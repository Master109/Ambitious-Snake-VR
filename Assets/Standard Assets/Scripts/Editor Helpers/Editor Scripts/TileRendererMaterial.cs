#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace AmbitiousSnake
{
	public class TileRendererMaterial : EditorScript
	{
		public Transform trs;
        public Renderer renderer;
		public float multiplyTextureScale;

		public override void Do ()
		{
            if (this == null)
                return;
			if (trs == null)
				trs = GetComponent<Transform>();
			if (renderer == null)
                renderer = GetComponent<Renderer>();
            renderer.material.mainTextureScale = trs.lossyScale * multiplyTextureScale;
		}
	}
}
#else
using UnityEngine;

namespace AmbitiousSnake
{
	public class TileRendererMaterial : EditorScript
	{
	}
}
#endif