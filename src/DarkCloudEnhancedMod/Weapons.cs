using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DarkCloud.Core.Weapons;
using DarkCloud.Memory.Windows;

namespace DarkCloudEnhancedMod
{
    public class Weapons
    {
        //Default Weapons ID
        public const int daggerid = Items.dagger;
        public const int woodenid = Items.woodenslingshot;
        public const int malletid = Items.mallet;
        public const int goldringid = Items.goldring;
        public const int stickid = Items.fightingstick;
        public const int machinegunid = Items.machinegun;

        //Base database table Dagger addresses
        public const int synth1 = 0x2027A717;       //Synth slot 1 (0 = None, 1 = Regular gray slot, 2 = Synth blue slot); (ALSO RUNTIME)
        public const int synth2 = 0x2027A718;       //Synth slot 2 (0 = None, 1 = Regular gray slot, 2 = Synth blue slot); (ALSO RUNTIME)
        public const int synth3 = 0x2027A719;       //Synth slot 3 (0 = None, 1 = Regular gray slot, 2 = Synth blue slot); (ALSO RUNTIME)
        public const int synth4 = 0x2027A71A;       //Synth slot 4 (0 = None, 1 = Regular gray slot, 2 = Synth blue slot); (ALSO RUNTIME)
        public const int synth5 = 0x2027A71B;       //Synth slot 5 (0 = None, 1 = Regular gray slot, 2 = Synth blue slot); (ALSO RUNTIME)
        public const int synth6 = 0x2027A71C;       //Synth slot 6 (0 = None, 1 = Regular gray slot, 2 = Synth blue slot); (ALSO RUNTIME)
        public const int ownership = 0x2027A716;    //0 = Toan, 1 = Xiao, 2 = Goro, 3 = Ruby, 4 = Ungaga, 5 = Osmond;
        public const int whp = 0x2027A70C;          //Base weapon health points;
        public const int abs = 0x2027A73C;          //Base weapon absorption points; (ALSO RUNTIME)
        public const int absadd = 0x2027A73E;       //How much abs to be added per weapon level; (ALSO RUNTIME)
        public const int attack = 0x2027A70E;       //Base weapon Attack stat;
        public const int maxattack = 0x2027A750;    //Base weapon Max Attack stat; (ALSO RUNTIME)
        public const int endurance = 0x2027A710;    //Base weapon Endurance stat;
        public const int speed = 0x2027A712;        //Base weapon Speed stat;
        public const int magic = 0x2027A714;        //Base weapon Magic stat;
        public const int maxmagic = 0x2027A752;     //Base weapon Max Magic stat; (ALSO RUNTIME)
        public const int fire = 0x2027A71E;         //Base weapon Fire stat;
        public const int ice = 0x2027A720;          //Base weapon Ice stat;
        public const int thunder = 0x2027A722;      //Base weapon Thunder stat;
        public const int wind = 0x2027A724;         //Base weapon Wind stat;
        public const int holy = 0x2027A726;         //Base weapon Holy stat;
        public const int dinoslayer = 0x2027A728;   //Base weapon Dino Slayer stat;
        public const int undead = 0x2027A72A;       //Base weapon Undead Buster stat;
        public const int sea = 0x2027A72C;          //Base weapon Sea Killer stat;
        public const int stone = 0x2027A72E;        //Base weapon Stone Breaker stat;
        public const int plant = 0x2027A730;        //Base weapon Plant Buster stat;
        public const int beast = 0x2027A732;        //Base weapon Beast Buster stat;
        public const int sky = 0x2027A734;          //Base weapon Sky Hunter stat;
        public const int metal = 0x2027A736;        //Base weapon Metal Breaker stat;
        public const int mimic = 0x2027A738;        //Base weapon Mimic Breaker stat;
        public const int mage = 0x2027A73A;         //Base weapon Mage Slayer stat;
        public const int effect = 0x2027A744;       //Base weapon special effects (Set 1); (ALSO RUNTIME) - 2=Big bucks, 4=poor, 8=quench, 16=thirst, 32=poison, 64=stop, 128=steal
        public const int effect2 = 0x2027A745;      //Base weapon special effects (Set 2); (ALSO RUNTIME) - 1=fragile, 2=durable, 4=drain, 8=heal, 16=critical, 32=absup
        public const int buildup = 0x2027A748;      //Base weapon build-up branches;

        //Offset between each weapon
        public const int weaponoffset = 0x4C;

        //Character offsets
        public const int xiaooffset = 0xC78;    //Xiao
        public const int gorooffset = 0x10EC;   //Goro
        public const int rubyoffset = 0x15F8;   //Ruby
        public const int ungagaoffset = 0x1AB8; //Ungaga
        public const int osmondoffset = 0x1F78; //Osmond

        //Lamb sword buff
        public const int lambTransformThreshold = 0x202A1818;
        public const int lambStatsThreshold = 0x202A188C;

        public static Thread weaponsMenuListener = null;

        static Random rnd = new Random();

        private static void HandleSynthSphere(WeaponSynthSphereService service, int level, int attack, int endurance, int speed, int magic, int slot1ItemId, int slot1SynthesisedItemId, int hasChangedBySynth, int weaponFormerStatsValue)
        {
            service.TryApplyBoost(new WeaponSlotAddresses(level, attack, endurance, speed, magic, slot1ItemId, slot1SynthesisedItemId, hasChangedBySynth, weaponFormerStatsValue));
        }

