using NUnit.Framework;
using VertigoGames.Common;

namespace VertigoGames.Tests
{
    public class ZoneRulesTests
    {
        [TestCase(0, ZoneType.Normal)]
        [TestCase(1, ZoneType.Normal)]
        [TestCase(4, ZoneType.Normal)]
        [TestCase(5, ZoneType.Safe)]
        [TestCase(25, ZoneType.Safe)]
        [TestCase(30, ZoneType.Super)]
        [TestCase(60, ZoneType.Super)]
        [TestCase(35, ZoneType.Safe)]
        public void GetZoneType_ReturnsExpected(int zone, ZoneType expected)
        {
            Assert.AreEqual(expected, ZoneRules.GetZoneType(zone));
        }

        [Test]
        public void GetNextZoneOfType_FindsUpcomingZones()
        {
            Assert.AreEqual(5, ZoneRules.GetNextZoneOfType(5, ZoneType.Safe));
            Assert.AreEqual(10, ZoneRules.GetNextZoneOfType(6, ZoneType.Safe));
            Assert.AreEqual(30, ZoneRules.GetNextZoneOfType(1, ZoneType.Super));
        }
    }
}