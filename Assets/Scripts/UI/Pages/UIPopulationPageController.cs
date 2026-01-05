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
        private VisualElement page;
        private VisualElement racePageMainView;
        private PopulationManagerSO data;
        private PopulationStateSO populationStateSO;
        private MultiColumnListView raceMultiColumnListView;
        private VisualTreeAsset raceInfoPanel;
        private Dictionary<string, VisualElement> cachedRaceInfoPanels;
        private List<RaceRow> raceRows = new();
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            this.page = page;
            this.data = data as PopulationManagerSO;
            if (this.data == null)
                Debug.Log("NO DATA SO FOUND");
            racePageMainView = page.Q<VisualElement>("populationRaceListVE");
            populationStateSO = Resources.Load<PopulationStateSO>("SO/PopulationState");
            raceInfoPanel = Resources.Load<VisualTreeAsset>("UI/Panel/RacePanelInfoAsset");
            cachedRaceInfoPanels= new Dictionary<string, VisualElement>();
            InitializeData();
        }
        private void InitializeData()
        {
            UpdatePopulationInfo();
            InitializeRaceTable();
            InitializeRaceInfoPanels();
        }
        private void UpdatePopulationInfo()
        {
            Label currentPopulationLabel = page.Q<Label>("currentPopulation");
            currentPopulationLabel.text = populationStateSO.CurrentPopulation.ToString();
        }
        private void InitializeRaceTable()
        {
            raceMultiColumnListView = page.Q<MultiColumnListView>("RacesMCLV");
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
            foreach(var race in populationStateSO.Races)
            {
                TemplateContainer raceInfoPanelTemplate = raceInfoPanel.CloneTree();
                raceInfoPanelTemplate.AddToClassList("race-info-panel");
                raceInfoPanelTemplate.style.display = DisplayStyle.None;
                raceInfoPanelTemplate.style.flexGrow = 1;
                Label raceNameLabel = raceInfoPanelTemplate.Q<Label>("raceNK");
                Button backButton = raceInfoPanelTemplate.Q<Button>("backButton");
                raceNameLabel.text = race.Race.ToString();
                backButton.RegisterCallback<ClickEvent, string>(OnRaceInfoPanelBackButtonClicked, race.Race.ToString());
                cachedRaceInfoPanels.Add(race.Race.ToString(), raceInfoPanelTemplate);
                racePageMainView.Add(raceInfoPanelTemplate);
            }
        }
        private void BuildRaceRows()
        {
            raceRows.Clear();
            foreach (var race in populationStateSO.Races)
            {
                raceRows.Add(new RaceRow
                {
                    Race = race.Race,
                    Population = race.Population,
                    Happiness = race.Happiness,
                    ActivePopulationAmount = race.ActivePopulationAmount,
                    DependablePopulationAmount = race.DependablePopulationAmount,
                    ChildrenAmount = race.ChildrenAmount,
                    AdultsAmount = race.AdultsAmount,
                    EldersAmount = race.EldersAmount,
                    Modifiers = string.Join(",", race.Modifiers)
                });
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
                    float h = raceRows[i].Happiness;
                    label.text = $"{h:P0}";
                    label.RemoveFromClassList("good");
                    label.RemoveFromClassList("bad");

                    if (h < 0.35)
                        label.AddToClassList("bad");
                    else
                    if (h > 0.75)
                        label.AddToClassList("good");
                },
                sortable = true,
                comparison = (a, b)
                    => raceRows[a].Happiness.CompareTo(raceRows[b].Happiness)
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
                    label.text = raceRows[i].Modifiers;
                },
                sortable = false
            };
        }
        private void OnRaceSelected(IEnumerable<object> selectedRace)
        {
            var firstItem = selectedRace.FirstOrDefault();
            if (firstItem == null)
                return;
            if(firstItem is RaceRow raceRow)
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
            page.style.display = DisplayStyle.Flex;
        }
        public void HidePage()
        {
            page.style.display = DisplayStyle.None;
        }
        public void UpdatePage()
        {
            //throw new System.NotImplementedException();
        }
    }
}