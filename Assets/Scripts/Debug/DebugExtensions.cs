using Managers;
using UnityEngine;
public static class DebugExtensions
{
    public static string GetCurrentDateString() => TimeManager.Instance.GetCurrentTimeString();
    public static void ConsoleGetCurrentDate() => Debug.Log(GetCurrentDateString());

    public static void WPCategoryNotFound(string category)
        => Debug.LogError($"Work Place Category Not Found!: {category}");
}
