using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Audio;

namespace VertigoGames.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class UIButtonSFX : MonoBehaviour
    {
        public enum SoundType
        {
            ButtonClick,
            Collect
        }

        [SerializeField] private Button button;
        [SerializeField] private SoundType soundType = SoundType.ButtonClick;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (button == null)
                button = GetComponent<Button>();
        }
#endif

        private void OnEnable()
        {
            button.onClick.AddListener(PlayClickSound);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            if (AudioManager.Instance == null)
                return;

            if (soundType == SoundType.Collect)
                AudioManager.Instance.PlayCollectButton();
            else
                AudioManager.Instance.PlayButtonClick();
        }
    }
}