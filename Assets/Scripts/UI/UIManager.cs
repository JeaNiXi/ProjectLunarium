using Managers;
using SO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/*  GLOBAL TO DO:
 *  1. Переработать в будущем боковую панель. Сейчас она не обновляется.
 */
namespace UI
{
    /*
     *  Класс используется как основной менеджер для управления UI всей игры. Здесь просиходит управление всеми главными категориями, а также другими данными интерфейса игрока.
     */
    public class UIManager : MonoBehaviour
    {
        /*
        *  Переменные и другое.
        */
        public static UIManager Instance { get; private set; }

        [Header("Main Information")]
        public UIDocument MainUIDocument;

        [Header("Visual Elements")]
        private VisualElement RootVE;
        private VisualElement MainViewVE;
        private VisualElement LedgerVE;

        [Header("Page Assets")]
        [SerializeField] private VisualTreeAsset mainMenuMainAsset;
        [SerializeField] private VisualTreeAsset populationMainAsset;
        [SerializeField] private VisualTreeAsset resourcesMainAsset;
        [SerializeField] private VisualTreeAsset technologyMainAsset;
        [SerializeField] private VisualTreeAsset workersMainAsset;
        [SerializeField] private VisualTreeAsset workPlaceMainAsset;

        [Header("Other Assets")]
        [SerializeField] private VisualTreeAsset ledgerResourceAsset;

        [Header("Manager Scriptable Objects")]
        [SerializeField] private MainMenuManagerSO mainMenuManagerSO;
        [SerializeField] private PopulationManagerSO populationManagerSO;
        [SerializeField] private ResourceManagerSO resourceManagerSO;
        [SerializeField] private TechnologyManagerSO technologyManagerSO;
        [SerializeField] private WorkersManagerSO workersManagerSO;
        [SerializeField] private WorkPlaceManagerSO workPlaceManagerSO;

        [Header("Localization")]
        [SerializeField] private UIMenuLocalizationSO uiMenuLocalizationSO;

        private LocalizationManager LM;
        private PopulationManager PM;
        private TimeManager TM;

        private Dictionary<string, VisualElement> cachedPages;
        private Dictionary<string, IUIPageController> cachedIUIPageControllers;
        private Dictionary<Button, bool> cachedCategoryButtonIsEnabledDictionary;

        private Button categoryMainMenuButton;
        private Button categoryPopulationButton;
        private Button categoryResourcesButton;
        private Button categoryTechnologyButton;
        private Button categoryWorkersButton;
        private Button categoryWorkPlaceButton;

        private Label infoPanelUpPopLabel;
        private Label infoPanelUpPopValueLabel;
        private Label infoPanelUpPopActiveLabel;
        private Label infoPanelUpPopActiveValueLabel;
        private Label infoPanelUpPopInactiveLabel;
        private Label infoPanelUpPopInactiveValueLabel;

        private Label infoCurrentDay;
        private Label infoCurrentDayValue;
        private Label infoCurrentMonth;
        private Label infoCurrentMonthValue;
        private Label infoCurrentYear;
        private Label infoCurrentYearValue;

        private VisualElement CurrentPage;
        private IUIPageController CurrentController;
        private string CurrentCategory;

        private LedgerManager ledgerManager;
        private HashSet<ResourceSO> ledgerResources;

