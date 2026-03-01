using Managers;
using SO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIWorkPlaceController : IUIPageController
    {
        private VisualElement RootVE;
        private VisualElement MainWPPagesVE;
        private WorkPlaceManagerSO ManagerData;

        private Button wpButtonBasicLiving;
        private Button wpButtonFoodGathering;

        private VisualTreeAsset wpPanelAsset;

        private Dictionary<string, VisualElement> cachedWPPages;
        private Dictionary<string, IUIPageController> cachedWPControllers;
        private Dictionary<Button, bool> cachedWPButtonIsEnabledDictionary;

        private VisualElement CurrentWPPage;
        private IUIPageController CurrentWPController;
        private string CurrentWPCategory;

        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            InitializeMainData(page, data);
            InitializeConnections();
            CacheAllWPPages();
            InitializeButtons();
            InitializeButtonEvents();
        }
        private void InitializeMainData(VisualElement page, ScriptableObject data)
        {
            RootVE = page;
            ManagerData = data as WorkPlaceManagerSO;
            if (ManagerData == null)
                Debug.LogError($"No Data SO Object Found in {this.GetType()}. Expected {ManagerData.GetType()}");

            cachedWPPages = new Dictionary<string, VisualElement>();
            cachedWPControllers = new Dictionary<string, IUIPageController>();
            cachedWPButtonIsEnabledDictionary = new Dictionary<Button, bool>();

            MainWPPagesVE = RootVE.Q<VisualElement>("mainWPPage");
        }
        private void InitializeConnections()
        {
            wpPanelAsset = Resources.Load<VisualTreeAsset>("UI/Panel/WPPanelAsset");
        }
        private void CacheAllWPPages()
        {
            CacheWPPage("basicLivingWPCategory");
            CacheWPPage("basicFoodGatheringWPCategory");
        }
        private void CacheWPPage(string workPlaceCategory)
        {
            if (WorkPlaceManager.Instance.GetWorkPlaceCategory(workPlaceCategory, out var category))
                CachePage(workPlaceCategory, wpPanelAsset, new UIWorkPlacePagesController(), category);
            else
                DebugExtensions.WPCategoryNotFound(category.CategoryID);
        }
        private void CachePage(string category, VisualTreeAsset asset, IUIPageController controller, ScriptableObject data)
        {
            VisualElement newPage = new()
            {
                style =
                {
                    flexGrow = 1,
                    width = Length.Percent(100f),
                    height = Length.Percent(100f)
                }
            };
            asset.CloneTree(newPage);
            controller.InitializePage(newPage, data);
            newPage.style.display = DisplayStyle.None;
            cachedWPPages.Add(category, newPage);
            cachedWPControllers.Add(category, controller);
            MainWPPagesVE.Add(newPage);
        }
        private void InitializeButtons()
        {
            wpButtonBasicLiving = RootVE.Q<Button>("basicLivingWPButton");
            SetButtonText(wpButtonBasicLiving, "nkwpcBasicLiving");
            wpButtonFoodGathering = RootVE.Q<Button>("foodWPButton");
            SetButtonText(wpButtonFoodGathering, "nkwpcFoodGathering");


            AddAllWPButtonsToList(
                wpButtonBasicLiving,
                wpButtonFoodGathering
                );
        }
        private void SetButtonText(Button button, string key)
        {
            LocalizationManager.Instance.GetLocalizedWorkPlaceCategorySOData(key, out var value);
            button.text = value;
        }
        private void AddAllWPButtonsToList(params Button[] buttons)
        {
            foreach (var button in buttons)
                cachedWPButtonIsEnabledDictionary.Add(button, false);
        }
        private void SetButtonEnabled(Button button, bool value)
            => button.SetEnabled(value);
        private void InitializeButtonEvents()
        {
            wpButtonBasicLiving.clicked += OnWPButtonBasicLivingClicked;
            wpButtonFoodGathering.clicked += OnWPButtonFoodGatheringClicked;
        }
        private void OnWPButtonBasicLivingClicked() => ShowWPPage("basicLivingWPCategory");
        private void OnWPButtonFoodGatheringClicked() => ShowWPPage("basicFoodGatheringWPCategory");

        public void ShowWPPage(string category)
        {
            HideWPPage();
            if (CurrentWPCategory != null && CurrentWPCategory == category)
            {
                CurrentWPCategory = null;
                return;
            }
            if (cachedWPPages.TryGetValue(category, out var page) && cachedWPControllers.TryGetValue(category, out var controller))
            {
                controller.ShowPage();
                CurrentWPPage = page;
                CurrentWPController = controller;
                CurrentWPCategory = category;
            }
        }
        private void HideWPPage()
        {
            if (CurrentWPPage != null && CurrentWPController != null)
                CurrentWPController.HidePage();
        }
        public void ShowPage()
            => RootVE.style.display = DisplayStyle.Flex;
        public void HidePage()
            => RootVE.style.display = DisplayStyle.None;
        public void UpdatePage()
        {
            if (CurrentWPController != null)
                CurrentWPController.UpdatePage();
        }
        //Write On Disable later? 
    }
}