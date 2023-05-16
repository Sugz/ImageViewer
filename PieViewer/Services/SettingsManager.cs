using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ImageViewer.Services;

internal sealed class SettingsManager
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private JsonObject? _settingsStorage;

    public void Serialize<T>(T serializable) where T : ISerializable
    {
        _settingsStorage ??= new JsonObject();

        JsonNode node = JsonSerializer.SerializeToNode(serializable, _jsonOptions)!;

        if (_settingsStorage.ContainsKey(serializable.GetSerializedName()))
            _settingsStorage.Remove(serializable.GetSerializedName());
        _settingsStorage.Add(serializable.GetSerializedName(), node);
    }

    public void Deserialize<T>(T serializable) where T : ISerializable
    {
        if (_settingsStorage is not null && _settingsStorage[serializable.GetSerializedName()] is JsonNode jsonNode)
            serializable.Deserialize(jsonNode);

    }

    public void Save()
    {
        string jsonString = JsonSerializer.Serialize(_settingsStorage, _jsonOptions);
        File.WriteAllText(_filePath, jsonString);
    }

    private void Load()
    {
        _settingsStorage = JsonSerializer.Deserialize<JsonObject>(File.ReadAllText(_filePath)); 
    }



    public SettingsManager()
    {
        string settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Sugz", "PieViewer");
        Directory.CreateDirectory(settingsDirectory);

        _filePath = Path.Combine(settingsDirectory, "UserSettings.json");
        if (File.Exists(_filePath))
            Load();
    }
}