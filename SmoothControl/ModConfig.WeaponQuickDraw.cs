using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Issacyee.SmoothControl;

public partial class ModConfig
{
    public WeaponQuickDrawConfig WeaponQuickDraw { get; set; } = new();

    public class WeaponQuickDrawConfig
    {
        public WeaponQuickDrawMode Mode { get; set; } = WeaponQuickDrawMode.AttackThenRevert;

        public KeybindList ShortcutKey { get; set; } = new(SButton.Q);
    }

    public enum WeaponQuickDrawMode
    {
        AttackThenRevert,
        CounterStrikeLike,
    }
}
