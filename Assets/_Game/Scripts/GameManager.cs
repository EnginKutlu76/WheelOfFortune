
using System.Collections.Generic;
using UnityEngine;
using VertigoGames.Data;
using VertigoGames.Gameplay;

namespace VertigoGames.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int CurrentZone { get; private set; } = 1;
        public int AccumulatedRewards { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsSpinning { get; private set; }
        public bool HasExited { get; private set; }
        public bool HasSpun { get; private set; }

        public ZoneType CurrentZoneType =>
            ZoneController.GetZoneType(CurrentZone);

        public bool CanExit =>
            HasSpun && !IsGameOver && !IsSpinning && !HasExited;

        public IReadOnlyList<RewardStack> RewardStacks => rewardStacks;

        private readonly List<RewardStack> rewardStacks = new();

        public sealed class RewardStack
        {
            public RewardData Reward { get; }
            public int Count { get; private set; }
            public int TotalValue { get; private set; }

            public RewardStack(RewardData reward, int value)
            {
                Reward = reward;
                Count = 1;
                TotalValue = value;
            }

            public void Add(int value)
            {
                Count++;
                TotalValue += value;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void SetSpinning(bool value)
        {
            IsSpinning = value;
        }

        public void AdvanceZone()
        {
            if (IsGameOver || IsSpinning || HasExited)
                return;

            HasSpun = true;
            CurrentZone++;

            Debug.Log($"Advanced to zone {CurrentZone} ({CurrentZoneType}).");
        }

        public void AddReward(int amount)
        {
            if (IsGameOver || HasExited || amount < 0)
                return;

            AccumulatedRewards += amount;
        }

        public void AddReward(RewardData reward, int amount)
        {
            if (reward == null || IsGameOver || HasExited || amount < 0)
                return;

            AccumulatedRewards += amount;

            foreach (RewardStack stack in rewardStacks)
            {
                if (stack.Reward == reward)
                {
                    stack.Add(amount);
                    return;
                }
            }

            rewardStacks.Add(new RewardStack(reward, amount));
        }

        public void TriggerBomb()
        {
            if (IsGameOver || HasExited)
                return;

            AccumulatedRewards = 0;
            rewardStacks.Clear();
            IsGameOver = true;
            IsSpinning = false;

            Debug.Log("Bomb hit. Accumulated rewards lost.");
        }

        public bool TryExit()
        {
            if (!CanExit)
                return false;

            HasExited = true;
            Debug.Log($"Player exited with {AccumulatedRewards} rewards.");
            return true;
        }

        public void RestartRun()
        {
            CurrentZone = 1;
            AccumulatedRewards = 0;
            rewardStacks.Clear();
            IsGameOver = false;
            IsSpinning = false;
            HasSpun = false;
            HasExited = false;
        }
    }
}
