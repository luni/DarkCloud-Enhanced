using System;
using System.Threading;
using System.Collections.Generic;
using static DarkCloud.Core.Dungeon.DungeonProgression;

namespace DarkCloudEnhancedMod
{
    public class Dungeon
    {
        static byte currentDungeon;
        static byte currentFloor;
        static ushort currentWeapon;
        static int currentAddress;
        static int prevFloor = 200;
        static int currentCharCursor = 0;
        static int prevCharCursor = 0;
        static ushort currentGilda = 0;
        static bool clownOnScreen = false;
        static bool chronicle2 = false;
        static bool[] monstersDead = new bool[15];
        static bool monsterQuestActive = false;
        static bool eventfloor = false;
        static bool squareActive = false;
        static bool dunEscapeConfirm = false;
        static bool dunEscapeConfirmSpamCheck = false;
        static bool dunUsedActiveEscape = false;
        static bool dunUsedEscapeCheck = false;
        static bool wepMenuOpen = false;
        static bool PPowdermenuOpen = false;
        static bool circlePressed = false;
        static bool hasClearMessageShown = false;
        static byte[] wepLevelArray = new byte[10];
        public static bool monsterQuestMachoActive = false;
        public static bool monsterQuestGobActive = false;
        public static bool monsterQuestJakeActive = false;
        public static bool monsterQuestChiefActive = false;
        public static bool sambaChallengeQuest = false;
        public static bool sambaChallengeQuestActive = false;
        public static bool sambaChallengeQuestCheck = false;
        public static bool mayorQuest = false;
        public static bool mayorQuestCheck = false;
        public static bool mayorQuestActive = false;
        public static bool hasMiniBoss = false;
        public static bool enemiesSpawn = false;
        public static bool doorIsOpen = false;
        public static bool magicCircleChanged = false;
        public static List<byte> excludeFloors;

        //THREADS
        //Runs at the start of each floor
        public static Thread spawnsCheck;
        public static Thread minibossProcess;
        public static Thread miniBossMessage;
        
        //Weapon threads, only 1 should run at a time
        public static Thread boneDoorThread = new Thread(() => CustomEffects.BoneDoorTrigger(CancellationToken.None)) { IsBackground = true };
        public static Thread seventhHeavenThread = new Thread(() => CustomEffects.SeventhHeaven(CancellationToken.None)) { IsBackground = true };
        public static Thread chronicleSwordThread = new Thread(() => CustomEffects.ChronicleSword(CancellationToken.None)) { IsBackground = true };
        public static Thread evilciseThread = new Thread(() => CustomEffects.Evilcise(CancellationToken.None)) { IsBackground = true };
        public static Thread angelGearThread = new Thread(() => CustomEffects.AngelGear(CancellationToken.None)) { IsBackground = true };
        public static Thread tallHammerThread = new Thread(() => CustomEffects.TallHammer(CancellationToken.None)) { IsBackground = true };
        public static Thread infernoHammerThread = new Thread(() => CustomEffects.Inferno(CancellationToken.None)) { IsBackground = true };
        public static Thread mobiusRingThread = new Thread(() => CustomEffects.MobiusRing(CancellationToken.None)) { IsBackground = true };
        public static Thread herculesWrathThread = new Thread(() => CustomEffects.HerculesWrath(CancellationToken.None)) { IsBackground = true };
        public static Thread babelSpearThread = new Thread(() => CustomEffects.BabelSpear(CancellationToken.None)) { IsBackground = true };
        public static Thread supernovaThread = new Thread(() => CustomEffects.Supernova(CancellationToken.None)) { IsBackground = true };
        public static Thread starBreakerThread = new Thread(() => CustomEffects.StarBreaker(CancellationToken.None)) { IsBackground = true };
        public static Thread elementSwapThread = new Thread(() => Dayuppy.ElementSwapping(CancellationToken.None)) { IsBackground = true }; //Create a new thread to run monitorElementSwapping()
        public static Thread dunEscapeConfirmThread;

        public static Thread cheatCodeThread = new Thread(() => CheatCodes.InputBuffer.Monitor(CancellationToken.None)) { IsBackground = true };
        public static void InsideDungeonThread()
        {
            InsideDungeonThread(CancellationToken.None);
        }

