using UnityEngine;
using UnityEngine.UIElements;

public interface IUIPageController
{
    void InitializePage(VisualElement page, ScriptableObject data);
    void ShowPage();
    void HidePage();
    void UpdatePage();
}
