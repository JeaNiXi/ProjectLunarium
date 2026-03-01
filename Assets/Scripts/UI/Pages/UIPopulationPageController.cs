using Managers;
using SO;
using State;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIPopulationPageController : IUIPageController
    {
        private VisualElement RootVE;
        private VisualElement racePageMainView;

        private PopulationManagerSO data;
        private PopulationStateSO populationStateSO;

        private MultiColumnListView raceMultiColumnListView;
        private VisualTreeAsset raceInfoPanel;
        private Dictionary<string, VisualElement> cachedRaceInfoPanels;
        private List<RaceRow> raceRows = new();

        private PopulationManager PopManager;

        Label currentPopulationLabel;

        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            RootVE = page;
            this.data = data as PopulationManagerSO;
            if (this.data == null)
                Debug.Log("NO DATA SO FOUND");
            InitializeData();
            InitializeConnections();
            InitializeRaceTable();
            InitializeRaceInfoPanels();
        }
        private void InitializeData()
        {
            PopManager = PopulationManager.Instance;
            racePageMainView = RootVE.Q<VisualElement>("populationRaceListVE");
            populationStateSO = Resources.Load<PopulationStateSO>("SO/PopulationState");
            raceInfoPanel = Resources.Load<VisualTreeAsset>("UI/Panel/RacePanelInfoAsset");
            cachedRaceInfoPanels = new Dictionary<string, VisualElement>();

            currentPopulationLabel = RootVE.Q<Label>("currentPopulation");
        }
        private void InitializeConnections()
        {
            PopManager.OnPopulationChanged += PopManager_OnPopulationChanged;
        }
        private void PopManager_OnPopulationChanged(bool value, ulong population)
            => UpdatePopulationInfo(population);
        private void UpdatePopulationInfo(ulong population)
            => currentPopulationLabel.text = population.ToString();
        private void InitializeRaceTable()
        {
            raceMultiColumnListView = RootVE.Q<MultiColumnListView>("RacesMCLV");
            raceMultiColumnListView.sortingMode = ColumnSortingMode.Default;
            raceMultiColumnListView.selectionType = SelectionType.Single;
            raceMultiColumnListView.itemsChosen += OnRaceSelected;
            BuildRaceRows();
            raceMultiColumnListView.itemsSource = raceRows;
            raceMultiColumnListView.columns.Clear();
            CreateRaceColumns();
        }
        private void InitializeRaceInfoPanels()
        {
            if (PopManager.GetAllPopulationGroupsData(out List<PopulationRaceGroup> raceData))
            {
                foreach (var race in raceData)
                {
                    TemplateContainer raceInfoPanelTemplate = raceInfoPanel.CloneTree();
                    raceInfoPanelTemplate.AddToClassList("race-info-panel");
                    raceInfoPanelTemplate.style.display = DisplayStyle.None;
                    raceInfoPanelTemplate.style.flexGrow = 1;
                    Label raceNameLabel = raceInfoPanelTemplate.Q<Label>("raceNK");
                    Button backButton = raceInfoPanelTemplate.Q<Button>("backButton");
                    if (LocalizationManager.Instance.GetLocalizedRaceData(race.GetRaceSO().NameKey, out string value))
                        UpdateLabel(raceNameLabel, value);
                    backButton.RegisterCallback<ClickEvent, string>(OnRaceInfoPanelBackButtonClicked, race.GetRaceSO().ID);
                    cachedRaceInfoPanels.Add(race.GetRaceSO().ID, raceInfoPanelTemplate);
                    racePageMainView.Add(raceInfoPanelTemplate);
                }
            }
        }
        private void UpdateLabel(Label label, string value)
            => label.text = value;
        private void BuildRaceRows()
        {
            raceRows.Clear();
            if (PopManager.GetAllPopulationGroupsData(out List<PopulationRaceGroup> raceData))
            {
                foreach (PopulationRaceGroup race in raceData)
                {
                    raceRows.Add(new RaceRow
                    {
                        Race = race.GetRaceSO(),
                        Population = race.GetTotalPopAmount(),
                        ChildrenAmount = race.GetChildAmount(),
                        AdultsAmount = race.GetAdultAmount(),
                        EldersAmount = race.GetElderAmount(),
                        ActivePopulationAmount = race.GetActiveAmount(),
                        DependablePopulationAmount = race.GetDependablesAmount(),
                    });
                }
            }
        }
        private void CreateRaceColumns()
        {
            raceMultiColumnListView.columns.Add(CreateRaceNameColumn());
            raceMultiColumnListView.columns.Add(CreateRacePopulationColumn());
            raceMultiColumnListView.columns.Add(CreateRaceHappinessColumn());
            raceMultiColumnListView.columns.Add(CreateRaceActivePopulationColumn());
            raceMultiColumnListView.columns.Add(CreateRaceDependablePopulationColumn());
            raceMultiColumnListView.columns.Add(CreateChilderPopulationColumn());
            raceMultiColumnListView.columns.Add(CreateAdultsPopulationColumn());
            raceMultiColumnListView.columns.Add(CreateEldersPopulationColumn());
            raceMultiColumnListView.columns.Add(CreateRaceModifiersColumn());
        }
        private Column CreateRaceNameColumn()
        {
            return new Column
            {
                width = 126,
                stretchable = false,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("race-header");

                    var headerLabel = new Label("Race");
                    headerLabel.AddToClassList("race-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("race-cell");

                    var label = new Label();
                    label.AddToClassList("race-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    label.text = raceRows[i].Race.ToString();
                },
                sortable = true,
                comparison = (a, b)
                    => string.Compare(raceRows[a].Race.ToString(), raceRows[b].Race.ToString())
            };
        }
        private Column CreateRacePopulationColumn()
        {
            return new Column
            {
                width = 100,
                stretchable = false,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("population-header");

                    var headerLabel = new Label("Population");
                    headerLabel.AddToClassList("population-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("population-cell");

                    var label = new Label();
                    label.AddToClassList("population-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    label.text = raceRows[i].Population.ToString("N0");
                },
                sortable = true,
                comparison = (a, b)
                    => raceRows[a].Population.CompareTo(raceRows[b].Population)
            };
        }
        private Column CreateRaceHappinessColumn()
        {
            return new Column
            {
                width = 100,
                stretchable = false,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("happiness-header");

                    var headerLabel = new Label("Happiness");
                    headerLabel.AddToClassList("happiness-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("happiness-cell");

                    var label = new Label();
                    label.AddToClassList("happiness-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    float h = 0f;
                    label.text = $"{h:P0}";
                    label.RemoveFromClassList("good");
                    label.RemoveFromClassList("bad");

                    if (h < 0.35)
                        label.AddToClassList("bad");
                    else
                    if (h > 0.75)
                        label.AddToClassList("good");
                },
                //sortable = true,
                //comparison = (a, b)
                //    => raceRows[a].Happiness.CompareTo(raceRows[b].Happiness)
            };
        }
        private Column CreateRaceActivePopulationColumn()
        {
            return new Column
            {
                width = 100,
                stretchable = false,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("active-population-header");

                    var headerLabel = new Label("Active Population");
                    headerLabel.AddToClassList("active-population-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("active-population-cell");

                    var label = new Label();
                    label.AddToClassList("active-population-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    label.text = raceRows[i].ActivePopulationAmount.ToString("N0");
                },
                sortable = true,
                comparison = (a, b)
                    => raceRows[a].ActivePopulationAmount.CompareTo(raceRows[b].ActivePopulationAmount)
            };
        }
        private Column CreateRaceDependablePopulationColumn()
        {
            return new Column
            {
                width = 100,
                stretchable = false,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("dependable-population-header");

                    var headerLabel = new Label("Dependable Population");
                    headerLabel.AddToClassList("dependable-population-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("dependable-population-cell");

                    var label = new Label();
                    label.AddToClassList("dependable-population-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    label.text = raceRows[i].DependablePopulationAmount.ToString("N0");
                },
                sortable = true,
                comparison = (a, b)
                    => raceRows[a].DependablePopulationAmount.CompareTo(raceRows[b].DependablePopulationAmount)
            };
        }
        private Column CreateChilderPopulationColumn()
        {
            return new Column
            {
                width = 100,
                stretchable = false,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("children-population-header");

                    var headerLabel = new Label("Children Population");
                    headerLabel.AddToClassList("children-population-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("children-population-cell");

                    var label = new Label();
                    label.AddToClassList("children-population-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    label.text = raceRows[i].ChildrenAmount.ToString("N0");
                },
                sortable = true,
                comparison = (a, b)
                    => raceRows[a].ChildrenAmount.CompareTo(raceRows[b].ChildrenAmount)
            };
        }
        private Column CreateAdultsPopulationColumn()
        {
            return new Column
            {
                width = 100,
                stretchable = false,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("adult-population-header");

                    var headerLabel = new Label("Adult Population");
                    headerLabel.AddToClassList("adult-population-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("adult-population-cell");

                    var label = new Label();
                    label.AddToClassList("adult-population-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    label.text = raceRows[i].AdultsAmount.ToString("N0");
                },
                sortable = true,
                comparison = (a, b)
                    => raceRows[a].AdultsAmount.CompareTo(raceRows[b].AdultsAmount)
            };
        }
        private Column CreateEldersPopulationColumn()
        {
            return new Column
            {
                width = 100,
                stretchable = false,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("elder-population-header");

                    var headerLabel = new Label("Elder Population");
                    headerLabel.AddToClassList("elder-population-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("elder-population-cell");

                    var label = new Label();
                    label.AddToClassList("elder-population-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    label.text = raceRows[i].EldersAmount.ToString("N0");
                },
                sortable = true,
                comparison = (a, b)
                    => raceRows[a].EldersAmount.CompareTo(raceRows[b].EldersAmount)
            };
        }
        private Column CreateRaceModifiersColumn()
        {
            return new Column
            {
                stretchable = true,
                optional = false,
                makeHeader = () =>
                {
                    var headerContainer = new VisualElement();
                    headerContainer.AddToClassList("modifiers-header");

                    var headerLabel = new Label("Modifiers");
                    headerLabel.AddToClassList("modifiers-header-label");

                    headerContainer.Add(headerLabel);
                    return headerContainer;
                },
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.AddToClassList("modifiers-cell");

                    var label = new Label();
                    label.AddToClassList("modifiers-cell-label");

                    container.Add(label);
                    return container;
                },
                bindCell = (e, i) =>
                {
                    var label = e.Q<Label>();
                    label.text = "NO";//raceRows[i].Modifiers;
                },
                sortable = false
            };
        }
        private void OnRaceSelected(IEnumerable<object> selectedRace)
        {
            var firstItem = selectedRace.FirstOrDefault();
            if (firstItem == null)
                return;
            if (firstItem is RaceRow raceRow)
            {
                var race = raceRow.Race;
                ShowRaceInfoPanel(race.ToString());
                Debug.Log($"raceSelected: {selectedRace}");
            }
        }
        private void OnRaceInfoPanelBackButtonClicked(ClickEvent evt, string race)
            => HideRaceInfoPanel(race);
        private void HideRaceList()
            => raceMultiColumnListView.style.display = DisplayStyle.None;
        private void ShowRaceList()
            => raceMultiColumnListView.style.display = DisplayStyle.Flex;
        private void HideRaceInfoPanel(string race)
        {
            if (cachedRaceInfoPanels.TryGetValue(race, out var visualElement))
                visualElement.style.display = DisplayStyle.None;
            ShowRaceList();
        }
        private void ShowRaceInfoPanel(string race)
        {
            if (cachedRaceInfoPanels.TryGetValue(race, out var visualElement))
                visualElement.style.display = DisplayStyle.Flex;
            HideRaceList();
        }
        public void ShowPage()
        {
            RootVE.style.display = DisplayStyle.Flex;
        }
        public void HidePage()
        {
            RootVE.style.display = DisplayStyle.None;
        }
        public void UpdatePage()
        {
            //throw new System.NotImplementedException();
        }
    }
}