using System.Diagnostics.CodeAnalysis;
using Issacyee.SmoothControl.Service;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Issacyee.SmoothControl;

[SuppressMessage("ReSharper", "UnusedType.Global")]
internal sealed class ModEntry : Mod
{
    private List<IService> Services = [];

    public override void Entry(IModHelper helper)
    {
        this.InitializeServices();
        foreach (IService service in this.Services) service._Entry(helper);
        helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    private void InitializeServices()
    {
        this.Services.Clear();
        this.Services.Add(new SwordQuickDrawService());
    }

    private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        foreach (IService service in this.Services) service._UpdateTicking(e);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        foreach (IService service in this.Services) service._ButtonPressed(e);
    }
}
