using System.Collections.Generic;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Pure progression rules for dungeons: gate-key items, back-floor keys,
    /// event floors, and enemy key-drops. These lookups contain no memory I/O
    /// and can be tested against plain dungeon identifiers.
    /// </summary>
    public static class DungeonProgression
    {
        public static IReadOnlyList<byte> GetGateKeyItems(byte dungeon)
        {
            switch (dungeon)
            {
                case 0: // Divine Beast Cave
                    return new byte[] { 195 }; // Dran's Crest
                case 1: // Wise Owl Forest
                    return new byte[] { 196, 198, 205 }; // Shiny Stone, Red Berry, Pointy Chestnut
                case 2: // Shipwreck
                    return new byte[] { 201 }; // Hook
                case 3: // Sun and Moon Temple
                    return new byte[] { 202 }; // King's Slate
                case 4: // Moon Sea
                    return new byte[] { 203 }; // Gunpowder
                case 5: // Gallery of Time
                    return new byte[] { 204 }; // Clock Hands
                case 6: // Demon Shaft
                    return new byte[] { 206 }; // Black Knight Crest
                default:
                    return new byte[0];
            }
        }

        public static byte GetBackFloorKeyItem(byte dungeon)
        {
            switch (dungeon)
            {
                case 0: // Divine Beast Cave
                    return 224; // Tram Oil
                case 1: // Wise Owl Forest
                    return 225; // Sun Dew
                case 2: // Shipwreck
                    return 226; // Flapping Fish
                case 3: // Sun and Moon Temple
                    return 228; // Secret Path Key
                case 4: // Moon Sea
                    return 229; // Bravery Launch
                case 5: // Gallery of Time
                    return 230; // Flapping Duster
                case 6: // Demon Shaft
                    return 231; // Crystal Eyeball
                default:
                    return byte.MaxValue;
            }
        }

        public static IReadOnlyList<byte> GetEventFloors(byte dungeon)
        {
            switch (dungeon)
            {
                case 0: // Divine Beast Cave
                    return new byte[] { 3, 7, 14 };
                case 1: // Wise Owl Forest
                    return new byte[] { 8, 16 };
                case 2: // Shipwreck
                    return new byte[] { 8, 17 };
                case 3: // Sun and Moon Temple
                    return new byte[] { 8, 17 };
                case 4: // Moon Sea
                    return new byte[] { 7, 14 };
                case 5: // Gallery of Time
                    return new byte[] { 24 };
                case 6: // Demon Shaft
                    return new byte[] { 99 };
                default:
                    return new byte[0];
            }
        }

        public static bool IsEventFloor(byte dungeon, byte floor)
        {
            foreach (byte eventFloor in GetEventFloors(dungeon))
            {
                if (eventFloor == floor)
                    return true;
            }

            return false;
        }

        public static bool EnemyDropsGateKey(byte dungeon, byte itemId)
        {
            foreach (byte keyItem in GetGateKeyItems(dungeon))
            {
                if (keyItem == itemId)
                    return true;
            }

            return false;
        }
    }
}
