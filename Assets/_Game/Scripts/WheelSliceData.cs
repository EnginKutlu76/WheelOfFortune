
using UnityEngine;

namespace VertigoGames.Data
{
    public enum SliceType
    {
        Reward,
        Bomb
    }

    [System.Serializable]
    public class WheelSliceData
    {
        [SerializeField] private SliceType type;
        [SerializeField] private RewardData reward;
        [SerializeField, Min(0f)] private float weight = 1f;

        public SliceType Type => type;
        public RewardData Reward => reward;
        public float Weight => weight;

        public bool IsValid =>
            weight >= 0f &&
            (type == SliceType.Bomb || reward != null);

        public WheelSliceData Clone()
        {
            return new WheelSliceData
            {
                type = type,
                reward = reward,
                weight = weight
            };
        }
    }
}
