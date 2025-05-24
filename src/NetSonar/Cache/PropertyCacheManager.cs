using System.Collections.Generic;

namespace NetSonar.Avalonia.Cache;

public class PropertyCacheManager
{
    private Dictionary<string, PropertyCache> _propertiesCache = new();

    public void PropertyChanged()
    {

    }
}