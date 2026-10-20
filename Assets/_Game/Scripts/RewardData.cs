
using UnityEngine;

namespace VertigoGames.Data
{
    [CreateAssetMenu(
        fileName = "NewReward",
        menuName = "Vertigo Games/Reward Data")]
    public class RewardData : ScriptableObject
    {
        [SerializeField] private string rewardId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private int value;

        public string RewardId => rewardId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public int Value => value;
    }
}
