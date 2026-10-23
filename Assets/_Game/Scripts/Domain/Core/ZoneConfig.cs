using System;
using System.Collections.Generic;
using UnityEngine;
using VertigoGames.Common;

namespace VertigoGames.Data
{
    [CreateAssetMenu(
        fileName = "NewZoneConfig",
        menuName = "Vertigo Games/Zone Config")]
    public class ZoneConfig : ScriptableObject
    {
        public const int SliceCount = 8;

        [Header("Zone")]
        [SerializeField] private ZoneType zoneType = ZoneType.Normal;

        [Header("Slices")]
        [SerializeField] private RewardPoolData rewardPool;
        [SerializeField, Range(0, SliceCount - 1)] private int bombSliceCount = 1;

        [Header("Visuals")]
        [SerializeField] private Sprite wheelSprite;
        [SerializeField] private Sprite pointerSprite;

        public Sprite WheelSprite => wheelSprite;
        public Sprite PointerSprite => pointerSprite;

        [Header("Reward Economy")]
        [SerializeField, Min(0.1f)] private float baseMultiplier = 1f;
        [SerializeField, Min(0f)] private float multiplierPerZone = 0.15f;

        public ZoneType Type => zoneType;

        private void OnValidate()
        {
            if (zoneType != ZoneType.Normal)
                bombSliceCount = 0;
        }

        public int GetRewardValue(RewardData reward, int currentZone)
        {
            if (reward == null)
                return 0;

            if (!reward.ScalesWithZone)
                return reward.Value;

            float multiplier = baseMultiplier +
                               Mathf.Max(0, currentZone - 1) * multiplierPerZone;

            return Mathf.Max(0, Mathf.RoundToInt(reward.Value * multiplier));
        }

        public List<WheelSliceData> CreateSpinSlices(
            Predicate<RewardData> canSelect,
            IRandom random)
        {
            if (rewardPool == null)
            {
                Debug.LogError($"{name}: Reward pool is not assigned.", this);
                return null;
            }

            int bombs = zoneType == ZoneType.Normal ? bombSliceCount : 0;
            int rewardCount = SliceCount - bombs;

            List<RewardData> rewards =
                rewardPool.GetRandomRewardsWithoutReplacement(
                    rewardCount, canSelect, random);

            if (rewards.Count != rewardCount)
            {
                Debug.LogError(
                    $"{name}: Pool needs at least {rewardCount} valid unique rewards.",
                    this);
                return null;
            }

            var result = new List<WheelSliceData>(SliceCount);

            for (int i = 0; i < bombs; i++)
                result.Add(WheelSliceData.Create(SliceType.Bomb, null));

            foreach (RewardData reward in rewards)
                result.Add(WheelSliceData.Create(SliceType.Reward, reward));

            for (int i = result.Count - 1; i > 0; i--)
            {
                int j = random.Range(0, i + 1);
                (result[i], result[j]) = (result[j], result[i]);
            }

            return result;
        }
    }
}