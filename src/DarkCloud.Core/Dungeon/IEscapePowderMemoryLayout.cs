using System.Collections.Generic;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the active-item slot addresses used by <see cref="EscapePowderService"/>.
    /// </summary>
    public interface IEscapePowderMemoryLayout
    {
        IReadOnlyList<long> ActiveItemAddresses { get; }
        IReadOnlyList<long> ActiveItemCountAddresses { get; }
    }
}
