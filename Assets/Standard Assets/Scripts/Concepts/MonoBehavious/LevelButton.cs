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
		public Image parTimeIcon;
		public Image starIcon;
		public Button button;
		public float maxDoubleClickRate;
		float lastClickedTime;

		void OnEnable ()
		{
#if UNITY_EDITOR
			levelNameText.text = "" + (trs.GetSiblingIndex() + 1);
			LevelSelect.instance = trs.root.GetComponent<LevelSelect>();
#endif
			lastClickedTime = Mathf.NegativeInfinity;
			Level level = LevelSelect.Instance.levels[trs.GetSiblingIndex()];
			if (level.HasWon)
				bestTimeText.text = "Best time: " + string.Format("{0:0.#}", level.FastestTime);
			parTimeIcon.enabled = level.GotParTime;
			starIcon.enabled = level.CollectedStar;
#if UNITY_EDITOR
			parTimeText.text = "Par time: " + level.parTime;
#endif
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(OnButtonClicked);
		}

		void OnButtonClicked ()
		{
			if (Time.realtimeSinceStartup - lastClickedTime <= maxDoubleClickRate)
				_SceneManager.instance.LoadSceneWithoutTransition (trs.GetSiblingIndex());
			lastClickedTime = Time.realtimeSinceStartup;
			if (LevelSelect.selectedLevelIndex != MathfExtensions.NULL_INT)
				LevelSelect.instance.StopLevelPreview (LevelSelect.selectedLevelIndex);
			LevelSelect.selectedLevelIndex = trs.GetSiblingIndex();
			LevelSelect.instance.StartLevelPreview (LevelSelect.selectedLevelIndex);
		}
	}
}