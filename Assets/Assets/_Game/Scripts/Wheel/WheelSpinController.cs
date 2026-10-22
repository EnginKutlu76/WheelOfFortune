using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Core;
using VertigoGames.Data;
using System.Collections;

namespace VertigoGames.Gameplay
{
    public class WheelSpinController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform wheelTransform;
        [SerializeField] private Button spinButton;

        [Header("Zone Configurations")]
        [SerializeField] private ZoneConfig normalZoneConfig;
        [SerializeField] private ZoneConfig safeZoneConfig;
        [SerializeField] private ZoneConfig superZoneConfig;

        [Header("Spin Settings")]
        [SerializeField, Min(1)] private int minFullRotations = 4;
        [SerializeField, Min(1)] private int maxFullRotations = 6;
        [SerializeField, Min(0.1f)] private float spinDuration = 3f;
        [SerializeField, Range(2f, 15f)] private float overshootAngle = 8f;
        [SerializeField, Min(0.05f)] private float reboundDuration = 0.18f;

        [Header("Wheel Visuals")]
        [SerializeField] private Image wheelImage;
        [SerializeField] private Image pointerImage;

        [Header("Bronze Sprites")]
        [SerializeField] private Sprite bronzeWheelSprite;
        [SerializeField] private Sprite bronzePointerSprite;

        [Header("Silver Sprites")]
        [SerializeField] private Sprite silverWheelSprite;
        [SerializeField] private Sprite silverPointerSprite;

        [Header("Gold Sprites")]
        [SerializeField] private Sprite goldWheelSprite;
        [SerializeField] private Sprite goldPointerSprite;

        [Header("Slice Visuals")]
        [SerializeField] private WheelSliceVisualController sliceVisualController;

        [Header("Reward Presentation")]
        [SerializeField] private RewardPresentationController rewardPresentationController;

        [SerializeField] private UnityEngine.UI.Image sparkleRightImage;

        private bool isSpinning;
        private Tween spinTween;
        private int selectedSliceIndex = -1;
        private ZoneConfig activeZoneConfig;

        private List<WheelSliceData> activeSlices;

        [SerializeField] private Image resultPanelImage;
        [SerializeField] private Color normalPanelColor = Color.white;
        [SerializeField] private Color bombPanelColor = new Color(0.35f, 0.05f, 0.05f, 1f);

        private void OnValidate()
        {
            maxFullRotations = Mathf.Max(
                minFullRotations,
                maxFullRotations);
        }

        private void OnEnable()
        {
            if (spinButton != null)
                spinButton.onClick.AddListener(Spin);

            StartCoroutine(InitializeWhenReady());
        }


        private IEnumerator InitializeWhenReady()
        {
            while (GameManager.Instance == null)
                yield return null;

            UpdateZoneVisuals();
            InitializeSliceVisuals();
        }


        private void InitializeSliceVisuals()
        {
            activeZoneConfig = GetCurrentZoneConfig();

            if (activeZoneConfig == null)
            {
                Debug.LogError("Missing zone configuration.");
                return;
            }

            activeSlices = activeZoneConfig.CreateSpinSlices();

            if (activeSlices == null || activeSlices.Count != 8)
            {
                Debug.LogError("The zone must provide exactly 8 slices.");
                return;
            }

            if (sliceVisualController == null)
            {
                Debug.LogError("Slice Visual Controller is not assigned.");
                return;
            }

            sliceVisualController.UpdateSliceVisuals(activeSlices);
        }

        private void OnDisable()
        {
            if (spinButton != null)
                spinButton.onClick.RemoveListener(Spin);

            spinTween?.Kill();
            spinTween = null;
            isSpinning = false;

            if (AudioManager.Instance != null)
                AudioManager.Instance.StopWheelSpin();
        }

        private ZoneConfig GetCurrentZoneConfig()
        {
            if (GameManager.Instance == null)
                return null;

            switch (GameManager.Instance.CurrentZoneType)
            {
                case ZoneType.Safe:
                    return safeZoneConfig;

                case ZoneType.Super:
                    return superZoneConfig;

                default:
                    return normalZoneConfig;
            }
        }

