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

        public RewardData GetRandomReward()
        {
            if (entries == null || entries.Count == 0)
                return null;

            float totalWeight = 0f;

            foreach (Entry entry in entries)
            {
                if (entry != null && entry.IsValid)
                    totalWeight += entry.Weight;
            }

            if (totalWeight <= 0f)
                return null;

            float roll = UnityEngine.Random.Range(0f, totalWeight);

            foreach (Entry entry in entries)
            {
                if (entry == null || !entry.IsValid || entry.Weight <= 0f)
                    continue;

                roll -= entry.Weight;

                if (roll < 0f)
                    return entry.Reward;
            }

            return null;
        }

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
            return GetRandomRewardsWithoutReplacement(count, null);
        }

        public List<RewardData> GetRandomRewardsWithoutReplacement(
            int count,
            Predicate<RewardData> canSelect)
        {
            var available = new List<Entry>();

            if (entries != null)
            {
                foreach (Entry entry in entries)
                {
                    if (entry == null ||
                        !entry.IsValid ||
                        entry.Weight <= 0f)
                        continue;

                    if (canSelect != null && !canSelect(entry.Reward))
                        continue;

                    // Avoid duplicate references to the same reward asset.
                    bool alreadyAdded = false;

                    foreach (Entry existing in available)
                    {
                        if (existing.Reward == entry.Reward)
                        {
                            alreadyAdded = true;
                            break;
                        }
                    }

                    if (!alreadyAdded)
                        available.Add(entry);
                }
            }

            var selected = new List<RewardData>();
            count = Mathf.Min(count, available.Count);

            while (selected.Count < count)
            {
                float totalWeight = 0f;

                foreach (Entry entry in available)
                    totalWeight += entry.Weight;

                if (totalWeight <= 0f)
                    break;

                float roll = UnityEngine.Random.Range(0f, totalWeight);
                int selectedIndex = -1;

                for (int i = 0; i < available.Count; i++)
                {
                    roll -= available[i].Weight;

                    if (roll < 0f)
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                if (selectedIndex < 0)
                    selectedIndex = available.Count - 1;

                selected.Add(available[selectedIndex].Reward);
                available.RemoveAt(selectedIndex);
            }

            return selected;
        }
    }
}
