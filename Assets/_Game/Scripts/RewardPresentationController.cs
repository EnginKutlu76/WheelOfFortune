using System;
using DG.Tweening;
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
            if (rewardSprite == null || rewardImage == null ||
                presentationRoot == null)
            {
                completed?.Invoke();
                return;
            }

            sequence?.Kill();
            onFinished = completed;
            IsPlaying = true;

            presentationRoot.SetActive(true);

            rewardImage.sprite = rewardSprite;
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
                rightSparkle.rectTransform.localRotation = Quaternion.identity;

            sequence = DOTween.Sequence();

            // Brief spark before the reward appears.
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

            // Reveal the reward while the right sparkle rotates.
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

            sequence.AppendInterval(holdDuration);

            sequence.Append(
                rewardImage.rectTransform
                    .DOScale(0f, hideDuration)
                    .SetEase(Ease.InBack));

            sequence.Join(rewardImage.DOFade(0f, hideDuration));

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

            sparkle.gameObject.SetActive(true);
            sparkle.color = Color.white;
            sparkle.rectTransform.localScale = Vector3.zero;
        }

        private void Finish()
        {
            presentationRoot.SetActive(false);
            IsPlaying = false;

            Action callback = onFinished;
            onFinished = null;
            callback?.Invoke();
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }
    }
}
