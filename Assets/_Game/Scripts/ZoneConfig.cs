
using System.Collections.Generic;
using UnityEngine;
using VertigoGames.Gameplay;

namespace VertigoGames.Data
{
    [CreateAssetMenu(
        fileName = "NewZoneConfig",
        menuName = "Vertigo Games/Zone Config")]
    public class ZoneConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int zoneNumber = 1;
        [SerializeField] private ZoneType zoneType = ZoneType.Normal;
        [SerializeField] private List<WheelSliceData> slices = new();

        public int ZoneNumber => zoneNumber;
        public ZoneType Type => zoneType;
        public IReadOnlyList<WheelSliceData> Slices => slices;
    }
}
