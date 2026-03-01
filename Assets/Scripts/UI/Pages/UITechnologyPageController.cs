using Managers;
using SO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UITechTopPanelViewHolder
    {
        public VisualElement Root;
        public Label NameLabel;
        public ProgressBar ProgressBar;
        public Button InfoButton;

        public VisualElement PossibleBreakthroughVE;
        public VisualElement TechsOpenVE;

        public UITechTopPanelViewHolder(VisualElement root)
        {
            Root = root;
            NameLabel = root.Q<Label>("currentRersearchNK");
            ProgressBar = root.Q<ProgressBar>();
            InfoButton = root.Q<Button>("currentResearchInfoButton");
            PossibleBreakthroughVE = root.Q<VisualElement>("possibleBreakthroughVE");
            TechsOpenVE = root.Q<VisualElement>("techOpensVE");
        }
        public void SetVisible(bool visible) =>
            Root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
    public class UITechPViewHolder
    {
        public Label NameLabel;
        public Button InfoButton;
        public VisualElement StatusIndicator;
        public TechnologySO BoundTech;

        public UITechPViewHolder(VisualElement root)
        {
            NameLabel = root.Q<Label>("techNameLabel");
            InfoButton = root.Q<Button>("mainTechInfoButton");
            StatusIndicator = root.Q<VisualElement>("mainColorElement");
        }
        public void UpdateVisuals(bool isAvailable, bool isResearched, bool isCurrent, float progressValue)
        {
            StatusIndicator.style.backgroundColor = isResearched ? Color.green : (isCurrent) ? Color.yellow : Color.red;
        }
    }
    public class UITechInfoBinder
    {
        private VisualElement Root;
        public Label NameLabel;
        public Label DescriptionLabel;
        public Button BackButton;
        public Button StartResearchButton;
        public TechnologySO CurrentTech;

        public UITechInfoBinder(VisualElement root, System.Action onBackPressed, System.Action<TechnologySO> onResearchStart)
        {
            Root = root;
            NameLabel = root.Q<Label>("nameLabelInfoPanel");
            DescriptionLabel = root.Q<Label>("infoPanelDescriptionLabel");
            BackButton = root.Q<Button>("backButton");
            StartResearchButton = root.Q<Button>("mainStartResearchButton");

            StartResearchButton.clicked += () => onResearchStart?.Invoke(CurrentTech);
        }
        public void Bind(TechnologySO tech)
        {
            CurrentTech = tech;
            NameLabel.text = LocalizationManager.Instance.GetLocalizedTechnologySOData(tech.Localization.Name.Key);
            DescriptionLabel.text = LocalizationManager.Instance.GetLocalizedTechnologySOData(tech.Localization.Description.Key);

            bool canAfford = TechnologyManager.Instance.AreResourcesAvailable(tech);
            StartResearchButton.SetEnabled(canAfford);
            StartResearchButton.text = canAfford ? "Начать исследование." : "Недостаточно ресурсов.";

            Root.style.display = DisplayStyle.Flex;
        }
        public void Hide() =>
            Root.style.display = DisplayStyle.None;
        public void Show() =>
            Root.style.display = DisplayStyle.Flex;
    }
    public class UITechnologyPageController : IUIPageController
    {
        private VisualElement RootVE;
        private TechnologyManagerSO TechManagerDataSO;
        private ScrollView MainScrollView;
        private VisualElement TechPageMainView;

        private VisualTreeAsset CardAsset;
        private VisualTreeAsset InfoAsset;

        private Dictionary<TechnologySO, UITechPViewHolder> TechCards;
        private UITechInfoBinder InfoPanelBinder;
        private UITechTopPanelViewHolder TechTopPanelViewHolder;

        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            RootVE = page;
            TechManagerDataSO = data as TechnologyManagerSO;

            MainScrollView = RootVE.Q<ScrollView>("mainScrollView");
            TechPageMainView = RootVE.Q<VisualElement>("techPageMainView");

            CardAsset = Resources.Load<VisualTreeAsset>("UI/Panel/TechPanelAsset");
            InfoAsset = Resources.Load<VisualTreeAsset>("UI/Panel/TechPanelInfoAsset");

            TechCards = new Dictionary<TechnologySO, UITechPViewHolder>();

            SetupTopPanel();
            SetupInfoPanel();

            TechnologyManager.Instance.OnOfferedTechsRefreshedEvent += TM_OnOfferedTechsRefreshed;

            BuildOfferedTechnologies();
        }
        private void SetupTopPanel()
        {
            var topVE = RootVE.Q<VisualElement>("upPanelCurrentResearch");
            TechTopPanelViewHolder = new UITechTopPanelViewHolder(topVE);

            TechTopPanelViewHolder.InfoButton.clicked += () =>
            {
                var current = TechnologyManager.Instance.GetCurrentResearchInProgressTechnology();
                if (current != null)
                    ShowTechDetail(current);
            };
            UpdateTopPanelVisuals();
        }
        private void UpdateTopPanelVisuals()
        {
            var current = TechnologyManager.Instance.GetCurrentResearchInProgressTechnology();
            if (current == null)
            {
                TechTopPanelViewHolder.SetVisible(true);
                return;
            }
            TechTopPanelViewHolder.SetVisible(true);
            TechTopPanelViewHolder.NameLabel.text = LocalizationManager.Instance.GetLocalizedTechnologySOData(current.Localization.Name.Key);
        }
        private void SetupInfoPanel()
        {
            var infoVE = RootVE.Q<VisualElement>("mainInfoPanelVisualElement");
            InfoPanelBinder = new UITechInfoBinder(infoVE,
            onBackPressed: () =>
            {
                InfoPanelBinder.Hide();
                MainScrollView.style.display = DisplayStyle.Flex;
            },
            onResearchStart: (tech) =>
            {
                if (tech != null)
                    TechnologyManager.Instance.StartResearch(tech);
            });
            InfoPanelBinder.Hide();
        }
        private void BuildOfferedTechnologies()
        {
            MainScrollView.Clear();
            TechCards.Clear();

            var offeredTechs = TechnologyManager.Instance.GetOfferedTechnologies();
            if (offeredTechs == null || offeredTechs.Count == 0)
            {
                Debug.Log("No Offered Techs at the Moment");
                return;
            }
            foreach (var tech in offeredTechs)
            {
                VisualElement cardVE = CardAsset.CloneTree();
                var holder = new UITechPViewHolder(cardVE);
                holder.BoundTech = tech;

                holder.NameLabel.text = LocalizationManager.Instance.GetLocalizedTechnologySOData(holder.BoundTech.Localization.Name.Key);

                holder.InfoButton.clicked += () => ShowTechDetail(tech);

                TechCards.Add(tech, holder);
                MainScrollView.Add(cardVE);
            }
            RefreshButtonsState();
        }
        private void ShowTechDetail(TechnologySO tech)
        {
            InfoPanelBinder.Bind(tech);
            InfoPanelBinder.Show();
        }
        private void StartResearch(TechnologySO tech)
        {
            TechnologyManager.Instance.StartResearch(tech);
            BuildOfferedTechnologies();
            UpdateTopResearchPanel();
        }
        public void UpdateTopResearchPanel()
        {
            var current = TechnologyManager.Instance.GetCurrentResearchInProgressTechnology();
            if (current != null)
            {
                TechTopPanelViewHolder.NameLabel.text = LocalizationManager.Instance.GetLocalizedTechnologySOData(current.Localization.Name.Key);
            }
            else
                TechTopPanelViewHolder.NameLabel.text = "Исследование не выбрано.";
        }
        private void RefreshButtonsState()
        {
            foreach (var entry in TechCards)
            {
                var tech = entry.Key;
                var holder = entry.Value;

                bool isResearched = TechnologyManager.Instance.IsTechnologyResearched(tech);
                bool isAvailable = TechnologyManager.Instance.IsTechResearchAvailable(tech);

                holder.StatusIndicator.style.backgroundColor = isResearched ? Color.green : Color.white;
            }
        }
        private void TM_OnOfferedTechsRefreshed()
        {
            BuildOfferedTechnologies();
            UpdateTopResearchPanel();
        }
        public void UpdatePage()
        {
            var current = TechnologyManager.Instance.GetCurrentResearchInProgressTechnology();
            if (current != null)
            {
                float progress = TechnologyManager.Instance.GetCurrentReseachProgressPercent(current) * 100f;
                TechTopPanelViewHolder.ProgressBar.value = progress;
                TechTopPanelViewHolder.ProgressBar.title = $"{Mathf.Round(progress)}%";
                if (TechCards.TryGetValue(current, out var holder))
                {
                    //holder.Progress.value = progress;
                }
            }
            else
            {
                TechTopPanelViewHolder.ProgressBar.value = 0;
                TechTopPanelViewHolder.ProgressBar.title = "Ожидание выбора...";
            }
            if (InfoPanelBinder != null && InfoPanelBinder.CurrentTech != null)
            {
                bool canAfford = TechnologyManager.Instance.AreResourcesAvailable(InfoPanelBinder.CurrentTech);
                if (InfoPanelBinder.StartResearchButton.enabledSelf != canAfford)
                {
                    InfoPanelBinder.StartResearchButton.SetEnabled(canAfford);
                    InfoPanelBinder.StartResearchButton.text = canAfford ? "Начать исследование" : "Недостаточно ресурсов";
                }
            }
        }
        public void ShowPage() =>
            RootVE.style.display = DisplayStyle.Flex;
        public void HidePage() =>
            RootVE.style.display = DisplayStyle.None;
    }
}