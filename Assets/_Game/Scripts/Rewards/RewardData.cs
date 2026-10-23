
using UnityEngine;

namespace VertigoGames.Data
{
    public enum RewardCategory
    {
        Standard,
        WeaponPoints,
        Costume
    }

    public enum WeaponPointType
    {
        Vest,
        Knife,
        Pistol,
        Rifle,
        Shotgun,
        SMG,
        Sniper
    }

    [CreateAssetMenu(
        fileName = "NewReward",
        menuName = "Vertigo Games/Reward Data")]
    public class RewardData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private string rewardId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite inventoryIcon;
        [SerializeField, Min(0)] private int value;

        [Header("Reward Type")]
        [SerializeField] private RewardCategory category;
        [SerializeField] private WeaponPointType weaponPointType;

        [Header("Economy")]
        [SerializeField] private bool scalesWithZone = true;

        public bool ScalesWithZone => scalesWithZone;

        public string RewardId => rewardId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public Sprite InventoryIcon =>
            inventoryIcon != null ? inventoryIcon : icon;
        public int Value => value;
        public RewardCategory Category => category;
        public WeaponPointType WeaponPointType => weaponPointType;
    }
}
