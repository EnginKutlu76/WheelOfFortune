
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public class RewardInventoryPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private RewardInventoryItemUI itemPrefab;

        private readonly Dictionary<GameManager.RewardStack,
            RewardInventoryItemUI> items = new();

        private void Awake()
        {
            if (contentRoot == null || itemPrefab == null)
            {
                Debug.LogError(
                    "Reward inventory references are missing.", this);
                return;
            }

            VerticalLayoutGroup layout =
                contentRoot.GetComponent<VerticalLayoutGroup>();

            if (layout == null)
                layout = contentRoot.gameObject
                    .AddComponent<VerticalLayoutGroup>();

            layout.childAlignment = TextAnchor.UpperLeft;
            layout.spacing = 110f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter =
                contentRoot.GetComponent<ContentSizeFitter>();

            if (fitter == null)
                fitter = contentRoot.gameObject
                    .AddComponent<ContentSizeFitter>();

            fitter.horizontalFit =
                ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit =
                ContentSizeFitter.FitMode.PreferredSize;
        }

        private void Update()
        {
            GameManager manager = GameManager.Instance;

            if (manager == null || contentRoot == null ||
                itemPrefab == null)
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
                if (stack == null || stack.Reward == null)
                    continue;

                RewardInventoryItemUI item =
                    Instantiate(itemPrefab, contentRoot);

                item.name = $"ui_reward_item_{stack.Reward.RewardId}";
                item.Bind(stack);
                items.Add(stack, item);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);
        }

        private void RefreshItems(
            IReadOnlyList<GameManager.RewardStack> stacks)
        {
            foreach (GameManager.RewardStack stack in stacks)
            {
                if (stack != null &&
                    items.TryGetValue(stack, out RewardInventoryItemUI item))
                {
                    item.Refresh(stack);
                }
            }
        }
    }
}
