using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Data;

namespace VertigoGames.Gameplay
{
    public sealed class WheelSliceVisualController : MonoBehaviour
    {
        private const int SliceCount = ZoneConfig.SliceCount;

        [Header("References")]
        [SerializeField] private RectTransform wheelRotation;
        [SerializeField] private Image[] sliceImages = new Image[SliceCount];
        [SerializeField] private TMP_Text[] valueTexts = new TMP_Text[SliceCount];
        [SerializeField] private Sprite bombIcon;
        [SerializeField] private string bombLabel = "BOMB";

        [Header("Layout")]
        [SerializeField, Range(0.1f, 0.5f)] private float radiusRatio = 0.30f;
        [SerializeField, Range(0.05f, 0.2f)] private float iconSizeRatio = 0.10f;
        [SerializeField, Range(0.01f, 0.15f)] private float valueOffsetRatio = 0.045f;
        [SerializeField] private float startAngle = 90f;

        public void Show(
            IReadOnlyList<WheelSliceData> slices,
            Func<RewardData, int> getValue)
        {
            if (slices == null || slices.Count != SliceCount ||
                sliceImages.Length != SliceCount ||
                valueTexts.Length != SliceCount)
            {
                Debug.LogError(
                    $"{SliceCount} slices, images and texts are required.", this);
                return;
            }

            for (int i = 0; i < SliceCount; i++)
            {
                WheelSliceData slice = slices[i];
                bool isBomb = slice.Type == SliceType.Bomb;

                Sprite icon = isBomb
                    ? bombIcon
                    : (slice.Reward != null ? slice.Reward.Icon : null);

                sliceImages[i].sprite = icon;
                sliceImages[i].preserveAspect = true;
                sliceImages[i].enabled = icon != null;

                valueTexts[i].text = isBomb
                    ? bombLabel
                    : $"x{getValue(slice.Reward)}";
                
            }


            Layout();
        }

        [ContextMenu("Re-layout Slices")]
        private void Layout()
        {
            if (wheelRotation == null)
                return;

            float diameter = Mathf.Min(
                wheelRotation.rect.width, wheelRotation.rect.height);

            if (diameter <= 0f)
                return;

            float iconRadius = diameter * radiusRatio;
            float textRadius = iconRadius - diameter * valueOffsetRatio;
            float iconSize = diameter * iconSizeRatio;
            float step = 360f / SliceCount;

            for (int i = 0; i < SliceCount; i++)
            {
                float angle = startAngle - i * step;
                float radians = angle * Mathf.Deg2Rad;
                var direction = new Vector2(
                    Mathf.Cos(radians), Mathf.Sin(radians));

                Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 90f);

                Place(
                    sliceImages[i].rectTransform,
                    direction * iconRadius,
                    new Vector2(iconSize, iconSize),
                    rotation);

                Place(
                    valueTexts[i].rectTransform,
                    direction * textRadius,
                    new Vector2(iconSize * 1.6f, iconSize * 0.55f),
                    rotation);
            }
        }

        private static void Place(
            RectTransform rect,
            Vector2 position,
            Vector2 size,
            Quaternion rotation)
        {
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            rect.localRotation = rotation;
        }
    }
}