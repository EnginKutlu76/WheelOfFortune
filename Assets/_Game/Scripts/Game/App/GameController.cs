using UnityEngine;
using VertigoGames.Common;
using VertigoGames.Core;
using VertigoGames.Data;
using VertigoGames.Gameplay;
using VertigoGames.UI;
using VertigoGames.Audio;

namespace VertigoGames.App
{
    [DefaultExecutionOrder(-50)]
    public sealed class GameController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private ZoneConfigSet zoneConfigSet;

        [Header("Gameplay")]
        [SerializeField] private WheelSpinController wheel;
        [SerializeField] private RewardPresentationController presentation;
        [SerializeField] private AudioManager audioManager;

        [Header("Views")]
        [SerializeField] private HudView hud;
        [SerializeField] private CashOutView cashOut;
        [SerializeField] private ResultView result;
        [SerializeField] private RewardInventoryView inventory;
        [SerializeField] private ZoneProgressView zoneProgress;

        private RunState state;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIRefs.AssignFirst(ref wheel);
            UIRefs.AssignFirst(ref presentation);
            UIRefs.AssignFirst(ref audioManager);
            UIRefs.AssignFirst(ref hud);
            UIRefs.AssignFirst(ref cashOut);
            UIRefs.AssignFirst(ref result);
            UIRefs.AssignFirst(ref inventory);
            UIRefs.AssignFirst(ref zoneProgress);
        }
#endif

        private void Awake()
        {
            state = new RunState();

            wheel.Initialize(state, zoneConfigSet, new UnityRandom());

            hud.Bind(state);
            cashOut.Bind(state);
            result.Bind(state);
            inventory.Bind(state);
            zoneProgress.Bind(state);
        }

        private void OnEnable()
        {
            wheel.SpinStarted += HandleSpinStarted;
            wheel.SpinFinished += HandleSpinFinished;
            cashOut.CashOutClicked += HandleCashOut;
            result.RestartClicked += HandleRestart;
            result.ReviveClicked += HandleRevive;
        }

        private void OnDisable()
        {
            wheel.SpinStarted -= HandleSpinStarted;
            wheel.SpinFinished -= HandleSpinFinished;
            cashOut.CashOutClicked -= HandleCashOut;
            result.RestartClicked -= HandleRestart;
            result.ReviveClicked -= HandleRevive;

            if (audioManager != null)
                audioManager.StopWheelSpin();
        }

        private void HandleSpinStarted()
        {
            audioManager.StartWheelSpin();
        }

        private void HandleSpinFinished(SpinResult spin)
        {
            audioManager.StopWheelSpin();

            if (spin.Slice.Type == SliceType.Bomb)
            {
                audioManager.PlayBombHit();
                state.HitBomb();
                presentation.ShowBomb();
                return;
            }

            RewardData reward = spin.Slice.Reward;
            int value = spin.Config.GetRewardValue(reward, state.CurrentZone);

            if (!state.ApplyReward(reward, value))
                Debug.LogWarning($"Reward rejected: {reward.name}", this);

            if (reward.Icon != null)
            {
                presentation.Play(
                    reward.Icon, reward.DisplayName, value, CompleteRound);
            }
            else
            {
                CompleteRound();
            }
        }

        private void CompleteRound()
        {
            state.CompleteRound();
            wheel.Prepare();
        }

        private void HandleCashOut()
        {
            state.TryExit();
        }

        private void HandleRevive()
        {
            if (!state.TryRevive())
                return;

            presentation.ResetPresentation();
            wheel.Prepare();
        }

        private void HandleRestart()
        {
            presentation.ResetPresentation();
            state.Restart();
            wheel.Prepare();
        }
    }
}