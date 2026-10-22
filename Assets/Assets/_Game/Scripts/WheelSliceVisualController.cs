
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoGames.Data;

namespace VertigoGames.Gameplay
{
    public class WheelSliceVisualController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform wheelRotation;
        [SerializeField] private List<Image> sliceImages = new();
        [SerializeField] private List<TMP_Text> valueTexts = new();
        [SerializeField] private Sprite bombIcon;

        [Header("Layout")]
        [SerializeField, Range(0.1f, 0.5f)]
        private float radiusRatio = 0.33f;

        [SerializeField, Range(0.05f, 0.2f)]
        private float iconSizeRatio = 0.12f;

        [SerializeField, Range(0.01f, 0.15f)]
        private float valueOffsetRatio = 0.07f;

        [SerializeField] private float startAngle = 90f;

        [Header("Value Text")]
        [SerializeField] private float valueFontSize = 18f;
        [SerializeField] private Color valueColor = Color.white;

        private const int SliceCount = 8;

        private void Awake()
        {
            ConfigureRotationRect();
            PositionIcons();
            ClearSliceVisuals();
        }

        private void OnValidate()
        {
            if (sliceImages != null && sliceImages.Count == SliceCount)
                PositionIcons();
        }

        private void ConfigureRotationRect()
        {
            if (wheelRotation == null)
                return;

            wheelRotation.anchorMin = Vector2.zero;
            wheelRotation.anchorMax = Vector2.one;
            wheelRotation.offsetMin = Vector2.zero;
            wheelRotation.offsetMax = Vector2.zero;
            wheelRotation.pivot = new Vector2(0.5f, 0.5f);
        }

        public void UpdateSliceVisuals(
            IReadOnlyList<WheelSliceData> slices)
        {
            if (slices == null || slices.Count != SliceCount)
            {
                Debug.LogError("Exactly 8 slices are required.");
                return;
            }

            if (sliceImages == null || sliceImages.Count != SliceCount)
            {
                Debug.LogError("Assign exactly 8 slice images.");
                return;
            }

            if (valueTexts == null || valueTexts.Count != SliceCount)
            {
                Debug.LogError("Assign exactly 8 value text objects.");
                return;
            }

            for (int i = 0; i < SliceCount; i++)
            {
                Image image = sliceImages[i];
                TMP_Text valueText = valueTexts[i];
                WheelSliceData slice = slices[i];

                if (slice == null)
                    continue;

                Sprite icon = slice.Type == SliceType.Bomb
                    ? bombIcon
                    : slice.Reward != null
                        ? slice.Reward.Icon
                        : null;

                if (image != null)
                {
                    image.sprite = icon;
                    image.enabled = icon != null;
                    image.preserveAspect = true;
                    image.raycastTarget = false;
                }

                if (valueText != null)
                {
                    valueText.text = slice.Type == SliceType.Bomb
                        ? "BOMB"
                        : GetRewardValueText(slice.Reward);

                    valueText.fontSize = valueFontSize;
                    valueText.color = valueColor;
                    valueText.alignment = TextAlignmentOptions.Center;
                    valueText.raycastTarget = false;
                    valueText.gameObject.SetActive(true);
                }
            }

            PositionIcons();
        }

        private string GetRewardValueText(RewardData reward)
        {
            if (reward == null)
                return string.Empty;

            return $"x{reward.Value}";
        }

        public void ClearSliceVisuals()
        {
            if (sliceImages != null)
            {
                foreach (Image image in sliceImages)
                {
                    if (image == null)
                        continue;

                    image.sprite = null;
                    image.enabled = false;
                }
            }

            if (valueTexts != null)
            {
                foreach (TMP_Text valueText in valueTexts)
                {
                    if (valueText == null)
                        continue;

                    valueText.text = string.Empty;
                    valueText.gameObject.SetActive(false);
                }
            }
        }

        private void PositionIcons()
        {
            if (wheelRotation == null ||
                sliceImages == null ||
                sliceImages.Count != SliceCount)
            {
                return;
            }

            float diameter = Mathf.Min(
                wheelRotation.rect.width,
                wheelRotation.rect.height);

            if (diameter <= 0f)
                return;

            float radius = diameter * 0.35f;
            float iconSize = diameter * 0.10f;

            for (int i = 0; i < SliceCount; i++)
            {
                float angleDegrees = startAngle - i * 45f;
                float angleRadians = angleDegrees * Mathf.Deg2Rad;

                Vector2 direction = new Vector2(
                    Mathf.Cos(angleRadians),
                    Mathf.Sin(angleRadians));

                if (sliceImages[i] != null)
                {
                    RectTransform iconRect =
                        sliceImages[i].rectTransform;

                    if (iconRect.parent != wheelRotation)
                        iconRect.SetParent(wheelRotation, false);

                    iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                    iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                    iconRect.pivot = new Vector2(0.5f, 0.5f);

                    iconRect.anchoredPosition = direction * radius;
                    iconRect.sizeDelta = new Vector2(iconSize, iconSize);

                    iconRect.localRotation = angleDegrees < 0f &&
                                             angleDegrees > -180f
                        ? Quaternion.Euler(0f, 0f, 180f)
                        : Quaternion.identity;
                }

                if (valueTexts == null ||
                    valueTexts.Count != SliceCount ||
                    valueTexts[i] == null)
                {
                    continue;
                }

                RectTransform textRect =
                    valueTexts[i].rectTransform;

                if (textRect.parent != wheelRotation)
                    textRect.SetParent(wheelRotation, false);

                textRect.anchorMin = new Vector2(0.5f, 0.5f);
                textRect.anchorMax = new Vector2(0.5f, 0.5f);
                textRect.pivot = new Vector2(0.5f, 0.5f);

                textRect.anchoredPosition =
                    direction * (radius - iconSize * 0.45f);

                textRect.sizeDelta = new Vector2(
                    iconSize * 1.6f,
                    iconSize * 0.55f);

                textRect.localRotation = Quaternion.identity;

                TMP_Text text = valueTexts[i];
                text.alignment = TextAlignmentOptions.Center;
                text.enableAutoSizing = true;
                text.fontSizeMin = 12f;
                text.fontSizeMax = 22f;
                text.fontSize = 18f;
                text.color = Color.white;
                text.overflowMode = TextOverflowModes.Overflow;
            }
        }
    }
}
