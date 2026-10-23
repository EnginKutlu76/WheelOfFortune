using System.Collections.Generic;
using UnityEngine;
using VertigoGames.Common;
using VertigoGames.Core;

namespace VertigoGames.UI
{
    public sealed class RewardInventoryView : MonoBehaviour
    {
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private RewardInventoryItemUI itemPrefab;

        private readonly Dictionary<RewardStack, RewardInventoryItemUI> active = new();
        private readonly Stack<RewardInventoryItemUI> pool = new();

        private RunState state;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIRefs.Assign(ref contentRoot, "ui_content_rewards");
        }
#endif

        public void Bind(RunState runState)
        {
            state = runState;
            state.Changed += Sync;
            Sync();
        }

        private void Sync()
        {
            IReadOnlyList<RewardStack> stacks = state.Stacks;

            if (stacks.Count < active.Count)
                ReleaseAll();

            foreach (RewardStack stack in stacks)
            {
                if (active.TryGetValue(stack, out RewardInventoryItemUI item))
                {
                    item.Refresh(stack);
                    continue;
                }

                item = Acquire();
                item.name = $"ui_reward_item_{stack.Reward.RewardId}";
                item.Bind(stack);
                active.Add(stack, item);
            }
        }

        private RewardInventoryItemUI Acquire()
        {
            RewardInventoryItemUI item = pool.Count > 0
                ? pool.Pop()
                : Instantiate(itemPrefab, contentRoot);

            item.gameObject.SetActive(true);
            item.transform.SetAsLastSibling();
            return item;
        }

        private void ReleaseAll()
        {
            foreach (RewardInventoryItemUI item in active.Values)
            {
                item.gameObject.SetActive(false);
                pool.Push(item);
            }

            active.Clear();
        }

        private void OnDestroy()
        {
            if (state != null)
                state.Changed -= Sync;
        }
    }
}