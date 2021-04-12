using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AmbitiousSnake
{
	[ExecuteInEditMode]
	public class LevelButton : MonoBehaviour
	{
		public Transform trs;
		public TMP_Text levelNameText;
		public TMP_Text bestTimeText;
		public Button button;

		void OnEnable ()
		{
			levelNameText.text = "" + (trs.GetSiblingIndex() + 1);
			Level level = LevelSelect.instance.levels[trs.GetSiblingIndex()];
			if (level.HasWon)
				bestTimeText.text = "Best time: " + string.Format("{0:0.#}", level.FastestTime);
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => {_SceneManager.instance.LoadSceneWithoutTransition(trs.GetSiblingIndex());});
		}
	}
}