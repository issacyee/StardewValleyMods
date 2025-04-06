using System.Diagnostics.CodeAnalysis;
using Issacyee.SmoothControl.Service;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Issacyee.SmoothControl;

[SuppressMessage("ReSharper", "UnusedType.Global")]
internal sealed class ModEntry : Mod
{
    public ModConfig Config { get; private set; } = ModConfig.Default;

    private List<BaseService> Services = [];

    public override void Entry(IModHelper helper)
    {
        this.LoadConfigOrDefault();
        this.InitializeServices(helper);
        helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    private void LoadConfigOrDefault()
    {
        try
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
        }
        catch (Exception)
        {
            this.Helper.WriteConfig(this.Config = ModConfig.Default);
        }
    }

    private void InitializeServices(IModHelper helper)
    {
        this.Services.Clear();
        this.Services.Add(new WeaponQuickDrawService(this, helper, this.Config));
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
