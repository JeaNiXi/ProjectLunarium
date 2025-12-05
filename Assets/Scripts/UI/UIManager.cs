using SO;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public UIDocument MainUIDocument;

        private VisualElement RootVE;
        private VisualElement MainView;
        private VisualElement CurrentPage;
        private string CurrentPageName;
        private IUIPageController CurrentController;
        [SerializeField] private VisualTreeAsset populationMainAsset;
        [SerializeField] private VisualTreeAsset resourcesMainAsset;
        [SerializeField] private PopulationManagerSO populationManagerSO;
        [SerializeField] private ResourceManagerSO resourceManagerSO;
        private Button categoryPopulationButton;
        private Button categoryResourcesButton;
        private Dictionary<string, VisualElement> cachedPages;
        private Dictionary<string, IUIPageController> cachedIUIPageControllers;
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
            cachedPages = new Dictionary<string, VisualElement>();
            cachedIUIPageControllers = new Dictionary<string, IUIPageController>();
        }
        private void Start()
        {
            InitializePagesDictionaries();
            InitializeButtons();
            InitializeButtonEvents();
        }
        private void FixedUpdate()
        {
            if (CurrentController != null)
                CurrentController.UpdatePage();
        }
        private void InitializePagesDictionaries()
        {
            CachePage("population", populationMainAsset, new UIPopulationPageController(), populationManagerSO);
            CachePage("resources", resourcesMainAsset, new UIResourcePageController(), resourceManagerSO);
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
            categoryPopulationButton = RootVE.Q<Button>("populationButton");
            categoryResourcesButton = RootVE.Q<Button>("resourcesButton");
        }
        private void InitializeButtonEvents()
        {
            categoryPopulationButton.clicked += () => ShowPage("population");
            categoryResourcesButton.clicked += () => ShowPage("resources");
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
        private void OnDestroy()
        {
            if (categoryPopulationButton != null)
                categoryPopulationButton.clicked -= () => ShowPage("population");
            if (categoryResourcesButton != null)
                categoryResourcesButton.clicked -= () => ShowPage("resources");
        }
    }
}
