using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIPopulationPageController : IUIPageController
    {
        private VisualElement page;
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            this.page = page;
        }
        public void ShowPage()
        {
            page.style.display = DisplayStyle.Flex;
        }
        public void HidePage()
        {
            page.style.display = DisplayStyle.None;
        }
        public void UpdatePage()
        {
            //throw new System.NotImplementedException();
        }
    }
}