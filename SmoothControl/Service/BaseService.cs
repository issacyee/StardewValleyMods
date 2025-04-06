using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Issacyee.SmoothControl.Service;

public abstract class BaseService(IMod mod, IModHelper helper, ModConfig config)
{
    protected IMod Mod { get; init; } = mod;
    protected IModHelper Helper { get; init; } = helper;
    protected ModConfig Config { get; set; } = config;

    internal virtual void _UpdateTicking(UpdateTickingEventArgs e)
    {
    }

    internal virtual void _ButtonPressed(ButtonPressedEventArgs e)
    {
    }

    #region Logging

    protected void Verbos(string message)
    {
        this.Mod.Monitor.VerboseLog(message);
    }

    protected void Trace(string message)
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        this.Mod.Monitor.Log(message, LogLevel.Trace);
    }

    protected void Debug(string message)
    {
        this.Mod.Monitor.Log(message, LogLevel.Debug);
    }

    protected void Info(string message)
    {
        this.Mod.Monitor.Log(message, LogLevel.Info);
    }

    protected void Warn(string message)
    {
        this.Mod.Monitor.Log(message, LogLevel.Warn);
    }

    protected void Error(string message)
    {
        this.Mod.Monitor.Log(message, LogLevel.Error);
    }

    protected void Alert(string message)
    {
        this.Mod.Monitor.Log(message, LogLevel.Alert);
    }

    protected void TraceOnce(string message)
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        this.Mod.Monitor.LogOnce(message, LogLevel.Trace);
    }

    protected void DebugOnce(string message)
    {
        this.Mod.Monitor.LogOnce(message, LogLevel.Debug);
    }

    protected void InfoOnce(string message)
    {
        this.Mod.Monitor.LogOnce(message, LogLevel.Info);
    }

    protected void WarnOnce(string message)
    {
        this.Mod.Monitor.LogOnce(message, LogLevel.Warn);
    }

    protected void ErrorOnce(string message)
    {
        this.Mod.Monitor.LogOnce(message, LogLevel.Error);
    }

    protected void AlertOnce(string message)
    {
        this.Mod.Monitor.LogOnce(message, LogLevel.Alert);
    }

    #endregion
}
