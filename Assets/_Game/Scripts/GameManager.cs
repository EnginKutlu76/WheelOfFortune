
using UnityEngine;
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

        public ZoneType CurrentZoneType =>
            ZoneController.GetZoneType(CurrentZone);

        public bool HasSpun { get; private set; }

        public bool CanExit =>
            HasSpun &&
            !IsGameOver &&
            !IsSpinning &&
            !HasExited;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
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

        public void TriggerBomb()
        {
            if (IsGameOver || HasExited)
                return;

            AccumulatedRewards = 0;
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
            IsGameOver = false;
            IsSpinning = false;
            HasSpun = false;
            HasExited = false;
        }
    }
}
