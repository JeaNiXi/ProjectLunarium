using Managers;
using UnityEngine;

public static class DebugExtensions
{
    public static string GetCurrentDateString() => TimeManager.Instance.GetCurrentTimeString();
    public static void ConsoleGetCurrentDate() => Debug.Log(GetCurrentDateString());
}
