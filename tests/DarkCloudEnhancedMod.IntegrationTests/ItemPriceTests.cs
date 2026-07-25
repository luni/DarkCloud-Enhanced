using System;
using Xunit;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    public class ItemPriceTests
    {
        static ItemPriceTests()
        {
            // Items has a static constructor that reads memory on first use.
            // Ensure a safe backend and process handle are available before that happens.
            SnapshotTestHelper.UseSnapshot(SnapshotTestHelper.CreateEmptyRam());
        }

        [Fact]
        public void Item_ValueBuy_Write_UpdatesMemory()
        {
            byte[] ram = SnapshotTestHelper.CreateEmptyRam();
            const int tableOffset = Addresses.ItemPriceTable - (int)SnapshotTestHelper.Ps2BaseAddress;

            // Initialize the price table with deterministic buy/sell prices.
            for (int i = 0; i < 376; i++)
            {
                int offset = tableOffset + i * 4;
                ram[offset] = 10;
                ram[offset + 1] = 0;
                ram[offset + 2] = 5;
                ram[offset + 3] = 0;
            }

            SnapshotTestHelper.UseSnapshot(ram);
            Items.PriceList = Memory.ReadByteArray(Addresses.ItemPriceTable, 1504);

            Items.item81.ValueBuy = 12345;

            ushort index = Items.GetPurchasePriceIndex(81);
            ushort newPrice = Memory.ReadUShort(Addresses.ItemPriceTable + index);
            Assert.Equal(12345, newPrice);
        }
    }
}
