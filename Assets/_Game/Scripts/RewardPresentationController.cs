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
            rewardImage.gameObject.SetActive(true);
            rewardImage.color = Color.white;
            rewardImage.rectTransform.localScale = Vector3.zero;

            ResetSparkle(leftSparkle);
            ResetSparkle(rightSparkle);

            sequence = DOTween.Sequence();

            sequence.Append(
                rewardImage.rectTransform
                    .DOScale(rewardScale, showDuration)
                    .SetEase(Ease.OutBack));

            if (leftSparkle != null)
            {
                sequence.Join(
                    leftSparkle.rectTransform
                        .DOScale(sparkleScale, showDuration)
                        .SetEase(Ease.OutBack));
            }

            if (rightSparkle != null)
            {
                sequence.Join(
                    rightSparkle.rectTransform
                        .DOScale(sparkleScale, showDuration)
                        .SetEase(Ease.OutBack));
            }

            sequence.AppendInterval(holdDuration);

            sequence.Append(
                rewardImage.rectTransform
                    .DOScale(0f, hideDuration)
                    .SetEase(Ease.InBack));

            sequence.Join(
                rewardImage.DOFade(0f, hideDuration));

            if (leftSparkle != null)
            {
                sequence.Join(
                    leftSparkle.DOFade(0f, hideDuration));
            }

            if (rightSparkle != null)
            {
                sequence.Join(
                    rightSparkle.DOFade(0f, hideDuration));
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
