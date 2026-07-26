using DarkCloud.Core.Dungeon;
using System;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class ClownServiceTests
    {
        [Fact]
        public void Check_WhenClownAppearsOnNonEventFloor_TriggersCallback()
        {
            bool triggered = false;
            var service = new ClownService();

            bool onScreen = service.Check(ClownService.ClownTriggerValue, false, false, () => triggered = true);

            Assert.True(triggered);
            Assert.True(onScreen);
        }

        [Fact]
        public void Check_WhenAlreadyOnScreen_DoesNotTriggerAgain()
        {
            bool triggered = false;
            var service = new ClownService();

            bool onScreen = service.Check(ClownService.ClownTriggerValue, false, true, () => triggered = true);

            Assert.False(triggered);
            Assert.True(onScreen);
        }

        [Fact]
        public void Check_WhenEventFloor_DoesNotTrigger()
        {
            bool triggered = false;
            var service = new ClownService();

            bool onScreen = service.Check(ClownService.ClownTriggerValue, true, false, () => triggered = true);

            Assert.False(triggered);
            Assert.False(onScreen);
        }

        [Fact]
        public void Check_WhenClownLeavesScreen_ReturnsFalse()
        {
            var service = new ClownService();

            bool onScreen = service.Check(0, false, true, () => { });

            Assert.False(onScreen);
        }
    }
}
