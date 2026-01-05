using SO;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIMainManuPageController : IUIPageController
    {
        private VisualElement page;
        private MainMenuManagerSO data;
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            this.page = page;
            this.data = data as MainMenuManagerSO;
            if (this.data == null)
                Debug.Log("NO DATA SO FOUND");
            InitializeData();
        }
        private void InitializeData()
        {

        }
        public void ShowPage()
            => page.style.display = DisplayStyle.Flex;
        public void HidePage()
            => page.style.display= DisplayStyle.None;
        public void UpdatePage()
        {
            //throw new System.NotImplementedException();
        }
    }
}