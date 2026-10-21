
using System.Collections.Generic;
using UnityEngine;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public class RewardInventoryPanelController : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot;
        [SerializeField] private RewardInventoryItemUI itemPrefab;

        private readonly Dictionary<GameManager.RewardStack,
            RewardInventoryItemUI> items = new();

        private void Update()
        {
            GameManager manager = GameManager.Instance;

            if (manager == null || contentRoot == null || itemPrefab == null)
                return;

            IReadOnlyList<GameManager.RewardStack> stacks =
                manager.RewardStacks;

            if (items.Count != stacks.Count)
                Rebuild(manager);
            else
                RefreshItems(stacks);
        }

        private void Rebuild(GameManager manager)
        {
            foreach (Transform child in contentRoot)
                Destroy(child.gameObject);

            items.Clear();

            foreach (GameManager.RewardStack stack in manager.RewardStacks)
            {
                RewardInventoryItemUI item = Instantiate(itemPrefab, contentRoot);
                item.Bind(stack);
                items.Add(stack, item);
            }
        }

        private void RefreshItems(
            IReadOnlyList<GameManager.RewardStack> stacks)
        {
            foreach (GameManager.RewardStack stack in stacks)
            {
                if (items.TryGetValue(stack, out RewardInventoryItemUI item))
                    item.Refresh(stack);
            }
        }
    }
}
