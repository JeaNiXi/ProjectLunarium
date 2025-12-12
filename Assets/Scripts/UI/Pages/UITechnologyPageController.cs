using Managers;
using SO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
namespace UI
{
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
        private int maxTechTier;
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
                    var techInfoPanelButton = ve.Q<Button>("mainTechInfoButton");
                    var techNameLabel = ve.Q<Label>("techNameLabel");
                    techInfoPanelButton.RegisterCallback<ClickEvent, string>(OnTechInfoPanelClicked, tech.ID);
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

        private void OnTechInfoPanelClicked(ClickEvent evt, string techID)
        {
            Debug.Log($"Clicked Button: {techID}");
            ShowTechInfoPanel(techID);
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
            //throw new System.NotImplementedException();
        }
    }
}