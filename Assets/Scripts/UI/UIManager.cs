using Managers;
using SO;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public UIDocument MainUIDocument;
        public ResourceSO testResource;
        private VisualElement RootVE;
        private VisualElement MainView;
        private VisualElement CurrentPage;
        private VisualElement LedgerVE;
        private string CurrentPageName;
        private IUIPageController CurrentController;
        [Header("Page Assets")]
        [SerializeField] private VisualTreeAsset mainMenuMainAsset;
        [SerializeField] private VisualTreeAsset populationMainAsset;
        [SerializeField] private VisualTreeAsset resourcesMainAsset;
        [SerializeField] private VisualTreeAsset technologyMainAsset;
        [SerializeField] private VisualTreeAsset workersMainAsset;
        [Header("Manager Scriptable Objects")]
        [SerializeField] private MainMenuManagerSO mainMenuManagerSO;
        [SerializeField] private PopulationManagerSO populationManagerSO;
        [SerializeField] private ResourceManagerSO resourceManagerSO;
        [SerializeField] private TechnologyManagerSO technologyManagerSO;
        [SerializeField] private WorkersManagerSO workersManagerSO;
        [Header("Ledger Assets")]
        [SerializeField] private VisualTreeAsset ledgerResourceAsset;
        private Button categoryMainMenuButton;
        private Button categoryPopulationButton;
        private Button categoryResourcesButton;
        private Button categoryTechnologyButton;
        private Button categoryWorkersButton;
        private Dictionary<string, VisualElement> cachedPages;
        private Dictionary<string, IUIPageController> cachedIUIPageControllers;
        private LedgerManager ledgerManager;
        private HashSet<ResourceSO> ledgerResources;
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            RootVE = MainUIDocument.rootVisualElement;
            MainView = RootVE.Q<VisualElement>("mainView");
            LedgerVE = RootVE.Q<VisualElement>("ledgerVE");
            cachedPages = new Dictionary<string, VisualElement>();
            cachedIUIPageControllers = new Dictionary<string, IUIPageController>();
            ledgerManager = new LedgerManager(LedgerVE);
            ledgerResources = new HashSet<ResourceSO>();
        }
        private void Start()
        {
            InitializePagesDictionaries();
            InitializeButtons();
            InitializeButtonEvents();
            AddLedgerElement(testResource);
        }
        private void FixedUpdate()
        {
            if (CurrentController != null)
                CurrentController.UpdatePage();
        }
        private void InitializePagesDictionaries()
        {
            CachePage("mainMenu", mainMenuMainAsset, new UIMainManuPageController(), mainMenuManagerSO);
            CachePage("population", populationMainAsset, new UIPopulationPageController(), populationManagerSO);
            CachePage("resources", resourcesMainAsset, new UIResourcePageController(), resourceManagerSO);
            CachePage("technologies", technologyMainAsset, new UITechnologyPageController(), technologyManagerSO);
            CachePage("workers", workersMainAsset, new UIWorkersPageController(), workersManagerSO);
        }
        private void CachePage(string category, VisualTreeAsset asset, IUIPageController controller, ScriptableObject data)
        {
            VisualElement newPage = new VisualElement()
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
            MainView.Add(newPage);
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
            categoryMainMenuButton.clicked += () => ShowPage("mainMenu");
            categoryPopulationButton.clicked += () => ShowPage("population");
            categoryResourcesButton.clicked += () => ShowPage("resources");
            categoryTechnologyButton.clicked += () => ShowPage("technologies");
            categoryWorkersButton.clicked += () => ShowPage("workers");
        }
        private void ShowPage(string category)
        {
            Debug.Log("ButtonClicked");
            if (CurrentPage != null && CurrentController != null)
            {
                CurrentController.HidePage();
                Debug.Log("Hiding Page: " + CurrentPageName);
            }
            if (cachedPages.TryGetValue(category, out var page) && cachedIUIPageControllers.TryGetValue(category, out var controller))
            {
                controller.ShowPage();
                CurrentPage = page;
                CurrentController = controller;
                CurrentPageName = category;
                Debug.Log("Showing Page: " + CurrentPageName);
            }
        }
        public void AddLedgerElement(ResourceSO resource)
        {
            ledgerManager.AddOrUpdate(new LedgerViewDescriptor
            {
                ID = resource.ID,
                Type = LedgerManager.LedgerEntryType.Resource,
                Asset = ledgerResourceAsset,
                Bind = ve =>
                {
                    ve.Q<Label>("titleLabel").text = resource.NameEN;
                    ve.Q<Label>("valueLabel").text = Managers.ResourceManager.Instance.GetResourceAmount(resource).ToString();
                    ve.Q<Label>("extraLabel").text = resource.ID;
                }
            });
            ObserveResource(resource);
        }
        private void ObserveResource(ResourceSO resource)
        {
            if (ledgerResources.Contains(resource))
                return;
            ledgerResources.Add(resource);
            Managers.ResourceManager.Instance.GetCurrentResourceState().OnResourceAmountChanged += OnObservedResourceAmountChanged;
        }
        private void OnObservedResourceAmountChanged(ResourceSO resource, int newAmount)
        {
            if (!ledgerResources.Contains(resource)) return;
            AddLedgerElement(resource);
        }
        private void OnDestroy()
        {
            if (categoryMainMenuButton != null)
                categoryMainMenuButton.clicked -= () => ShowPage("mainMenu");
            if (categoryPopulationButton != null)
                categoryPopulationButton.clicked -= () => ShowPage("population");
            if (categoryResourcesButton != null)
                categoryResourcesButton.clicked -= () => ShowPage("resources");
            if (categoryTechnologyButton != null)
                categoryTechnologyButton.clicked -= () => ShowPage("technologies");
            if (categoryWorkersButton != null)
                categoryWorkersButton.clicked -= () => ShowPage("workers");
            if (ledgerResources != null)
                Managers.ResourceManager.Instance.GetCurrentResourceState().OnResourceAmountChanged -= OnObservedResourceAmountChanged;
        }
    }
}
