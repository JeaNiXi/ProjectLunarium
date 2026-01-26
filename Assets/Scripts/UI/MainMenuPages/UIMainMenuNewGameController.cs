using SO;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIMainMenuNewGameController : IUIPageController
    {
        private VisualElement RootVE;
        private MainMenuNewGameSO ManagerData;
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            RootVE = page;
            ManagerData = data as MainMenuNewGameSO;
            if (ManagerData == null)
                Debug.Log("NO DATA SO FOUND");
        }
        public void ShowPage()
            => RootVE.style.display = DisplayStyle.Flex;
        public void HidePage()
            => RootVE.style.display = DisplayStyle.None;
        public void UpdatePage()
        {
            //throw new System.NotImplementedException();
        }
    }
}