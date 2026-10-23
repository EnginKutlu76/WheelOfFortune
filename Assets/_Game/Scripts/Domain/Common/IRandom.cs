namespace VertigoGames.Common
{
    public interface IRandom
    {
        int Range(int minInclusive, int maxExclusive);

        float Range(float minInclusive, float maxInclusive);

        float Value { get; }
    }

    public sealed class UnityRandom : IRandom
    {
        public int Range(int minInclusive, int maxExclusive) =>
            UnityEngine.Random.Range(minInclusive, maxExclusive);

        public float Range(float minInclusive, float maxInclusive) =>
            UnityEngine.Random.Range(minInclusive, maxInclusive);

        public float Value => UnityEngine.Random.value;
    }
}