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
        #region Vars
        public static UIManager Instance { get; private set; }

        [Header("Main Information")]
        public UIDocument MainUIDocument;

        [Header("Visual Elements")]
        private VisualElement RootVE;
        private VisualElement MainViewVE;
        private VisualElement LedgerVE;

        [Header("Page and Other Assets")]
        [SerializeField] private VisualTreeAsset mainMenuMainAsset;
        [SerializeField] private VisualTreeAsset populationMainAsset;
        [SerializeField] private VisualTreeAsset resourcesMainAsset;
        [SerializeField] private VisualTreeAsset technologyMainAsset;
        [SerializeField] private VisualTreeAsset workersMainAsset;

        [SerializeField] private VisualTreeAsset ledgerResourceAsset;

        [Header("Dictionaries")]
        private Dictionary<string, VisualElement> cachedPages;
        private Dictionary<string, IUIPageController> cachedIUIPageControllers;

        [Header("Manager Scriptable Objects")]
        [SerializeField] private MainMenuManagerSO mainMenuManagerSO;
        [SerializeField] private PopulationManagerSO populationManagerSO;
        [SerializeField] private ResourceManagerSO resourceManagerSO;
        [SerializeField] private TechnologyManagerSO technologyManagerSO;
        [SerializeField] private WorkersManagerSO workersManagerSO;

        [Header("Main Category Buttons")]
        private Button categoryMainMenuButton;
        private Button categoryPopulationButton;
        private Button categoryResourcesButton;
        private Button categoryTechnologyButton;
        private Button categoryWorkersButton;

        [Header("Page and Controller Info")]
        private VisualElement CurrentPage;
        private IUIPageController CurrentController;

        [Header("Ledger Elements")]
        private LedgerManager ledgerManager;
        private HashSet<ResourceSO> ledgerResources;
        #endregion
        /*
        *  Awake
        */
        #region Awake
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
        #endregion
        /*
         *  Инициализация
         */
        #region Initialization
        /*  
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
            ledgerManager = new LedgerManager(LedgerVE);
            ledgerResources = new HashSet<ResourceSO>();
        }
        public void InitializeUI()
        {
            InitializeCategoryPagesDictionaries();
            InitializeButtons();
            InitializeButtonEvents();
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
        }
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
        #endregion
        /*
         *  Управление UI
         */
        #region UIActions
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
        #endregion
        /*
         *  Управление боковой панелью.
         */
        #region LedgerActions
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
        #endregion
        /*
         *  OnDestroy
         */
        #region OnDestroy
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
        #endregion
    }
}
