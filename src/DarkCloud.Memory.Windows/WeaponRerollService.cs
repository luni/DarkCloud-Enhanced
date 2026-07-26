using System;
using System.Threading;
using DarkCloud.Core.Threading;
using DarkCloud.Core.Weapons;
using DarkCloudEnhancedMod;

namespace DarkCloud.Memory.Windows
{
    /// <summary>
    /// Shared service that rerolls base-weapon special attributes. This is the
    /// extracted domain logic formerly in <c>DarkCloudEnhancedMod.Weapons.RerollWeaponSpecialAttributes</c>.
    /// </summary>
    internal sealed class WeaponRerollService
    {
        private readonly Random _random = new Random();

        public void Run(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (MainMenuThread.userMode)
                {
                    if (global::DarkCloudEnhancedMod.Memory.ReadByte(Addresses.mode) == 0 || global::DarkCloudEnhancedMod.Memory.ReadByte(Addresses.mode) == 1)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        ThreadingHelper.Sleep(100, cancellationToken);

                        if (global::DarkCloudEnhancedMod.Memory.ReadByte(Addresses.mode) == 0 || global::DarkCloudEnhancedMod.Memory.ReadByte(Addresses.mode) == 1)
                        {
                            Console.WriteLine(GetTimestamp() + "Not ingame anymore! Exited from WeaponRerollEffectsThread!");
                            break;
                        }
                    }
                }

                // Base weapon special effects (Set 1); (ALSO RUNTIME) - 2=Big bucks, 4=poor, 8=quench, 16=thirst, 32=poison, 64=stop, 128=steal
                // Base weapon special effects (Set 2); (ALSO RUNTIME) - 1=fragile, 2=durable, 4=drain, 8=heal, 16=critical, 32=absup

                var statService = new WeaponStatService(new LegacyProcessGameMemory(), new WeaponMemoryLayout());
                var roller = new WeaponSpecialAttributeRoller(() => _random.Next(100));

                /*********************
                 *   Heavens Cloud   *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollHeavensCloud();
                    statService.TryWriteByte(Items.heavenscloud, WeaponCharacter.Toan, Items.dagger, WeaponStat.Effect, values.Effect);
                    statService.TryWriteByte(Items.heavenscloud, WeaponCharacter.Toan, Items.dagger, WeaponStat.Effect2, values.Effect2);
                }

                /**********************
                 *     Dark Cloud     *
                 **********************/

                {
                    WeaponEffectValues values = roller.RollDarkCloud();
                    statService.TryWriteByte(Items.darkcloud, WeaponCharacter.Toan, Items.dagger, WeaponStat.Effect, values.Effect);
                }

                /*********************
                 *      Big Bang     *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollBigBang();
                    statService.TryWriteByte(Items.bigbang, WeaponCharacter.Toan, Items.dagger, WeaponStat.Effect, values.Effect);
                    statService.TryWriteByte(Items.bigbang, WeaponCharacter.Toan, Items.dagger, WeaponStat.Effect2, values.Effect2);
                }

                /************************
                 *   Atlamillia Sword   *
                 ************************/

                {
                    WeaponEffectValues values = roller.RollAtlamilliaSword();
                    statService.TryWriteByte(Items.atlamilliasword, WeaponCharacter.Toan, Items.dagger, WeaponStat.Effect, values.Effect);
                    statService.TryWriteByte(Items.atlamilliasword, WeaponCharacter.Toan, Items.dagger, WeaponStat.Effect2, values.Effect2);
                }

                /*********************
                 *       Dagger      *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollDusack();
                    statService.TryWriteByte(Items.dusack, WeaponCharacter.Toan, Items.dagger, WeaponStat.Effect, values.Effect);
                }

                /**********************
                 *    Goddess Ring    *
                 **********************/

                {
                    WeaponEffectValues values = roller.RollGoddessRing();
                    statService.TryWriteByte(Items.goddessring, WeaponCharacter.Ruby, Items.goldring, WeaponStat.Effect2, values.Effect2);
                }

                /************************
                 *   Destruction Ring   *
                 ************************/

                {
                    WeaponEffectValues values = roller.RollDestructionRing();
                    statService.TryWriteByte(Items.destructionring, WeaponCharacter.Ruby, Items.goldring, WeaponStat.Effect2, values.Effect2);
                }

                /*********************
                 *    Satans Ring    *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollSatansRing();
                    statService.TryWriteByte(Items.satansring, WeaponCharacter.Ruby, Items.goldring, WeaponStat.Effect2, values.Effect2);
                }

                /*********************
                 *       Skunk       *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollSkunk();
                    statService.TryWriteByte(Items.skunk, WeaponCharacter.Osmond, Items.machinegun, WeaponStat.Effect, values.Effect);
                }

                /*********************
                 *      Swallow      *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollSwallow();
                    statService.TryWriteByte(Items.swallow, WeaponCharacter.Osmond, Items.machinegun, WeaponStat.Effect, values.Effect);
                }

                if (cancellationToken.IsCancellationRequested)
                    break;

                ThreadingHelper.Sleep(1000, cancellationToken);
            }
        }

        private static string GetTimestamp()
        {
            return "[" + DateTime.UtcNow.ToString("HH:mm:ss") + "] ";
        }
    }
}
