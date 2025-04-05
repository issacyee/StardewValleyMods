using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Issacyee.SmoothControl.Service;

public interface IService
{
    void _Entry(IModHelper helper)
    {
    }

    void _UpdateTicking(UpdateTickingEventArgs e)
    {
    }

    void _ButtonPressed(ButtonPressedEventArgs e)
    {
    }
}