        public void Spin()
        {
            if (isSpinning ||
                wheelTransform == null ||
                GameManager.Instance == null ||
                GameManager.Instance.IsGameOver ||
                GameManager.Instance.HasExited)
            {
                return;
            }

            activeZoneConfig = GetCurrentZoneConfig();

            if (activeZoneConfig == null)
            {
                Debug.LogError(
                    "Missing configuration for the current zone type.");

                return;
            }

            activeSlices = activeZoneConfig.CreateSpinSlices();

            if (activeSlices == null || activeSlices.Count != 8)
            {
                Debug.LogError(
                    "The wheel configuration must contain exactly 8 slices.");

                return;
            }

            sliceVisualController?.UpdateSliceVisuals(activeSlices);

            bool isSafeZone =
                GameManager.Instance.CurrentZoneType != ZoneType.Normal;

            if (isSafeZone)
            {
                for (int i = 0; i < activeSlices.Count; i++)
                {
                    if (activeSlices[i].Type == SliceType.Bomb)
                    {
                        Debug.LogError(
                            "Safe and super configurations cannot contain bomb slices.");

                        return;
                    }
                }
            }

            selectedSliceIndex = SelectSliceIndex();

            if (selectedSliceIndex < 0)
                return;

            isSpinning = true;
            GameManager.Instance.SetSpinning(true);

            if (spinButton != null)
                spinButton.interactable = false;

            if (AudioManager.Instance != null)
                AudioManager.Instance.StartWheelSpin();

            int rotations = Random.Range(
                minFullRotations,
                maxFullRotations + 1);

            float currentAngle = wheelTransform.localEulerAngles.z;
            float sliceAngle = 360f / activeSlices.Count;

            float currentNormalizedAngle = Mathf.Repeat(currentAngle, 360f);

            float targetNormalizedAngle = Mathf.Repeat(
                selectedSliceIndex * sliceAngle,
                360f);

            float alignmentAngle = Mathf.DeltaAngle(
                currentNormalizedAngle,
                targetNormalizedAngle);

            float targetAngle =
                currentAngle + rotations * 360f + alignmentAngle;

            bool useRebound = Random.value < 0.4f;

            Sequence spinSequence = DOTween.Sequence();

            if (useRebound)
            {
                float overshootTargetAngle = targetAngle + overshootAngle;

                spinSequence.Append(
                    wheelTransform
                        .DOLocalRotate(
                            new Vector3(0f, 0f, overshootTargetAngle),
                            spinDuration,
                            RotateMode.FastBeyond360)
                        .SetEase(Ease.OutCubic));

                spinSequence.Append(
                    wheelTransform
                        .DOLocalRotate(
                            new Vector3(0f, 0f, targetAngle),
                            reboundDuration,
                            RotateMode.Fast)
                        .SetEase(Ease.OutSine));
            }
            else
            {
                spinSequence.Append(
                    wheelTransform
                        .DOLocalRotate(
                            new Vector3(0f, 0f, targetAngle),
                            spinDuration,
                            RotateMode.FastBeyond360)
                        .SetEase(Ease.OutCubic));
            }

            spinSequence.OnComplete(CompleteSpin);

            spinTween = spinSequence;
        }

