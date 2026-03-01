using Managers;
using SO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    /*
     *  Класс используется для управления одной из основных категорий, а именно главным меню.
     *  Эта категория автоматически открывается при запуске игры, и выбор других категорий недоступен до запуска новой игры или загрузки существующего сохранения.
     */
    public class UIMainMenuPageController : IUIPageController
    {
        private VisualElement RootVE;
        private VisualElement MainMenuViewVE;
        private VisualElement CurrentMenuCategoryPage;
        private IUIPageController CurrentMenuController;

        private VisualTreeAsset newGameMenuAsset;
        private VisualTreeAsset continueGameMenuAsset;

        private MainMenuNewGameSO mainMenuNewGameSO;

        private Dictionary<string, VisualElement> cachedMenuPages;
        private Dictionary<string, IUIPageController> cachedMenuIUIPageControllers;

        private Button categoryMainMenuNewGameButton;

        private MainMenuManagerSO ManagerData;

        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            RootVE = page;
            ManagerData = data as MainMenuManagerSO;
            if (ManagerData == null)
                Debug.Log("NO DATA SO FOUND");
            InitializeMainData();
            InitializeMenuPages();
            InitializeButtons();
            InitializeButtonEvents();
        }
        private void InitializeMainData()
        {
            MainMenuViewVE = RootVE.Q<VisualElement>("mainMenuMainVE");
            cachedMenuPages = new Dictionary<string, VisualElement>();
            cachedMenuIUIPageControllers = new Dictionary<string, IUIPageController>();
            newGameMenuAsset = Resources.Load<VisualTreeAsset>("UI/Menu/NewGameMenuPage");

            mainMenuNewGameSO = Resources.Load<MainMenuNewGameSO>("SO/MainMenuPages/MainMenuNewGame");
        }
        private void InitializeMenuPages()
        {
            CacheMenuPage("mainMenuNewGame", newGameMenuAsset, new UIMainMenuNewGameController(), mainMenuNewGameSO);
        }
        private void CacheMenuPage(string category, VisualTreeAsset asset, IUIPageController controller, ScriptableObject data)
        {
            VisualElement newPage = new()
            {
                style =
                {
                    flexGrow=1,
                    width=Length.Percent(100f),
                    height=Length.Percent(100f)
                }
            };
            asset.CloneTree(newPage);
            controller.InitializePage(newPage, data);
            newPage.style.display = DisplayStyle.None;
            cachedMenuPages.Add(category, newPage);
            cachedMenuIUIPageControllers.Add(category, controller);
            MainMenuViewVE.Add(newPage);
        }
        private void InitializeButtons()
        {
            categoryMainMenuNewGameButton = RootVE.Q<Button>("newGameButton");
            categoryMainMenuNewGameButton.text = LocalizationManager.Instance.GetLocalizedUIMenuData(
                UIManager.Instance.GetMenuLocalizationDataSO().MainMenuNewGameKey);
        }
        private void InitializeButtonEvents()
        {
            categoryMainMenuNewGameButton.clicked += OnNewGameButtonClicked;
        }
        public void ShowPage()
            => RootVE.style.display = DisplayStyle.Flex;
        public void HidePage()
            => RootVE.style.display = DisplayStyle.None;
        private void OnNewGameButtonClicked() => ShowMenuCategoryPage("mainMenuNewGame");
        private void ShowMenuCategoryPage(string category)
        {
            if (CurrentMenuCategoryPage != null && CurrentMenuController != null)
                CurrentMenuController.HidePage();
            if (cachedMenuPages.TryGetValue(category, out var page) && cachedMenuIUIPageControllers.TryGetValue(category, out var controller))
            {
                controller.ShowPage();
                CurrentMenuCategoryPage = page;
                CurrentMenuController = controller;
            }
        }
        public void UpdatePage()
        {
            //throw new System.NotImplementedException();
        }
    }
}