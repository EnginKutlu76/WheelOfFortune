
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Data;

namespace VertigoGames.Gameplay
{
    public class WheelSliceVisualController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform wheelRotation;
        [SerializeField] private List<Image> sliceImages = new();
        [SerializeField] private Sprite bombIcon;

        [Header("Layout")]
        [SerializeField, Range(0.1f, 0.5f)]
        private float radiusRatio = 0.33f;

        [SerializeField, Range(0.05f, 0.2f)]
        private float iconSizeRatio = 0.12f;

        [SerializeField] private float startAngle = 90f;

        private const int SliceCount = 8;

        private void Awake()
        {
            ConfigureRotationRect();
            PositionIcons();
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

            for (int i = 0; i < SliceCount; i++)
            {
                Image image = sliceImages[i];
                WheelSliceData slice = slices[i];

                if (image == null || slice == null)
                    continue;

                Sprite icon = slice.Type == SliceType.Bomb
                    ? bombIcon
                    : slice.Reward != null
                        ? slice.Reward.Icon
                        : null;

                image.sprite = icon;
                image.enabled = icon != null;
                image.preserveAspect = true;
                image.raycastTarget = false;
            }

            PositionIcons();
        }

        public void ClearSliceVisuals()
        {
            foreach (Image image in sliceImages)
            {
                if (image == null)
                    continue;

                image.sprite = null;
                image.enabled = false;
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

            float radius = diameter * radiusRatio;
            float iconSize = diameter * iconSizeRatio;

            for (int i = 0; i < SliceCount; i++)
            {
                Image image = sliceImages[i];

                if (image == null)
                    continue;

                RectTransform rect = image.rectTransform;
                float angle =
                    (startAngle - i * 45f) * Mathf.Deg2Rad;

                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);

                rect.anchoredPosition = new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius);

                rect.sizeDelta = new Vector2(iconSize, iconSize);
                rect.localRotation = Quaternion.identity;
            }
        }
    }
}
