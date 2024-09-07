using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace MiniUpdater;

internal static class SettingsHandler
{
    public static JObject? Settings { get; private set; }

    public static void Setup(bool forServer)
    {
        //MAIN SETTINGS
        var fileName = forServer ? "Server" : "Client";
        var json = File.ReadAllText($"./{fileName}Settings.json");
        Settings = JObject.Parse(json);
    }

    public static void Save(bool forServer)
    {
        if (Settings == null)
            throw new InvalidOperationException("Settings not initialized.");

        var json = Settings.ToString(); // Converts JObject back to JSON string
        var fileName = forServer ? "Server" : "Client";
        File.WriteAllText($"./{fileName}Settings.json", json);
    }
    
    // Example method to get a setting value
    public static T GetSetting<T>(string key)
    {
        if (Settings is null)
            throw new InvalidOperationException("Settings not initialized.");

        if (!Settings.TryGetValue(key, out var setting))
            throw new KeyNotFoundException();

        if (setting.ToObject<T>() is null)
            throw new SerializationException();
        
        return setting.ToObject<T>()!;
    }

    // Example method to update a setting value
    public static void UpdateSetting(string key, object value)
    {
        if (Settings == null)
            throw new InvalidOperationException("Settings not initialized.");
        
        Settings[key] = JToken.FromObject(value);
    }
    
}