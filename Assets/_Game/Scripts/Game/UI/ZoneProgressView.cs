using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Common;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public sealed class ZoneProgressView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform markersRoot;
        [SerializeField] private ZoneMarkerView markerPrefab;
        [SerializeField] private RectTransform selector;
        [SerializeField] private Image selectorImage;

        [Header("Layout")]
        [SerializeField, Min(3)] private int visibleZoneCount = 9;
        [SerializeField, Range(0f, 0.45f)] private float selectorInset = 0.1f;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color safeColor = Color.green;
        [SerializeField] private Color superColor = new Color(1f, 0.75f, 0.2f);

        [Header("Animation")]
        [SerializeField, Min(0.05f)] private float moveDuration = 0.25f;

        private readonly List<ZoneMarkerView> markers = new();

        private RunState state;
        private int lastZone = -1;
        private Tween selectorTween;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIRefs.Assign(ref markersRoot, "ui_transform_zone_markers");
            UIRefs.Assign(ref selector, "ui_image_zone_selector");
            UIRefs.Assign(ref selectorImage, "ui_image_zone_selector");
        }
#endif

        public void Bind(RunState runState)
        {
            state = runState;

            for (int i = 0; i < visibleZoneCount; i++)
            {
                ZoneMarkerView marker = Instantiate(markerPrefab, markersRoot);
                marker.name = $"ui_zone_marker_{i}";
                markers.Add(marker);
            }

            state.Changed += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            int zone = state.CurrentZone;

            if (zone == lastZone)
                return;

            bool animate = lastZone >= 0;
            lastZone = zone;

            int firstVisible = Mathf.Max(1, zone - visibleZoneCount / 2);
            int selectedIndex = Mathf.Clamp(
                zone - firstVisible, 0, markers.Count - 1);

            for (int i = 0; i < markers.Count; i++)
            {
                int markerZone = firstVisible + i;
                markers[i].Set(markerZone, GetColor(markerZone));
            }

            selectorImage.color = state.CurrentZoneType == ZoneType.Normal
                ? normalColor
                : GetColor(zone);

            MoveSelector(selectedIndex, animate);
        }

        private Color GetColor(int zone)
        {
            switch (ZoneRules.GetZoneType(zone))
            {
                case ZoneType.Super: return superColor;
                case ZoneType.Safe: return safeColor;
                default: return normalColor;
            }
        }

        private void MoveSelector(int index, bool animate)
        {
            float slot = 1f / markers.Count;
            var min = new Vector2((index + selectorInset) * slot, 0f);
            var max = new Vector2((index + 1f - selectorInset) * slot, 1f);

            selectorTween?.Kill();

            if (!animate)
            {
                ApplySelector(min, max);
                return;
            }

            Vector2 startMin = selector.anchorMin;
            Vector2 startMax = selector.anchorMax;

            selectorTween = DOVirtual.Float(0f, 1f, moveDuration, t =>
                    ApplySelector(Vector2.LerpUnclamped(startMin, min, t),
                                  Vector2.LerpUnclamped(startMax, max, t)))
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject);
        }

        private void ApplySelector(Vector2 min, Vector2 max)
        {
            selector.anchorMin = min;
            selector.anchorMax = max;
            selector.offsetMin = selector.offsetMax = Vector2.zero;
        }

        private void OnDestroy()
        {
            selectorTween?.Kill();

            if (state != null)
                state.Changed -= Refresh;
        }
    }
}