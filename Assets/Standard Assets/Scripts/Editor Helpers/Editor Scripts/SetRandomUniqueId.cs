#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Extensions;

namespace AmbitiousSnake
{
	public class SetRandomUniqueId : EditorScript
	{
		public SaveAndLoadObject saveAndLoadObject;

		public override void Do ()
		{
			if (saveAndLoadObject == null)
				saveAndLoadObject = GetComponent<SaveAndLoadObject>();
			_Do (saveAndLoadObject);
		}

		static void _Do (SaveAndLoadObject saveAndLoadObject)
		{
			saveAndLoadObject.uniqueId = Random.Range(int.MinValue, int.MaxValue);
		}
		
		[MenuItem("Tools/Randomize selected SaveAndLoadObjects' unique ids")]
		static void _Do ()
		{
			Transform[] selectedTransforms = Selection.transforms;
			for (int i = 0; i < selectedTransforms.Length; i ++)
			{
				Transform selectedTrs = selectedTransforms[i];
				SaveAndLoadObject saveAndLoadObject = selectedTrs.GetComponent<SaveAndLoadObject>();
				if (saveAndLoadObject != null)
					_Do (saveAndLoadObject);
			}
		}
	}
}
#else
namespace AmbitiousSnake
{
	public class SetRandomUniqueId : EditorScript
	{
	}
}
#endif