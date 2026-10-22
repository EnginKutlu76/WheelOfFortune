using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoGames.Gameplay
{
    public class RewardPresentationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject presentationRoot;
        [SerializeField] private Image rewardImage;
        [SerializeField] private Image leftSparkle;
        [SerializeField] private Image rightSparkle;
        [SerializeField] private Sprite bombIcon;
        [SerializeField] private TMP_Text rewardNameText;
        [SerializeField] private TMP_Text rewardCountText;

        [Header("Animation")]
        [SerializeField, Min(0.1f)] private float showDuration = 0.3f;
        [SerializeField, Min(0f)] private float holdDuration = 0.7f;
        [SerializeField, Min(0.1f)] private float hideDuration = 0.35f;
        [SerializeField, Min(1f)] private float rewardScale = 1.35f;
        [SerializeField, Min(1f)] private float sparkleScale = 1.5f;

        private Sequence sequence;
        private Action onFinished;

        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            if (presentationRoot != null)
                presentationRoot.SetActive(false);
        }

        public void Play(Sprite rewardSprite, Action completed)
        {
            PlayInternal(rewardSprite, completed, false);
        }

        public void Play(
            Sprite rewardSprite,
            string rewardName,
            int rewardCount,
            Action completed)
        {
            if (rewardNameText != null)
                rewardNameText.text = rewardName;

            if (rewardCountText != null)
                rewardCountText.text = $"x{rewardCount}";

            PlayInternal(rewardSprite, completed, false);
        }

        public void ShowBomb()
        {
            if (bombIcon == null)
            {
                Debug.LogError("Bomb icon is not assigned.");
                return;
            }

            if (rewardNameText != null)
                rewardNameText.gameObject.SetActive(false);

            if (rewardCountText != null)
                rewardCountText.gameObject.SetActive(false);

            PlayInternal(bombIcon, null, true);
        }

        private void PlayInternal(
            Sprite sprite,
            Action completed,
            bool keepVisible)
        {
            if (sprite == null ||
                rewardImage == null ||
                presentationRoot == null)
            {
                completed?.Invoke();
                return;
            }

            sequence?.Kill();
            sequence = null;

            onFinished = completed;
            IsPlaying = true;

            presentationRoot.SetActive(true);

            if (!keepVisible)
            {
                if (rewardNameText != null)
                    rewardNameText.gameObject.SetActive(true);

                if (rewardCountText != null)
                    rewardCountText.gameObject.SetActive(true);
            }

            rewardImage.sprite = sprite;
            rewardImage.type = Image.Type.Simple;
            rewardImage.preserveAspect = true;
            rewardImage.color = Color.white;
            rewardImage.gameObject.SetActive(true);
            rewardImage.rectTransform.localScale = Vector3.zero;

            ResetSparkle(leftSparkle);
            ResetSparkle(rightSparkle);

            if (leftSparkle != null)
                leftSparkle.rectTransform.localRotation = Quaternion.identity;

            if (rightSparkle != null)
            {
                rightSparkle.rectTransform.localRotation = Quaternion.identity;

                if (keepVisible)
                    rightSparkle.color = new Color32(139, 0, 0, 255);
            }

            sequence = DOTween.Sequence();

            if (leftSparkle != null)
            {
                sequence.Append(
                    leftSparkle.rectTransform
                        .DOScale(sparkleScale, 0.08f)
                        .SetEase(Ease.OutQuad));

                sequence.Append(
                    leftSparkle.rectTransform
                        .DOScale(0f, 0.12f)
                        .SetEase(Ease.InQuad));
            }

            sequence.Append(
                rewardImage.rectTransform
                    .DOScale(rewardScale, showDuration)
                    .SetEase(Ease.OutBack));

            if (rightSparkle != null)
            {
                sequence.Join(
                    rightSparkle.rectTransform
                        .DOScale(sparkleScale, showDuration)
                        .SetEase(Ease.OutBack));

                sequence.Join(
                    rightSparkle.rectTransform
                        .DOLocalRotate(
                            new Vector3(0f, 0f, 360f),
                            showDuration,
                            RotateMode.FastBeyond360)
                        .SetEase(Ease.OutCubic));
            }

            if (keepVisible)
            {
                sequence.OnComplete(() =>
                {
                    IsPlaying = false;
                    sequence = null;
                });

                return;
            }

            sequence.AppendInterval(holdDuration);

            sequence.Append(
                rewardImage.rectTransform
                    .DOScale(0f, hideDuration)
                    .SetEase(Ease.InBack));

            sequence.Join(
                rewardImage.DOFade(0f, hideDuration));

            if (rightSparkle != null)
            {
                sequence.Join(
                    rightSparkle.rectTransform
                        .DOScale(0f, hideDuration)
                        .SetEase(Ease.InBack));
            }

            sequence.OnComplete(Finish);
        }

        private void ResetSparkle(Image sparkle)
        {
            if (sparkle == null)
                return;

            sparkle.rectTransform.DOKill();
            sparkle.gameObject.SetActive(true);
            sparkle.color = Color.white;
            sparkle.rectTransform.localScale = Vector3.zero;
        }

        private void Finish()
        {
            if (presentationRoot != null)
                presentationRoot.SetActive(false);

            IsPlaying = false;
            sequence = null;

            Action callback = onFinished;
            onFinished = null;
            callback?.Invoke();
        }

        public void ResetPresentation()
        {
            sequence?.Kill();
            sequence = null;
            onFinished = null;
            IsPlaying = false;

            if (presentationRoot != null)
                presentationRoot.SetActive(false);

            if (rewardImage != null)
            {
                rewardImage.rectTransform.DOKill();
                rewardImage.rectTransform.localScale = Vector3.zero;
                rewardImage.color = Color.white;
            }

            if (rewardNameText != null)
                rewardNameText.gameObject.SetActive(true);

            if (rewardCountText != null)
                rewardCountText.gameObject.SetActive(true);

            ResetSparkle(leftSparkle);
            ResetSparkle(rightSparkle);

            if (leftSparkle != null)
                leftSparkle.rectTransform.localRotation = Quaternion.identity;

            if (rightSparkle != null)
            {
                rightSparkle.rectTransform.localRotation = Quaternion.identity;
                rightSparkle.color = Color.white;
            }
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }
    }
}
