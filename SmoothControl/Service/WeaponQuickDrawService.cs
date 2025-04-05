using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Inventories;

namespace Issacyee.SmoothControl.Service;

/// <summary>
/// Weapon Quick Draw feature, allowing players to instantly unsheathe their blade during any state.
/// </summary>
public class WeaponQuickDrawService(IMod mod, IModHelper helper) : BaseService(mod, helper)
{
    private int? OriginalItem;

    internal override void _UpdateTicking(UpdateTickingEventArgs e)
    {
        if (this.TryGetOriginalItem(out int originalItem))
            this.RevertToOriginalItem(originalItem);
    }

    private bool TryGetOriginalItem(out int originalItem)
    {
        originalItem = -1;
        Farmer player = Game1.player;
        if (player.UsingTool) return false;
        if (this.OriginalItem is null) return false;
        originalItem = this.OriginalItem.Value;
        if (!this.IsValidItemIndex(player, originalItem)) return false;

        return true;
    }

    private void RevertToOriginalItem(int originalItem)
    {
        Farmer player = Game1.player;
        if (this.IsValidItemIndex(player, originalItem)) player.CurrentToolIndex = originalItem;
        this.OriginalItem = null;
    }

    internal override void _ButtonPressed(ButtonPressedEventArgs e)
    {
        if (e.Button != SButton.Q) return;
        if (this.TryGetWeaponItem(out int sowrdItem))
            this.ExecuteWeaponAttack(sowrdItem);
    }

    private bool TryGetWeaponItem(out int sowrdItem)
    {
        sowrdItem = -1;

        Inventory inventory = Game1.player.Items;
        for (int i = 0; i < inventory.Count; i++)
        {
            Item item = inventory[i];
            if (item is null) continue;
            if (item.Category == Object.weaponCategory)
            {
                sowrdItem = i;
                return true;
            }
        }

        return false;
    }

    private void ExecuteWeaponAttack(int weaponItem)
    {
        if (!Context.IsPlayerFree) return;
        Farmer player = Game1.player;
        if (!this.IsValidItemIndex(player, weaponItem)) return;
        if (this.IsUsingWeapon(player)) return;
        this.OriginalItem = player.CurrentToolIndex;
        player.CurrentToolIndex = weaponItem;
        if (player.usingSlingshot) player.usingSlingshot = false;
        player.BeginUsingTool();
    }

    private bool IsValidItemIndex(Farmer player, int itemIndex)
    {
        return itemIndex >= 0 && itemIndex < player.Items.Count;
    }

    private bool IsUsingWeapon(Farmer player)
    {
        if (player.CurrentItem is null) return false;
        return player.CurrentItem.Category == Object.weaponCategory && player.UsingTool;
    }
}
