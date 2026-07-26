using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Handles using active items (Escape Powder and Repair Powder) from the
    /// in-dungeon shortcut slots. UI and timer concerns are returned to the
    /// caller so the service stays free of WinForms/threading code.
    /// </summary>
    public sealed class ActiveItemService
    {
        public const ushort SquareButton = 0x0080; // CheatCodes.InputBuffer.Button.Square
        public const byte EscapeFlagValue = 170;

        private readonly IGameMemory _memory;
        private readonly IActiveItemMemoryLayout _layout;

        public ActiveItemService(IGameMemory memory, IActiveItemMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public ActiveItemResult Process(bool squareActive, bool dunEscapeConfirm, bool dunEscapeConfirmSpamCheck)
        {
            var result = new ActiveItemResult();

            if (!TryReadUShort(_layout.ButtonInputsAddress, out ushort buttonInputs) || buttonInputs != SquareButton)
            {
                result.SquareActive = false;
                return result;
            }

            if (!TryReadByte(_layout.ActiveItemUsableFlagAddress, out byte usableFlag) || usableFlag == 0)
            {
                result.SquareActive = false;
                return result;
            }

            if (!TryReadInt(_layout.ActiveItemUsableIntAddress, out int usableInt) || usableInt != -1)
            {
                result.SquareActive = false;
                return result;
            }

            if (!TryReadInt(_layout.CurrentSlotAddress, out int currentSlot))
            {
                result.SquareActive = false;
                return result;
            }

            if (currentSlot < 1 || currentSlot > _layout.ActiveItemSlotCount)
            {
                result.SquareActive = false;
                return result;
            }

            long currentActiveItem = _layout.ActiveItemBaseAddress + ((long)_layout.ActiveItemSlotSize * currentSlot);
            if (!TryReadShort(currentActiveItem, out short itemId))
            {
                result.SquareActive = false;
                return result;
            }

            if (!TryReadByte(_layout.AnimationIdAddress, out byte animationId))
            {
                result.SquareActive = false;
                return result;
            }

            if (!IsAllowedAnimation(animationId))
            {
                result.SquareActive = false;
                return result;
            }

            if (squareActive)
            {
                result.SquareActive = true;
                return result;
            }

            if (itemId == ActiveItemConstants.EscapePowderItemId)
            {
                ProcessEscapePowder(currentSlot, currentActiveItem, dunEscapeConfirm, dunEscapeConfirmSpamCheck, result);
            }
            else if (itemId == ActiveItemConstants.RepairPowderItemId)
            {
                ProcessRepairPowder(currentSlot, currentActiveItem, result);
            }
            else
            {
                result.SquareActive = false;
            }

            return result;
        }

        private void ProcessEscapePowder(int currentSlot, long currentActiveItem, bool dunEscapeConfirm, bool dunEscapeConfirmSpamCheck, ActiveItemResult result)
        {
            if (!dunEscapeConfirm)
            {
                result.SquareActive = true;
                result.EscapeConfirmRequested = true;
                result.DisplayMessage = "^RAre you sure you want to leave?\n^WPress square to use Escape Powder.";
                return;
            }

            if (!dunEscapeConfirmSpamCheck)
            {
                result.SquareActive = true;
                return;
            }

            if (!TryReadByte(_layout.EscapeFlagAddress, out byte escapeFlag) || escapeFlag != 0)
            {
                result.SquareActive = true;
                return;
            }

            TryWriteByte(_layout.EscapeFlagAddress, EscapeFlagValue);

            long countAddress = _layout.GetPowderCountAddress(currentSlot);
            if (TryReadByte(countAddress, out byte currentPowders) && currentPowders > 0)
            {
                currentPowders--;
                TryWriteByte(countAddress, currentPowders);

                if (currentPowders == 0)
                {
                    TryWriteUShort(currentActiveItem, ActiveItemConstants.EmptyItemValue);
                }
            }

            result.SquareActive = true;
            result.EscapeActivated = true;
            result.DunUsedActiveEscape = true;
        }

        private void ProcessRepairPowder(int currentSlot, long currentActiveItem, ActiveItemResult result)
        {
            if (!TryReadUShort(_layout.CurrentWeaponMaxWhpAddress, out ushort currentMaxWHP))
            {
                result.SquareActive = false;
                return;
            }

            if (!TryReadByte(_layout.CurrentCharacterAddress, out byte currentChar))
            {
                result.SquareActive = false;
                return;
            }

            long currentWeaponSlotBase = _layout.CurrentWeaponSlotAddress + currentChar;
            if (!TryReadByte(currentWeaponSlotBase, out byte currentWepNum))
            {
                result.SquareActive = false;
                return;
            }

            long whpAddress = _layout.GetCharacterWeaponWhpAddress(currentChar, currentWepNum);
            if (!TryReadFloat(whpAddress, out float currentWHP))
            {
                result.SquareActive = false;
                return;
            }

            if (currentWHP >= currentMaxWHP)
            {
                result.SquareActive = false;
                return;
            }

            TryWriteFloat(whpAddress, currentMaxWHP);

            long countAddress = _layout.GetPowderCountAddress(currentSlot);
            if (TryReadByte(countAddress, out byte currentPowders) && currentPowders > 0)
            {
                currentPowders--;
                TryWriteByte(countAddress, currentPowders);

                if (currentPowders == 0)
                {
                    TryWriteUShort(currentActiveItem, ActiveItemConstants.EmptyItemValue);
                }
            }

            result.SquareActive = true;
            result.RepairPowderUsed = true;
            result.DisplayMessage = "Used Repair Powder!";
        }

        private static bool IsAllowedAnimation(byte animationId)
        {
            return animationId == 0 || animationId == 1 || animationId == 2 || animationId == 18;
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

        private bool TryReadShort(long address, out short value)
        {
            var buffer = new byte[2];
            if (!_memory.TryRead(address, buffer, 0, 2))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToInt16(buffer, 0);
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

        private bool TryReadInt(long address, out int value)
        {
            var buffer = new byte[4];
            if (!_memory.TryRead(address, buffer, 0, 4))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToInt32(buffer, 0);
            return true;
        }

        private bool TryReadFloat(long address, out float value)
        {
            var buffer = new byte[4];
            if (!_memory.TryRead(address, buffer, 0, 4))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToSingle(buffer, 0);
            return true;
        }

        private bool TryWriteByte(long address, byte value)
        {
            return _memory.TryWrite(address, new byte[] { value }, 0, 1);
        }

        private bool TryWriteUShort(long address, ushort value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2);
        }

        private bool TryWriteFloat(long address, float value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4);
        }
    }
}
