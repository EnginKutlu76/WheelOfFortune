using System;
using System.Collections.Generic;
using UnityEngine;
using VertigoGames.Common;

namespace VertigoGames.Data
{
    [CreateAssetMenu(
        fileName = "ZoneConfigSet",
        menuName = "Vertigo Games/Zone Config Set")]
    public class ZoneConfigSet : ScriptableObject
    {
        [Serializable]
        private struct Tier
        {
            public int fromZone;
            public ZoneConfig config;
        }

        [SerializeField] private List<Tier> normalTiers = new();
        [SerializeField] private ZoneConfig safeConfig;
        [SerializeField] private ZoneConfig superConfig;

        public ZoneConfig GetConfig(int zone)
        {
            switch (ZoneRules.GetZoneType(zone))
            {
                case ZoneType.Safe: return safeConfig;
                case ZoneType.Super: return superConfig;
            }

            ZoneConfig best = null;
            int bestFrom = int.MinValue;

            foreach (Tier tier in normalTiers)
            {
                if (tier.config == null || tier.fromZone > zone)
                    continue;

                if (tier.fromZone >= bestFrom)
                {
                    best = tier.config;
                    bestFrom = tier.fromZone;
                }
            }

            return best;
        }
    }
}