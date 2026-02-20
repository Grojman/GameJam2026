using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigManager
{
    private Dictionary<string, string> config = new Dictionary<string, string>();

    public ConfigManager(string path)
    {
        LoadFile(path);
    }

    private void LoadFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Archivo de configuración no encontrado: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(new string[] { "//" }, StringSplitOptions.None);

            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                config[key] = value;
            }
        }
    }

    public T Get<T>(string key, T defaultValue = default)
    {
        if (!config.ContainsKey(key))
            return defaultValue;

        string value = config[key];


        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            Debug.LogWarning($"No se pudo convertir la clave '{key}' al tipo {typeof(T)}");
            return defaultValue;
        }
    }
}
