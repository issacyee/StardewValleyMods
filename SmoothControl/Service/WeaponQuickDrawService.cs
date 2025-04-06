using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Inventories;

namespace Issacyee.SmoothControl.Service;

/// <summary>
/// Weapon Quick Draw feature, allowing player to quickly draw your weapon in most states.
/// </summary>
public class WeaponQuickDrawService : BaseService
{
    private Dictionary<ModConfig.WeaponQuickDrawMode, BaseWeaponQuickDrawStrategy> Strategies;

    public WeaponQuickDrawService(IMod mod, IModHelper helper, ModConfig config) : base(mod, helper, config)
    {
        IEnumerable<BaseWeaponQuickDrawStrategy> strategies =
        [
            new AttackThenRevertStrategy(),
            new CounterStrikeLikeStrategy(),
        ];
        this.Strategies = strategies.ToDictionary(x => x.Mode, x => x);
    }

    internal override void _UpdateTicking(UpdateTickingEventArgs e)
    {
        if (this.Strategies.TryGetValue(this.Config.WeaponQuickDraw.Mode, out BaseWeaponQuickDrawStrategy? strategy))
        {
            strategy._UpdateTicking(e);
            if (this.Config.WeaponQuickDraw.ShortcutKey.JustPressed())
                strategy.ExecuteWeaponQuickDraw();
        }
    }

    private abstract class BaseWeaponQuickDrawStrategy(ModConfig.WeaponQuickDrawMode mode)
    {
        public ModConfig.WeaponQuickDrawMode Mode { get; init; } = mode;

        protected int? PreviousItem;

        public abstract void ExecuteWeaponQuickDraw();

        public virtual void _UpdateTicking(UpdateTickingEventArgs e)
        {
        }

        protected bool TryGetWeaponItem(out int weaponItem)
        {
            weaponItem = -1;

            Inventory inventory = Game1.player.Items;
            for (int i = 0; i < inventory.Count; i++)
            {
                Item item = inventory[i];
                if (item is null) continue;
                if (item.Category == Object.weaponCategory)
                {
                    weaponItem = i;
                    return true;
                }
            }

            return false;
        }

        protected bool IsValidItemIndex(Farmer player, int itemIndex)
        {
            return itemIndex >= 0 && itemIndex < player.Items.Count;
        }

        protected bool IsUsingWeapon(Farmer player)
        {
            if (player.CurrentItem is null) return false;
            return player.CurrentItem.Category == Object.weaponCategory && player.UsingTool;
        }
    }

    private class AttackThenRevertStrategy : BaseWeaponQuickDrawStrategy
    {
        public AttackThenRevertStrategy() : base(ModConfig.WeaponQuickDrawMode.AttackThenRevert)
        {
        }

        public override void ExecuteWeaponQuickDraw()
        {
            if (this.TryGetWeaponItem(out int weaponItem))
                this.ExecuteWeaponAttack(weaponItem);
        }

        private void ExecuteWeaponAttack(int weaponItem)
        {
            if (!Context.IsPlayerFree) return;
            Farmer player = Game1.player;
            if (!this.IsValidItemIndex(player, weaponItem)) return;
            if (this.IsUsingWeapon(player)) return;

            if (player.usingSlingshot) player.usingSlingshot = false;
            this.PreviousItem = player.CurrentToolIndex;
            player.CurrentToolIndex = weaponItem;
            player.BeginUsingTool();
        }

        public override void _UpdateTicking(UpdateTickingEventArgs e)
        {
            if (this.TryGetPreviousItem(out int previousItem))
                this.RevertToPreviousItem(previousItem);
        }

        private bool TryGetPreviousItem(out int previousItem)
        {
            previousItem = -1;
            Farmer player = Game1.player;
            if (player.UsingTool) return false;
            if (this.PreviousItem is null) return false;
            previousItem = this.PreviousItem.Value;
            if (!this.IsValidItemIndex(player, previousItem)) return false;

            return true;
        }

        private void RevertToPreviousItem(int previousItem)
        {
            Farmer player = Game1.player;
            if (this.IsValidItemIndex(player, previousItem)) player.CurrentToolIndex = previousItem;
            this.PreviousItem = null;
        }
    }

    private class CounterStrikeLikeStrategy : BaseWeaponQuickDrawStrategy
    {
        public CounterStrikeLikeStrategy() : base(ModConfig.WeaponQuickDrawMode.CounterStrikeLike)
        {
        }

        public override void ExecuteWeaponQuickDraw()
        {
            if (!this.TryGetWeaponItem(out int weaponItem)) return;

            Farmer player = Game1.player;
            if (player.usingSlingshot) player.usingSlingshot = false;
            if (weaponItem == player.CurrentToolIndex)
            {
                if (this.PreviousItem is not null)
                    player.CurrentToolIndex = this.PreviousItem.Value;
            }
            else
                player.CurrentToolIndex = weaponItem;
        }

        public override void _UpdateTicking(UpdateTickingEventArgs e)
        {
            Farmer player = Game1.player;
            if (player.CurrentItem?.Category == Object.weaponCategory) return;
            this.PreviousItem = player.CurrentToolIndex;
        }
    }
}
