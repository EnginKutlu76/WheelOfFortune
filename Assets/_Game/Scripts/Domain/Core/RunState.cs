using System;
using System.Collections.Generic;
using VertigoGames.Common;
using VertigoGames.Data;

namespace VertigoGames.Core
{
    public enum RunPhase
    {
        Playing,
        BombHit,
        Exited
    }

    public sealed class RunState
    {
        private readonly List<RewardStack> stacks = new();
        private readonly HashSet<string> ownedCostumeIds = new();
        private bool reviveUsed;

        public int CurrentZone { get; private set; } = 1;
        public RunPhase Phase { get; private set; } = RunPhase.Playing;

        public bool IsBusy { get; private set; }

        public ZoneType CurrentZoneType => ZoneRules.GetZoneType(CurrentZone);
        public IReadOnlyList<RewardStack> Stacks => stacks;

        public bool CanSpin => Phase == RunPhase.Playing && !IsBusy;

        public bool CanExit =>
            Phase == RunPhase.Playing &&
            !IsBusy &&
            CurrentZoneType != ZoneType.Normal;

        public bool CanRevive => Phase == RunPhase.BombHit && !reviveUsed;

        public event Action Changed;

        public bool HasCostume(RewardData reward) =>
            reward != null &&
            !string.IsNullOrWhiteSpace(reward.RewardId) &&
            ownedCostumeIds.Contains(reward.RewardId);

        public bool TryBeginSpin()
        {
            if (!CanSpin)
                return false;

            IsBusy = true;
            Changed?.Invoke();
            return true;
        }

        public void CancelSpin()
        {
            if (Phase != RunPhase.Playing || !IsBusy)
                return;

            IsBusy = false;
            Changed?.Invoke();
        }

        public bool ApplyReward(RewardData reward, int amount)
        {
            if (reward == null || Phase != RunPhase.Playing || amount < 0)
                return false;

            if (reward.Category == RewardCategory.Costume)
            {
                if (string.IsNullOrWhiteSpace(reward.RewardId) ||
                    HasCostume(reward))
                {
                    return false;
                }

                ownedCostumeIds.Add(reward.RewardId);
            }

            RewardStack existing = stacks.Find(s => s.Reward == reward);

            if (existing != null)
                existing.Add(amount);
            else
                stacks.Add(new RewardStack(reward, amount));

            Changed?.Invoke();
            return true;
        }

        public void CompleteRound()
        {
            if (Phase != RunPhase.Playing)
                return;

            CurrentZone++;
            IsBusy = false;
            Changed?.Invoke();
        }

        public void HitBomb()
        {
            if (Phase != RunPhase.Playing)
                return;

            Phase = RunPhase.BombHit;
            IsBusy = false;

            if (reviveUsed)
                ClearRewards();

            Changed?.Invoke();
        }

        public bool TryExit()
        {
            if (!CanExit)
                return false;

            Phase = RunPhase.Exited;
            Changed?.Invoke();
            return true;
        }

        public bool TryRevive()
        {
            if (!CanRevive)
                return false;

            reviveUsed = true;
            Phase = RunPhase.Playing;
            IsBusy = false;
            Changed?.Invoke();
            return true;
        }

        public void Restart()
        {
            CurrentZone = 1;
            Phase = RunPhase.Playing;
            IsBusy = false;
            reviveUsed = false;
            ClearRewards();
            Changed?.Invoke();
        }

        private void ClearRewards()
        {
            stacks.Clear();
            ownedCostumeIds.Clear();
        }
    }
}