        public static void InsideDungeonThread(CancellationToken cancellationToken)
        {
            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Dungeon Thread Activated");

            // Restart the shared feature threads so they observe the current session's
            // cancellation token. RestartThread handles stale threads from a previous
            // session that may still be sleeping.
            ThreadingHelper.RestartThread(ref elementSwapThread, () => Dayuppy.ElementSwapping(cancellationToken));
            ThreadingHelper.RestartThread(ref cheatCodeThread, () =>
            {
                Resources.initiateRubyMemeFix();
                CheatCodes.InputBuffer.Monitor(cancellationToken);
            });
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (Player.InDungeonFloor())
                {
                    if (!Player.CheckDunIsPaused() && Player.CheckDunIsWalkingMode())
                    {
                        switch (Player.CurrentCharacterNum())
                        {
                            //Toan
                            case Player.ToanId:
                                if(magicCircleChanged) CustomEffects.SecretArmletDisable(); magicCircleChanged = false;

                                switch (Player.Weapon.GetCurrentWeaponId())
                                {
                                    case Items.bonerapier:
                                        CustomEffects.BoneRapierEffect(true);

                                        if (!boneDoorThread.IsAlive)
                                        {
                                            boneDoorThread = new Thread(() => CustomEffects.BoneDoorTrigger(cancellationToken)) { IsBackground = true };
                                            boneDoorThread.Start();
                                        }
                                        break;
                                    case Items.seventhheaven:
                                        CustomEffects.BoneRapierEffect(false);

                                        if (!seventhHeavenThread.IsAlive)
                                        {
                                            seventhHeavenThread = new Thread(() => CustomEffects.SeventhHeaven(cancellationToken)) { IsBackground = true };
                                            seventhHeavenThread.Start();
                                        }
                                        break;
                                    case Items.chroniclesword:
                                        CustomEffects.BoneRapierEffect(false);

                                        if (!chronicleSwordThread.IsAlive)
                                        {
                                            chronicleSwordThread = new Thread(() => CustomEffects.ChronicleSword(cancellationToken)) { IsBackground = true };
                                            chronicleSwordThread.Start();
                                        }
                                        break;

                                    default:
                                        CustomEffects.BoneRapierEffect(false);
                                        break;
                                }
                                break;

                            //Xiao
                            case Player.XiaoId:
                                CustomEffects.BoneRapierEffect(false);
                                if (magicCircleChanged) CustomEffects.SecretArmletDisable(); magicCircleChanged = false;

                                switch (Player.Weapon.GetCurrentWeaponId())
                                {
                                    case Items.angelgear:
                                        if (!angelGearThread.IsAlive)
                                        {
                                            angelGearThread = new Thread(() => CustomEffects.AngelGear(cancellationToken)) { IsBackground = true };
                                            angelGearThread.Start();
                                        }
                                        break;
                                }
                                break;

                            //Goro
                            case Player.GoroId:
                                CustomEffects.BoneRapierEffect(false);
                                if (magicCircleChanged) CustomEffects.SecretArmletDisable(); magicCircleChanged = false;

                                switch (Player.Weapon.GetCurrentWeaponId())
                                {
                                    case Items.tallhammer:
                                        if (!tallHammerThread.IsAlive)
                                        {
                                            tallHammerThread = new Thread(() => CustomEffects.TallHammer(cancellationToken)) { IsBackground = true };
                                            tallHammerThread.Start();
                                        }
                                        break;
                                    case Items.inferno:
                                        if (!infernoHammerThread.IsAlive)
                                        {
                                            infernoHammerThread = new Thread(() => CustomEffects.Inferno(cancellationToken)) { IsBackground = true };
                                            infernoHammerThread.Start();
                                        }
                                        break;

                                    default:
                                        break;
                                }
                                break;
                             
                            //Ruby
                            case Player.RubyId:
                                CustomEffects.BoneRapierEffect(false);

                                switch (Player.Weapon.GetCurrentWeaponId())
                                {
                                    case Items.mobiusring:
                                        if (magicCircleChanged) CustomEffects.SecretArmletDisable(); magicCircleChanged = false;

                                        if (!mobiusRingThread.IsAlive)
                                        {
                                            mobiusRingThread = new Thread(() => CustomEffects.MobiusRing(cancellationToken)) { IsBackground = true };
                                            mobiusRingThread.Start();
                                        }
                                        break;
                                    case Items.secretarmlet:
                                        if (!magicCircleChanged) { 
                                            bool executed = CustomEffects.SecretArmletEnable();
                                            if(executed) magicCircleChanged = true;
                                        }
                                        break;
                                    default:
                                        if (magicCircleChanged) CustomEffects.SecretArmletDisable(); magicCircleChanged = false;
                                        break;
                                }
                                break;

                            //Ungaga
                            case Player.UngagaId:
                                CustomEffects.BoneRapierEffect(false);
                                if (magicCircleChanged) CustomEffects.SecretArmletDisable(); magicCircleChanged = false;


                                switch (Player.Weapon.GetCurrentWeaponId())
                                {
                                    case Items.herculeswrath:
                                        if (!herculesWrathThread.IsAlive)
                                        {
                                            herculesWrathThread = new Thread(() => CustomEffects.HerculesWrath(cancellationToken)) { IsBackground = true };
                                            herculesWrathThread.Start();
                                        }
                                        break;

                                    case Items.babelsspear:
                                        if (!babelSpearThread.IsAlive)
                                        {
                                            babelSpearThread = new Thread(() => CustomEffects.BabelSpear(cancellationToken)) { IsBackground = true };
                                            babelSpearThread.Start();
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            //Osmond
                            case Player.OsmondId:
                                CustomEffects.BoneRapierEffect(false);
                                if (magicCircleChanged) CustomEffects.SecretArmletDisable(); magicCircleChanged = false;

                                switch (Player.Weapon.GetCurrentWeaponId())
                                {
                                    case Items.supernova:
                                        if (!supernovaThread.IsAlive)
                                        {
                                            supernovaThread = new Thread(() => CustomEffects.Supernova(cancellationToken)) { IsBackground = true };
                                            supernovaThread.Start();
                                        }
                                        break;

                                    case Items.starbreaker:
                                        if (!starBreakerThread.IsAlive)
                                        {
                                            starBreakerThread = new Thread(() => CustomEffects.StarBreaker(cancellationToken)) { IsBackground = true };
                                            starBreakerThread.Start();
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                        }
                        

                        CheckActiveItems(cancellationToken);
                    }

                    //Check if player is inside the weapon customize menu
                    if (Player.CheckIsWeaponCustomizeMenu())
                    {
                        //The Synthsphere Listener thread
                        ThreadingHelper.RestartThread(ref Weapons.weaponsMenuListener, () => Weapons.WeaponListenForSynthSphere(cancellationToken));
                    }

                    //Check if the player has killed all the floor enemies
                    if (ReusableFunctions.CheckIfAllEnemiesKilled() && !hasClearMessageShown)
                    {
                        Dayuppy.DisplayMessage("DUMMY", 0, 0, 4000, true, cancellationToken: cancellationToken);

                        hasClearMessageShown = true;
                    }

                    //Get current Dungeon
                    currentDungeon = Memory.ReadByte(Addresses.checkDungeon);

                    //Define event and boss floors
                    excludeFloors = GetDungeonEventFloors(currentDungeon);

                   
                    //Get current Floor
                    currentFloor = Memory.ReadByte(Addresses.checkFloor);
                    

                    //Check if the player has entered a new floor
                    if (currentFloor != prevFloor)
                    {
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Floor changed!");
                        ThreadingHelper.Sleep(120, cancellationToken);  // check if player is still in dungeon(to prevent a new floor process when leaving dungeon)
                        if (Player.InDungeonFloor())
                        {
                            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Player has entered a new floor!");

                            doorIsOpen = false;
                            magicCircleChanged = false;
                            dunUsedActiveEscape = false;
                            dunUsedEscapeCheck = false;
                            hasClearMessageShown = false;
                            MiniBoss.miniBossRolled = false;

                            //Check if player is not on an event floor and call the Mini Boss
                            if (!excludeFloors.Contains(currentFloor))
                            {
                                //Initialize the spawns check
                                Memory.WriteInt(Enemies.Enemy14.hp, 1);
                                ThreadingHelper.RestartThread(ref spawnsCheck, () => CheckSpawns(cancellationToken), joinTimeoutMs: 200);

                                eventfloor = false;
                            }
                            else
                            {
                                eventfloor = true;
                                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Player has entered an event floor!");
                            }

                            FixUngagaDoors(currentDungeon);

                            //Save current weapon
                            currentWeapon = Player.Weapon.GetCurrentWeaponId();

                            //Once everything is done, we set this so it wont reroll again in same floor
                            prevFloor = currentFloor;
                        }
                    }

                    CheckUngagaSwap(cancellationToken);
                    CheckWepLvlUp();
                    CheckClown();
                    CheckCurrentSidequests(cancellationToken);
                    CheckDungeonLeaving();
                    CheckMiniBossStamina();
                    if (CheckWeaponChange(currentWeapon))
                    {
                        ReusableFunctions.ClearRecentDamageAndDamageSource();
                        currentWeapon = Player.Weapon.GetCurrentWeaponId();
                    }


                }
                //Used to reset the floor data when going back to dungeon
                else prevFloor = 200;

                if (Memory.ReadByte(Addresses.dungeonMode) == 4) //Check if in floor selection menu
                {
                    FloorSelectionScreen();
                }

                if (MainMenuThread.userMode == true)
                {
                    if (Memory.ReadByte(Addresses.mode) == 0 || Memory.ReadByte(Addresses.mode) == 1)
                    {
                        ThreadingHelper.Sleep(100, cancellationToken);
                        if (Memory.ReadByte(Addresses.mode) == 0 || Memory.ReadByte(Addresses.mode) == 1)
                        {
                            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Not ingame anymore! Exited from Dungeon!");
                            break;
                        }
                    }
                }

                ThreadingHelper.Sleep(10, cancellationToken);
            }
        }

        /// <summary>
        /// Returns a list with the dungeon key ids for the given dungeon.
        /// </summary>
        /// <param name="dungeon">The dungeon id:
        /// <br>0 = Divine Beast Cave</br>
        /// <br>1 = Wise Owl</br>
        /// <br>2 = Shipwreck</br>
        /// <br>3 = Sun and Moon</br>
        /// <br>4 = Moon Sea</br>
        /// <br>5 = Gallery of Time</br>
        /// <br>6 = Demon Shaft</br></param>
        /// <returns></returns>
        public static List<byte> GetDungeonGateKey(byte dungeon)
        {
            return new List<byte>(GetGateKeyItems(dungeon));
        }

        public static byte GetDungeonBackFloorKey(byte dungeon)
        { 
            return GetBackFloorKeyItem(dungeon);
        }

        public static List<byte> GetDungeonEventFloors(byte dungeon)
        {
            return new List<byte>(GetEventFloors(dungeon));
        }

        public static void CheckEnemyKill(int currentEnemyAddress, CancellationToken cancellationToken = default)
        {
            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Checking quest...");
            if (monsterQuestMachoActive)
            {
                //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Macho quest active");
                int currentEnemyAddress2 = currentEnemyAddress + 0x0000001E;
                if (Memory.ReadByte(currentEnemyAddress2) == Memory.ReadByte(0x21CE4406))
                {
                    Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest progress +1!");
                    byte killsleft = Memory.ReadByte(0x21CE4405);
                    killsleft--;
                    Memory.WriteByte(0x21CE4405, killsleft);

                    if (killsleft == 0)
                    {
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest complete!!");
                        Dayuppy.DisplayMessage("You completed Macho's quest!\nWell done!", 2, 30, 4000, cancellationToken: cancellationToken);
                        Memory.WriteByte(0x21CE4402, 2);
                        monsterQuestMachoActive = false;
                    }
                }
            }
            if (monsterQuestGobActive)
            {
                //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Gob quest active");
                int currentEnemyAddress2 = currentEnemyAddress + 0x0000001E;
                if (Memory.ReadByte(currentEnemyAddress2) == Memory.ReadByte(0x21CE440B))
                {
                    Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest progress +1!");
                    byte killsleft = Memory.ReadByte(0x21CE440A);
                    killsleft--;
                    Memory.WriteByte(0x21CE440A, killsleft);

                    if (killsleft == 0)
                    {
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest complete!!");
                        Dayuppy.DisplayMessage("You completed Gob's quest!\nWell done!", 2, 30, 4000, cancellationToken: cancellationToken);
                        Memory.WriteByte(0x21CE4407, 2);
                        monsterQuestGobActive = false;
                    }
                }
            }
            if (monsterQuestJakeActive)
            {
                //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Jake quest active");
                int currentEnemyAddress2 = currentEnemyAddress + 0x0000001E;
                if (Memory.ReadByte(currentEnemyAddress2) == Memory.ReadByte(0x21CE4410))
                {
                    Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest progress +1!");
                    byte killsleft = Memory.ReadByte(0x21CE440F);
                    killsleft--;
                    Memory.WriteByte(0x21CE440F, killsleft);

                    if (killsleft == 0)
                    {
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest complete!!");
                        Dayuppy.DisplayMessage("You completed Jake's quest!\nWell done!", 2, 30, 4000, cancellationToken: cancellationToken);
                        Memory.WriteByte(0x21CE440C, 2);
                        monsterQuestJakeActive = false;
                    }
                }
            }
            if (monsterQuestChiefActive)
            {
                //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Chief quest active");
                int currentEnemyAddress2 = currentEnemyAddress + 0x0000001E;
                if (Memory.ReadByte(currentEnemyAddress2) == Memory.ReadByte(0x21CE4415))
                {
                    Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest progress +1!");
                    byte killsleft = Memory.ReadByte(0x21CE4414);
                    killsleft--;
                    Memory.WriteByte(0x21CE4414, killsleft);

                    if (killsleft == 0)
                    {
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest complete!!");
                        Dayuppy.DisplayMessage("You completed Chief Bonka´s quest!\nWell done!", 2, 35, 4000, cancellationToken: cancellationToken);
                        Memory.WriteByte(0x21CE4411, 2);
                        monsterQuestChiefActive = false;
                    }
                }
            }
        }

        /// <summary>
        /// Check enemy spawns upon entering a dungeon floor
        /// </summary>
        public static void CheckSpawns()
        {
            CheckSpawns(CancellationToken.None);
        }

        public static void CheckSpawns(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Checking spawns...");

            int ms = 0;
            byte numNormalEnemies = 0;

            if(prevFloor == 200)
            {
                //Listens for the enemy render address value to change, from 0 or 10 seconds have passed
                //We use the enemy render value here because enemies spawn after chests
                while (Memory.ReadByte(Enemies.Enemy14.renderStatus) == 255 && ms < 10000)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    ThreadingHelper.Sleep(100, cancellationToken);
                    ms += 100;
                    continue;
                }
            }
            else
            {
                //Listens for the enemy hp address value to change, from 0 or 10 seconds have passed
                //We use the enemy render value here because enemies spawn after chests
                while (Memory.ReadByte(Enemies.Enemy14.hp) == 1 && ms < 10000)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    ThreadingHelper.Sleep(100, cancellationToken);
                    ms += 100;
                    continue;
                }
            }

            //Set the flag to true
            if(Memory.ReadByte(Enemies.Enemy0.renderStatus) > 0) enemiesSpawn = true;

            //Get all the current floor enemy ids
            List<ushort> enemyFloorIds = Enemies.GetFloorEnemiesIds();

            //Calculate the amount of non-flying enemies in the floor
            foreach (ushort enemy in enemyFloorIds)
            {
                if (Enemies.GetNormalEnemies().ContainsKey(enemy)) numNormalEnemies++;
            }

            //Check if there are more than 3 normal enemies in the floor
            //This is to account for the Wise Owl 3 keys
            //There needs to be enough normal enemies to roll for the miniboss in order to avoid infinite retries
            if (numNormalEnemies > 3)
            {
                //Initialize the mini boss thread
                minibossProcess = new Thread(() => DoMinibossSpawn(currentDungeon, cancellationToken)) { IsBackground = true };

                //Start the next thread
                minibossProcess.Start();
            }
            else Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Not enough normal enemies in floor!");

            chronicle2 = CustomEffects.CheckChronicle2(chronicle2);
            CustomChests.ChestRandomizer(currentDungeon, currentFloor, chronicle2, cancellationToken); //Randomize the chest loot

            CheckSidequests(cancellationToken);

            CustomEffects.chronicleNewFloor = true;
            ReusableFunctions.ClearRecentDamageAndDamageSource();

            monsterQuestActive = SideQuestManager.CheckCurrentDungeonQuests(currentDungeon);

            for (int i = 0; i < monstersDead.Length; i++)
            {
                monstersDead[i] = false;
            }

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Finished spawn checking");
        }

        /// <summary>
        /// Returns true if the given weapon ID is different to the one the player is currently using
        /// </summary>
        /// <param name="weapon">The weapon ID to check</param>
        public static bool CheckWeaponChange(ushort weapon)
        {
            if (Player.Weapon.GetCurrentWeaponId() != weapon) return true;

            return false;
        }

        /// <summary>
        /// Process to start the mini boss spawn
        /// </summary>
        /// <param name="currentDungeon">The current dungeon ID</param>
        public static void DoMinibossSpawn(byte currentDungeon, CancellationToken cancellationToken)
        {
            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Processing mini boss...");
           
            hasMiniBoss = MiniBoss.MiniBossSpawn(false, currentDungeon, currentFloor, cancellationToken); 

            //If the mini boss spawned, start its warning message thread
            if (hasMiniBoss) { 
                miniBossMessage = new Thread(() => MiniBossMessage(cancellationToken)) { IsBackground = true };
                miniBossMessage.Start();
            }
            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Mini boss has rolled: " + hasMiniBoss);
            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Finished mini boss process!");

        }

        /// <summary>
        /// Displays the mini boss screen message
        /// </summary>
        public static void MiniBossMessage(CancellationToken cancellationToken)
        {
            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Working on the message...");

            if (cancellationToken.IsCancellationRequested)
                return;

            int ms = 0;

            //Wait until we get control, we use the HUD display as a flag
            while (Memory.ReadByte(Addresses.hideHud) == 1 && ms < 8000)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                ThreadingHelper.Sleep(100, cancellationToken);
                ms += 100;
            }

            Dayuppy.DisplayMessage("A mysterious enemy lurks\naround. Be careful!", 2, 24, 4000, cancellationToken: cancellationToken);

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Finished message process!");
        }

