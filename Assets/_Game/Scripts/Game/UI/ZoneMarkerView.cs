using TMPro;
using UnityEngine;
using VertigoGames.Common;

namespace VertigoGames.UI
{
    public sealed class ZoneMarkerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text zoneValueText;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIRefs.AssignChild(this, ref zoneValueText, "ui_text_zone_marker_value");
        }
#endif

        public void Set(int zone, Color color)
        {
            zoneValueText.text = zone.ToString();
            zoneValueText.color = color;
        }
    }
}