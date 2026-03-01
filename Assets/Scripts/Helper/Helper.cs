using System;
using UnityEngine.UIElements;

public static class Helper
{
    public static void SetLabelText(Label label, string text) =>
        label.text = text;
    public static void SetLabelText(Label label, Func<ulong> dataFunc) =>
        label.text = dataFunc().ToString();
    public static void SetLabelText(Label label, Func<string> dataFunc) =>
        label.text = dataFunc();
    public static void SetLabelCurrentXMaxText(Label label, ulong currentValue, ulong maxValue) =>
        label.text = $"{currentValue:N0} / {maxValue:N0}";

}
