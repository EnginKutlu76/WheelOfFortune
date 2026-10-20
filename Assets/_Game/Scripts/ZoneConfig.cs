
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

        public List<WheelSliceData> CreateSpinSlices()
        {
            if (slices == null || slices.Count != 8)
            {
                Debug.LogError(
                    $"{name}: Exactly 8 slices are required.",
                    this);

                return null;
            }

            var result = new List<WheelSliceData>(slices.Count);

            foreach (WheelSliceData slice in slices)
            {
                if (slice == null || !slice.IsValid)
                {
                    Debug.LogError(
                        $"{name}: A slice has invalid data.",
                        this);

                    return null;
                }

                if (zoneType != ZoneType.Normal &&
                    slice.Type == SliceType.Bomb)
                {
                    Debug.LogError(
                        $"{name}: Safe and super zones cannot contain bombs.",
                        this);

                    return null;
                }

                result.Add(slice.Clone());
            }

            for (int i = result.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);

                (result[i], result[randomIndex]) =
                    (result[randomIndex], result[i]);
            }

            return result;
        }
    }
}
