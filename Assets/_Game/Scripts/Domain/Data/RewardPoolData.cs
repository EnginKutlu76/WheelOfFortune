using System;
using System.Collections.Generic;
using UnityEngine;

namespace VertigoGames.Data
{
    [CreateAssetMenu(
        fileName = "NewRewardPool",
        menuName = "Vertigo Games/Reward Pool")]
    public class RewardPoolData : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            [SerializeField] private RewardData reward;
            [SerializeField, Min(0f)] private float weight = 1f;

            public RewardData Reward => reward;
            public float Weight => weight;

            public bool IsValid =>
                reward != null &&
                weight >= 0f;
        }

        [SerializeField] private List<Entry> entries = new();

        public IReadOnlyList<Entry> Entries => entries;

        //public RewardData GetRandomReward()
        //{
        //    if (entries == null || entries.Count == 0)
        //        return null;

        //    float totalWeight = 0f;

        //    foreach (Entry entry in entries)
        //    {
        //        if (entry != null && entry.IsValid)
        //            totalWeight += entry.Weight;
        //    }

        //    if (totalWeight <= 0f)
        //        return null;

        //    float roll = UnityEngine.Random.Range(0f, totalWeight);

        //    foreach (Entry entry in entries)
        //    {
        //        if (entry == null || !entry.IsValid || entry.Weight <= 0f)
        //            continue;

        //        roll -= entry.Weight;

        //        if (roll < 0f)
        //            return entry.Reward;
        //    }

        //    return null;
        //}

        private void OnValidate()
        {
            if (entries == null)
                return;

            foreach (Entry entry in entries)
            {
                if (entry != null && !entry.IsValid)
                {
                    Debug.LogWarning(
                        $"{name}: Check reward pool entries.",
                        this);
                    return;
                }
            }
        }

        public List<RewardData> GetRandomRewardsWithoutReplacement(int count)
        {
            return GetRandomRewardsWithoutReplacement(
                count,
                null,
                new VertigoGames.Common.UnityRandom());
        }

        public List<RewardData> GetRandomRewardsWithoutReplacement(
            int count,
            Predicate<RewardData> canSelect,
            VertigoGames.Common.IRandom random)
        {
            var available = new List<Entry>();
            var seen = new HashSet<RewardData>();

            foreach (Entry entry in entries)
            {
                if (entry == null || !entry.IsValid || entry.Weight <= 0f)
                    continue;

                if (canSelect != null && !canSelect(entry.Reward))
                    continue;

                if (seen.Add(entry.Reward))
                    available.Add(entry);
            }

            var selected = new List<RewardData>();
            count = Mathf.Min(count, available.Count);

            while (selected.Count < count)
            {
                float total = 0f;
                foreach (Entry e in available)
                    total += e.Weight;

                float roll = random.Range(0f, total);
                int index = available.Count - 1;

                for (int i = 0; i < available.Count; i++)
                {
                    roll -= available[i].Weight;
                    if (roll < 0f)
                    {
                        index = i;
                        break;
                    }
                }

                selected.Add(available[index].Reward);
                available.RemoveAt(index);
            }

            return selected;
        }
    }
}