        /*
         *  Awake и инициализация всех данных.
         */
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeMainDataOnAwake();
        }
        /*
         *  Инициализация
         *
         *  Используется для инициализации основного интерфейса.
         *  Здесь можно также выбирать начинать новую игру, заходить в настройки, загружать игру, выходить из игры как через обычное меню.  
         */
        private void InitializeMainDataOnAwake()
        {
            RootVE = MainUIDocument.rootVisualElement;
            MainViewVE = RootVE.Q<VisualElement>("mainView");
            LedgerVE = RootVE.Q<VisualElement>("ledgerVE");
            cachedPages = new Dictionary<string, VisualElement>();
            cachedIUIPageControllers = new Dictionary<string, IUIPageController>();
            cachedCategoryButtonIsEnabledDictionary = new Dictionary<Button, bool>();
            ledgerManager = new LedgerManager(LedgerVE);
            ledgerResources = new HashSet<ResourceSO>();
        }
        public void InitializeUI()
        {
            Debug.Log("[UIManager] UI Initialization Started!");
            InitializeConnections();
            InitializeCategoryPagesDictionaries();
            InitializeButtons();
            InitializeButtonEvents();
            InitializeMainUI();
            InitializeData();
            SetButtonEnabled(categoryMainMenuButton, true);
        }
        private void InitializeConnections()
        {
            PM = PopulationManager.Instance;
            LM = LocalizationManager.Instance;
            TM = TimeManager.Instance;
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            PM.OnPopulationChanged += PM_OnPopulationChanged;
            PM.OnActivePopChanged += PM_OnActivePopChanged;
            PM.OnInactivePopChanged += PM_OnInactivePopChanged;
            TM.OnDayChangedEvent += TM_OnDayChangedEvent;
            TM.OnMonthChangedEvent += TM_OnMonthChangedEvent;
            TM.OnYearChangedEvent += TM_OnYearChangedEvent;
        }
        //  Используется для кэширования всех основных категорий.
        private void InitializeCategoryPagesDictionaries()
        {
            CachePage("mainMenu", mainMenuMainAsset, new UIMainMenuPageController(), mainMenuManagerSO);
            CachePage("population", populationMainAsset, new UIPopulationPageController(), populationManagerSO);
            CachePage("resources", resourcesMainAsset, new UIResourcePageController(), resourceManagerSO);
            CachePage("technologies", technologyMainAsset, new UITechnologyPageController(), technologyManagerSO);
            CachePage("workers", workersMainAsset, new UIWorkersPageController(), workersManagerSO);
            CachePage("workPlace", workPlaceMainAsset, new UIWorkPlaceController(), workPlaceManagerSO);
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
            cachedPages.Add(category, newPage);
            cachedIUIPageControllers.Add(category, controller);
            MainViewVE.Add(newPage);
        }
        private void InitializeButtons()
        {
            categoryMainMenuButton = RootVE.Q<Button>("menuButton");
            categoryMainMenuButton.text = LocalizationManager.Instance.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.CategoryMainMenuKey);
            categoryPopulationButton = RootVE.Q<Button>("populationButton");
            categoryPopulationButton.text = LocalizationManager.Instance.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.CategoryPopulationKey);
            categoryResourcesButton = RootVE.Q<Button>("resourcesButton");
            categoryTechnologyButton = RootVE.Q<Button>("technologyButton");
            categoryWorkersButton = RootVE.Q<Button>("workersButton");
            categoryWorkPlaceButton = RootVE.Q<Button>("workPlaceButton");
            categoryWorkPlaceButton.text = LM.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.CategoryWorkPlaceKey);

            AddAllCategoryButtonsToList(
                categoryMainMenuButton,
                categoryPopulationButton,
                categoryResourcesButton,
                categoryTechnologyButton,
                categoryWorkersButton,
                categoryWorkPlaceButton
                );
        }
        private void AddAllCategoryButtonsToList(params Button[] buttons)
        {
            foreach (var button in buttons)
                cachedCategoryButtonIsEnabledDictionary.Add(button, false);
        }
        private void SetButtonEnabled(Button button, bool value)
            => button.SetEnabled(value);
        private void EnableAllCategoryButtons()
        {
            foreach (Button button in cachedCategoryButtonIsEnabledDictionary.Keys)
                button.SetEnabled(true);
        }
        private bool IsButtonEnabled(Button button)
            => cachedCategoryButtonIsEnabledDictionary.TryGetValue(button, out var value);
        private void InitializeButtonEvents()
        {
            categoryMainMenuButton.clicked += OnMainMenuButtonClicked;
            categoryPopulationButton.clicked += OnPopulationButtonClicked;
            categoryResourcesButton.clicked += OnResourcesButtonClicked;
            categoryTechnologyButton.clicked += OnTechnologyButtonClicked;
            categoryWorkersButton.clicked += OnWorkersButtonClicked;
            categoryWorkPlaceButton.clicked += OnWorkPlaceButtonClicked;
        }
        private void OnMainMenuButtonClicked() => ShowPage("mainMenu");
        private void OnPopulationButtonClicked() => ShowPage("population");
        private void OnResourcesButtonClicked() => ShowPage("resources");
        private void OnTechnologyButtonClicked() => ShowPage("technologies");
        private void OnWorkersButtonClicked() => ShowPage("workers");
        private void OnWorkPlaceButtonClicked() => ShowPage("workPlace");
        private void OnGameStateChanged(GameManager.GameState gameState)
        {
            if (gameState == GameManager.GameState.RUNNING)
                EnableAllCategoryButtons();
        }
        private void InitializeMainUI()
        {

            infoPanelUpPopLabel = RootVE.Q<Label>("infoPanelUpPopLabel");
            infoPanelUpPopValueLabel = RootVE.Q<Label>("infoPanelUpPopValueLabel");
            infoPanelUpPopActiveLabel = RootVE.Q<Label>("infoPanelUpPopActiveLabel");
            infoPanelUpPopActiveValueLabel = RootVE.Q<Label>("infoPanelUpPopActiveValueLabel");
            infoPanelUpPopInactiveLabel = RootVE.Q<Label>("infoPanelUpPopInactiveLabel");
            infoPanelUpPopInactiveValueLabel = RootVE.Q<Label>("infoPanelUpPopInactiveValueLabel");

            infoCurrentDay = RootVE.Q<Label>("infoCurrentDay");
            infoCurrentDayValue = RootVE.Q<Label>("infoCurrentDayValue");
            infoCurrentMonth = RootVE.Q<Label>("infoCurrentMonth");
            infoCurrentMonthValue = RootVE.Q<Label>("infoCurrentMonthValue");
            infoCurrentYear = RootVE.Q<Label>("infoCurrentYear");
            infoCurrentYearValue = RootVE.Q<Label>("infoCurrentYearValue");
        }
        private void InitializeData()
        {
            infoPanelUpPopLabel.text = LM.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.InfoPanelUpPopLabelKey);
            infoPanelUpPopActiveLabel.text = LM.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.InfoPanelUpPopActiveLabelKey);
            infoPanelUpPopInactiveLabel.text = LM.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.InfoPanelUpPopInactiveLabelKey);

            ChangeLabelText(infoPanelUpPopValueLabel, () =>
            PM.GetCurrentPopulation(out ulong population) ? population.ToString("N0") : population.ToString());
            ChangeLabelText(infoPanelUpPopActiveValueLabel, () =>
            PM.GetActivePopulation(out ulong population) ? population.ToString("N0") : population.ToString());

            infoCurrentDay.text = LM.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.InfoCurrentDayKey);
            infoCurrentMonth.text = LM.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.InfoCurrentMonthKey);
            infoCurrentYear.text = LM.GetLocalizedUIMenuData(
                uiMenuLocalizationSO.InfoCurrentYearKey);

            ChangeLabelText(infoCurrentDayValue, () =>
            TM.GetCurrentDay());
            ChangeLabelText(infoCurrentMonthValue, () =>
            TM.GetCurrentMonth());
            ChangeLabelText(infoCurrentYearValue, () =>
            TM.GetCurrentYear());
        }
        private void ChangeLabelText(Label label, Func<string> labelData) =>
            label.text = labelData();
        private void ChangeLabelText(Label label, Func<int> labelData) =>
            label.text = labelData().ToString();
        private void ChangeLabelText(Label label, string text) =>
            label.text = text;
        private void ChangeLabelText(Label label, ulong value) =>
            label.text = value.ToString();
        private void ChangeLabelText(Label label, int value) =>
            label.text = value.ToString();

        private void PM_OnPopulationChanged(bool value, ulong newPopAmount) =>
            ChangeLabelText(infoPanelUpPopValueLabel, newPopAmount);
        private void PM_OnActivePopChanged(bool value, ulong newActivePopAmount) =>
            ChangeLabelText(infoPanelUpPopActiveValueLabel, newActivePopAmount);
        private void PM_OnInactivePopChanged(bool value, ulong newInactivePopAmount) =>
            ChangeLabelText(infoPanelUpPopInactiveValueLabel, newInactivePopAmount);

        private void TM_OnYearChangedEvent(int value) =>
            ChangeLabelText(infoCurrentYearValue, value);
        private void TM_OnMonthChangedEvent(int value) =>
            ChangeLabelText(infoCurrentMonthValue, value);
        private void TM_OnDayChangedEvent(int value) =>
            ChangeLabelText(infoCurrentDayValue, value);
        /*
         *  Управление UI
         */
        private void ShowPage(string category)
        {
            HidePage();
            if (CurrentCategory != null && CurrentCategory == category)
            {
                CurrentCategory = null;
                return;
            }
            if (cachedPages.TryGetValue(category, out var page) && cachedIUIPageControllers.TryGetValue(category, out var controller))
            {
                controller.ShowPage();
                CurrentPage = page;
                CurrentController = controller;
                CurrentCategory = category;
            }
        }
        private void HidePage()
        {
            if (CurrentPage != null && CurrentController != null)
                CurrentController.HidePage();
        }
        public void HideCurrentPage()
            => HidePage();
        private void FixedUpdate()
        {
            if (CurrentController != null)
                CurrentController.UpdatePage();
        }
        public UIMenuLocalizationSO GetMenuLocalizationDataSO()
            => uiMenuLocalizationSO;
        /*
         *  Управление боковой панелью.
         */
        public void AddOrUpdateLedgerElement(ResourceSO resource)
        {
            ledgerManager.AddOrUpdate(new LedgerViewDescriptor
            {
                ID = resource.ID,
                Type = LedgerManager.LedgerEntryType.Resource,
                Asset = ledgerResourceAsset,
                Bind = ve =>
                {
                    ve.Q<Label>("titleLabel").text = resource.Localization.Name.Key;
                    ve.Q<Label>("valueLabel").text = ResourceManager.Instance.GetResourceAmount(resource).ToString();
                    ve.Q<Label>("extraLabel").text = resource.ID;
                }
            });
            ObserveLedgerElement(resource);
        }
        private void ObserveLedgerElement(ResourceSO resource)
        {
            if (ledgerResources.Contains(resource))
                return;
            ledgerResources.Add(resource);
            ResourceManager.Instance.GetCurrentResourceState().OnResourceAmountChanged += OnObservedResourceAmountChanged;
        }
        private void OnObservedResourceAmountChanged(ResourceSO resource, int newAmount)
        {
            if (!ledgerResources.Contains(resource)) return;
            AddOrUpdateLedgerElement(resource);
        }
        private void OnDestroy()
        {
            categoryMainMenuButton.clicked -= OnMainMenuButtonClicked;
            categoryPopulationButton.clicked -= OnPopulationButtonClicked;
            categoryResourcesButton.clicked -= OnResourcesButtonClicked;
            categoryTechnologyButton.clicked -= OnTechnologyButtonClicked;
            categoryWorkersButton.clicked -= OnWorkersButtonClicked;
            categoryWorkPlaceButton.clicked -= OnWorkPlaceButtonClicked;
            if (ledgerResources != null)
                ResourceManager.Instance.GetCurrentResourceState().OnResourceAmountChanged -= OnObservedResourceAmountChanged;
        }
    }
}
