using System;
using System.Collections.Generic;

namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// Applies a collection of weapon balance changes to the in-memory weapon database.
    /// </summary>
    public sealed class WeaponBalanceService
    {
        private readonly WeaponStatService _statService;
        private readonly IReadOnlyList<IWeaponBalanceChange> _changes;

        public WeaponBalanceService(WeaponStatService statService, IReadOnlyList<IWeaponBalanceChange> changes)
        {
            _statService = statService ?? throw new ArgumentNullException(nameof(statService));
            _changes = changes ?? throw new ArgumentNullException(nameof(changes));
        }

        public void ApplyAll()
        {
            foreach (var change in _changes)
            {
                change.Apply(_statService);
            }
        }
    }
}
