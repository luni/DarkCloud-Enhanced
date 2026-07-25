using System.Text;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Session
{
    public class GameSessionDetectorTests
    {
        private const long BaseAddress = 0x20000000L;
        private const int Capacity = 0x02000000;

        [Fact]
        public void Detect_NullMemory_ReturnsNoEmulator()
        {
            var detector = new GameSessionDetector();
            Assert.Equal(GameSessionState.NoEmulator, detector.Detect(null, GameSessionState.None));
        }

        [Fact]
        public void Detect_BootMarkerMissing_ReturnsEmulatorWithoutGame()
        {
            var memory = new InMemoryGameMemory(BaseAddress, Capacity);
            var detector = new GameSessionDetector();

            Assert.Equal(GameSessionState.EmulatorWithoutGame, detector.Detect(memory, GameSessionState.NoEmulator));
        }

        [Fact]
        public void Detect_PnachDisabled_ReturnsPnachDisabled()
        {
            var memory = CreateBootedMemory();
            var writer = new GameMemoryWriter(memory);
            writer.WriteByte(0x21F10020L, 0);

            var detector = new GameSessionDetector();
            Assert.Equal(GameSessionState.PnachDisabled, detector.Detect(memory, GameSessionState.EmulatorWithoutGame));
        }

        [Fact]
        public void Detect_ModFlagOwnedByAnother_ReturnsModAlreadyOpen()
        {
            var memory = CreateBootedMemory();
            var writer = new GameMemoryWriter(memory);
            writer.WriteByte(0x21F10024L, 1);

            var detector = new GameSessionDetector();
            Assert.Equal(GameSessionState.ModAlreadyOpen, detector.Detect(memory, GameSessionState.EmulatorWithoutGame));
        }

        [Fact]
        public void Detect_ClaimsModFlagAndReturnsMainMenu()
        {
            var memory = CreateBootedMemory();

            var detector = new GameSessionDetector();
            Assert.Equal(GameSessionState.MainMenu, detector.Detect(memory, GameSessionState.EmulatorWithoutGame));
            Assert.Equal(GameSessionState.MainMenu, detector.Detect(memory, GameSessionState.MainMenu));
        }

        [Fact]
        public void Detect_TownModeWithOpeningBook_ReturnsTitleScreen()
        {
            var memory = CreateBootedMemory();
            var writer = new GameMemoryWriter(memory);
            writer.WriteByte(0x202A3420L, 9);

            var detector = new GameSessionDetector();
            Assert.Equal(GameSessionState.TitleScreen, detector.Detect(memory, GameSessionState.EmulatorWithoutGame));
        }

        [Fact]
        public void Detect_DungeonMode_ReturnsInGame()
        {
            var memory = CreateBootedMemory();
            var writer = new GameMemoryWriter(memory);
            writer.WriteByte(0x202A2534L, 2);

            var detector = new GameSessionDetector();
            Assert.Equal(GameSessionState.InGame, detector.Detect(memory, GameSessionState.MainMenu));
        }

        [Fact]
        public void Detect_GeoramaMode_ReturnsInGame()
        {
            var memory = CreateBootedMemory();
            var writer = new GameMemoryWriter(memory);
            writer.WriteByte(0x202A2534L, 3);

            var detector = new GameSessionDetector();
            Assert.Equal(GameSessionState.InGame, detector.Detect(memory, GameSessionState.MainMenu));
        }

        [Fact]
        public void Detect_IntroMode_ReturnsInGame()
        {
            var memory = CreateBootedMemory();
            var writer = new GameMemoryWriter(memory);
            writer.WriteByte(0x202A2534L, 5);

            var detector = new GameSessionDetector();
            Assert.Equal(GameSessionState.InGame, detector.Detect(memory, GameSessionState.MainMenu));
        }

        [Fact]
        public void Detect_FrameCounterJump_ReturnsSaveStateDetected()
        {
            var memory = CreateBootedMemory();
            var writer = new GameMemoryWriter(memory);
            writer.WriteByte(0x202A2534L, 2);
            writer.WriteInt32(0x202A2400L, 100);

            var detector = new GameSessionDetector();
            detector.Detect(memory, GameSessionState.MainMenu);

            writer.WriteInt32(0x202A2400L, 500);
            Assert.Equal(GameSessionState.SaveStateDetected, detector.Detect(memory, GameSessionState.InGame));

            writer.WriteInt32(0x202A2400L, 501);
            Assert.Equal(GameSessionState.InGame, detector.Detect(memory, GameSessionState.SaveStateDetected));
        }

        [Fact]
        public void Detect_EmulatorExited_AfterInGame_ReturnsEmulatorExited()
        {
            var memory = CreateBootedMemory();
            var writer = new GameMemoryWriter(memory);
            writer.WriteByte(0x202A2534L, 2);

            var detector = new GameSessionDetector();
            detector.Detect(memory, GameSessionState.MainMenu);

            // A memory buffer that is too small to hold the boot marker read forces TryRead to fail.
            var empty = new InMemoryGameMemory(BaseAddress, 0x1000);
            Assert.Equal(GameSessionState.EmulatorExited, detector.Detect(empty, GameSessionState.InGame));
        }

        private static InMemoryGameMemory CreateBootedMemory()
        {
            var memory = new InMemoryGameMemory(BaseAddress, Capacity);
            var writer = new GameMemoryWriter(memory, Encoding.ASCII);

            // Boot marker "Dark" in little-endian.
            writer.WriteInt32(0x20299540L, 0x6B726144);
            writer.WriteByte(0x21F10020L, 1);
            writer.WriteByte(0x21F10024L, 0);
            writer.WriteInt32(0x202A2400L, 1);
            writer.WriteByte(0x202A2534L, 0);
            writer.WriteByte(0x202A3420L, 0);

            return memory;
        }
    }
}
