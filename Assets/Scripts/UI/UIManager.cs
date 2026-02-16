using Managers;
using SO;
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

        [Header("Other Assets")]
        [SerializeField] private VisualTreeAsset ledgerResourceAsset;

        [Header("Manager Scriptable Objects")]
        [SerializeField] private MainMenuManagerSO mainMenuManagerSO;
        [SerializeField] private PopulationManagerSO populationManagerSO;
        [SerializeField] private ResourceManagerSO resourceManagerSO;
        [SerializeField] private TechnologyManagerSO technologyManagerSO;
        [SerializeField] private WorkersManagerSO workersManagerSO;

        private Dictionary<string, VisualElement> cachedPages;
        private Dictionary<string, IUIPageController> cachedIUIPageControllers;
        private Dictionary<Button, bool> cachedCategoryButtonIsEnabledDictionary;

        private Button categoryMainMenuButton;
        private Button categoryPopulationButton;
        private Button categoryResourcesButton;
        private Button categoryTechnologyButton;
        private Button categoryWorkersButton;

        private VisualElement CurrentPage;
        private IUIPageController CurrentController;

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
            InitializeConnections();
            InitializeCategoryPagesDictionaries();
            InitializeButtons();
            InitializeButtonEvents();
            SetButtonEnabled(categoryMainMenuButton, true);
        }
        private void InitializeConnections()
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
        //  Используется для кэширования всех основных категорий.
        private void InitializeCategoryPagesDictionaries()
        {
            CachePage("mainMenu", mainMenuMainAsset, new UIMainMenuPageController(), mainMenuManagerSO);
            CachePage("population", populationMainAsset, new UIPopulationPageController(), populationManagerSO);
            CachePage("resources", resourcesMainAsset, new UIResourcePageController(), resourceManagerSO);
            CachePage("technologies", technologyMainAsset, new UITechnologyPageController(), technologyManagerSO);
            CachePage("workers", workersMainAsset, new UIWorkersPageController(), workersManagerSO);
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
            categoryPopulationButton = RootVE.Q<Button>("populationButton");
            categoryResourcesButton = RootVE.Q<Button>("resourcesButton");
            categoryTechnologyButton = RootVE.Q<Button>("technologyButton");
            categoryWorkersButton = RootVE.Q<Button>("workersButton");

            AddAllCategoryButtonsToList(
                categoryMainMenuButton,
                categoryPopulationButton,
                categoryResourcesButton,
                categoryTechnologyButton,
                categoryWorkersButton);
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
        }
        private void OnMainMenuButtonClicked() => ShowPage("mainMenu");
        private void OnPopulationButtonClicked() => ShowPage("population");
        private void OnResourcesButtonClicked() => ShowPage("resources");
        private void OnTechnologyButtonClicked() => ShowPage("technologies");
        private void OnWorkersButtonClicked() => ShowPage("workers");
        private void OnGameStateChanged(GameManager.GameState gameState)
        {
            if (gameState == GameManager.GameState.RUNNING)
                EnableAllCategoryButtons();
        }
        /*
         *  Управление UI
         */
        private void ShowPage(string category)
        {
            if (CurrentPage != null && CurrentController != null)
                CurrentController.HidePage();
            if (cachedPages.TryGetValue(category, out var page) && cachedIUIPageControllers.TryGetValue(category, out var controller))
            {
                controller.ShowPage();
                CurrentPage = page;
                CurrentController = controller;
            }
        }
        private void FixedUpdate()
        {
            if (CurrentController != null)
                CurrentController.UpdatePage();
        }
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
                    ve.Q<Label>("titleLabel").text = resource.NameEN;
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
            if (ledgerResources != null)
                ResourceManager.Instance.GetCurrentResourceState().OnResourceAmountChanged -= OnObservedResourceAmountChanged;
        }
    }
}
