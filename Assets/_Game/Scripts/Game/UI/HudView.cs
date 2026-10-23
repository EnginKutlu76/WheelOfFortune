using TMPro;
using UnityEngine;
using VertigoGames.Common;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text zoneValueText;
        [SerializeField] private TMP_Text nextSafeValueText;
        [SerializeField] private TMP_Text nextSuperValueText;

        [Header("Formats")]
        [SerializeField] private string zoneFormat = "ZONE {0}";
        [SerializeField] private string safeFormat = "SAFE ZONE {0}";
        [SerializeField] private string superFormat = "SUPER ZONE {0}";

        private RunState state;
        private int lastZone = -1;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIRefs.Assign(ref zoneValueText, "ui_text_zone_value");
            UIRefs.Assign(ref nextSafeValueText, "ui_text_next_safe_value");
            UIRefs.Assign(ref nextSuperValueText, "ui_text_next_super_value");
        }
#endif

        public void Bind(RunState runState)
        {
            state = runState;
            state.Changed += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            int zone = state.CurrentZone;

            if (zone == lastZone)
                return;

            lastZone = zone;

            if (zoneValueText != null)
                zoneValueText.text = string.Format(zoneFormat, zone);

            if (nextSafeValueText != null)
            {
                nextSafeValueText.text = string.Format(
                    safeFormat,
                    ZoneRules.GetNextZoneOfType(zone, ZoneType.Safe));
            }

            if (nextSuperValueText != null)
            {
                nextSuperValueText.text = string.Format(
                    superFormat,
                    ZoneRules.GetNextZoneOfType(zone, ZoneType.Super));
            }
        }

        private void OnDestroy()
        {
            if (state != null)
                state.Changed -= Refresh;
        }
    }
}