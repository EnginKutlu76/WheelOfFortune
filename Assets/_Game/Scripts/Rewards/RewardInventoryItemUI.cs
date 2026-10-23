using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Common;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public sealed class RewardInventoryItemUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text totalValueText;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIRefs.AssignChild(this, ref iconImage, "ui_image_reward_icon");
            UIRefs.AssignChild(this, ref totalValueText, "ui_text_reward_total_value");
        }
#endif

        public void Bind(RewardStack stack)
        {
            Sprite icon = stack.Reward.InventoryIcon;

            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
            iconImage.maskable = true;
            iconImage.enabled = icon != null;

            Refresh(stack);
        }

        public void Refresh(RewardStack stack)
        {
            totalValueText.text = stack.TotalValue.ToString();
        }
    }
}