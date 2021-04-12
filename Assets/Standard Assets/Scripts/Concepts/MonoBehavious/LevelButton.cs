using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class LevelButton : MonoBehaviour
	{
		public Transform trs;
		public TMP_Text levelNameText;
		public TMP_Text bestTimeText;
		public TMP_Text parTimeText;
		public GameObject parTimeIconGo;
		public GameObject starIconGo;
		public Button button;

		void OnEnable ()
		{
#if UNITY_EDITOR
			levelNameText.text = "" + (trs.GetSiblingIndex() + 1);
			LevelSelect.instance = trs.root.GetComponent<LevelSelect>();
#endif
			Level level = LevelSelect.Instance.levels[trs.GetSiblingIndex()];
			if (level.HasWon)
				bestTimeText.text = "Best time: " + string.Format("{0:0.#}", level.FastestTime);
			parTimeIconGo.SetActive(level.FastestTime <= level.parTime);
			starIconGo.SetActive(PlayerPrefsExtensions.GetBool(level.name + " star collected", false));
#if UNITY_EDITOR
			parTimeText.text = "Par time: " + level.parTime;
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => {_SceneManager.instance.LoadSceneWithoutTransition (trs.GetSiblingIndex());});
#endif
		}
	}
}