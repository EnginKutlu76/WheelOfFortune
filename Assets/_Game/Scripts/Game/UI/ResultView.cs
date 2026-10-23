using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Common;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public sealed class ResultView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Image panelImage;
        [SerializeField] private TMP_Text titleValueText;
        [SerializeField] private TMP_Text messageValueText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button reviveButton;

        [Header("Texts")]
        [SerializeField] private string bombTitle = "BOMB HIT";
        [SerializeField] private string collectedTitle = "REWARDS COLLECTED";
        [SerializeField] private string reviveMessage = "Revive to keep your rewards.";
        [SerializeField] private string lostMessage = "All rewards lost.";
        [SerializeField] private string collectedFormat = "You collected {0} rewards.";

        [Header("Colors")]
        [SerializeField] private Color normalPanelColor = Color.white;
        [SerializeField]
        private Color bombPanelColor =
            new Color(0.35f, 0.05f, 0.05f, 1f);

        public event Action RestartClicked;
        public event Action ReviveClicked;

        private RunState state;
        private RunPhase? lastPhase;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIRefs.Assign(ref panelImage, "ui_panel_result");
            UIRefs.Assign(ref titleValueText, "ui_text_result_title_value");
            UIRefs.Assign(ref messageValueText, "ui_text_result_message_value");
            UIRefs.Assign(ref restartButton, "ui_button_restart");
            UIRefs.Assign(ref reviveButton, "ui_button_revive");

            if (panelRoot == null && panelImage != null)
                panelRoot = panelImage.gameObject;
        }
#endif

        public void Bind(RunState runState)
        {
            state = runState;
            state.Changed += Refresh;

            restartButton.onClick.AddListener(OnRestartClicked);
            reviveButton.onClick.AddListener(OnReviveClicked);

            Refresh();
        }

        private void OnRestartClicked() => RestartClicked?.Invoke();
        private void OnReviveClicked() => ReviveClicked?.Invoke();

        private void Refresh()
        {
            RunPhase phase = state.Phase;

            if (phase == lastPhase)
                return;

            lastPhase = phase;

            bool show = phase != RunPhase.Playing;
            panelRoot.SetActive(show);

            if (!show)
                return;

            bool bomb = phase == RunPhase.BombHit;

            if (panelImage != null)
                panelImage.color = bomb ? bombPanelColor : normalPanelColor;

            titleValueText.text = bomb ? bombTitle : collectedTitle;

            messageValueText.text = bomb
                ? (state.CanRevive ? reviveMessage : lostMessage)
                : string.Format(collectedFormat, CountRewards());

            reviveButton.gameObject.SetActive(bomb && state.CanRevive);
        }

        private int CountRewards()
        {
            int total = 0;

            foreach (RewardStack stack in state.Stacks)
                total += stack.Count;

            return total;
        }

        private void OnDestroy()
        {
            if (state != null)
                state.Changed -= Refresh;

            if (restartButton != null)
                restartButton.onClick.RemoveListener(OnRestartClicked);

            if (reviveButton != null)
                reviveButton.onClick.RemoveListener(OnReviveClicked);
        }
    }
}