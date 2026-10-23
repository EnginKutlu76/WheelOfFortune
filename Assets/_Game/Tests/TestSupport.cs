using System.Reflection;
using UnityEngine;
using VertigoGames.Common;
using VertigoGames.Data;

namespace VertigoGames.Tests
{
    internal sealed class FakeRandom : IRandom
    {
        public float Fraction;

        public int Range(int min, int max) => min;
        public float Range(float min, float max) => Mathf.Lerp(min, max, Fraction);
        public float Value => Fraction;
    }

    internal static class TestData
    {
        public static RewardData Reward(
            string id,
            int value = 10,
            RewardCategory category = RewardCategory.Standard)
        {
            var reward = ScriptableObject.CreateInstance<RewardData>();
            Set(reward, "rewardId", id);
            Set(reward, "value", value);
            Set(reward, "category", category);
            return reward;
        }

        private static void Set(RewardData target, string field, object value)
        {
            typeof(RewardData)
                .GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(target, value);
        }
    }
}