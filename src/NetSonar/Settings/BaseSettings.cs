using CommunityToolkit.Mvvm.ComponentModel;

namespace NetSonar.Avalonia.Settings;

public class BaseSettings : ObservableObject
{
    public virtual void OnLoaded(bool fromFile)
    {

    }
}