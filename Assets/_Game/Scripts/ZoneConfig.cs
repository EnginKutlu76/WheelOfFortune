
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
        [Header("Zone")]
        [SerializeField, Min(1)] private int zoneNumber = 1;
        [SerializeField] private ZoneType zoneType = ZoneType.Normal;
        [SerializeField] private List<WheelSliceData> slices = new();

        [Header("Reward Economy")]
        [SerializeField, Min(0.1f)] private float normalBaseMultiplier = 1f;
        [SerializeField, Min(0f)] private float normalMultiplierPerZone = 0.15f;
        [SerializeField, Min(0.1f)] private float safeZoneMultiplier = 2f;
        [SerializeField, Min(0.1f)] private float superZoneMultiplier = 10f;

        public int ZoneNumber => zoneNumber;
        public ZoneType Type => zoneType;
        public IReadOnlyList<WheelSliceData> Slices => slices;

        [Header("Reward Pool")]
        [SerializeField] private RewardPoolData rewardPool;
        [SerializeField] private bool useRewardPool;
        [SerializeField, Range(0, 8)] private int bombSliceCount = 1;

        public int GetRewardValue(RewardData reward, int currentZone)
        {
            if (reward == null)
                return 0;

            float multiplier = zoneType switch
            {
                ZoneType.Safe => safeZoneMultiplier,
                ZoneType.Super => superZoneMultiplier,
                _ => normalBaseMultiplier +
                     Mathf.Max(0, currentZone - 1) *
                     normalMultiplierPerZone
            };

            return Mathf.Max(
                0,
                Mathf.RoundToInt(reward.Value * multiplier));
        }

        public List<WheelSliceData> CreateSpinSlices()
        {
            if (useRewardPool)
                return CreateSlicesFromRewardPool();

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




        private List<WheelSliceData> CreateSlicesFromRewardPool()
        {
            if (rewardPool == null)
            {
                Debug.LogError($"{name}: Reward pool is not assigned.", this);
                return null;
            }

            if (zoneType != ZoneType.Normal && bombSliceCount > 0)
            {
                Debug.LogError(
                    $"{name}: Protected zones cannot contain bombs.",
                    this);
                return null;
            }

            if (zoneType == ZoneType.Normal &&
                (bombSliceCount < 0 || bombSliceCount >= 8))
            {
                Debug.LogError(
                    $"{name}: Normal zones require between 0 and 7 bomb slices.",
                    this);
                return null;
            }

            int rewardCount = 8 - (
                zoneType == ZoneType.Normal ? bombSliceCount : 0);

            List<RewardData> rewards =
                rewardPool.GetRandomRewardsWithoutReplacement(rewardCount);

            if (rewards == null || rewards.Count != rewardCount)
            {
                Debug.LogError(
                    $"{name}: The reward pool needs at least {rewardCount} valid, unique rewards.",
                    this);
                return null;
            }

            var result = new List<WheelSliceData>(8);

            if (zoneType == ZoneType.Normal)
            {
                for (int i = 0; i < bombSliceCount; i++)
                {
                    result.Add(
                        WheelSliceData.Create(SliceType.Bomb, null));
                }
            }

            foreach (RewardData reward in rewards)
            {
                result.Add(
                    WheelSliceData.Create(SliceType.Reward, reward));
            }

            for (int i = result.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);

                (result[i], result[randomIndex]) =
                    (result[randomIndex], result[i]);
            }

            return result;
        }

        private WheelSliceData CreateBombSlice(WheelSliceData slice)
        {
            return CreateSlice(SliceType.Bomb, null, 1f);
        }

        private WheelSliceData CreateRewardSlice(RewardData reward)
        {
            return CreateSlice(SliceType.Reward, reward, 1f);
        }

        private WheelSliceData CreateSlice(
            SliceType type,
            RewardData reward,
            float weight)
        {
            var template = new List<WheelSliceData>(slices ?? new());

            foreach (WheelSliceData existingSlice in template)
            {
                if (existingSlice == null || existingSlice.Type != type)
                    continue;

                WheelSliceData clone = existingSlice.Clone();

                if (type == SliceType.Reward && clone.Reward == reward)
                    return clone;
            }

            Debug.LogError(
                $"{name}: The current WheelSliceData API does not support creating arbitrary slices. Update WheelSliceData.cs to add a factory method.",
                this);

            return null;
        }
    }
}
