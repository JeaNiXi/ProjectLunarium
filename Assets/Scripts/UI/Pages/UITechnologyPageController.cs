using Managers;
using SO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
namespace UI
{
    // Research Not available Color D74E63
    // Research Available Color
    // Researched Color
    // Research in Progress Color
    public class UITechnologyPageController : IUIPageController
    {
        private VisualElement page;
        private VisualElement techPageMainView;
        private TechnologyManagerSO data;
        private VisualTreeAsset technologyPanelAsset;
        private VisualTreeAsset technologyPanelInfoAsset;
        private ScrollView scrollView;
        private TechnologyStateSO technologyStateSO;
        private Dictionary<string, VisualElement> cachedTechInfoPanels;
        private Dictionary<TechnologySO, VisualElement> visibleElements;
        private TechnologySO currentResearchedTechCache;
        private int maxTechTier;
        private bool isUpdating;
        private readonly float updateTime = 0.2f;
        private float pageTime;
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            this.page = page;
            this.data = data as TechnologyManagerSO;
            if (this.data == null)
                Debug.Log("NO DATA SO FOUND");
            techPageMainView = page.Q<VisualElement>("techPageMainView");
            scrollView = page.Q<ScrollView>("mainScrollView");
            technologyPanelAsset = Resources.Load<VisualTreeAsset>("UI/Panel/TechPanelAsset");
            technologyPanelInfoAsset = Resources.Load<VisualTreeAsset>("UI/Panel/TechPanelInfoAsset");
            technologyStateSO = Resources.Load<TechnologyStateSO>("SO/TechnologyState");
            cachedTechInfoPanels = new Dictionary<string, VisualElement>();
            visibleElements = new Dictionary<TechnologySO, VisualElement>();
            InitializeData(this.data);
            InitializeScrollView();
            CreateTechInfoPanels(this.data);
        }
        private void InitializeData(TechnologyManagerSO data)
        {
            maxTechTier = data.GetMaxTechTier();
        }
        private void InitializeScrollView()
        {
            UpdateScrollView(data);
        }
        private void UpdateScrollView(TechnologyManagerSO data)
        {
            for (int tier = 0; tier <= maxTechTier; tier++)
            {
                VisualElement column = new VisualElement();
                column.AddToClassList("techColumn");
                List<TechnologySO> techTierList = data.GetTechByTier(tier);
                foreach (TechnologySO tech in techTierList)
                {
                    TemplateContainer ve = technologyPanelAsset.CloneTree();
                    ve.AddToClassList("techPanel");
                    visibleElements.Add(tech, ve);
                    var techMainColorVE = ve.Q<VisualElement>("mainColorElement");
                    var techInfoPanelButton = ve.Q<Button>("mainTechInfoButton");
                    var techNameLabel = ve.Q<Label>("techNameLabel");
                    var techResearchProgressBar = ve.Q<ProgressBar>("techResearchProgressBar");
                    var techStartResearchButton = ve.Q<Button>("techStartResearchButton");
                    techMainColorVE.style.backgroundColor = GetTechResearchColor(tech);
                    techInfoPanelButton.RegisterCallback<ClickEvent, string>(OnTechInfoPanelButtonClicked, tech.ID);
                    techStartResearchButton.SetEnabled(GetResearchButtonAvailability(tech));
                    techStartResearchButton.text = GetResearchButtonLabelText(tech);
                    techStartResearchButton.RegisterCallback<ClickEvent, TechnologySO>(OnStartResearchButtonClicked, tech);
                    techNameLabel.text = tech.NameKey;
                    column.Add(ve);
                    Debug.Log($"Adding Technology Tier: {tech.Tier}, Name: {tech.NameKey}");
                }
                scrollView.Add(column);
                Debug.Log($"Adding Column with Tiers: {tier}");
            }
        }
        private void CreateTechInfoPanels(TechnologyManagerSO data)
        {
            foreach (var tech in data.allTechnologies)
            {
                TemplateContainer techInfoPanelTemplate = technologyPanelInfoAsset.CloneTree();
                techInfoPanelTemplate.AddToClassList("techInfoPanel");
                techInfoPanelTemplate.style.display = DisplayStyle.None;
                techInfoPanelTemplate.style.flexGrow = 1;
                Label techNameLabel = techInfoPanelTemplate.Q<Label>("techNameLabel");
                Button backButton = techInfoPanelTemplate.Q<Button>("backButton");
                backButton.RegisterCallback<ClickEvent, string>(OnTechInfoPanelBackClicked, tech.ID);
                techNameLabel.text = tech.NameKey;
                cachedTechInfoPanels.Add(tech.ID, techInfoPanelTemplate);
                techPageMainView.Add(techInfoPanelTemplate);
                Debug.Log($"Added to Cache Tech Info Panel: {tech.ID}");
            }
        }
        private Color GetTechResearchColor(TechnologySO tech)
        {
            if (technologyStateSO.researchedTechnologies.Contains(tech))
                return Color.green;
            else return Color.red;
        }
        private bool GetResearchButtonAvailability(TechnologySO tech)
            => TechnologyManager.Instance.IsTechResearchAvailable(tech);
        private string GetResearchButtonLabelText(TechnologySO tech)
            => TechnologyManager.Instance.GetReseachButtonLabelText(tech);
        private bool IsTechnologyResearchInProgress()
            => TechnologyManager.Instance.IsTechnologyResearchInProgress();
        private TechnologySO GetCurrentResearchedTechnology()
            => TechnologyManager.Instance.GetCurrentResearchInProgressTechnology();
        private VisualElement GetElementByTechSO(TechnologySO tech)
        {
            if (visibleElements.TryGetValue(tech, out var element))
                return element;
            return null;
        }
        private float GetCurrentReseachProgressBarValue()
            => TechnologyManager.Instance.GetCurrentReseachProgressBarValue();
        private void OnTechInfoPanelButtonClicked(ClickEvent evt, string techID)
        {
            Debug.Log($"Clicked Button: {techID}");
            ShowTechInfoPanel(techID);
        }
        private void OnStartResearchButtonClicked(ClickEvent evt, TechnologySO tech)
        {
            Debug.Log($"Started researching: {tech}.");
            TechnologyManager.Instance.QueueTechnologyResearch(tech);
        }
        private void OnTechInfoPanelBackClicked(ClickEvent evt, string techID)
        {
            Debug.Log($"Clicked Go Back Button From Tech: {techID}");
            HideInfoPanel(techID);
        }
        public void ShowPage()
        {
            page.style.display = DisplayStyle.Flex;
        }
        public void HidePage()
        {
            page.style.display = DisplayStyle.None;
        }
        private void ShowTechInfoPanel(string techID)
        {
            scrollView.style.display = DisplayStyle.None;
            if (cachedTechInfoPanels.TryGetValue(techID, out var techVisualElement))
                techVisualElement.style.display = DisplayStyle.Flex;
        }
        private void HideInfoPanel(string techID)
        {
            if (cachedTechInfoPanels.TryGetValue(techID, out var techVisualElement))
                techVisualElement.style.display = DisplayStyle.None;
            scrollView.style.display = DisplayStyle.Flex;
        }
        public void UpdatePage()
        {
            if (!isUpdating)
            {
                pageTime += Time.deltaTime;
                if (pageTime > updateTime)
                {
                    if (visibleElements == null || visibleElements.Count == 0)
                        return;
                    isUpdating = true;
                    UpdateTechPanels();
                    UpdateTechnologyResearchStatus();
                    pageTime = 0;
                    isUpdating = false;
                }
            }
        }
        private void UpdateTechPanels()
        {
            foreach (var key in visibleElements)
                UpdateButtons(key.Value, key.Key);
        }
        private void UpdateButtons(VisualElement element, TechnologySO tech)
        {
            var researchButton = element.Q<Button>("techStartResearchButton");
            if (researchButton != null)
            {
                researchButton.SetEnabled(GetResearchButtonAvailability(tech));
                researchButton.text = GetResearchButtonLabelText(tech);
            }
        }
        private void UpdateTechnologyResearchStatus()
        {
            var previousTech = currentResearchedTechCache;

            if (!IsTechnologyResearchInProgress() && previousTech == null)
                return;
            if (!IsTechnologyResearchInProgress() && previousTech != null)
                SetProgressBarValue(previousTech, 100f);
            currentResearchedTechCache = GetCurrentResearchedTechnology();
            if (currentResearchedTechCache == null)
                return;
            SetProgressBarValue(currentResearchedTechCache, GetCurrentReseachProgressBarValue());
        }
        private void SetProgressBarValue(TechnologySO tech, float value)
        {
            var techProgressBarElement = GetElementByTechSO(tech);
            var techProgressBar = techProgressBarElement.Q<ProgressBar>("techResearchProgressBar");
            if (techProgressBar != null)
                techProgressBar.value = value;
        }
    }
}