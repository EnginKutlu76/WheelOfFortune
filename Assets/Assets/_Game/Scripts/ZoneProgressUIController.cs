
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Core;
using VertigoGames.Gameplay;

namespace VertigoGames.UI
{
    public class ZoneProgressUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform markersRoot;
        [SerializeField] private RectTransform selector;
        [SerializeField] private Image selectorImage;
        [SerializeField] private Image backgroundImage;

        [Header("Marker Settings")]
        [SerializeField, Min(3)] private int visibleZoneCount = 9;
        [SerializeField, Min(20f)] private float markerWidth = 200f;
        [SerializeField] private TMP_FontAsset zoneFont;
        [SerializeField, Min(1f)] private float markerFontSize = 36f;
        [SerializeField] private FontStyles markerFontStyle = FontStyles.Bold;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color safeColor = Color.green;
        [SerializeField] private Color superColor = new Color(1f, 0.75f, 0.2f);
        [SerializeField] private Color selectorColor = Color.white;
        [SerializeField, Min(0.1f)]
        private float selectorWidthRatio = 0.85f;

        [SerializeField, Min(0.1f)]
        private float selectorHeightRatio = 0.85f;

        [Header("Animation")]
        [SerializeField, Min(0.05f)] private float moveDuration = 0.25f;

        private readonly List<TMP_Text> markerTexts = new();
        private int lastZone = -1;
        private Tween selectorTween;

        private void Start()
        {
            BuildMarkers();
            Refresh(true);
        }

        private void Update()
        {
            Refresh(false);
        }

        private void BuildMarkers()
        {
            if (markersRoot == null)
            {
                Debug.LogError("Zone markers root is not assigned.");
                return;
            }

            if (selector != null)
            {
                selector.SetParent(markersRoot, false);

                selector.anchorMin = new Vector2(0f, 0.5f);
                selector.anchorMax = new Vector2(0f, 0.5f);
                selector.pivot = new Vector2(0.5f, 0.5f);

                float selectorWidth = markerWidth * selectorWidthRatio;
                float selectorHeight = markerWidth * selectorHeightRatio;

                selector.sizeDelta = new Vector2(
                    selectorWidth,
                    selectorHeight);

                selector.anchoredPosition = Vector2.zero;
                selector.SetAsFirstSibling();
            }

            for (int i = markersRoot.childCount - 1; i >= 0; i--)
            {
                Transform child = markersRoot.GetChild(i);

                if (selector != null && child == selector)
                    continue;

                Destroy(child.gameObject);
            }

            markerTexts.Clear();

            float totalWidth = visibleZoneCount * markerWidth;

            markersRoot.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                totalWidth);

            for (int i = 0; i < visibleZoneCount; i++)
            {
                GameObject marker = new GameObject(
                    $"ui_zone_marker_{i + 1}",
                    typeof(RectTransform),
                    typeof(TextMeshProUGUI));

                marker.transform.SetParent(markersRoot, false);

                RectTransform rect = marker.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(markerWidth, 0f);
                rect.anchoredPosition = new Vector2(
                    markerWidth * (i + 0.5f),
                    0f);

                TMP_Text text = marker.GetComponent<TMP_Text>();

                if (zoneFont != null)
                    text.font = zoneFont;

                text.text = (i + 1).ToString();
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = markerFontSize;
                text.fontStyle = markerFontStyle;
                text.color = normalColor;
                text.raycastTarget = false;

                markerTexts.Add(text);
            }

            if (selector != null)
                selector.SetAsFirstSibling();
        }

        private void Refresh(bool force)
        {
            if (GameManager.Instance == null ||
                markersRoot == null ||
                selector == null ||
                markerTexts.Count == 0)
            {
                return;
            }

            int zone = GameManager.Instance.CurrentZone;

            if (!force && zone == lastZone)
                return;

            lastZone = zone;

            int firstVisibleZone = Mathf.Max(
                1,
                zone - visibleZoneCount / 2);

            int selectedIndex = zone - firstVisibleZone;
            selectedIndex = Mathf.Clamp(
                selectedIndex,
                0,
                markerTexts.Count - 1);

            RectTransform selectedMarker =
                markerTexts[selectedIndex].rectTransform;

            float targetX = selectedMarker.anchoredPosition.x;

            selectorTween?.Kill();

            if (force)
            {
                selector.anchoredPosition = new Vector2(targetX, 0f);
            }
            else
            {
                selectorTween = selector
                    .DOAnchorPos(new Vector2(targetX, 0f), moveDuration)
                    .SetEase(Ease.OutCubic);
            }

            for (int i = 0; i < markerTexts.Count; i++)
            {
                int markerZone = firstVisibleZone + i;
                TMP_Text text = markerTexts[i];

                text.text = markerZone.ToString();

                ZoneType type = ZoneController.GetZoneType(markerZone);

                text.color = type switch
                {
                    ZoneType.Super => superColor,
                    ZoneType.Safe => safeColor,
                    _ => normalColor
                };
            }

            if (selectorImage != null)
            {
                ZoneType currentType =
                    GameManager.Instance.CurrentZoneType;

                selectorImage.color = currentType switch
                {
                    ZoneType.Super => superColor,
                    ZoneType.Safe => safeColor,
                    _ => selectorColor
                };
            }
        }

        private void OnDestroy()
        {
            selectorTween?.Kill();
        }
    }
}
