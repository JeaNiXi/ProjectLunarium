using Managers;
using SO;
using State;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    /*
    *  Класс используется для управления меню созданием новой игры.
    */
    public class UIMainMenuNewGameController : IUIPageController
    {
        #region Vars
        private VisualElement RootVE;
        private MainMenuNewGameSO ManagerData;

        private Button newGameForwardPageButton;
        private Button newGameBackPageButton;
        private Button newGameStartPageButton;

        private Toggle acceptToggle;

        private List<VisualElement> newGamePages;
        private int currentPageIndex;

        private NewGameState newGameState;

        private Button startRaceSelectHumansButton;
        private Button startRaceSelectElfButton;

        private Label raceDescriptionLabel;

        #endregion
        #region Initialization
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            RootVE = page;
            ManagerData = data as MainMenuNewGameSO;
            if (ManagerData == null)
                Debug.Log("NO DATA SO FOUND");
            InitializeMainData();
            InitializeButtons();
            InitializeButtonEvents();
            ActivatePage(0);
        }
        private void InitializeMainData()
        {
            newGamePages = new List<VisualElement>
            {
                RootVE.Q<VisualElement>("page1VE"),
                RootVE.Q<VisualElement>("page2VE")
            };
            currentPageIndex = 0;
            newGameState = new NewGameState();
            newGameState.OnStateChanged += UpdateStartButton;

            raceDescriptionLabel = RootVE.Q<Label>("raceDescriptionLabel");

            acceptToggle = RootVE.Q<Toggle>("AcceptToggle");
        }
        private void InitializeButtons()
        {
            newGameForwardPageButton = RootVE.Q<Button>("newGameNextButtonPage");
            newGameBackPageButton = RootVE.Q<Button>("newGameBackButtonPage");
            newGameStartPageButton = RootVE.Q<Button>("newGameStartButtonPage");

            startRaceSelectHumansButton = RootVE.Q<Button>("StartRaceSelectHumansButton");
            startRaceSelectElfButton = RootVE.Q<Button>("StartRaceSelectElfButton");
        }
        private void InitializeButtonEvents()
        {
            newGameForwardPageButton.clicked += OnNextPageButtonClicked;
            newGameBackPageButton.clicked += OnBackPageButtonClicked;
            newGameStartPageButton.clicked += OnStartPageButtonClicked;

            startRaceSelectHumansButton.clicked += OnRaceHumansSelected;
            startRaceSelectElfButton.clicked += OnRaceElfSelected;
        }
        private void InitializeFirstPage()
        {

        }
        #endregion
        #region PageManipulation
        private void OnNextPageButtonClicked()
        {
            if (!HasNextPage())
                return;
            ActivatePage(currentPageIndex + 1);
        }
        private void OnBackPageButtonClicked()
        {
            if (!HasPreviousPage())
                return;
            ActivatePage(currentPageIndex - 1);
        }
        private void OnStartPageButtonClicked()
        {

        }
        private void ActivatePage(int index)
        {
            newGamePages[currentPageIndex].style.display = DisplayStyle.None;
            newGamePages[index].style.display = DisplayStyle.Flex;
            currentPageIndex = index;

            UpdateButtons();
        }
        private void UpdateButtons()
        {
            newGameForwardPageButton.SetEnabled(HasNextPage());
            newGameBackPageButton.SetEnabled(HasPreviousPage());

            newGameStartPageButton.style.display = HasNextPage() ? DisplayStyle.None : DisplayStyle.Flex;
        }
        private void UpdateStartButton()
        {
            newGameStartPageButton.SetEnabled(newGameState.IsDataCompleted);
        }
        private bool HasNextPage()
            => currentPageIndex < newGamePages.Count - 1;
        private bool HasPreviousPage()
            => currentPageIndex > 0;
        public void ShowPage()
            => RootVE.style.display = DisplayStyle.Flex;
        public void HidePage()
            => RootVE.style.display = DisplayStyle.None;
        public void UpdatePage()
        {
            //throw new System.NotImplementedException();
        }
        #endregion
        #region NewGameConfig
        private void OnRaceHumansSelected()
            => SelectRace(RaceDatabaseSO.RaceType.Human);
        private void OnRaceElfSelected()
            => SelectRace(RaceDatabaseSO.RaceType.Elf);
        private void SelectRace(RaceDatabaseSO.RaceType raceType)
        {
            if (PopulationManager.Instance.TryGetRace(raceType, out var race))
            {
                newGameState.SetStartingRace(race);
                UpdateRaceDescriptionLabel(race);
                Debug.Log($"StartingRace = {race}");
            }
        }
        private void UpdateRaceDescriptionLabel(RaceSO race)
        {
            raceDescriptionLabel.text = LocalizationManager.Instance.GetLocalizedRaceDescription(race.DescriptionKey);
        }
        #endregion
    }
}