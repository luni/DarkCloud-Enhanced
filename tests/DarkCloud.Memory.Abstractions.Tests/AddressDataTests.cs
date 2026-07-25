using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace DarkCloud.Memory.Abstractions.Tests
{
    public class AddressDataTests
    {
        private static readonly HashSet<string> ValidDataTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "UInt64", "Int64",
            "Single", "Double", "Boolean", "String"
        };

        private class AddressEntry
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("dataType")]
            public string DataType { get; set; }

            [JsonPropertyName("ntsc")]
            public string Ntsc { get; set; }

            [JsonPropertyName("pal")]
            public string Pal { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }
        }

        private class AddressRoot
        {
            [JsonPropertyName("addresses")]
            public List<AddressEntry> Addresses { get; set; }
        }

        private static AddressRoot LoadRoot()
        {
            string path = FindAddressDataFile();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AddressRoot>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private static string FindAddressDataFile()
        {
            DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                string candidate = Path.Combine(directory.FullName, "data", "addresses.json");
                if (File.Exists(candidate))
                    return candidate;
                directory = directory.Parent;
            }

            throw new FileNotFoundException("Could not find data/addresses.json from the test output directory.");
        }

        private static long ParseHex(string value)
        {
            string cleaned = value.Trim();
            if (cleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring(2);
            return long.Parse(cleaned, System.Globalization.NumberStyles.HexNumber);
        }

        [Fact]
        public void AddressData_ParsesSuccessfully()
        {
            var root = LoadRoot();

            Assert.NotNull(root);
            Assert.NotNull(root.Addresses);
            Assert.NotEmpty(root.Addresses);
        }

        [Fact]
        public void AddressData_NamesAreUnique()
        {
            var root = LoadRoot();
            var names = root.Addresses.Select(a => a.Name).ToList();

            Assert.Equal(names.Count, names.Distinct(StringComparer.Ordinal).Count());
        }

        [Fact]
        public void AddressData_HexValuesAreValid()
        {
            var root = LoadRoot();

            foreach (var entry in root.Addresses)
            {
                Assert.True(long.TryParse(entry.Ntsc.Trim().Substring(2), System.Globalization.NumberStyles.HexNumber, null, out _),
                    $"Invalid NTSC hex value for {entry.Name}: {entry.Ntsc}");
                Assert.True(long.TryParse(entry.Pal.Trim().Substring(2), System.Globalization.NumberStyles.HexNumber, null, out _),
                    $"Invalid PAL hex value for {entry.Name}: {entry.Pal}");
            }
        }

        [Fact]
        public void AddressData_TypesAreValid()
        {
            var root = LoadRoot();

            foreach (var entry in root.Addresses)
            {
                Assert.Contains(entry.DataType, ValidDataTypes);
            }
        }

        [Fact]
        public void AddressData_AddressesAreInPs2Range()
        {
            const long ps2Base = 0x20000000L;
            const long ps2End = 0x21FFFFFFL; // rough uncached/user memory range
            var root = LoadRoot();

            foreach (var entry in root.Addresses)
            {
                long ntsc = ParseHex(entry.Ntsc);
                long pal = ParseHex(entry.Pal);

                Assert.InRange(ntsc, ps2Base, ps2End);
                Assert.InRange(pal, ps2Base, ps2End);
            }
        }

        [Fact]
        public void AddressData_IsSortedByName()
        {
            var root = LoadRoot();
            var names = root.Addresses.Select(a => a.Name).ToList();

            Assert.Equal(names, names.OrderBy(n => n, StringComparer.Ordinal).ToList());
        }
    }
}
