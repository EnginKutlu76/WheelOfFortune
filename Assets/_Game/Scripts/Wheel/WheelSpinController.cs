using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Common;
using VertigoGames.Core;
using VertigoGames.Data;

namespace VertigoGames.Gameplay
{
    public sealed class WheelSpinController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform wheelTransform;
        [SerializeField] private Image wheelImage;
        [SerializeField] private Image pointerImage;
        [SerializeField] private Button spinButton;
        [SerializeField] private WheelSliceVisualController sliceVisuals;

        [Header("Spin")]
        [SerializeField, Min(1)] private int minFullRotations = 4;
        [SerializeField, Min(1)] private int maxFullRotations = 6;
        [SerializeField, Min(0.1f)] private float spinDuration = 3f;
        [SerializeField, Range(0f, 1f)] private float reboundChance = 0.4f;
        [SerializeField, Range(2f, 15f)] private float overshootAngle = 8f;
        [SerializeField, Min(0.05f)] private float reboundDuration = 0.18f;

        public event Action SpinStarted;
        public event Action<SpinResult> SpinFinished;

        private RunState state;
        private ZoneConfigSet configs;
        private IRandom random;

        private ZoneConfig activeConfig;
        private List<WheelSliceData> activeSlices;
        private Tween spinTween;

#if UNITY_EDITOR
        private void OnValidate()
        {
            maxFullRotations = Mathf.Max(minFullRotations, maxFullRotations);

            UIRefs.Assign(ref spinButton, "ui_button_spin");
            UIRefs.Assign(ref wheelTransform, "ui_transform_wheel_rotation");
            UIRefs.Assign(ref wheelImage, "ui_image_wheel");
            UIRefs.Assign(ref pointerImage, "ui_image_wheel_pointer");
            UIRefs.AssignFirst(ref sliceVisuals);
        }
#endif

        public void Initialize(
            RunState runState,
            ZoneConfigSet zoneConfigSet,
            IRandom randomSource)
        {
            state = runState;
            configs = zoneConfigSet;
            random = randomSource;

            spinButton.onClick.AddListener(OnSpinClicked);
            state.Changed += RefreshButton;

            Prepare();
        }

        public void Prepare()
        {
            activeConfig = configs.GetConfig(state.CurrentZone);
            activeSlices = null;

            if (activeConfig == null)
            {
                Debug.LogError(
                    $"No zone config for zone {state.CurrentZone}.", this);
                RefreshButton();
                return;
            }

            activeSlices = activeConfig.CreateSpinSlices(CanOffer, random);

            if (activeSlices == null)
            {
                RefreshButton();
                return;
            }

            ApplySkin(activeConfig);

            ZoneConfig config = activeConfig;
            int zone = state.CurrentZone;

            sliceVisuals.Show(
                activeSlices,
                reward => config.GetRewardValue(reward, zone));

            RefreshButton();
        }

        private bool CanOffer(RewardData reward) =>
            reward.Category != RewardCategory.Costume ||
            !state.HasCostume(reward);

        private void ApplySkin(ZoneConfig config)
        {
            if (wheelImage != null && config.WheelSprite != null)
                wheelImage.sprite = config.WheelSprite;

            if (pointerImage != null && config.PointerSprite != null)
                pointerImage.sprite = config.PointerSprite;
        }

        private void RefreshButton()
        {
            spinButton.interactable = activeSlices != null && state.CanSpin;
        }

        private void OnSpinClicked()
        {
            if (activeSlices == null || !state.CanSpin)
                return;

            int index = WheelSelector.SelectIndex(activeSlices, random);

            if (index < 0)
            {
                Debug.LogError("All slice weights are zero.", this);
                return;
            }

            if (!state.TryBeginSpin())
                return;

            SpinStarted?.Invoke();

            int rotations = random.Range(
                minFullRotations, maxFullRotations + 1);

            float target = WheelMath.GetTargetAngle(
                wheelTransform.localEulerAngles.z,
                index,
                activeSlices.Count,
                rotations);

            bool rebound = random.Value < reboundChance;
            float firstTarget = rebound ? target + overshootAngle : target;

            Sequence sequence = DOTween.Sequence().SetLink(gameObject);

            sequence.Append(
                wheelTransform
                    .DOLocalRotate(
                        new Vector3(0f, 0f, firstTarget),
                        spinDuration,
                        RotateMode.FastBeyond360)
                    .SetEase(Ease.OutCubic));

            if (rebound)
            {
                sequence.Append(
                    wheelTransform
                        .DOLocalRotate(
                            new Vector3(0f, 0f, target),
                            reboundDuration)
                        .SetEase(Ease.OutSine));
            }

            sequence.OnComplete(() => OnSpinCompleted(index));
            spinTween = sequence;
        }

        private void OnSpinCompleted(int index)
        {
            spinTween = null;

            SpinFinished?.Invoke(
                new SpinResult(activeSlices[index], activeConfig));
        }

        private void OnDisable()
        {
            if (spinTween != null && spinTween.IsActive())
            {
                spinTween.Kill();
                spinTween = null;
                state?.CancelSpin();
            }
        }

        private void OnDestroy()
        {
            spinTween?.Kill();

            if (state != null)
                state.Changed -= RefreshButton;

            if (spinButton != null)
                spinButton.onClick.RemoveListener(OnSpinClicked);
        }
    }
}