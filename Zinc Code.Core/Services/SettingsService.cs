using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Zinc_Code.Core.Abstractions;

namespace Zinc_Code.Core.Services;

public class SettingsService : ISettingsService
{
    private static SettingsService? _instance;
    private static readonly object _lock = new();

    private readonly string _settingsPath;
    private Dictionary<string, object> _settings;
    private bool _isLoaded;

    public static SettingsService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new SettingsService();
                }
            }
            return _instance;
        }
    }

    private SettingsService()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ZincCode");

        if (!Directory.Exists(appDataPath))
            Directory.CreateDirectory(appDataPath);

        _settingsPath = Path.Combine(appDataPath, "settings.json");
        _settings = new Dictionary<string, object>();
        _isLoaded = false;
    }

    public void Preload()
    {
        if (_isLoaded) return;

        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var loaded = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (loaded != null)
                {
                    _settings = loaded;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"预加载设置失败: {ex.Message}");
        }

        _isLoaded = true;
    }

    public T Read<T>(string key, T defaultValue = default!)
    {
        EnsureLoaded();

        if (_settings.TryGetValue(key, out var value))
        {
            try
            {
                if (value is JsonElement element)
                {
                    return JsonSerializer.Deserialize<T>(element.GetRawText()) ?? defaultValue;
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }

    public void Write(string key, object value)
    {
        EnsureLoaded();
        _settings[key] = value;
    }

    public void WriteAndSave(string key, object value)
    {
        Write(key, value);
        Save();
    }

    public void Save()
    {
        EnsureLoaded();

        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(_settings, options);
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"保存设置失败: {ex.Message}");
        }
    }

    private void EnsureLoaded()
    {
        if (!_isLoaded)
        {
            Preload();
        }
    }
}