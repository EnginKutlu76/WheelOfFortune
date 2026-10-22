
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    [RequireComponent(typeof(Button))]
    public class UIButtonSFX : MonoBehaviour
    {
        public enum SoundType
        {
            ButtonClick,
            Collect
        }

        [SerializeField]
        private SoundType soundType =
            SoundType.ButtonClick;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (button == null)
                button = GetComponent<Button>();

            button.onClick.AddListener(PlayClickSound);
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            if (AudioManager.Instance == null)
                return;

            switch (soundType)
            {
                case SoundType.Collect:
                    AudioManager.Instance.PlayCollectButton();
                    break;

                default:
                    AudioManager.Instance.PlayButtonClick();
                    break;
            }
        }
    }
}
