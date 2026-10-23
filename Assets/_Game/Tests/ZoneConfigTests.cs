using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VertigoGames.Common;
using VertigoGames.Data;

namespace VertigoGames.Tests
{
    public class ZoneConfigTests
    {
        private const BindingFlags Flags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private static void Set(object target, string field, object value) =>
            target.GetType().GetField(field, Flags).SetValue(target, value);

        private static RewardPoolData Pool(int count)
        {
            var pool = ScriptableObject.CreateInstance<RewardPoolData>();
            var entries = new List<RewardPoolData.Entry>();
            for (int i = 0; i < count; i++)
            {
                var entry = new RewardPoolData.Entry();
                Set(entry, "reward", TestData.Reward($"r{i}"));
                Set(entry, "weight", 1f);
                entries.Add(entry);
            }
            Set(pool, "entries", entries);
            return pool;
        }

        private static ZoneConfig Config(ZoneType type, int bombs, int poolSize)
        {
            var config = ScriptableObject.CreateInstance<ZoneConfig>();
            Set(config, "zoneType", type);
            Set(config, "bombSliceCount", bombs);
            Set(config, "rewardPool", Pool(poolSize));
            return config;
        }

        [Test]
        public void GetRewardValue_ScalesWithZone()
        {
            ZoneConfig config = Config(ZoneType.Normal, 1, 11);
            RewardData reward = TestData.Reward("coin", 100);

            Assert.AreEqual(100, config.GetRewardValue(reward, 1));
            Assert.AreEqual(250, config.GetRewardValue(reward, 11)); // 1 + 10 * 0.15
        }

        [Test]
        public void GetRewardValue_WithoutScaling_ReturnsBaseValue()
        {
            ZoneConfig config = Config(ZoneType.Normal, 1, 11);
            RewardData reward = TestData.Reward("coin", 100);
            Set(reward, "scalesWithZone", false);

            Assert.AreEqual(100, config.GetRewardValue(reward, 11));
        }

        [Test]
        public void CreateSpinSlices_Normal_HasEightSlicesAndOneBomb()
        {
            List<WheelSliceData> slices =
                Config(ZoneType.Normal, 1, 11).CreateSpinSlices(null, new FakeRandom());

            Assert.AreEqual(8, slices.Count);
            Assert.AreEqual(1, slices.FindAll(s => s.Type == SliceType.Bomb).Count);
        }

        [Test]
        public void CreateSpinSlices_SafeZone_HasNoBomb()
        {
            List<WheelSliceData> slices =
                Config(ZoneType.Safe, 1, 9).CreateSpinSlices(null, new FakeRandom());

            Assert.AreEqual(8, slices.Count);
            Assert.AreEqual(0, slices.FindAll(s => s.Type == SliceType.Bomb).Count);
        }

        [Test]
        public void CreateSpinSlices_HasNoDuplicateRewards()
        {
            List<WheelSliceData> slices =
                Config(ZoneType.Super, 0, 10).CreateSpinSlices(null, new FakeRandom());

            var seen = new HashSet<RewardData>();
            foreach (WheelSliceData slice in slices)
                Assert.IsTrue(seen.Add(slice.Reward));
        }

        [Test]
        public void CreateSpinSlices_PoolTooSmall_ReturnsNull()
        {
            LogAssert.Expect(LogType.Error, new Regex("Pool needs"));

            Assert.IsNull(
                Config(ZoneType.Normal, 1, 3).CreateSpinSlices(null, new FakeRandom()));
        }

        [Test]
        public void ZoneConfigSet_PicksConfigByZoneType_AndTier()
        {
            ZoneConfig low = Config(ZoneType.Normal, 1, 11);
            ZoneConfig high = Config(ZoneType.Normal, 1, 11);
            ZoneConfig safe = Config(ZoneType.Safe, 0, 9);
            ZoneConfig super = Config(ZoneType.Super, 0, 10);

            var set = ScriptableObject.CreateInstance<ZoneConfigSet>();
            Set(set, "safeConfig", safe);
            Set(set, "superConfig", super);

            var tierType = typeof(ZoneConfigSet).GetNestedType("Tier", BindingFlags.NonPublic);
            var tiers = (IList)typeof(ZoneConfigSet).GetField("normalTiers", Flags).GetValue(set);

            foreach ((int from, ZoneConfig cfg) in new[] { (1, low), (10, high) })
            {
                object tier = System.Activator.CreateInstance(tierType);
                Set(tier, "fromZone", from);
                Set(tier, "config", cfg);
                tiers.Add(tier);
            }

            Assert.AreSame(low, set.GetConfig(4));
            Assert.AreSame(high, set.GetConfig(12));
            Assert.AreSame(safe, set.GetConfig(5));
            Assert.AreSame(super, set.GetConfig(30));
        }
    }
}