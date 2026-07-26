using System;
using System.Collections.Generic;
using System.Threading;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Manages Samba's Dagger challenge side quest while inside a dungeon floor.
    /// </summary>
    public sealed class SambaChallengeService
    {
        private readonly IGameMemory _memory;
        private readonly ISambaChallengeMemoryLayout _layout;

        public SambaChallengeService(IGameMemory memory, ISambaChallengeMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public SideQuestChallengeResult Process(bool questCheck, bool questActive, bool quest, bool[] monstersDead, CancellationToken cancellationToken)
        {
            var result = new SideQuestChallengeResult
            {
                QuestCheck = questCheck,
                QuestActive = questActive,
                Quest = quest,
                MonstersDead = monstersDead,
            };

            if (cancellationToken.IsCancellationRequested)
                return result;

            if (!TryReadByte(_layout.InDungeonFlagAddress, out byte inDungeonFlag))
                return result;

            if (!questCheck && inDungeonFlag == 1)
            {
                if (TryReadByte(_layout.HideHudAddress, out byte hideHud) && hideHud == 0)
                {
                    if (TryReadByte(_layout.CurrentAllyAddress, out byte currentAlly) && currentAlly == 0)
                    {
                        if (TryReadUShort(_layout.CurrentWeaponIdAddress, out ushort weaponId) && IsAllowedWeapon(weaponId))
                        {
                            TryWriteInt(_layout.QuestTimerAddress, 0);
                            result.Messages.Add(new SideQuestMessage("Samba's quest started!\nClear all enemies using only Dagger!\nUsing a throwable also\ncancels the mission.", 4, 40, 8000));
                            result.QuestActive = true;
                            ResetMonstersDead(monstersDead);
                        }
                        else
                        {
                            result.Messages.Add(new SideQuestMessage("Samba's quest did not start.\nRe-enter with Dagger equipped.", 2, 30, 4000));
                            result.QuestActive = false;
                        }
                    }

                    result.QuestCheck = true;
                }
            }
            else if (questCheck && inDungeonFlag == 0)
            {
                result.QuestCheck = false;
                result.QuestActive = false;
            }

            if (result.QuestActive)
            {
                if ((TryReadUShort(_layout.CurrentWeaponIdAddress, out ushort currentWeaponId) && !IsAllowedWeapon(currentWeaponId))
                    || IsCancelAnimation())
                {
                    result.ShouldDelayFirstMessage = true;
                    result.Messages.Add(new SideQuestMessage("Samba's quest has been cancelled.\nRe-enter in order to activate it.", 2, 40, 4000));
                    result.QuestActive = false;
                }
                else
                {
                    int enemiesKilled = TrackEnemies(monstersDead);
                    if (enemiesKilled == _layout.EnemyCount)
                    {
                        result.Messages.Add(new SideQuestMessage("Samba's quest completed!\nWell done!", 2, 28, 4000));
                        TryWriteByte(_layout.CompletionAddress, 1);
                        result.Quest = false;
                        result.QuestActive = false;
                    }
                }
            }

            return result;
        }

        private bool IsAllowedWeapon(ushort weaponId)
        {
            foreach (ushort allowed in _layout.AllowedWeaponIds)
            {
                if (weaponId == allowed)
                    return true;
            }

            return false;
        }

        private bool IsCancelAnimation()
        {
            if (!TryReadByte(_layout.AnimationIdAddress, out byte animationId))
                return false;

            return animationId == 26 || animationId == 27;
        }

        private void ResetMonstersDead(bool[] monstersDead)
        {
            for (int i = 0; i < monstersDead.Length; i++)
            {
                monstersDead[i] = false;
            }
        }

        private int TrackEnemies(bool[] monstersDead)
        {
            int enemiesKilled = 0;
            for (int i = 0; i < _layout.EnemyCount; i++)
            {
                long address = _layout.GetEnemyHpAddress(i);
                if (!TryReadUShort(address, out ushort hp))
                    continue;

                if (hp > 0)
                {
                    monstersDead[i] = false;
                }
                else
                {
                    monstersDead[i] = true;
                    enemiesKilled++;
                }
            }

            return enemiesKilled;
        }

        private bool TryReadByte(long address, out byte value)
        {
            var buffer = new byte[1];
            if (!_memory.TryRead(address, buffer, 0, 1))
            {
                value = 0;
                return false;
            }

            value = buffer[0];
            return true;
        }

        private bool TryReadUShort(long address, out ushort value)
        {
            var buffer = new byte[2];
            if (!_memory.TryRead(address, buffer, 0, 2))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToUInt16(buffer, 0);
            return true;
        }

        private bool TryWriteByte(long address, byte value)
        {
            return _memory.TryWrite(address, new byte[] { value }, 0, 1);
        }

        private bool TryWriteInt(long address, int value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4);
        }
    }
}
