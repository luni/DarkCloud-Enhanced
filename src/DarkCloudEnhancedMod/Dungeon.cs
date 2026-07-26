using System;
using System.Threading;
using System.Collections.Generic;
using DarkCloud.Core.Dungeon;
using DarkCloudEnhancedMod.Logging;
using static DarkCloud.Core.Dungeon.DungeonProgression;

namespace DarkCloudEnhancedMod
{
    public class Dungeon
    {
        static byte currentDungeon;
        static byte currentFloor;
        static ushort currentWeapon;
        static long currentAddress;
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
        static WeaponLevelUpService _weaponLevelUpService;
        private static readonly Lazy<LegacyProcessGameMemory> _memory = new Lazy<LegacyProcessGameMemory>(() => new LegacyProcessGameMemory());
        private static readonly Lazy<DungeonMemoryLayout> _layout = new Lazy<DungeonMemoryLayout>(() => new DungeonMemoryLayout());
        static bool circlePressed = false;
        static bool hasClearMessageShown = false;
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
                                if (magicCircleChanged) CustomEffects.SecretArmletDisable(); magicCircleChanged = false;

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
                                        if (!magicCircleChanged)
                                        {
                                            bool executed = CustomEffects.SecretArmletEnable();
                                            if (executed) magicCircleChanged = true;
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
                            if (!IsEventFloor(currentDungeon, currentFloor))
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

        public static void CheckEnemyKill(long currentEnemyAddress, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Checking quest...");

            IReadOnlyList<MonsterQuestDefinition> quests = _layout.Value.MonsterQuestDefinitions;
            var service = new MonsterQuestService(_memory.Value, quests);
            var active = new[] { monsterQuestMachoActive, monsterQuestGobActive, monsterQuestJakeActive, monsterQuestChiefActive };
            MonsterQuestResult result = service.Process(currentEnemyAddress, active);

            foreach (int index in result.ProgressedQuestIndices)
            {
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest progress +1!");
            }

            foreach (int index in result.CompletedQuestIndices)
            {
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Quest complete!!");
                MonsterQuestDefinition quest = quests[index];
                Dayuppy.DisplayMessage(quest.CompletionMessage, 2, quest.DisplayHeight, 4000, cancellationToken: cancellationToken);

                switch (index)
                {
                    case 0: monsterQuestMachoActive = false; break;
                    case 1: monsterQuestGobActive = false; break;
                    case 2: monsterQuestJakeActive = false; break;
                    case 3: monsterQuestChiefActive = false; break;
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

            var spawnService = new SpawnDetectionService(_memory.Value, _layout.Value);
            enemiesSpawn = spawnService.WaitForSpawn(prevFloor, cancellationToken);

            //Get all the current floor enemy ids
            List<ushort> enemyFloorIds = Enemies.GetFloorEnemiesIds();

            //Calculate the amount of non-flying enemies in the floor
            byte numNormalEnemies = 0;
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
            if (hasMiniBoss)
            {
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

            var service = new MiniBossMessageService(_memory.Value, _layout.Value);
            service.WaitAndDisplay(cancellationToken, (message, token) =>
                Dayuppy.DisplayMessage(message, 2, 24, 4000, cancellationToken: token));

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Finished message process!");
        }

        /// <summary>
        /// Returns true if the bone door opening trigger is active
        /// </summary>
        public static bool IsBypassBoneDoor()
        {
            var service = new BoneDoorService(_memory.Value, _layout.Value);
            return service.IsOpen();
        }

        /// <summary>
        /// Activates or deactivates the door trigger
        /// </summary>
        /// <param name="flag">True if to activate the door</param>
        public static void SetBypassBoneDoor(bool flag)
        {
            var service = new BoneDoorService(_memory.Value, _layout.Value);
            service.SetOpen(flag);
        }

        public static void FixUngagaDoors(byte currentdng)
        {
            var service = new UngagaDoorService(_memory.Value, _layout.Value);
            if (service.TryFixDoors(currentdng))
            {
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Fixed Ungaga Doors");
            }
            else
            {
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Couldn't fix ungaga doors, or they were fixed already");
            }
        }

        public static void CheckUngagaSwap(CancellationToken cancellationToken = default)
        {
            if (!TryReadByte(_layout.Value.CurrentCharacterCursorAddress, out byte currentChar))
                return;

            currentCharCursor = currentChar;

            if (currentCharCursor != prevCharCursor)
            {
                if (currentCharCursor == 4)
                {
                    var service = new UngagaSwapService(_memory.Value, _layout.Value);
                    int timer = 0;
                    while (timer < 10)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        ThreadingHelper.Sleep(100, cancellationToken);
                        timer++;

                        if (!TryReadByte(_layout.Value.DungeonIndicatorAddress, out byte dungeon))
                            continue;

                        if (service.IsModelLoaded(dungeon))
                            break;
                    }

                    service.TryWriteUngagaModel();
                }
            }

            prevCharCursor = currentCharCursor;
        }

        public static void CheckClown()
        {
            int clownValue = Memory.ReadInt(Addresses.clownCheck);
            var service = new ClownService();
            clownOnScreen = service.Check(clownValue, eventfloor, clownOnScreen, () => CustomChests.ClownRandomizer(chronicle2));
        }

        public static void CheckSidequests(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var service = new SideQuestStateService(_memory.Value, _layout.Value);
            var state = service.GetState((byte)currentDungeon, (byte)currentFloor);
            sambaChallengeQuest = state.SambaChallengeActive;
            mayorQuest = state.MayorQuestActive;
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

                        currentAddress = _layout.Value.GetEnemyHpAddress(i);

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
            var service = new SambaChallengeService(_memory.Value, _layout.Value);
            SideQuestChallengeResult result = service.Process(sambaChallengeQuestCheck, sambaChallengeQuestActive, sambaChallengeQuest, monstersDead, cancellationToken);

            sambaChallengeQuestCheck = result.QuestCheck;
            sambaChallengeQuestActive = result.QuestActive;
            sambaChallengeQuest = result.Quest;

            DisplaySideQuestMessages(result, cancellationToken);
        }

        public static void MayorQuest(CancellationToken cancellationToken = default)
        {
            var service = new MayorQuestService(_memory.Value, _layout.Value);
            SideQuestChallengeResult result = service.Process(mayorQuestCheck, mayorQuestActive, mayorQuest, monstersDead, cancellationToken);

            mayorQuestCheck = result.QuestCheck;
            mayorQuestActive = result.QuestActive;
            mayorQuest = result.Quest;

            DisplaySideQuestMessages(result, cancellationToken);
        }

        private static void DisplaySideQuestMessages(SideQuestChallengeResult result, CancellationToken cancellationToken)
        {
            for (int i = 0; i < result.Messages.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (i == 0 && result.ShouldDelayFirstMessage)
                {
                    ThreadingHelper.Sleep(500, cancellationToken);
                }

                SideQuestMessage message = result.Messages[i];
                Dayuppy.DisplayMessage(message.Text, message.Height, message.Width, message.DisplayTime, cancellationToken: cancellationToken);
            }
        }

        public static void FloorSelectionScreen()
        {
            var service = new FloorSelectionService(_memory.Value, _layout.Value);
            service.Update(ref circlePressed, out currentGilda);
        }

        public static void CheckActiveItems(CancellationToken cancellationToken)
        {
            var service = new ActiveItemService(_memory.Value, _layout.Value);
            var result = service.Process(squareActive, dunEscapeConfirm, dunEscapeConfirmSpamCheck);

            squareActive = result.SquareActive;

            if (result.EscapeConfirmRequested)
            {
                Dayuppy.DisplayMessage(result.DisplayMessage, 2, 36, 3000, cancellationToken: cancellationToken);
                dunEscapeConfirmThread = new Thread(() => DunEscapeConfirmTimer(cancellationToken)) { IsBackground = true };
                dunEscapeConfirmThread.Start();
                dunEscapeConfirm = true;
                dunEscapeConfirmSpamCheck = false;
            }
            else if (result.EscapeActivated)
            {
                dunUsedActiveEscape = true;
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Activated escape powder!");
            }
            else if (result.RepairPowderUsed)
            {
                Dayuppy.DisplayMessage(result.DisplayMessage, 1, 20, 2000, cancellationToken: cancellationToken);
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
                if (TryReadByte(_layout.Value.EscapeFlagAddress, out byte escapeFlag) && escapeFlag == 171)
                {
                    CheckEscapePowders();
                    dunUsedEscapeCheck = true;
                }
            }
        }

        public static void CheckEscapePowders()
        {
            var service = new EscapePowderService(
                _memory.Value,
                _layout.Value,
                () => SideQuestManager.CheckItemQuestReward(175, true, false));

            if (service.TryConsumeEscapePowder())
            {
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Consumed escape powder from active slots");
            }
        }

        public static void CheckMiniBossStamina()
        {
            var service = new MiniBossStaminaService(_memory.Value, _layout.Value);
            MiniBoss.miniBossRolled = service.Update(MiniBoss.enemyNumber, MiniBoss.miniBossRolled);
        }

        public static void CheckWepLvlUp()
        {
            if (_weaponLevelUpService == null)
            {
                var memory = _memory.Value;
                var layout = _layout.Value;
                var sozService = new SwordOfZeusService(memory, layout);
                _weaponLevelUpService = new WeaponLevelUpService(memory, layout, sozService, new ConsoleModLogger());
            }

            _weaponLevelUpService.Update();
        }

        public static void RecalculateSwordOfZeusMaxAttack()
        {
            var service = new SwordOfZeusService(_memory.Value, _layout.Value);
            service.RecalculateMaxAttack();
        }

        private static bool TryReadByte(long address, out byte value)
        {
            var buffer = new byte[1];
            if (!_memory.Value.TryRead(address, buffer, 0, 1))
            {
                value = 0;
                return false;
            }

            value = buffer[0];
            return true;
        }

    }
}