        /// <summary>
        /// Returns true if the bone door opening trigger is active
        /// </summary>
        public static bool IsBypassBoneDoor()
        {
            return Memory.ReadByte(Addresses.BoneDoorOpenType) == 5 ? true: false;
        }

        /// <summary>
        /// Activates or deactivates the door trigger
        /// </summary>
        /// <param name="flag">True if to activate the door</param>
        public static void SetBypassBoneDoor(bool flag)
        {
            byte n;
            if (flag) n = 5;
            else n = 21;
            Memory.WriteByte(Addresses.BoneDoorOpenType, n);
        }

        public static void FixUngagaDoors(byte currentdng)
        {
            switch (currentdng)
            {
                case 3:
                    if (Memory.ReadFloat(0x20928670) == 150)
                    {
                        Memory.WriteByte(0x20985E0, 30);
                        Memory.WriteFloat(0x20928670, 50);
                        Memory.WriteFloat(0x20928928, 50);
                        Memory.WriteByte(0x20928B14, 30);
                        Memory.WriteByte(0x20928AE4, 30);
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Fixed Ungaga Doors");
                    }
                    else
                    {
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Couldn't fix ungaga doors, or they were fixed already");
                    }
                    break;

                case 4:
                    if (Memory.ReadFloat(0x2092FA08) == 150)
                    {
                        Memory.WriteByte(0x2092F978, 30);
                        Memory.WriteFloat(0x2092FA08, 50);
                        Memory.WriteFloat(0x2092FCC0, 50);
                        Memory.WriteByte(0x2092FEAC, 30);
                        Memory.WriteByte(0x2092FE7C, 30);
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Fixed Ungaga Doors");
                    }
                    else
                    {
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Couldn't fix ungaga doors, or they were fixed already");
                    }
                    break;

                case 5:
                    if (Memory.ReadFloat(0x209244AC) == 150)
                    {
                        Memory.WriteByte(0x2092441C, 30);
                        Memory.WriteFloat(0x209244AC, 50);
                        Memory.WriteFloat(0x20924764, 50);
                        Memory.WriteByte(0x20924920, 30);
                        Memory.WriteByte(0x20924950, 30);
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Fixed Ungaga Doors");
                    }
                    else
                    {
                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Couldn't fix ungaga doors, or they were fixed already");
                    }
                    break;

                default:
                    break;

            }
        }

