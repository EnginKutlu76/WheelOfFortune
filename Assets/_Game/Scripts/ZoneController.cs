
namespace VertigoGames.Gameplay
{
    public static class ZoneController
    {
        public static ZoneType GetZoneType(int zoneNumber)
        {
            if (zoneNumber <= 0)
                return ZoneType.Normal;

            if (zoneNumber % 30 == 0)
                return ZoneType.Super;

            if (zoneNumber % 5 == 0)
                return ZoneType.Safe;

            return ZoneType.Normal;
        }
    }
}
