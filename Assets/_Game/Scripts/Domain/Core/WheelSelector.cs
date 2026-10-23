using System.Collections.Generic;
using VertigoGames.Common;
using VertigoGames.Data;

namespace VertigoGames.Core
{
    public static class WheelSelector
    {
        public static int SelectIndex(
            IReadOnlyList<WheelSliceData> slices,
            IRandom random)
        {
            float total = 0f;

            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i].Weight > 0f)
                    total += slices[i].Weight;
            }

            if (total <= 0f)
                return -1;

            float roll = random.Range(0f, total);

            for (int i = 0; i < slices.Count; i++)
            {
                float weight = slices[i].Weight;

                if (weight <= 0f)
                    continue;

                roll -= weight;

                if (roll < 0f)
                    return i;
            }

            return slices.Count - 1;
        }
    }
}