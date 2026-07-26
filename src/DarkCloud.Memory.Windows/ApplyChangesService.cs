using System;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;
using DarkCloud.Core.Weapons;
using DarkCloudEnhancedMod;

namespace DarkCloud.Memory.Windows
{
    /// <summary>
    /// Shared implementation of <see cref="IApplyChangesService"/> that uses the
    /// extracted weapon domain services and the legacy <see cref="Shop"/> price
    /// table. It is safe for both the legacy and modern hosts because it reads
    /// and writes through the static <see cref="Memory"/> backend, which each
    /// host initializes through <see cref="ModWindowGameMemoryProvider"/>.
    /// </summary>
    internal sealed class ApplyChangesService : IApplyChangesService
    {
        // Weapon IDs that match DarkCloudEnhancedMod.Items / DarkCloudEnhancedMod.Weapons.
        private const int DaggerId = 258;
        private const int BaselardId = 259;

        public Task ApplyChangesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var statService = new WeaponStatService(new LegacyProcessGameMemory(), new WeaponMemoryLayout());

            if (statService.TryReadUShort(BaselardId, WeaponCharacter.Toan, DaggerId, WeaponStat.Endurance, out ushort currentEndurance)
                && currentEndurance == 30)
            {
                Console.WriteLine(GetTimestamp() + "New weapon changes have already been applied!");
                return Task.CompletedTask;
            }

            Console.WriteLine(GetTimestamp() + "Applying the new weapon changes...");

            var balanceService = new WeaponBalanceService(statService, WeaponBalanceTable.AllChanges);
            balanceService.ApplyAll();

            cancellationToken.ThrowIfCancellationRequested();

            Shop.UpdateShopPrices();

            Console.WriteLine(GetTimestamp() + "Finished applying new weapon changes!");
            return Task.CompletedTask;
        }

        private static string GetTimestamp()
        {
            return "[" + DateTime.UtcNow.ToString("HH:mm:ss") + "] ";
        }
    }
}