        public static void CheckUngagaSwap(CancellationToken cancellationToken = default)
        {
            currentCharCursor = Memory.ReadByte(0x202A2DE8); //current char

            if (currentCharCursor != prevCharCursor)
            {
                if (currentCharCursor == 4)
                {
                    int timer = 0;
                    while (timer < 10)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        ThreadingHelper.Sleep(100, cancellationToken);
                        timer++;

                        if (Memory.ReadByte(0x202A2010) == 3)
                        {
                            if (Memory.ReadUShort(0x2193A013) == 12850)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (Memory.ReadUShort(0x217E5453) == 12850)
                            {
                                break;
                            }
                        }

                        
                    }

                    if (Memory.ReadByte(0x202A2010) == 3)
                    {
                        Memory.WriteByte(0x2193A013, 52);
                        Memory.WriteByte(0x2193A014, 52);
                    }
                    else
                    {
                        Memory.WriteByte(0x217E5453, 52);
                        Memory.WriteByte(0x217E5454, 52);
                    }
                }
            }

            prevCharCursor = currentCharCursor;
        }
        


        public static void CheckClown()
        {
            //Check if clown is triggered, then change loot table
            if (Memory.ReadInt(Addresses.clownCheck) == 30707852 && clownOnScreen == false && eventfloor == false)
            {
                CustomChests.ClownRandomizer(chronicle2);
                clownOnScreen = true;
            }
            else
            {
                if (clownOnScreen)
                {
                    if (Memory.ReadInt(Addresses.clownCheck) != 30707852)
                    {
                        clownOnScreen = false;
                    }
                }
            }
        }

