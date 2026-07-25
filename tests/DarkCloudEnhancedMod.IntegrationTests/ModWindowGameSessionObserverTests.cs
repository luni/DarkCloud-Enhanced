using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    public class ModWindowGameSessionObserverTests
    {
        [Fact]
        public async Task OnStateChanged_NoEmulator_ReportsNoEmulatorsAndResets()
        {
            var sink = new RecordingModStatusSink();
            var clock = new NoDelayClock();
            var observer = new ModWindowGameSessionObserver(sink, clock);
            var context = new GameSessionContext(null, new PassthroughAddressTranslator());

            await observer.OnStateChanged(GameSessionState.None, GameSessionState.NoEmulator, context);

            Assert.Equal("ReportNoEmulators", sink.LastCallName);

            // After a reset, a transition back to MainMenu should report boot again.
            SetupMainMenu(out context);
            await observer.OnStateChanged(GameSessionState.NoEmulator, GameSessionState.MainMenu, context);

            Assert.Equal("ReportMainMenu", sink.LastCallName);
            Assert.Contains(sink.Calls, c => c.Name == nameof(RecordingModStatusSink.ReportBooted));

            await observer.OnShutdown();
        }

        [Fact]
        public async Task OnStateChanged_EmulatorWithoutGame_ResetsBootState()
        {
            SetupMainMenu(out var context);
            var sink = new RecordingModStatusSink();
            var clock = new NoDelayClock();
            var observer = new ModWindowGameSessionObserver(sink, clock);

            await observer.OnStateChanged(GameSessionState.None, GameSessionState.MainMenu, context);
            sink.Calls.Clear();

            await observer.OnStateChanged(GameSessionState.MainMenu, GameSessionState.EmulatorWithoutGame, context);
            await observer.OnStateChanged(GameSessionState.EmulatorWithoutGame, GameSessionState.MainMenu, context);

            // The second MainMenu should report booted again because the state was reset.
            Assert.Contains(sink.Calls, c => c.Name == nameof(RecordingModStatusSink.ReportBooted));

            await observer.OnShutdown();
        }

        [Fact]
        public async Task OnStateChanged_EmulatorExited_DoesNotResetBootState()
        {
            SetupMainMenu(out var context);
            var sink = new RecordingModStatusSink();
            var clock = new NoDelayClock();
            var observer = new ModWindowGameSessionObserver(sink, clock);

            await observer.OnStateChanged(GameSessionState.None, GameSessionState.MainMenu, context);
            sink.Calls.Clear();

            await observer.OnStateChanged(GameSessionState.MainMenu, GameSessionState.EmulatorExited, context);
            await observer.OnStateChanged(GameSessionState.EmulatorExited, GameSessionState.MainMenu, context);

            // Transient read failures should not reset the boot state.
            Assert.DoesNotContain(sink.Calls, c => c.Name == nameof(RecordingModStatusSink.ReportBooted));

            await observer.OnShutdown();
        }

        [Fact]
        public async Task OnStateChanged_InGame_NonEnhancedSave_WritesModeResetAndReports()
        {
            var ram = SnapshotTestHelper.CreateEmptyRam();
            SnapshotTestHelper.UseSnapshot(ram, Region.NTSC);

            var writer = new GameMemoryWriter(new LegacyProcessGameMemory());
            writer.WriteByte(Addresses.mode, 0);
            writer.WriteByte(0x21CE448A, 1);
            writer.WriteByte((long)Addresses.checkFloor + 1, 255);

            var sink = new RecordingModStatusSink();
            var clock = new NoDelayClock();
            var observer = new ModWindowGameSessionObserver(sink, clock);
            var context = new GameSessionContext(new LegacyProcessGameMemory(), new PassthroughAddressTranslator());

            await observer.OnStateChanged(GameSessionState.None, GameSessionState.MainMenu, context);
            sink.Calls.Clear();

            writer.WriteByte(0x21CE448A, 0);
            writer.WriteByte(Addresses.mode, 2);

            await observer.OnStateChanged(GameSessionState.MainMenu, GameSessionState.InGame, context);

            Assert.Equal(nameof(RecordingModStatusSink.ReportNotEnhancedModSaveFile), sink.LastCallName);

            var reader = new GameMemoryReader(new LegacyProcessGameMemory());
            Assert.Equal(1, reader.ReadByte(Addresses.mode));

            await observer.OnShutdown();
        }

        [Fact]
        public async Task OnStateChanged_SaveStateDetected_WritesTownSoftReset()
        {
            var ram = SnapshotTestHelper.CreateEmptyRam();
            SnapshotTestHelper.UseSnapshot(ram, Region.NTSC);

            var writer = new GameMemoryWriter(new LegacyProcessGameMemory());
            writer.WriteByte(Addresses.mode, 0);
            writer.WriteByte(0x21CE448A, 1);
            writer.WriteByte((long)Addresses.checkFloor + 1, 255);

            var sink = new RecordingModStatusSink();
            var clock = new NoDelayClock();
            var observer = new ModWindowGameSessionObserver(sink, clock);
            var context = new GameSessionContext(new LegacyProcessGameMemory(), new PassthroughAddressTranslator());

            await observer.OnStateChanged(GameSessionState.None, GameSessionState.MainMenu, context);
            sink.Calls.Clear();

            writer.WriteByte(Addresses.mode, 2);

            await observer.OnStateChanged(GameSessionState.MainMenu, GameSessionState.SaveStateDetected, context);

            Assert.Equal(nameof(RecordingModStatusSink.ReportSaveStateDetected), sink.LastCallName);

            var reader = new GameMemoryReader(new LegacyProcessGameMemory());
            Assert.Equal(1, reader.ReadByte(Addresses.townSoftReset));

            await observer.OnShutdown();
        }

        [Fact]
        public async Task OnStateChanged_FirstLaunchInGame_WritesTownSoftResetWhenPromptAccepted()
        {
            var ram = SnapshotTestHelper.CreateEmptyRam();
            SnapshotTestHelper.UseSnapshot(ram, Region.NTSC);

            var writer = new GameMemoryWriter(new LegacyProcessGameMemory());
            writer.WriteByte((long)Addresses.checkFloor + 1, 255);

            var sink = new RecordingModStatusSink { PromptForGameResetResult = true };
            var clock = new NoDelayClock();
            var observer = new ModWindowGameSessionObserver(sink, clock);
            var context = new GameSessionContext(new LegacyProcessGameMemory(), new PassthroughAddressTranslator());

            await observer.OnStateChanged(GameSessionState.None, GameSessionState.InGame, context);

            Assert.Equal(nameof(RecordingModStatusSink.PromptForGameReset), sink.LastCallName);

            var reader = new GameMemoryReader(new LegacyProcessGameMemory());
            Assert.Equal(1, reader.ReadByte(Addresses.townSoftReset));

            await observer.OnShutdown();
        }

        [Fact]
        public async Task OnStateChanged_InGame_AfterEmulatorExited_DoesNotPromptForReset()
        {
            var ram = SnapshotTestHelper.CreateEmptyRam();
            SnapshotTestHelper.UseSnapshot(ram, Region.NTSC);

            var writer = new GameMemoryWriter(new LegacyProcessGameMemory());
            writer.WriteByte(Addresses.mode, 0);
            writer.WriteByte(0x21CE448A, 1);
            writer.WriteByte((long)Addresses.checkFloor + 1, 255);

            var sink = new RecordingModStatusSink();
            var clock = new NoDelayClock();
            var observer = new ModWindowGameSessionObserver(sink, clock);
            var context = new GameSessionContext(new LegacyProcessGameMemory(), new PassthroughAddressTranslator());

            // Boot from the main menu, then enter in-game.
            await observer.OnStateChanged(GameSessionState.None, GameSessionState.MainMenu, context);
            sink.Calls.Clear();

            writer.WriteByte(Addresses.mode, 2);
            await observer.OnStateChanged(GameSessionState.MainMenu, GameSessionState.InGame, context);

            // Simulate a transient read failure (EmulatorExited) and recovery.
            await observer.OnStateChanged(GameSessionState.InGame, GameSessionState.EmulatorExited, context);
            await observer.OnStateChanged(GameSessionState.EmulatorExited, GameSessionState.InGame, context);

            Assert.DoesNotContain(sink.Calls, c => c.Name == nameof(RecordingModStatusSink.PromptForGameReset));
            Assert.Equal(nameof(RecordingModStatusSink.ReportInGame), sink.LastCallName);

            var reader = new GameMemoryReader(new LegacyProcessGameMemory());
            Assert.Equal(2, reader.ReadByte(Addresses.mode));

            await observer.OnShutdown();
        }

        private static void SetupMainMenu(out GameSessionContext context)
        {
            var ram = SnapshotTestHelper.CreateEmptyRam();
            SnapshotTestHelper.UseSnapshot(ram, Region.NTSC);
            context = new GameSessionContext(new LegacyProcessGameMemory(), new PassthroughAddressTranslator());
        }

        /// <summary>
        /// Clock implementation that completes delays synchronously so the
        /// integration tests do not have to wait for real time.
        /// </summary>
        private sealed class NoDelayClock : IClock
        {
            public System.DateTimeOffset UtcNow => System.DateTimeOffset.UtcNow;

            public Task Delay(System.TimeSpan delay, CancellationToken cancellationToken)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.FromCanceled(cancellationToken);

                return Task.CompletedTask;
            }
        }
    }
}
