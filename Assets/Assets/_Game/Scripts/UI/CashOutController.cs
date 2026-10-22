
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public class CashOutController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button cashOutButton;
        [SerializeField] private TMP_Text cashOutText;

        private void OnValidate()
        {
            if (cashOutButton == null)
                cashOutButton = GetComponent<Button>();

            if (cashOutText == null)
                cashOutText = GetComponentInChildren<TMP_Text>(true);
        }

        private void OnEnable()
        {
            if (cashOutButton != null)
                cashOutButton.onClick.AddListener(HandleCashOut);
        }

        private void OnDisable()
        {
            if (cashOutButton != null)
                cashOutButton.onClick.RemoveListener(HandleCashOut);
        }

        private void Update()
        {
            if (cashOutButton == null ||
                GameManager.Instance == null)
            {
                return;
            }

            cashOutButton.interactable =
                GameManager.Instance.CanExit;

            if (cashOutText != null)
            {
                cashOutText.text =
                    GameManager.Instance.CanExit
                        ? "COLLECT"
                        : "LOCKED";
            }
        }

        private void HandleCashOut()
        {
            if (GameManager.Instance == null)
                return;

            if (!GameManager.Instance.TryExit())
                return;

            Debug.Log(
                $"Player exited with {GameManager.Instance.AccumulatedRewards} rewards.");
        }
    }
}
