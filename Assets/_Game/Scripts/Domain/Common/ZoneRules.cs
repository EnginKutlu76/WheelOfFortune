using System;

namespace VertigoGames.Common
{
    public static class ZoneRules
    {
        public const int SafeInterval = 5;
        public const int SuperInterval = 30;

        public static ZoneType GetZoneType(int zone)
        {
            if (zone <= 0)
                return ZoneType.Normal;

            if (zone % SuperInterval == 0)
                return ZoneType.Super;

            if (zone % SafeInterval == 0)
                return ZoneType.Safe;

            return ZoneType.Normal;
        }

        public static int GetNextZoneOfType(int fromZone, ZoneType type)
        {
            for (int zone = Math.Max(1, fromZone); ; zone++)
            {
                if (GetZoneType(zone) == type)
                    return zone;
            }
        }
    }
}