        public static void CheckSidequests(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (currentDungeon == 4 && currentFloor == 6 && Memory.ReadByte(0x21CE445E) == 1)
            {
                //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Yellow drops challenge active");
                sambaChallengeQuest = true;
            }
            else
            {
                sambaChallengeQuest = false;
            }

            if (currentDungeon == 6)
            {
                if (Memory.ReadByte(0x21CE4468) == 1) //Mayor quest flag
                {
                    if (currentFloor == Memory.ReadByte(0x21CE4469) -1)
                    {
                        mayorQuest = true;
                        //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Mayor quest active in this floor");
                    }
                    else
                    {
                        mayorQuest = false;
                    }
                }
                else
                {
                    mayorQuest = false;
                }
            }
            else
            {
                mayorQuest = false;
            }
        }

        public static void CheckCurrentSidequests(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (monsterQuestActive)
            {
                if (currentDungeon != 6)
                {
                    for (int i = 0; i < monstersDead.Length; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        currentAddress = 0x21E16BC4 + (i * 0x190);

                        if (Memory.ReadUShort(currentAddress) > 0)
                        {
                            monstersDead[i] = false;
                        }
                        else
                        {
                            if (monstersDead[i] == false)
                            {
                                CheckEnemyKill(currentAddress, cancellationToken);
                            }

                            monstersDead[i] = true;
                        }
                    }
                }
            }

            if (sambaChallengeQuest)
            {
                SambaChallengeQuest(cancellationToken);
            }

            if (mayorQuest)
            {
                MayorQuest(cancellationToken);
            }
        }

