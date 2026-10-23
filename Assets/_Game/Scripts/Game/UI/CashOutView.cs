using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class CashOutView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text labelValueText;
        [SerializeField] private string unlockedLabel = "COLLECT";
        [SerializeField] private string lockedLabel = "LOCKED";

        public event Action CashOutClicked;

        private RunState state;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (labelValueText == null)
                labelValueText = GetComponentInChildren<TMP_Text>(true);
        }
#endif

        public void Bind(RunState runState)
        {
            state = runState;
            state.Changed += Refresh;
            button.onClick.AddListener(OnClicked);
            Refresh();
        }

        private void OnClicked()
        {
            CashOutClicked?.Invoke();
        }

        private void Refresh()
        {
            bool canExit = state.CanExit;

            button.interactable = canExit;

            if (labelValueText != null)
                labelValueText.text = canExit ? unlockedLabel : lockedLabel;
        }

        private void OnDestroy()
        {
            if (state != null)
                state.Changed -= Refresh;

            if (button != null)
                button.onClick.RemoveListener(OnClicked);
        }
    }
}