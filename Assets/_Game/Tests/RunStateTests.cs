using NUnit.Framework;
using VertigoGames.Core;
using VertigoGames.Data;

namespace VertigoGames.Tests
{
    public class RunStateTests
    {
        private static RunState AtZone(int zone)
        {
            var state = new RunState();
            while (state.CurrentZone < zone)
                state.CompleteRound();
            return state;
        }

        [Test]
        public void CanExit_IsFalse_InNormalZone()
        {
            Assert.IsFalse(AtZone(2).CanExit);
        }

        [Test]
        public void CanExit_IsTrue_InSafeAndSuperZone()
        {
            Assert.IsTrue(AtZone(5).CanExit);
            Assert.IsTrue(AtZone(30).CanExit);
        }

        [Test]
        public void CanExit_IsFalse_WhileSpinning()
        {
            RunState state = AtZone(5);
            state.TryBeginSpin();
            Assert.IsFalse(state.CanExit);
        }

        [Test]
        public void TryExit_InNormalZone_IsRejected()
        {
            RunState state = AtZone(3);
            Assert.IsFalse(state.TryExit());
            Assert.AreEqual(RunPhase.Playing, state.Phase);
        }

        [Test]
        public void Bomb_WithReviveAvailable_KeepsRewards()
        {
            var state = new RunState();
            state.ApplyReward(TestData.Reward("coin"), 10);

            state.HitBomb();

            Assert.AreEqual(RunPhase.BombHit, state.Phase);
            Assert.IsTrue(state.CanRevive);
            Assert.AreEqual(1, state.Stacks.Count);
        }

        [Test]
        public void Bomb_AfterReviveUsed_ClearsRewards()
        {
            var state = new RunState();
            state.ApplyReward(TestData.Reward("coin"), 10);
            state.HitBomb();
            state.TryRevive();

            state.HitBomb();

            Assert.IsFalse(state.CanRevive);
            Assert.AreEqual(0, state.Stacks.Count);
        }

        [Test]
        public void SameReward_StacksTogether()
        {
            var state = new RunState();
            RewardData coin = TestData.Reward("coin");

            state.ApplyReward(coin, 10);
            state.ApplyReward(coin, 15);

            Assert.AreEqual(1, state.Stacks.Count);
            Assert.AreEqual(25, state.Stacks[0].TotalValue);
            Assert.AreEqual(2, state.Stacks[0].Count);
        }

        [Test]
        public void DuplicateCostume_IsRejected()
        {
            var state = new RunState();
            RewardData hat = TestData.Reward("hat", 1, RewardCategory.Costume);

            Assert.IsTrue(state.ApplyReward(hat, 1));
            Assert.IsFalse(state.ApplyReward(hat, 1));
        }

        [Test]
        public void Restart_ResetsRun()
        {
            RunState state = AtZone(7);
            state.ApplyReward(TestData.Reward("coin"), 10);
            state.HitBomb();

            state.Restart();

            Assert.AreEqual(1, state.CurrentZone);
            Assert.AreEqual(RunPhase.Playing, state.Phase);
            Assert.AreEqual(0, state.Stacks.Count);
        }

        [Test]
        public void CancelSpin_ReleasesBusyState()
        {
            var state = new RunState();
            state.TryBeginSpin();

            state.CancelSpin();

            Assert.IsTrue(state.CanSpin);
        }

        [Test]
        public void TryBeginSpin_WhileBusy_IsRejected()
        {
            var state = new RunState();
            state.TryBeginSpin();

            Assert.IsFalse(state.TryBeginSpin());
        }
    }
}