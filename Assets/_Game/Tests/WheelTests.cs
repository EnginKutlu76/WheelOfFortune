using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using VertigoGames.Core;
using VertigoGames.Data;

namespace VertigoGames.Tests
{
    public class WheelTests
    {
        private static List<WheelSliceData> Slices(params float[] weights)
        {
            RewardData reward = TestData.Reward("r");
            var list = new List<WheelSliceData>();

            foreach (float weight in weights)
                list.Add(WheelSliceData.Create(SliceType.Reward, reward, weight));

            return list;
        }

        [Test]
        public void SelectIndex_FollowsRoll()
        {
            var random = new FakeRandom();
            List<WheelSliceData> slices = Slices(1, 1, 1, 1);

            random.Fraction = 0f;
            Assert.AreEqual(0, WheelSelector.SelectIndex(slices, random));

            random.Fraction = 0.99f;
            Assert.AreEqual(3, WheelSelector.SelectIndex(slices, random));
        }

        [Test]
        public void SelectIndex_SkipsZeroWeight()
        {
            var random = new FakeRandom { Fraction = 0f };
            Assert.AreEqual(1, WheelSelector.SelectIndex(Slices(0, 1), random));
        }

        [Test]
        public void SelectIndex_ReturnsMinusOne_WhenNoWeight()
        {
            Assert.AreEqual(
                -1, WheelSelector.SelectIndex(Slices(0, 0), new FakeRandom()));
        }

        [TestCase(0f, 0, 4)]
        [TestCase(0f, 2, 5)]
        [TestCase(100f, 0, 4)]
        [TestCase(725f, 7, 6)]
        public void TargetAngle_LandsOnSlice_AndOnlyMovesForward(
            float current, int index, int rotations)
        {
            float target = WheelMath.GetTargetAngle(current, index, 8, rotations);

            Assert.GreaterOrEqual(target, current + rotations * 360f);
            Assert.AreEqual(
                index * 45f,
                Mathf.Repeat(target, 360f),
                0.001f);
        }
    }
}