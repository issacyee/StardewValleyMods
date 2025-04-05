using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Inventories;

namespace Issacyee.SmoothControl.Service;

/// <summary>
/// Sword Quick Draw feature, allowing players to instantly unsheathe their blade during any state.
/// </summary>
public class SwordQuickDrawService : IService
{
    public readonly int CategorySword = -98;

    private int? OriginalItem;

    public void _UpdateTicking(UpdateTickingEventArgs e)
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

    public void _ButtonPressed(ButtonPressedEventArgs e)
    {
        if (e.Button != SButton.Q) return;
        if (this.TryGetSwordItem(out int sowrdItem))
            this.ExecuteSwordAttack(sowrdItem);
    }

    private bool TryGetSwordItem(out int sowrdItem)
    {
        sowrdItem = -1;

        Inventory inventory = Game1.player.Items;
        for (int i = 0; i < inventory.Count; i++)
        {
            Item item = inventory[i];
            if (item is null) continue;
            if (item.Category == this.CategorySword)
            {
                sowrdItem = i;
                return true;
            }
        }

        return false;
    }

    private void ExecuteSwordAttack(int swordItem)
    {
        Farmer player = Game1.player;
        if (!this.IsValidItemIndex(player, swordItem)) return;
        if (this.IsUsingSword(player)) return;
        this.OriginalItem = player.CurrentToolIndex;
        player.CurrentToolIndex = swordItem;
        player.BeginUsingTool();
    }

    private bool IsValidItemIndex(Farmer player, int itemIndex)
    {
        return itemIndex >= 0 && itemIndex < player.Items.Count;
    }

    private bool IsUsingSword(Farmer player)
    {
        if (player.CurrentItem is null) return false;
        return player.CurrentItem.Category == this.CategorySword && player.UsingTool;
    }
}
