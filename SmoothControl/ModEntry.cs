using System.Diagnostics.CodeAnalysis;
using Force.DeepCloner;
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
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        this.InitializeServices(helper);
    }

    private void InitializeServices(IModHelper helper)
    {
        this.Services.Clear();
        this.Services.Add(new WeaponQuickDrawService(this, helper, this.Config));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.LoadConfigOrDefault();
        this.IntegrateGenericModConfigMenu();
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

    private void IntegrateGenericModConfigMenu()
    {
        // get Generic Mod Config Menu's API (if it's installed)
        IGenericModConfigMenuApi? configMenu =
            this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: this.ModManifest,
            reset: () => ModConfig.Default.DeepCloneTo(this.Config),
            save: () => this.Helper.WriteConfig(this.Config)
        );

        // add config options
        // Weapon Quick Draw
        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Weapon Quick Draw",
            tooltip: () => "Allows you to quickly draw your weapon in most states."
        );
        configMenu.AddTextOption(
            mod: this.ModManifest,
            name: () => "Mode",
            getValue: () => this.Config.WeaponQuickDraw.Mode.ToString(),
            setValue: value =>
                this.Config.WeaponQuickDraw.Mode = Enum.TryParse(value, out ModConfig.WeaponQuickDrawMode result)
                    ? result
                    : ModConfig.WeaponQuickDrawMode.AttackThenRevert,
            allowedValues: Enum.GetNames(typeof(ModConfig.WeaponQuickDrawMode))
        );
        configMenu.AddKeybindList(
            mod: this.ModManifest,
            name: () => "Shortcut Key",
            getValue: () => this.Config.WeaponQuickDraw.ShortcutKey,
            setValue: value => this.Config.WeaponQuickDraw.ShortcutKey = value
        );
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
