
using TMPro;
using UnityEngine;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public class GameUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text zoneValueText;
        [SerializeField] private TMP_Text rewardValueText;

        private int lastZone = -1;
        private int lastReward = -1;

        private void OnValidate()
        {
            TMP_Text[] textComponents =
                GetComponentsInChildren<TMP_Text>(true);

            foreach (TMP_Text textComponent in textComponents)
            {
                if (textComponent.gameObject.name == "ui_text_zone_value")
                    zoneValueText = textComponent;

                if (textComponent.gameObject.name == "ui_text_reward_value")
                    rewardValueText = textComponent;
            }
        }

        private void OnEnable()
        {
            RefreshUI(true);
        }

        private void Update()
        {
            RefreshUI(false);
        }

        private void RefreshUI(bool forceRefresh)
        {
            if (GameManager.Instance == null)
                return;

            int currentZone = GameManager.Instance.CurrentZone;
            int currentReward = GameManager.Instance.AccumulatedRewards;

            if (zoneValueText != null &&
                (forceRefresh || currentZone != lastZone))
            {
                zoneValueText.text = $"ZONE {currentZone}";
                lastZone = currentZone;
            }

            if (rewardValueText != null &&
                (forceRefresh || currentReward != lastReward))
            {
                rewardValueText.text = $"TOTAL REWARD {currentReward}";
                lastReward = currentReward;
            }
        }
    }
}