        /// <summary>
        /// Adds a listener to the customize weapon menu to check for custom synthspheres and apply its effects if used
        /// </summary>
        public static void WeaponListenForSynthSphere()
        {
            WeaponListenForSynthSphere(CancellationToken.None);
        }

        public static void WeaponListenForSynthSphere(CancellationToken cancellationToken)
        {
            var service = new WeaponSynthSphereService(new LegacyProcessGameMemory());

            while (!cancellationToken.IsCancellationRequested && Player.CheckIsWeaponCustomizeMenu())
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                int character = Memory.ReadByte(Addresses.weaponMenuCurrentCharacterHover);
                int weapon = Memory.ReadByte(Addresses.weaponMenuCurrentWeaponHover);

                switch (character)
                {
                    case 0:
                        switch (weapon)
                        {
                            case 0:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot0.level, Player.Toan.WeaponSlot0.attack, Player.Toan.WeaponSlot0.endurance, Player.Toan.WeaponSlot0.speed, Player.Toan.WeaponSlot0.magic, Player.Toan.WeaponSlot0.slot1_itemId, Player.Toan.WeaponSlot0.slot1_synthesisedItemId, Player.Toan.WeaponSlot0.hasChangedBySynth, Player.Toan.WeaponSlot0.weaponFormerStatsValue);
                                break;

                            case 1:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot1.level, Player.Toan.WeaponSlot1.attack, Player.Toan.WeaponSlot1.endurance, Player.Toan.WeaponSlot1.speed, Player.Toan.WeaponSlot1.magic, Player.Toan.WeaponSlot1.slot1_itemId, Player.Toan.WeaponSlot1.slot1_synthesisedItemId, Player.Toan.WeaponSlot1.hasChangedBySynth, Player.Toan.WeaponSlot1.weaponFormerStatsValue);
                                break;

                            case 2:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot2.level, Player.Toan.WeaponSlot2.attack, Player.Toan.WeaponSlot2.endurance, Player.Toan.WeaponSlot2.speed, Player.Toan.WeaponSlot2.magic, Player.Toan.WeaponSlot2.slot1_itemId, Player.Toan.WeaponSlot2.slot1_synthesisedItemId, Player.Toan.WeaponSlot2.hasChangedBySynth, Player.Toan.WeaponSlot2.weaponFormerStatsValue);
                                break;

                            case 3:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot3.level, Player.Toan.WeaponSlot3.attack, Player.Toan.WeaponSlot3.endurance, Player.Toan.WeaponSlot3.speed, Player.Toan.WeaponSlot3.magic, Player.Toan.WeaponSlot3.slot1_itemId, Player.Toan.WeaponSlot3.slot1_synthesisedItemId, Player.Toan.WeaponSlot3.hasChangedBySynth, Player.Toan.WeaponSlot3.weaponFormerStatsValue);
                                break;

