using System;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Tracks whether the mimic-clown enemy is on screen and triggers the loot
    /// table randomizer when it first appears on a non-event floor.
    /// </summary>
    public sealed class ClownService
    {
        public const int ClownTriggerValue = 30707852;

        public bool Check(int clownValue, bool isEventFloor, bool clownOnScreen, Action onTriggered)
        {
            if (clownValue == ClownTriggerValue && !isEventFloor && !clownOnScreen)
            {
                onTriggered?.Invoke();
                return true;
            }

            if (clownOnScreen && clownValue != ClownTriggerValue)
                return false;

            return clownOnScreen;
        }
    }
}
