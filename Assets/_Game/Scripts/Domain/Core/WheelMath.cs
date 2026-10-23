using UnityEngine;

namespace VertigoGames.Core
{
    public static class WheelMath
    {
        public static float GetTargetAngle(
            float currentAngle,
            int sliceIndex,
            int sliceCount,
            int fullRotations)
        {
            float sliceAngle = 360f / sliceCount;
            float aligned = sliceIndex * sliceAngle;

            float forward = Mathf.Repeat(
                aligned - Mathf.Repeat(currentAngle, 360f), 360f);

            return currentAngle + fullRotations * 360f + forward;
        }
    }
}