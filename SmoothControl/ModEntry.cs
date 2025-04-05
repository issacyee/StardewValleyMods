using System.Diagnostics.CodeAnalysis;
using Issacyee.SmoothControl.Service;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Issacyee.SmoothControl;

[SuppressMessage("ReSharper", "UnusedType.Global")]
internal sealed class ModEntry : Mod
{
    private List<BaseService> Services = [];

    public override void Entry(IModHelper helper)
    {
        this.InitializeServices(helper);
        helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    private void InitializeServices(IModHelper helper)
    {
        this.Services.Clear();
        this.Services.Add(new WeaponQuickDrawService(this, helper));
    }

    private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        foreach (BaseService service in this.Services) service._UpdateTicking(e);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        foreach (BaseService service in this.Services) service._ButtonPressed(e);
    }
}
