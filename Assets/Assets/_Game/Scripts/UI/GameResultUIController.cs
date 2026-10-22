
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Core;
using VertigoGames.Gameplay;

namespace VertigoGames.UI
{
    public class GameResultUIController : MonoBehaviour
    {
        [Header("Result UI")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text resultTitleText;
        [SerializeField] private TMP_Text resultRewardText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button spinButton;

        [Header("Gameplay References")]
        [SerializeField] private WheelSpinController wheelSpinController;
        [SerializeField] private RewardPresentationController rewardPresentationController;

        private bool resultShown;

        private void OnEnable()
        {
            if (restartButton != null)
                restartButton.onClick.AddListener(RestartGame);

            if (reviveButton != null)
                reviveButton.onClick.AddListener(ReviveGame);

            if (resultPanel != null)
                resultPanel.SetActive(false);

            resultShown = false;
        }

        private void OnDisable()
        {
            if (restartButton != null)
                restartButton.onClick.RemoveListener(RestartGame);

            if (reviveButton != null)
                reviveButton.onClick.RemoveListener(ReviveGame);
        }

        private void Update()
        {
            if (resultShown || GameManager.Instance == null)
                return;

            if (GameManager.Instance.IsGameOver)
                ShowResult(false);
            else if (GameManager.Instance.HasExited)
                ShowResult(true);
        }

        private void ShowResult(bool collected)
        {
            resultShown = true;

            if (resultPanel != null)
                resultPanel.SetActive(true);

            if (resultTitleText != null)
            {
                resultTitleText.text = collected
                    ? "REWARDS COLLECTED"
                    : "BOMB HIT";
            }

            if (resultRewardText != null)
            {
                resultRewardText.text = collected
                    ? $"FINAL REWARD: {GameManager.Instance.AccumulatedRewards}"
                    : "ALL ACCUMULATED REWARDS LOST";
            }

            if (reviveButton != null)
            {
                reviveButton.gameObject.SetActive(
                    !collected && GameManager.Instance.CanRevive);
            }

            if (spinButton != null)
                spinButton.interactable = false;
        }

        private void ReviveGame()
        {
            if (GameManager.Instance == null)
                return;

            if (!GameManager.Instance.ReviveAfterBomb())
                return;

            if (rewardPresentationController == null)
            {
                rewardPresentationController =
                    FindFirstObjectByType<RewardPresentationController>(
                        FindObjectsInactive.Include);
            }

            if (rewardPresentationController != null)
                rewardPresentationController.ResetPresentation();

            if (resultPanel != null)
                resultPanel.SetActive(false);

            resultShown = false;

            if (wheelSpinController == null)
            {
                wheelSpinController =
                    FindFirstObjectByType<WheelSpinController>();
            }

            if (wheelSpinController != null)
                wheelSpinController.RefreshAfterRestart();

            if (spinButton != null)
                spinButton.interactable = true;

            Debug.Log(
                $"Revive successful. Continuing from zone " +
                $"{GameManager.Instance.CurrentZone}.");
        }

        private void RestartGame()
        {
            if (GameManager.Instance == null)
                return;

            GameManager.Instance.RestartRun();

            if (rewardPresentationController == null)
            {
                rewardPresentationController =
                    FindFirstObjectByType<RewardPresentationController>(
                        FindObjectsInactive.Include);
            }

            if (rewardPresentationController != null)
                rewardPresentationController.ResetPresentation();

            if (resultPanel != null)
                resultPanel.SetActive(false);

            resultShown = false;

            if (wheelSpinController == null)
            {
                wheelSpinController =
                    FindFirstObjectByType<WheelSpinController>();
            }

            if (wheelSpinController != null)
                wheelSpinController.RefreshAfterRestart();

            if (spinButton != null)
                spinButton.interactable = true;
        }
    }
}
