namespace Issacyee.SmoothControl;

public partial class ModConfig
{
    public WeaponQuickDrawConfig WeaponQuickDraw { get; set; } = new();

    public class WeaponQuickDrawConfig
    {
        public WeaponQuickDrawMode Mode { get; set; } = WeaponQuickDrawMode.AttackThenRevert;
    }

    public enum WeaponQuickDrawMode
    {
        AttackThenRevert,
        CounterStrikeLike,
    }
}
