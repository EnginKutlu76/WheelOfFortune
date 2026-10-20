using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Core;
using VertigoGames.Data;

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

        private bool isSpinning;
        private Tween spinTween;
        private int selectedSliceIndex = -1;
        private ZoneConfig activeZoneConfig;

        private List<WheelSliceData> activeSlices;

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

            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager is not initialized.");
                return;
            }

            UpdateZoneVisuals();
            InitializeSliceVisuals();
        }


        private void InitializeSliceVisuals()
        {
            activeZoneConfig = GetCurrentZoneConfig();

            if (activeZoneConfig == null)
            {
                Debug.LogError("Missing zone configuration at initialization.");
                return;
            }

            activeSlices = activeZoneConfig.CreateSpinSlices();

            if (activeSlices == null)
                return;

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

            activeSlices = activeZoneConfig != null
                ? activeZoneConfig.CreateSpinSlices()
                : null;

            if (activeSlices == null)
                return;

            sliceVisualController?.UpdateSliceVisuals(activeSlices);

            if (activeZoneConfig == null)
            {
                Debug.LogError("Missing configuration for the current zone type.");
                return;
            }

            var slices = activeSlices;

            if (slices == null || slices.Count != 8)
            {
                Debug.LogError("The wheel configuration must contain exactly 8 slices.");
                return;
            }

            bool isSafeZone =
                GameManager.Instance.CurrentZoneType != ZoneType.Normal;

            if (isSafeZone)
            {
                for (int i = 0; i < slices.Count; i++)
                {
                    if (slices[i].Type == SliceType.Bomb)
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

            int rotations = Random.Range(
                minFullRotations,
                maxFullRotations + 1);

            float currentAngle = wheelTransform.localEulerAngles.z;
            float sliceAngle = 360f / activeSlices.Count;

            float currentNormalizedAngle = Mathf.Repeat(
                currentAngle,
                360f);

            float targetNormalizedAngle = Mathf.Repeat(
                selectedSliceIndex * sliceAngle,
                360f);

            float alignmentAngle = Mathf.DeltaAngle(
                currentNormalizedAngle,
                targetNormalizedAngle);

            float targetAngle =
                currentAngle
                + rotations * 360f
                + alignmentAngle;

            spinTween = wheelTransform
                .DOLocalRotate(
                    new Vector3(0f, 0f, targetAngle),
                    spinDuration,
                    RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .OnComplete(CompleteSpin);
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
            isSpinning = false;

            if (GameManager.Instance != null)
                GameManager.Instance.SetSpinning(false);

            ResolveSpin();

            if (spinButton != null &&
                GameManager.Instance != null &&
                !GameManager.Instance.IsGameOver &&
                !GameManager.Instance.HasExited)
            {
                spinButton.interactable = true;
            }
        }

        private void ResolveSpin()
        {
            if (activeZoneConfig == null ||
                GameManager.Instance == null)
            {
                Debug.LogError("Missing active zone configuration or GameManager.");
                return;
            }

            var slices = activeSlices;

            if (selectedSliceIndex < 0 ||
                selectedSliceIndex >= slices.Count)
            {
                Debug.LogError("Selected slice index is invalid.");
                return;
            }

            WheelSliceData selectedSlice = slices[selectedSliceIndex];
            Debug.Log($"Selected slice index: {selectedSliceIndex}");

            if (selectedSlice.Type == SliceType.Bomb)
            {
                if (GameManager.Instance.CurrentZoneType != ZoneType.Normal)
                {
                    Debug.LogError("Bomb result detected in a protected zone.");
                    return;
                }

                GameManager.Instance.TriggerBomb();
                Debug.Log("Bomb hit. Accumulated rewards lost.");
                return;
            }

            if (selectedSlice.Reward == null)
            {
                Debug.LogWarning("Selected reward slice has no RewardData assigned.");
                return;
            }

            GameManager.Instance.AddReward(
                selectedSlice.Reward.Value);

            Debug.Log(
                $"Reward collected: {selectedSlice.Reward.DisplayName}");

            GameManager.Instance.AdvanceZone();

            UpdateZoneVisuals();
            InitializeSliceVisuals();
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

    }
}