        public static void SambaChallengeQuest(CancellationToken cancellationToken = default)
        {
            ushort currentweaponID = Memory.ReadUShort(0x21EA7590);
            if (sambaChallengeQuestCheck == false && Memory.ReadByte(0x202A34CC) == 1)
            {
                if (Memory.ReadByte(Addresses.hideHud) == 0)
                {
                    if (Memory.ReadByte(0x202A3570) == 0 && (currentweaponID == 258 || currentweaponID == 257))
                    {
                        Memory.WriteInt(0x21CE205C, 0);
                        Dayuppy.DisplayMessage("Samba's quest started!\nClear all enemies using only Dagger!\nUsing a throwable also\ncancels the mission.", 4, 40, 8000, cancellationToken: cancellationToken);
                        sambaChallengeQuestActive = true;

                        for (int i = 0; i < 8; i++)
                        {
                            monstersDead[i] = false;
                        }
                    }
                    else if (Memory.ReadByte(0x202A3570) == 0 && currentweaponID != 258 && currentweaponID != 257)
                    {
                        Dayuppy.DisplayMessage("Samba's quest did not start.\nRe-enter with Dagger equipped.", 2, 30, 4000, cancellationToken: cancellationToken);
                        sambaChallengeQuestActive = false;
                    }
                    sambaChallengeQuestCheck = true;
                }
            }
            else if (sambaChallengeQuestCheck == true && Memory.ReadByte(0x202A34CC) == 0)
            {
                sambaChallengeQuestCheck = false;
                sambaChallengeQuestActive = false;
            }

            if (sambaChallengeQuestActive)
            {
                if ((currentweaponID != 258 && currentweaponID != 257) || Memory.ReadByte(0x21DC4484) == 26 || Memory.ReadByte(0x21DC4484) == 27)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    ThreadingHelper.Sleep(500, cancellationToken);
                    Dayuppy.DisplayMessage("Samba's quest has been cancelled.\nRe-enter in order to activate it.", 2, 40, 4000, cancellationToken: cancellationToken);
                    sambaChallengeQuestActive = false;
                }
                byte enemieskilled = 0;
                for (int i = 0; i < 8; i++)
                {
                    currentAddress = 0x21E16BC4 + (i * 0x190);

                    if (Memory.ReadUShort(currentAddress) > 0)
                    {
                        monstersDead[i] = false;
                    }
                    else
                    {
                        monstersDead[i] = true;
                        enemieskilled++;
                    }
                }

                if (enemieskilled == 8)
                {
                    Dayuppy.DisplayMessage("Samba's quest completed!\nWell done!", 2, 28, 4000, cancellationToken: cancellationToken);
                    Memory.WriteByte(0x21CE4462, 1);
                    sambaChallengeQuest = false;
                }
            }
        }

        public static void MayorQuest(CancellationToken cancellationToken = default)
        {
            if (mayorQuestCheck == false && Memory.ReadByte(0x202A34CC) == 1)
            {
                if (Memory.ReadByte(Addresses.hideHud) == 0)
                {
                    if (Memory.ReadByte(0x202A3570) == Memory.ReadByte(0x21CE446A)) //check if correct ally for quest
                    {
                        Memory.WriteInt(0x21CE205C, 0);
                        Dayuppy.DisplayMessage("Mayor's quest started!\nClear all enemies.\nCannot change character.\nThrowables are not allowed.", 4, 26, 5000, cancellationToken: cancellationToken);

                        mayorQuestActive = true;

                        for (int i = 0; i < 8; i++)
                        {
                            monstersDead[i] = false;
                        }
                    }
                    else
                    {
                        Dayuppy.DisplayMessage("Mayor's quest did not start.\nRe-enter with correct ally.", 2, 30, 4000, cancellationToken: cancellationToken);
                        mayorQuestActive = false;
                    }
                    mayorQuestCheck = true;
                }
            }
            else if (mayorQuestCheck == true && Memory.ReadByte(0x202A34CC) == 0)
            {
                mayorQuestCheck = false;
                mayorQuestActive = false;
            }

            if (mayorQuestActive)
            {
                if (Memory.ReadByte(0x21DC4484) == 26 || Memory.ReadByte(0x21DC4484) == 27)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    ThreadingHelper.Sleep(500, cancellationToken);
                    Dayuppy.DisplayMessage("Mayor's quest has been cancelled.\nRe-enter in order to re-attempt it.", 2, 40, 4000, cancellationToken: cancellationToken);
                    mayorQuestActive = false;
                }

                byte enemieskilled = 0;
                for (int i = 0; i < 8; i++)
                {
                    currentAddress = 0x21E16BC4 + (i * 0x190);

                    if (Memory.ReadUShort(currentAddress) > 0)
                    {
                        monstersDead[i] = false;
                    }
                    else
                    {
                        monstersDead[i] = true;
                        enemieskilled++;
                    }
                }

                if (enemieskilled == 8)
                {
                    Dayuppy.DisplayMessage("Mayor's quest completed!\nWell done!", 2, 28, 4000, cancellationToken: cancellationToken);
                    Memory.WriteByte(0x21CE4468, 2);
                    mayorQuest = false;
                }
            }
        }

        public static void FloorSelectionScreen()
        {
            if (circlePressed == false)
            {
                if (Memory.ReadUShort(Addresses.buttonInputs) == (ushort)CheatCodes.InputBuffer.Button.Circle)
                {
                    circlePressed = true;                 
                }
            }
            else
            {
                if (Memory.ReadUShort(Addresses.buttonInputs) != (ushort)CheatCodes.InputBuffer.Button.Circle)
                {
                    currentGilda = Memory.ReadUShort(Addresses.gilda);
                    Memory.WriteUShort(Addresses.dungeonDebugMenu, 170);
                    Memory.WriteByte(Addresses.dungeonMode, 1);
                    circlePressed = false;
                }
            }
        }

        public static void CheckActiveItems(CancellationToken cancellationToken)
        {
            if (Memory.ReadUShort(Addresses.buttonInputs) == (ushort)CheatCodes.InputBuffer.Button.Square && (Memory.ReadByte(0x21D5676D) > 0 && Memory.ReadInt(0x21D56770) == -1) )
            {
                int currentSlot = Memory.ReadInt(0x202A3598);
                int currentActiveItem = 0x21CDD8AC + (0x2 * currentSlot);

                if (Memory.ReadShort(currentActiveItem) == 175)
                {
                    byte animationID = Memory.ReadByte(0x21DC4484);
                    if (animationID == 0 || animationID == 1 || animationID == 2 || animationID == 18)
                    {
                        if (squareActive == false)
                        {
                            if (dunEscapeConfirm == false)
                            {
                                squareActive = true;
                                Dayuppy.DisplayMessage("^RAre you sure you want to leave?\n^WPress square to use Escape Powder.", 2, 36, 3000, cancellationToken: cancellationToken);
                                dunEscapeConfirmThread = new Thread(() => DunEscapeConfirmTimer(cancellationToken)) { IsBackground = true };
                                dunEscapeConfirmThread.Start();
                                dunEscapeConfirm = true;
                                dunEscapeConfirmSpamCheck = false;
                            }
                            else if (dunEscapeConfirm)
                            {
                                if (dunEscapeConfirmSpamCheck == true)
                                {
                                    if (Memory.ReadByte(0x202A35EC) == 0)
                                    {
                                        squareActive = true;
                                        dunUsedActiveEscape = true;
                                        Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Activated escape powder!");
                                        Memory.WriteByte(0x202A35EC, 170);
                                        byte currentPowders = Memory.ReadByte(0x21CDD8B2 + (0x2 * currentSlot));
                                        currentPowders--;
                                        Memory.WriteByte(0x21CDD8B2 + (0x2 * currentSlot), currentPowders);
                                        if (currentPowders == 0)
                                        {
                                            Memory.WriteUShort(currentActiveItem, 65535);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Memory.ReadShort(currentActiveItem) == 177)
                {
                    byte animationID = Memory.ReadByte(0x21DC4484);
                    if (animationID == 0 || animationID == 1 || animationID == 2 || animationID == 18)
                    {
                        if (squareActive == false)
                        {
                            ushort currentmaxWHP = Player.Weapon.GetCurrentWeaponMaxWhp();

                            int currentChar = Memory.ReadByte(0x21CD9550);
                            int currentWepNum = Memory.ReadByte(0x21CDD88C + (0x1 * currentChar));
                            int whp;

                            if (currentChar == 0)
                            {
                                whp = Player.Toan.WeaponSlot0.whp + (0xF8 * currentWepNum);
                            }
                            else if (currentChar == 1)
                            {
                                whp = Player.Xiao.WeaponSlot0.whp + (0xF8 * currentWepNum);
                            }
                            else if (currentChar == 2)
                            {
                                whp = Player.Goro.WeaponSlot0.whp + (0xF8 * currentWepNum);
                            }
                            else if (currentChar == 3)
                            {
                                whp = Player.Ruby.WeaponSlot0.whp + (0xF8 * currentWepNum);
                            }
                            else if (currentChar == 4)
                            {
                                whp = Player.Ungaga.WeaponSlot0.whp + (0xF8 * currentWepNum);
                            }
                            else
                            {
                                whp = Player.Osmond.WeaponSlot0.whp + (0xF8 * currentWepNum);
                            }
                            float currentWHP = Memory.ReadFloat(whp);
                            if (currentWHP < currentmaxWHP)
                            {                         
                                Memory.WriteFloat(whp, currentmaxWHP);
                                Dayuppy.DisplayMessage("Used Repair Powder!", 1, 20, 2000, cancellationToken: cancellationToken);
                                byte currentPowders = Memory.ReadByte(0x21CDD8B2 + (0x2 * currentSlot));
                                currentPowders--;
                                Memory.WriteByte(0x21CDD8B2 + (0x2 * currentSlot), currentPowders);
                                squareActive = true;
                                if (currentPowders == 0)
                                {
                                    Memory.WriteUShort(currentActiveItem, 65535);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                squareActive = false;
            }          
        }

        public static void DunEscapeConfirmTimer(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            ThreadingHelper.Sleep(500, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            dunEscapeConfirmSpamCheck = true;
            ThreadingHelper.Sleep(2500, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            dunEscapeConfirm = false;
        }

        public static void CheckDungeonLeaving()
        {
            if (dunUsedActiveEscape == false && dunUsedEscapeCheck == false)
            {
                if (Memory.ReadByte(0x202A35EC) == 171)
                {
                    CheckEscapePowders();
                    dunUsedEscapeCheck = true;
                }
            }
        }

        public static void CheckEscapePowders()
        {
            bool hasEscapeP = SideQuestManager.CheckItemQuestReward(175, true, false);

            if (hasEscapeP == false)
            {
                if (Memory.ReadByte(0x21CDD8AE) == 175)
                {
                    byte currentPowders = Memory.ReadByte(0x21CDD8B4);
                    currentPowders--;
                    Memory.WriteByte(0x21CDD8B4, currentPowders);
                    if (currentPowders == 0)
                    {
                        Memory.WriteUShort(0x21CDD8AE, 0);
                    }
                    Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Consumed escape powder from active slots");
                }
                else if (Memory.ReadByte(0x21CDD8B0) == 175) 
                {
                    byte currentPowders = Memory.ReadByte(0x21CDD8B6);
                    currentPowders--;
                    Memory.WriteByte(0x21CDD8B6, currentPowders);
                    if (currentPowders == 0)
                    {
                        Memory.WriteUShort(0x21CDD8B0, 0);
                    }
                    Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Consumed escape powder from active slots");

                }
                else if (Memory.ReadByte(0x21CDD8B2) == 175)
                {
                    byte currentPowders = Memory.ReadByte(0x21CDD8B8);
                    currentPowders--;
                    Memory.WriteByte(0x21CDD8B8, currentPowders);
                    if (currentPowders == 0)
                    {
                        Memory.WriteUShort(0x21CDD8B2, 0);
                    }
                    Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Consumed escape powder from active slots");
                }
            }
        }

        public static void CheckMiniBossStamina()
        {
            if (MiniBoss.miniBossRolled == true)
            {
                if (Memory.ReadInt(Enemies.Enemy0.staminaTimer + (0x190 * MiniBoss.enemyNumber)) < 60)
                {
                    Memory.WriteInt(Enemies.Enemy0.staminaTimer + (0x190 * MiniBoss.enemyNumber), 60000);
                }
            }

            if (Memory.ReadByte(Addresses.dunBackFloorFlag) != 0)
            {
                MiniBoss.miniBossRolled = false;    //if player enters backfloor, remove miniboss stamina value
            }
        }

        public static void CheckWepLvlUp()
        {
            byte menuMode = Memory.ReadByte(0x202A2010);
            if (menuMode == 2 || menuMode == 1)
            {

                if (wepMenuOpen == false)
                {
                    for (int i = 0; i < wepLevelArray.Length; i++)
                    {
                        wepLevelArray[i] = Memory.ReadByte(0x21CDDA5A + (i * 0xF8));
                    }
                    wepMenuOpen = true;
                }
                else
                {
                    if (menuMode == 1) 
                    {
                        if (Memory.ReadByte(0x21D9EC08) == 6)
                        {
                            for (int i = 0; i < wepLevelArray.Length; i++)
                            {
                                wepLevelArray[i] = Memory.ReadByte(0x21CDDA5A + (i * 0xF8));
                            }
                            PPowdermenuOpen = true;
                        }
                        else
                        {
                            if (PPowdermenuOpen == true)
                            {
                                for (int i = 0; i < wepLevelArray.Length; i++)
                                {
                                    if (Memory.ReadByte(0x21CDDA5A + (i * 0xF8)) > wepLevelArray[i])
                                    {
                                        CheckSoZEffect(i);
                                        wepLevelArray[i] = Memory.ReadByte(0x21CDDA5A + (i * 0xF8));
                                    }
                                }
                            }
                            PPowdermenuOpen = false;
                        }                                            
                    }
                    else if (menuMode == 2)
                    {
                        for (int i = 0; i < wepLevelArray.Length; i++)
                        {
                            if (Memory.ReadByte(0x21CDDA5A + (i * 0xF8)) > wepLevelArray[i])
                            {
                                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Weapon(sword) leveled up!");
                                CheckSoZEffect(i);
                                wepLevelArray[i] = Memory.ReadByte(0x21CDDA5A + (i * 0xF8));
                            }
                        }
                    }
                }
            }
            else
            {
                wepMenuOpen = false;
            }
        }

        public static void CheckSoZEffect(int wepOffset)
        {
            ushort wepID = Memory.ReadUShort(Player.Toan.WeaponSlot0.id + (0xF8 * wepOffset));

            if (wepID == 296)
            {
                //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "SoZ leveled up!");
                byte currentThunder = Memory.ReadByte(Player.Toan.WeaponSlot0.thunder + (0xF8 * wepOffset));
                ushort storedThunder = (ushort)(Memory.ReadUShort(0x21CE446D) + currentThunder);
                if (storedThunder > 30000)
                {
                    storedThunder = 30000;
                }
                Memory.WriteByte(Player.Toan.WeaponSlot0.thunder + (0xF8 * wepOffset), 0);
                if (Memory.ReadByte(Player.Toan.WeaponSlot0.elementHUD + (0xF8 * wepOffset)) == 2)
                {
                    Memory.WriteByte(Player.Toan.WeaponSlot0.elementHUD + (0xF8 * wepOffset), 5);
                }
                Memory.WriteUShort(0x21CE446D, storedThunder);
                ChangeSoZMaxAtt(storedThunder);

            }
        }

        public static void ChangeSoZMaxAtt(ushort storedThunder)
        {
            ushort maxAttack = 199;
            if (storedThunder > 200)
            {
                if (storedThunder > 500)
                {
                    if (storedThunder > 1000)
                    {
                        if (storedThunder > 2000)
                        {
                            maxAttack = 599;
                            storedThunder -= 2000;

                            ushort attackboost = (ushort)(storedThunder / 20);
                            maxAttack = (ushort)(maxAttack + attackboost);
                        }
                        else
                        {
                            maxAttack = 499;
                            storedThunder -= 1000;

                            ushort attackboost = (ushort)(storedThunder / 10);
                            maxAttack = (ushort)(maxAttack + attackboost);
                        }
                    }
                    else
                    {
                        maxAttack = 399;
                        storedThunder -= 500;

                        ushort attackboost = (ushort)(storedThunder / 5);
                        maxAttack = (ushort)(maxAttack + attackboost);
                    }
                }
                else
                {
                    maxAttack = 299;
                    storedThunder -= 200;

                    ushort attackboost = (ushort)(storedThunder / 3);
                    maxAttack = (ushort)(maxAttack + attackboost);
                }
            }
            else
            {
                ushort attackboost = (ushort)(storedThunder / 2);
                maxAttack = (ushort)(maxAttack + attackboost);
                //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "maxattack: " + maxAttack);
            }
            //Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "SoZ max attack changed!");
            Memory.WriteUShort(0x2027B298, maxAttack);
        }

    }
}