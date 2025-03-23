using UnityEngine;

public static class GameLogger
{
    public static bool EnableLogging = true;

    public static void Log(string message)
    {
        if (EnableLogging)
            Debug.Log("[LOG] " + message);
    }

    public static void Warn(string message)
    {
        if (EnableLogging)
            Debug.LogWarning("[WARN] " + message);
    }

    public static void Error(string message)
    {
        Debug.LogError("[ERROR] " + message);
    }
}