                            case 4:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot4.level, Player.Toan.WeaponSlot4.attack, Player.Toan.WeaponSlot4.endurance, Player.Toan.WeaponSlot4.speed, Player.Toan.WeaponSlot4.magic, Player.Toan.WeaponSlot4.slot1_itemId, Player.Toan.WeaponSlot4.slot1_synthesisedItemId, Player.Toan.WeaponSlot4.hasChangedBySynth, Player.Toan.WeaponSlot4.weaponFormerStatsValue);
                                break;

                            case 5:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot5.level, Player.Toan.WeaponSlot5.attack, Player.Toan.WeaponSlot5.endurance, Player.Toan.WeaponSlot5.speed, Player.Toan.WeaponSlot5.magic, Player.Toan.WeaponSlot5.slot1_itemId, Player.Toan.WeaponSlot5.slot1_synthesisedItemId, Player.Toan.WeaponSlot5.hasChangedBySynth, Player.Toan.WeaponSlot5.weaponFormerStatsValue);
                                break;

                            case 6:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot6.level, Player.Toan.WeaponSlot6.attack, Player.Toan.WeaponSlot6.endurance, Player.Toan.WeaponSlot6.speed, Player.Toan.WeaponSlot6.magic, Player.Toan.WeaponSlot6.slot1_itemId, Player.Toan.WeaponSlot6.slot1_synthesisedItemId, Player.Toan.WeaponSlot6.hasChangedBySynth, Player.Toan.WeaponSlot6.weaponFormerStatsValue);
                                break;

                            case 7:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot7.level, Player.Toan.WeaponSlot7.attack, Player.Toan.WeaponSlot7.endurance, Player.Toan.WeaponSlot7.speed, Player.Toan.WeaponSlot7.magic, Player.Toan.WeaponSlot7.slot1_itemId, Player.Toan.WeaponSlot7.slot1_synthesisedItemId, Player.Toan.WeaponSlot7.hasChangedBySynth, Player.Toan.WeaponSlot7.weaponFormerStatsValue);
                                break;

                            case 8:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot8.level, Player.Toan.WeaponSlot8.attack, Player.Toan.WeaponSlot8.endurance, Player.Toan.WeaponSlot8.speed, Player.Toan.WeaponSlot8.magic, Player.Toan.WeaponSlot8.slot1_itemId, Player.Toan.WeaponSlot8.slot1_synthesisedItemId, Player.Toan.WeaponSlot8.hasChangedBySynth, Player.Toan.WeaponSlot8.weaponFormerStatsValue);
                                break;

                            case 9:

                                HandleSynthSphere(service, Player.Toan.WeaponSlot9.level, Player.Toan.WeaponSlot9.attack, Player.Toan.WeaponSlot9.endurance, Player.Toan.WeaponSlot9.speed, Player.Toan.WeaponSlot9.magic, Player.Toan.WeaponSlot9.slot1_itemId, Player.Toan.WeaponSlot9.slot1_synthesisedItemId, Player.Toan.WeaponSlot9.hasChangedBySynth, Player.Toan.WeaponSlot9.weaponFormerStatsValue);
                                break;
                        }
                        break;

                    case 1:
                        switch (weapon)
                        {
                            case 0:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot0.level, Player.Xiao.WeaponSlot0.attack, Player.Xiao.WeaponSlot0.endurance, Player.Xiao.WeaponSlot0.speed, Player.Xiao.WeaponSlot0.magic, Player.Xiao.WeaponSlot0.slot1_itemId, Player.Xiao.WeaponSlot0.slot1_synthesisedItemId, Player.Xiao.WeaponSlot0.hasChangedBySynth, Player.Xiao.WeaponSlot0.weaponFormerStatsValue);
                                break;

                            case 1:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot1.level, Player.Xiao.WeaponSlot1.attack, Player.Xiao.WeaponSlot1.endurance, Player.Xiao.WeaponSlot1.speed, Player.Xiao.WeaponSlot1.magic, Player.Xiao.WeaponSlot1.slot1_itemId, Player.Xiao.WeaponSlot1.slot1_synthesisedItemId, Player.Xiao.WeaponSlot1.hasChangedBySynth, Player.Xiao.WeaponSlot1.weaponFormerStatsValue);
                                break;

                            case 2:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot2.level, Player.Xiao.WeaponSlot2.attack, Player.Xiao.WeaponSlot2.endurance, Player.Xiao.WeaponSlot2.speed, Player.Xiao.WeaponSlot2.magic, Player.Xiao.WeaponSlot2.slot1_itemId, Player.Xiao.WeaponSlot2.slot1_synthesisedItemId, Player.Xiao.WeaponSlot2.hasChangedBySynth, Player.Xiao.WeaponSlot2.weaponFormerStatsValue);
                                break;

                            case 3:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot3.level, Player.Xiao.WeaponSlot3.attack, Player.Xiao.WeaponSlot3.endurance, Player.Xiao.WeaponSlot3.speed, Player.Xiao.WeaponSlot3.magic, Player.Xiao.WeaponSlot3.slot1_itemId, Player.Xiao.WeaponSlot3.slot1_synthesisedItemId, Player.Xiao.WeaponSlot3.hasChangedBySynth, Player.Xiao.WeaponSlot3.weaponFormerStatsValue);
                                break;

                            case 4:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot4.level, Player.Xiao.WeaponSlot4.attack, Player.Xiao.WeaponSlot4.endurance, Player.Xiao.WeaponSlot4.speed, Player.Xiao.WeaponSlot4.magic, Player.Xiao.WeaponSlot4.slot1_itemId, Player.Xiao.WeaponSlot4.slot1_synthesisedItemId, Player.Xiao.WeaponSlot4.hasChangedBySynth, Player.Xiao.WeaponSlot4.weaponFormerStatsValue);
                                break;

                            case 5:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot5.level, Player.Xiao.WeaponSlot5.attack, Player.Xiao.WeaponSlot5.endurance, Player.Xiao.WeaponSlot5.speed, Player.Xiao.WeaponSlot5.magic, Player.Xiao.WeaponSlot5.slot1_itemId, Player.Xiao.WeaponSlot5.slot1_synthesisedItemId, Player.Xiao.WeaponSlot5.hasChangedBySynth, Player.Xiao.WeaponSlot5.weaponFormerStatsValue);
                                break;

                            case 6:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot6.level, Player.Xiao.WeaponSlot6.attack, Player.Xiao.WeaponSlot6.endurance, Player.Xiao.WeaponSlot6.speed, Player.Xiao.WeaponSlot6.magic, Player.Xiao.WeaponSlot6.slot1_itemId, Player.Xiao.WeaponSlot6.slot1_synthesisedItemId, Player.Xiao.WeaponSlot6.hasChangedBySynth, Player.Xiao.WeaponSlot6.weaponFormerStatsValue);
                                break;

                            case 7:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot7.level, Player.Xiao.WeaponSlot7.attack, Player.Xiao.WeaponSlot7.endurance, Player.Xiao.WeaponSlot7.speed, Player.Xiao.WeaponSlot7.magic, Player.Xiao.WeaponSlot7.slot1_itemId, Player.Xiao.WeaponSlot7.slot1_synthesisedItemId, Player.Xiao.WeaponSlot7.hasChangedBySynth, Player.Xiao.WeaponSlot7.weaponFormerStatsValue);
                                break;

                            case 8:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot8.level, Player.Xiao.WeaponSlot8.attack, Player.Xiao.WeaponSlot8.endurance, Player.Xiao.WeaponSlot8.speed, Player.Xiao.WeaponSlot8.magic, Player.Xiao.WeaponSlot8.slot1_itemId, Player.Xiao.WeaponSlot8.slot1_synthesisedItemId, Player.Xiao.WeaponSlot8.hasChangedBySynth, Player.Xiao.WeaponSlot8.weaponFormerStatsValue);
                                break;

                            case 9:

                                HandleSynthSphere(service, Player.Xiao.WeaponSlot9.level, Player.Xiao.WeaponSlot9.attack, Player.Xiao.WeaponSlot9.endurance, Player.Xiao.WeaponSlot9.speed, Player.Xiao.WeaponSlot9.magic, Player.Xiao.WeaponSlot9.slot1_itemId, Player.Xiao.WeaponSlot9.slot1_synthesisedItemId, Player.Xiao.WeaponSlot9.hasChangedBySynth, Player.Xiao.WeaponSlot9.weaponFormerStatsValue);
                                break;
                        }
                        break;

                    case 2:
                        switch (weapon)
                        {
                            case 0:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot0.level, Player.Goro.WeaponSlot0.attack, Player.Goro.WeaponSlot0.endurance, Player.Goro.WeaponSlot0.speed, Player.Goro.WeaponSlot0.magic, Player.Goro.WeaponSlot0.slot1_itemId, Player.Goro.WeaponSlot0.slot1_synthesisedItemId, Player.Goro.WeaponSlot0.hasChangedBySynth, Player.Goro.WeaponSlot0.weaponFormerStatsValue);
                                break;

                            case 1:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot1.level, Player.Goro.WeaponSlot1.attack, Player.Goro.WeaponSlot1.endurance, Player.Goro.WeaponSlot1.speed, Player.Goro.WeaponSlot1.magic, Player.Goro.WeaponSlot1.slot1_itemId, Player.Goro.WeaponSlot1.slot1_synthesisedItemId, Player.Goro.WeaponSlot1.hasChangedBySynth, Player.Goro.WeaponSlot1.weaponFormerStatsValue);
                                break;

                            case 2:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot2.level, Player.Goro.WeaponSlot2.attack, Player.Goro.WeaponSlot2.endurance, Player.Goro.WeaponSlot2.speed, Player.Goro.WeaponSlot2.magic, Player.Goro.WeaponSlot2.slot1_itemId, Player.Goro.WeaponSlot2.slot1_synthesisedItemId, Player.Goro.WeaponSlot2.hasChangedBySynth, Player.Goro.WeaponSlot2.weaponFormerStatsValue);
                                break;

                            case 3:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot3.level, Player.Goro.WeaponSlot3.attack, Player.Goro.WeaponSlot3.endurance, Player.Goro.WeaponSlot3.speed, Player.Goro.WeaponSlot3.magic, Player.Goro.WeaponSlot3.slot1_itemId, Player.Goro.WeaponSlot3.slot1_synthesisedItemId, Player.Goro.WeaponSlot3.hasChangedBySynth, Player.Goro.WeaponSlot3.weaponFormerStatsValue);
                                break;

                            case 4:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot4.level, Player.Goro.WeaponSlot4.attack, Player.Goro.WeaponSlot4.endurance, Player.Goro.WeaponSlot4.speed, Player.Goro.WeaponSlot4.magic, Player.Goro.WeaponSlot4.slot1_itemId, Player.Goro.WeaponSlot4.slot1_synthesisedItemId, Player.Goro.WeaponSlot4.hasChangedBySynth, Player.Goro.WeaponSlot4.weaponFormerStatsValue);
                                break;

                            case 5:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot5.level, Player.Goro.WeaponSlot5.attack, Player.Goro.WeaponSlot5.endurance, Player.Goro.WeaponSlot5.speed, Player.Goro.WeaponSlot5.magic, Player.Goro.WeaponSlot5.slot1_itemId, Player.Goro.WeaponSlot5.slot1_synthesisedItemId, Player.Goro.WeaponSlot5.hasChangedBySynth, Player.Goro.WeaponSlot5.weaponFormerStatsValue);
                                break;

                            case 6:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot6.level, Player.Goro.WeaponSlot6.attack, Player.Goro.WeaponSlot6.endurance, Player.Goro.WeaponSlot6.speed, Player.Goro.WeaponSlot6.magic, Player.Goro.WeaponSlot6.slot1_itemId, Player.Goro.WeaponSlot6.slot1_synthesisedItemId, Player.Goro.WeaponSlot6.hasChangedBySynth, Player.Goro.WeaponSlot6.weaponFormerStatsValue);
                                break;

                            case 7:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot7.level, Player.Goro.WeaponSlot7.attack, Player.Goro.WeaponSlot7.endurance, Player.Goro.WeaponSlot7.speed, Player.Goro.WeaponSlot7.magic, Player.Goro.WeaponSlot7.slot1_itemId, Player.Goro.WeaponSlot7.slot1_synthesisedItemId, Player.Goro.WeaponSlot7.hasChangedBySynth, Player.Goro.WeaponSlot7.weaponFormerStatsValue);
                                break;

                            case 8:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot8.level, Player.Goro.WeaponSlot8.attack, Player.Goro.WeaponSlot8.endurance, Player.Goro.WeaponSlot8.speed, Player.Goro.WeaponSlot8.magic, Player.Goro.WeaponSlot8.slot1_itemId, Player.Goro.WeaponSlot8.slot1_synthesisedItemId, Player.Goro.WeaponSlot8.hasChangedBySynth, Player.Goro.WeaponSlot8.weaponFormerStatsValue);
                                break;

                            case 9:

                                HandleSynthSphere(service, Player.Goro.WeaponSlot9.level, Player.Goro.WeaponSlot9.attack, Player.Goro.WeaponSlot9.endurance, Player.Goro.WeaponSlot9.speed, Player.Goro.WeaponSlot9.magic, Player.Goro.WeaponSlot9.slot1_itemId, Player.Goro.WeaponSlot9.slot1_synthesisedItemId, Player.Goro.WeaponSlot9.hasChangedBySynth, Player.Goro.WeaponSlot9.weaponFormerStatsValue);
                                break;
                        }
                        break;

                    case 3:
                        switch (weapon)
                        {
                            case 0:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot0.level, Player.Ruby.WeaponSlot0.attack, Player.Ruby.WeaponSlot0.endurance, Player.Ruby.WeaponSlot0.speed, Player.Ruby.WeaponSlot0.magic, Player.Ruby.WeaponSlot0.slot1_itemId, Player.Ruby.WeaponSlot0.slot1_synthesisedItemId, Player.Ruby.WeaponSlot0.hasChangedBySynth, Player.Ruby.WeaponSlot0.weaponFormerStatsValue);
                                break;

                            case 1:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot1.level, Player.Ruby.WeaponSlot1.attack, Player.Ruby.WeaponSlot1.endurance, Player.Ruby.WeaponSlot1.speed, Player.Ruby.WeaponSlot1.magic, Player.Ruby.WeaponSlot1.slot1_itemId, Player.Ruby.WeaponSlot1.slot1_synthesisedItemId, Player.Ruby.WeaponSlot1.hasChangedBySynth, Player.Ruby.WeaponSlot1.weaponFormerStatsValue);
                                break;

                            case 2:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot2.level, Player.Ruby.WeaponSlot2.attack, Player.Ruby.WeaponSlot2.endurance, Player.Ruby.WeaponSlot2.speed, Player.Ruby.WeaponSlot2.magic, Player.Ruby.WeaponSlot2.slot1_itemId, Player.Ruby.WeaponSlot2.slot1_synthesisedItemId, Player.Ruby.WeaponSlot2.hasChangedBySynth, Player.Ruby.WeaponSlot2.weaponFormerStatsValue);
                                break;

                            case 3:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot3.level, Player.Ruby.WeaponSlot3.attack, Player.Ruby.WeaponSlot3.endurance, Player.Ruby.WeaponSlot3.speed, Player.Ruby.WeaponSlot3.magic, Player.Ruby.WeaponSlot3.slot1_itemId, Player.Ruby.WeaponSlot3.slot1_synthesisedItemId, Player.Ruby.WeaponSlot3.hasChangedBySynth, Player.Ruby.WeaponSlot3.weaponFormerStatsValue);
                                break;

                            case 4:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot4.level, Player.Ruby.WeaponSlot4.attack, Player.Ruby.WeaponSlot4.endurance, Player.Ruby.WeaponSlot4.speed, Player.Ruby.WeaponSlot4.magic, Player.Ruby.WeaponSlot4.slot1_itemId, Player.Ruby.WeaponSlot4.slot1_synthesisedItemId, Player.Ruby.WeaponSlot4.hasChangedBySynth, Player.Ruby.WeaponSlot4.weaponFormerStatsValue);
                                break;

                            case 5:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot5.level, Player.Ruby.WeaponSlot5.attack, Player.Ruby.WeaponSlot5.endurance, Player.Ruby.WeaponSlot5.speed, Player.Ruby.WeaponSlot5.magic, Player.Ruby.WeaponSlot5.slot1_itemId, Player.Ruby.WeaponSlot5.slot1_synthesisedItemId, Player.Ruby.WeaponSlot5.hasChangedBySynth, Player.Ruby.WeaponSlot5.weaponFormerStatsValue);
                                break;

                            case 6:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot6.level, Player.Ruby.WeaponSlot6.attack, Player.Ruby.WeaponSlot6.endurance, Player.Ruby.WeaponSlot6.speed, Player.Ruby.WeaponSlot6.magic, Player.Ruby.WeaponSlot6.slot1_itemId, Player.Ruby.WeaponSlot6.slot1_synthesisedItemId, Player.Ruby.WeaponSlot6.hasChangedBySynth, Player.Ruby.WeaponSlot6.weaponFormerStatsValue);
                                break;

                            case 7:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot7.level, Player.Ruby.WeaponSlot7.attack, Player.Ruby.WeaponSlot7.endurance, Player.Ruby.WeaponSlot7.speed, Player.Ruby.WeaponSlot7.magic, Player.Ruby.WeaponSlot7.slot1_itemId, Player.Ruby.WeaponSlot7.slot1_synthesisedItemId, Player.Ruby.WeaponSlot7.hasChangedBySynth, Player.Ruby.WeaponSlot7.weaponFormerStatsValue);
                                break;

                            case 8:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot8.level, Player.Ruby.WeaponSlot8.attack, Player.Ruby.WeaponSlot8.endurance, Player.Ruby.WeaponSlot8.speed, Player.Ruby.WeaponSlot8.magic, Player.Ruby.WeaponSlot8.slot1_itemId, Player.Ruby.WeaponSlot8.slot1_synthesisedItemId, Player.Ruby.WeaponSlot8.hasChangedBySynth, Player.Ruby.WeaponSlot8.weaponFormerStatsValue);
                                break;

                            case 9:

                                HandleSynthSphere(service, Player.Ruby.WeaponSlot9.level, Player.Ruby.WeaponSlot9.attack, Player.Ruby.WeaponSlot9.endurance, Player.Ruby.WeaponSlot9.speed, Player.Ruby.WeaponSlot9.magic, Player.Ruby.WeaponSlot9.slot1_itemId, Player.Ruby.WeaponSlot9.slot1_synthesisedItemId, Player.Ruby.WeaponSlot9.hasChangedBySynth, Player.Ruby.WeaponSlot9.weaponFormerStatsValue);
                                break;
                        }
                        break;

                    case 4:
                        switch (weapon)
                        {
                            case 0:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot0.level, Player.Ungaga.WeaponSlot0.attack, Player.Ungaga.WeaponSlot0.endurance, Player.Ungaga.WeaponSlot0.speed, Player.Ungaga.WeaponSlot0.magic, Player.Ungaga.WeaponSlot0.slot1_itemId, Player.Ungaga.WeaponSlot0.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot0.hasChangedBySynth, Player.Ungaga.WeaponSlot0.weaponFormerStatsValue);
                                break;

                            case 1:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot1.level, Player.Ungaga.WeaponSlot1.attack, Player.Ungaga.WeaponSlot1.endurance, Player.Ungaga.WeaponSlot1.speed, Player.Ungaga.WeaponSlot1.magic, Player.Ungaga.WeaponSlot1.slot1_itemId, Player.Ungaga.WeaponSlot1.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot1.hasChangedBySynth, Player.Ungaga.WeaponSlot1.weaponFormerStatsValue);
                                break;

                            case 2:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot2.level, Player.Ungaga.WeaponSlot2.attack, Player.Ungaga.WeaponSlot2.endurance, Player.Ungaga.WeaponSlot2.speed, Player.Ungaga.WeaponSlot2.magic, Player.Ungaga.WeaponSlot2.slot1_itemId, Player.Ungaga.WeaponSlot2.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot2.hasChangedBySynth, Player.Ungaga.WeaponSlot2.weaponFormerStatsValue);
                                break;

                            case 3:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot3.level, Player.Ungaga.WeaponSlot3.attack, Player.Ungaga.WeaponSlot3.endurance, Player.Ungaga.WeaponSlot3.speed, Player.Ungaga.WeaponSlot3.magic, Player.Ungaga.WeaponSlot3.slot1_itemId, Player.Ungaga.WeaponSlot3.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot3.hasChangedBySynth, Player.Ungaga.WeaponSlot3.weaponFormerStatsValue);
                                break;

                            case 4:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot4.level, Player.Ungaga.WeaponSlot4.attack, Player.Ungaga.WeaponSlot4.endurance, Player.Ungaga.WeaponSlot4.speed, Player.Ungaga.WeaponSlot4.magic, Player.Ungaga.WeaponSlot4.slot1_itemId, Player.Ungaga.WeaponSlot4.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot4.hasChangedBySynth, Player.Ungaga.WeaponSlot4.weaponFormerStatsValue);
                                break;

                            case 5:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot5.level, Player.Ungaga.WeaponSlot5.attack, Player.Ungaga.WeaponSlot5.endurance, Player.Ungaga.WeaponSlot5.speed, Player.Ungaga.WeaponSlot5.magic, Player.Ungaga.WeaponSlot5.slot1_itemId, Player.Ungaga.WeaponSlot5.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot5.hasChangedBySynth, Player.Ungaga.WeaponSlot5.weaponFormerStatsValue);
                                break;

                            case 6:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot6.level, Player.Ungaga.WeaponSlot6.attack, Player.Ungaga.WeaponSlot6.endurance, Player.Ungaga.WeaponSlot6.speed, Player.Ungaga.WeaponSlot6.magic, Player.Ungaga.WeaponSlot6.slot1_itemId, Player.Ungaga.WeaponSlot6.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot6.hasChangedBySynth, Player.Ungaga.WeaponSlot6.weaponFormerStatsValue);
                                break;

                            case 7:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot7.level, Player.Ungaga.WeaponSlot7.attack, Player.Ungaga.WeaponSlot7.endurance, Player.Ungaga.WeaponSlot7.speed, Player.Ungaga.WeaponSlot7.magic, Player.Ungaga.WeaponSlot7.slot1_itemId, Player.Ungaga.WeaponSlot7.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot7.hasChangedBySynth, Player.Ungaga.WeaponSlot7.weaponFormerStatsValue);
                                break;

                            case 8:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot8.level, Player.Ungaga.WeaponSlot8.attack, Player.Ungaga.WeaponSlot8.endurance, Player.Ungaga.WeaponSlot8.speed, Player.Ungaga.WeaponSlot8.magic, Player.Ungaga.WeaponSlot8.slot1_itemId, Player.Ungaga.WeaponSlot8.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot8.hasChangedBySynth, Player.Ungaga.WeaponSlot8.weaponFormerStatsValue);
                                break;

                            case 9:

                                HandleSynthSphere(service, Player.Ungaga.WeaponSlot9.level, Player.Ungaga.WeaponSlot9.attack, Player.Ungaga.WeaponSlot9.endurance, Player.Ungaga.WeaponSlot9.speed, Player.Ungaga.WeaponSlot9.magic, Player.Ungaga.WeaponSlot9.slot1_itemId, Player.Ungaga.WeaponSlot9.slot1_synthesisedItemId, Player.Ungaga.WeaponSlot9.hasChangedBySynth, Player.Ungaga.WeaponSlot9.weaponFormerStatsValue);
                                break;
                        }
                        break;

                    case 5:
                        switch (weapon)
                        {
                            case 0:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot0.level, Player.Osmond.WeaponSlot0.attack, Player.Osmond.WeaponSlot0.endurance, Player.Osmond.WeaponSlot0.speed, Player.Osmond.WeaponSlot0.magic, Player.Osmond.WeaponSlot0.slot1_itemId, Player.Osmond.WeaponSlot0.slot1_synthesisedItemId, Player.Osmond.WeaponSlot0.hasChangedBySynth, Player.Osmond.WeaponSlot0.weaponFormerStatsValue);
                                break;

                            case 1:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot1.level, Player.Osmond.WeaponSlot1.attack, Player.Osmond.WeaponSlot1.endurance, Player.Osmond.WeaponSlot1.speed, Player.Osmond.WeaponSlot1.magic, Player.Osmond.WeaponSlot1.slot1_itemId, Player.Osmond.WeaponSlot1.slot1_synthesisedItemId, Player.Osmond.WeaponSlot1.hasChangedBySynth, Player.Osmond.WeaponSlot1.weaponFormerStatsValue);
                                break;

                            case 2:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot2.level, Player.Osmond.WeaponSlot2.attack, Player.Osmond.WeaponSlot2.endurance, Player.Osmond.WeaponSlot2.speed, Player.Osmond.WeaponSlot2.magic, Player.Osmond.WeaponSlot2.slot1_itemId, Player.Osmond.WeaponSlot2.slot1_synthesisedItemId, Player.Osmond.WeaponSlot2.hasChangedBySynth, Player.Osmond.WeaponSlot2.weaponFormerStatsValue);
                                break;

                            case 3:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot3.level, Player.Osmond.WeaponSlot3.attack, Player.Osmond.WeaponSlot3.endurance, Player.Osmond.WeaponSlot3.speed, Player.Osmond.WeaponSlot3.magic, Player.Osmond.WeaponSlot3.slot1_itemId, Player.Osmond.WeaponSlot3.slot1_synthesisedItemId, Player.Osmond.WeaponSlot3.hasChangedBySynth, Player.Osmond.WeaponSlot3.weaponFormerStatsValue);
                                break;

                            case 4:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot4.level, Player.Osmond.WeaponSlot4.attack, Player.Osmond.WeaponSlot4.endurance, Player.Osmond.WeaponSlot4.speed, Player.Osmond.WeaponSlot4.magic, Player.Osmond.WeaponSlot4.slot1_itemId, Player.Osmond.WeaponSlot4.slot1_synthesisedItemId, Player.Osmond.WeaponSlot4.hasChangedBySynth, Player.Osmond.WeaponSlot4.weaponFormerStatsValue);
                                break;

                            case 5:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot5.level, Player.Osmond.WeaponSlot5.attack, Player.Osmond.WeaponSlot5.endurance, Player.Osmond.WeaponSlot5.speed, Player.Osmond.WeaponSlot5.magic, Player.Osmond.WeaponSlot5.slot1_itemId, Player.Osmond.WeaponSlot5.slot1_synthesisedItemId, Player.Osmond.WeaponSlot5.hasChangedBySynth, Player.Osmond.WeaponSlot5.weaponFormerStatsValue);
                                break;

                            case 6:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot6.level, Player.Osmond.WeaponSlot6.attack, Player.Osmond.WeaponSlot6.endurance, Player.Osmond.WeaponSlot6.speed, Player.Osmond.WeaponSlot6.magic, Player.Osmond.WeaponSlot6.slot1_itemId, Player.Osmond.WeaponSlot6.slot1_synthesisedItemId, Player.Osmond.WeaponSlot6.hasChangedBySynth, Player.Osmond.WeaponSlot6.weaponFormerStatsValue);
                                break;

                            case 7:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot7.level, Player.Osmond.WeaponSlot7.attack, Player.Osmond.WeaponSlot7.endurance, Player.Osmond.WeaponSlot7.speed, Player.Osmond.WeaponSlot7.magic, Player.Osmond.WeaponSlot7.slot1_itemId, Player.Osmond.WeaponSlot7.slot1_synthesisedItemId, Player.Osmond.WeaponSlot7.hasChangedBySynth, Player.Osmond.WeaponSlot7.weaponFormerStatsValue);
                                break;

                            case 8:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot8.level, Player.Osmond.WeaponSlot8.attack, Player.Osmond.WeaponSlot8.endurance, Player.Osmond.WeaponSlot8.speed, Player.Osmond.WeaponSlot8.magic, Player.Osmond.WeaponSlot8.slot1_itemId, Player.Osmond.WeaponSlot8.slot1_synthesisedItemId, Player.Osmond.WeaponSlot8.hasChangedBySynth, Player.Osmond.WeaponSlot8.weaponFormerStatsValue);
                                break;

                            case 9:

                                HandleSynthSphere(service, Player.Osmond.WeaponSlot9.level, Player.Osmond.WeaponSlot9.attack, Player.Osmond.WeaponSlot9.endurance, Player.Osmond.WeaponSlot9.speed, Player.Osmond.WeaponSlot9.magic, Player.Osmond.WeaponSlot9.slot1_itemId, Player.Osmond.WeaponSlot9.slot1_synthesisedItemId, Player.Osmond.WeaponSlot9.hasChangedBySynth, Player.Osmond.WeaponSlot9.weaponFormerStatsValue);
                                break;
                        }
                        break;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                ThreadingHelper.Sleep(64, cancellationToken);
            }
        }

        /// <summary>
        /// Applies all the weapon changes to their base values (This runs once when starting the mod)
        /// </summary>
        public static void WeaponsBalanceChanges()
        {
            var statService = new WeaponStatService(new LegacyProcessGameMemory(), new WeaponMemoryLayout());
            if (statService.TryReadUShort(Items.baselard, WeaponCharacter.Toan, daggerid, WeaponStat.Endurance, out ushort currentEndurance)
                && currentEndurance == 30)
            {
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "New weapon changes have already been applied!");
                return;
            }

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Applying the new weapon changes...");

            var balanceService = new WeaponBalanceService(statService, WeaponBalanceTable.AllChanges);
            balanceService.ApplyAll();

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Finished applying new weapon changes!");
        }

        /// <summary>
        /// Process to roll the new weapon special attributes on weapons that now may have them
        /// </summary>
        public static void RerollWeaponSpecialAttributes()
        {
            RerollWeaponSpecialAttributes(CancellationToken.None);
        }

        public static void RerollWeaponSpecialAttributes(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (MainMenuThread.userMode == true)
                {
                    if (Memory.ReadByte(Addresses.mode) == 0 || Memory.ReadByte(Addresses.mode) == 1)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        ThreadingHelper.Sleep(100, cancellationToken);

                        if (Memory.ReadByte(Addresses.mode) == 0 || Memory.ReadByte(Addresses.mode) == 1)
                        {
                            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Not ingame anymore! Exited from WeaponRerollEffectsThread!");
                            break;
                        }
                    }
                }

                //Base weapon special effects (Set 1); (ALSO RUNTIME) - 2=Big bucks, 4=poor, 8=quench, 16=thirst, 32=poison, 64=stop, 128=steal
                //Base weapon special effects (Set 2); (ALSO RUNTIME) - 1=fragile, 2=durable, 4=drain, 8=heal, 16=critical, 32=absup

                var statService = new WeaponStatService(new LegacyProcessGameMemory(), new WeaponMemoryLayout());
                var roller = new WeaponSpecialAttributeRoller(() => rnd.Next(100));

                /*********************
                 *   Heavens Cloud   *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollHeavensCloud();
                    statService.TryWriteByte(Items.heavenscloud, WeaponCharacter.Toan, daggerid, WeaponStat.Effect, values.Effect);
                    statService.TryWriteByte(Items.heavenscloud, WeaponCharacter.Toan, daggerid, WeaponStat.Effect2, values.Effect2);
                }


                /**********************
                 *     Dark Cloud     *
                 **********************/

                {
                    WeaponEffectValues values = roller.RollDarkCloud();
                    statService.TryWriteByte(Items.darkcloud, WeaponCharacter.Toan, daggerid, WeaponStat.Effect, values.Effect);
                }

                /*********************
                 *      Big Bang     *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollBigBang();
                    statService.TryWriteByte(Items.bigbang, WeaponCharacter.Toan, daggerid, WeaponStat.Effect, values.Effect);
                    statService.TryWriteByte(Items.bigbang, WeaponCharacter.Toan, daggerid, WeaponStat.Effect2, values.Effect2);
                }

                /************************
                 *   Atlamillia Sword   *
                 ************************/

                {
                    WeaponEffectValues values = roller.RollAtlamilliaSword();
                    statService.TryWriteByte(Items.atlamilliasword, WeaponCharacter.Toan, daggerid, WeaponStat.Effect, values.Effect);
                    statService.TryWriteByte(Items.atlamilliasword, WeaponCharacter.Toan, daggerid, WeaponStat.Effect2, values.Effect2);
                }

                /*********************
                 *       Dagger      *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollDusack();
                    statService.TryWriteByte(Items.dusack, WeaponCharacter.Toan, daggerid, WeaponStat.Effect, values.Effect);
                }

                /**********************
                 *    Goddess Ring    *
                 **********************/

                {
                    WeaponEffectValues values = roller.RollGoddessRing();
                    statService.TryWriteByte(Items.goddessring, WeaponCharacter.Ruby, goldringid, WeaponStat.Effect2, values.Effect2);
                }

                /************************
                 *   Destruction Ring   *
                 ************************/

                {
                    WeaponEffectValues values = roller.RollDestructionRing();
                    statService.TryWriteByte(Items.destructionring, WeaponCharacter.Ruby, goldringid, WeaponStat.Effect2, values.Effect2);
                }

                /*********************
                 *    Satans Ring    *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollSatansRing();
                    statService.TryWriteByte(Items.satansring, WeaponCharacter.Ruby, goldringid, WeaponStat.Effect2, values.Effect2);
                }

                /*********************
                 *       Skunk       *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollSkunk();
                    statService.TryWriteByte(Items.skunk, WeaponCharacter.Osmond, machinegunid, WeaponStat.Effect, values.Effect);
                }

                /*********************
                 *      Swallow      *
                 *********************/

                {
                    WeaponEffectValues values = roller.RollSwallow();
                    statService.TryWriteByte(Items.swallow, WeaponCharacter.Osmond, machinegunid, WeaponStat.Effect, values.Effect);
                }

                if (cancellationToken.IsCancellationRequested)
                    break;

                ThreadingHelper.Sleep(1000, cancellationToken);
            }
        }

    }
}
