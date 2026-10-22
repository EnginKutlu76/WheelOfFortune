
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public class RewardInventoryItemUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text totalValueText;

        public void Bind(GameManager.RewardStack stack)
        {
            if (stack == null || stack.Reward == null)
                return;

            if (iconImage != null)
            {
                iconImage.sprite = stack.Reward.InventoryIcon;
                iconImage.preserveAspect = true;
                iconImage.color = Color.white;
                iconImage.enabled = iconImage.sprite != null;
            }

            Refresh(stack);
        }

        public void Refresh(GameManager.RewardStack stack)
        {
            if (stack == null)
                return;

            if (totalValueText != null)
                totalValueText.text = stack.TotalValue.ToString();
        }
    }
}
