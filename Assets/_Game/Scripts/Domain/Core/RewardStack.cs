using VertigoGames.Data;

namespace VertigoGames.Core
{
    public sealed class RewardStack
    {
        public RewardData Reward { get; }
        public int Count { get; private set; }
        public int TotalValue { get; private set; }

        public RewardStack(RewardData reward, int value)
        {
            Reward = reward;
            Count = 1;
            TotalValue = value;
        }

        public void Add(int value)
        {
            Count++;
            TotalValue += value;
        }
    }
}