        private int SelectSliceIndex()
        {
            var slices = activeSlices;
            float totalWeight = 0f;

            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i].Weight > 0f)
                    totalWeight += slices[i].Weight;
            }

            if (totalWeight <= 0f)
            {
                Debug.LogError("All slice weights are zero.");
                return -1;
            }

            float roll = Random.Range(0f, totalWeight);

            for (int i = 0; i < slices.Count; i++)
            {
                float weight = slices[i].Weight;

                if (weight <= 0f)
                    continue;

                roll -= weight;

                if (roll < 0f)
                    return i;
            }

            return slices.Count - 1;
        }

        private void CompleteSpin()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopWheelSpin();

            if (GameManager.Instance != null)
                GameManager.Instance.SetSpinning(false);

            ResolveSpin();
        }

        private void ResolveSpin()
        {
            if (activeZoneConfig == null ||
                GameManager.Instance == null)
            {
                Debug.LogError(
                    "Missing active zone configuration or GameManager.");

                UnlockSpinButton();
                return;
            }

            if (selectedSliceIndex < 0 ||
                selectedSliceIndex >= activeSlices.Count)
            {
                Debug.LogError("Selected slice index is invalid.");
                UnlockSpinButton();
                return;
            }

            WheelSliceData selectedSlice = activeSlices[selectedSliceIndex];

            Debug.Log($"Selected slice index: {selectedSliceIndex}");

            if (selectedSlice.Type == SliceType.Bomb)
            {
                if (GameManager.Instance.CurrentZoneType != ZoneType.Normal)
                {
                    Debug.LogError(
                        "Bomb result detected in a protected zone.");

                    UnlockSpinButton();
                    return;
                }

                UpdateResultPanelColor(true);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayBombHit();

                GameManager.Instance.TriggerBomb();

                if (sparkleRightImage != null)
                    sparkleRightImage.color = new Color32(139, 0, 0, 255);

                if (rewardPresentationController != null)
                    rewardPresentationController.ShowBomb();

                Debug.Log("Bomb hit. Accumulated rewards lost.");
                return;
            }

            if (selectedSlice.Reward == null)
            {
                Debug.LogWarning(
                    "Selected reward slice has no RewardData assigned.");

                UnlockSpinButton();
                return;
            }

            UpdateResultPanelColor(false);

            int rewardValue = activeZoneConfig.GetRewardValue(
                selectedSlice.Reward,
                GameManager.Instance.CurrentZone);

            GameManager.Instance.AddReward(
                selectedSlice.Reward,
                rewardValue);

            Debug.Log(
                $"Reward collected: {selectedSlice.Reward.DisplayName} (+{rewardValue})");

            if (rewardPresentationController != null &&
                selectedSlice.Reward.Icon != null)
            {
                rewardPresentationController.Play(
                    selectedSlice.Reward.Icon,
                    selectedSlice.Reward.DisplayName,
                    1,
                    FinishRewardPresentation);
            }
            else
            {
                FinishRewardPresentation();
            }
        }

        private void FinishRewardPresentation()
        {
            if (GameManager.Instance == null)
                return;

            if (!GameManager.Instance.IsGameOver &&
                !GameManager.Instance.HasExited)
            {
                GameManager.Instance.AdvanceZone();

                UpdateZoneVisuals();
                InitializeSliceVisuals();
            }

            UnlockSpinButton();
        }

        private void UnlockSpinButton()
        {
            isSpinning = false;

            if (spinButton != null &&
                GameManager.Instance != null &&
                !GameManager.Instance.IsGameOver &&
                !GameManager.Instance.HasExited)
            {
                spinButton.interactable = true;
            }
        }

        private void OnDestroy()
        {
            spinTween?.Kill();
        }


        private void UpdateZoneVisuals()
        {
            if (GameManager.Instance == null)
                return;

            Sprite wheelSprite;
            Sprite pointerSprite;

            switch (GameManager.Instance.CurrentZoneType)
            {
                case ZoneType.Safe:
                    wheelSprite = silverWheelSprite;
                    pointerSprite = silverPointerSprite;
                    break;

                case ZoneType.Super:
                    wheelSprite = goldWheelSprite;
                    pointerSprite = goldPointerSprite;
                    break;

                default:
                    wheelSprite = bronzeWheelSprite;
                    pointerSprite = bronzePointerSprite;
                    break;
            }

            if (wheelImage != null && wheelSprite != null)
                wheelImage.sprite = wheelSprite;

            if (pointerImage != null && pointerSprite != null)
                pointerImage.sprite = pointerSprite;
        }

        public void RefreshAfterRestart()
        {
            if (GameManager.Instance == null)
                return;

            spinTween?.Kill();
            spinTween = null;
            isSpinning = false;
            selectedSliceIndex = -1;

            GameManager.Instance.SetSpinning(false);

            if (spinButton != null)
                spinButton.interactable = true;

            if (rewardPresentationController != null)
                rewardPresentationController.ResetPresentation();

            if (sparkleRightImage != null)
                sparkleRightImage.color = Color.white;

            UpdateResultPanelColor(false);

            UpdateZoneVisuals();
            InitializeSliceVisuals();
        }

        private void UpdateResultPanelColor(bool isBomb)
        {
            if (resultPanelImage == null)
                return;

            resultPanelImage.color = isBomb
                ? bombPanelColor
                : normalPanelColor;
        }
    }
}
