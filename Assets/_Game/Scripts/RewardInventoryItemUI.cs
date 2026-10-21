
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public class RewardInventoryItemUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private TMP_Text totalValueText;

        public void Bind(GameManager.RewardStack stack)
        {
            if (stack == null || stack.Reward == null)
                return;

            if (iconImage != null)
                iconImage.sprite = stack.Reward.Icon;

            if (nameText != null)
                nameText.text = stack.Reward.DisplayName;

            Refresh(stack);
        }

        public void Refresh(GameManager.RewardStack stack)
        {
            if (stack == null)
                return;

            if (countText != null)
                countText.text = $"x{stack.Count}";

            if (totalValueText != null)
                totalValueText.text = stack.TotalValue.ToString();
        }
    }
}
