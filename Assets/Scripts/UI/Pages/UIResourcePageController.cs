using Managers;
using SO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIResourcePViewHolder
    {
        public Label NameLabel, WorkersLabel, AmountLabel;
        public Image IconImage;
        public Button AddWorkerButton;
        public ResourceSO BoundResource;

        public UIResourcePViewHolder(VisualElement root)
        {
            NameLabel = root.Q<Label>("nameLabel");
            WorkersLabel = root.Q<Label>("currentWorkers");
            AmountLabel = root.Q<Label>("currentAmount");
            AddWorkerButton = root.Q<Button>("addWorkerButton");
            IconImage = root.Q<Image>("resourceImage");

            AddWorkerButton.clicked += () =>
            {
                // SomeLogicMaybe?
            };

            NameLabel.RegisterCallback<MouseEnterEvent>(evt =>
            {
                if (BoundResource != null)
                    TooltipManager.Instance.Show(
                        LocalizationManager.Instance.GetLocalizedResourceData(BoundResource.Localization.Name.Key), evt.mousePosition);
            });
            NameLabel.RegisterCallback<MouseMoveEvent>(evt =>
            {
                TooltipManager.Instance.Move(evt.mousePosition);
            });
            NameLabel.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                TooltipManager.Instance.Hide();
            });
        }
    }
    public class UIResourcePageController : IUIPageController, IDisposable
    {
        private VisualElement RootVE;
        private ResourceManagerSO ResourceManagerDataSO;
        private VisualTreeAsset ResourcePanelAsset;
        private ListView MainResourceListView;

        private Managers.ResourceManager RM;
        private LocalizationManager LM;

        private List<ResourceSO> AllResourcesList;
        private List<ResourceSO> VisibleResourcesList;

        private Dictionary<ResourceSO, VisualElement> VisibleResourceElements;

        private TechnologyStateSO technologyStateSO;
        private WorkersStateSO workersStateSO;

        private float textUpdateTimer = 0f;
        private float spriteUpdateTimer = 0f;
        private float globalFrameTime = 0.2f;

        private bool Disposed;

        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            RootVE = page;
            ResourceManagerDataSO = data as ResourceManagerSO;
            if (ResourceManagerDataSO == null)
                Debug.LogError($"NO DATA SO FOUND. Current data type is {data.GetType()}, but expected {typeof(ResourceManagerSO)}");

            InitializeData(ResourceManagerDataSO);
            InitializeListView();
        }
        private void InitializeData(ResourceManagerSO data)
        {
            MainResourceListView = RootVE.Q<ListView>("mainListView");

            RM = Managers.ResourceManager.Instance;
            LM = LocalizationManager.Instance;

            RM.OnVisibleUIResourcesUpdateNeeded += RM_OnVisibleUIResourcesUpdateNeeded;

            ResourcePanelAsset = Resources.Load<VisualTreeAsset>("UI/Panel/ResourcePanelAsset");
            technologyStateSO = Resources.Load<TechnologyStateSO>("SO/TechnologyState");
            workersStateSO = Resources.Load<WorkersStateSO>("SO/WorkersState");

            AllResourcesList = new List<ResourceSO>();
            VisibleResourcesList = new List<ResourceSO>();
            VisibleResourceElements = new Dictionary<ResourceSO, VisualElement>();

            foreach (var resource in data.AllResourcesList)
                AllResourcesList.Add(resource);
            UpdateVisibleResources(data);
        }

        private void RM_OnVisibleUIResourcesUpdateNeeded()
        {
            UpdateVisibleResources(ResourceManagerDataSO);
            RefreshListView();
        }

        private void UpdateVisibleResources(ResourceManagerSO data)
        {
            VisibleResourcesList?.Clear();
            foreach (var resource in AllResourcesList)
            {
                if (data.IsResourceVisible(resource, technologyStateSO) == true)
                {
                    VisibleResourcesList.Add(resource);
                    //Debug.Log($"Adding to Visible: {resource.ID}");
                }
            }
        }
        private void RefreshListView() =>
            MainResourceListView.RefreshItems();
        private void InitializeListView()
        {
            MainResourceListView.itemsSource = VisibleResourcesList;
            UpdateListView();
        }
        private void UpdateListView()
        {
            MainResourceListView.makeItem = () =>
            {
                TemplateContainer ve = ResourcePanelAsset.CloneTree();
                ve.userData = new UIResourcePViewHolder(ve);
                return ve;
            };

            MainResourceListView.bindItem = (element, index) =>
            {
                var resource = VisibleResourcesList[index];
                var holder = element.userData as UIResourcePViewHolder;

                holder.BoundResource = resource;
                VisibleResourceElements[resource] = element;

                holder.NameLabel.text = LocalizationManager.Instance.GetLocalizedResourceData(resource.Localization.Name.Key);

                UpdateSprite(holder);
                UpdateText(holder);
            };
            MainResourceListView.unbindItem = (element, index) =>
            {
                var holder = element.userData as UIResourcePViewHolder;
                if (holder.BoundResource != null)
                    VisibleResourceElements.Remove(holder.BoundResource);
                holder.BoundResource = null;
            };
        }
        private void UpdateText(UIResourcePViewHolder holder)
        {
            var res = holder.BoundResource;
            if (res == null)
                return;
            var amount = RM.GetResourceAmount(res);
            var income = RM.GetCurrentResourceState().GetResourceIncome(res);

            string sign = income > 0 ? "+" : "-";
            holder.AmountLabel.text = $"{amount} ({sign}{income})";
            holder.WorkersLabel.text = workersStateSO.GetWorkersAmount(res).ToString();
        }
        private void UpdateSprite(UIResourcePViewHolder holder)
        {
            var res = holder.BoundResource;
            if (res.AnimationSprites == null || res.AnimationSprites.Count == 0)
                return;
            int frame = GetCurrentFrame(res);
            holder.IconImage.sprite = res.AnimationSprites[frame];
        }
        private int GetCurrentFrame(ResourceSO resource)
        {
            if (resource.AnimationSprites == null || resource.AnimationSprites.Count <= 1)
                return 0;
            int frameIndex = (int)(Time.fixedTime / globalFrameTime);
            return frameIndex % resource.AnimationSprites.Count;
        }
        public void ShowPage() =>
            RootVE.style.display = DisplayStyle.Flex;
        public void HidePage() =>
            RootVE.style.display = DisplayStyle.None;
        public void UpdatePage()
        {
            textUpdateTimer += Time.fixedDeltaTime;
            spriteUpdateTimer += Time.fixedDeltaTime;

            bool shouldUpdateText = textUpdateTimer >= 0.25f;
            bool shouldUpdateSprite = spriteUpdateTimer >= globalFrameTime;

            if (!shouldUpdateText && !shouldUpdateSprite)
                return;

            int globalFrameIndex = (int)(Time.fixedTime / globalFrameTime);

            foreach (var entry in VisibleResourceElements)
            {
                if (entry.Value.userData is UIResourcePViewHolder holder)
                {
                    var res = holder.BoundResource;
                    if (shouldUpdateText)
                        UpdateText(holder);
                    if (shouldUpdateSprite && res.AnimationSprites.Count > 1)
                    {
                        holder.IconImage.sprite = res.AnimationSprites[globalFrameIndex % res.AnimationSprites.Count];
                    }
                }
            }
            if (shouldUpdateText)
                textUpdateTimer = 0f;
            if (shouldUpdateSprite)
                spriteUpdateTimer = 0f;
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            Disposed = true;
            if (RM != null)
                RM.OnVisibleUIResourcesUpdateNeeded -= RM_OnVisibleUIResourcesUpdateNeeded;

            VisibleResourceElements?.Clear();
            VisibleResourceElements = null;
            VisibleResourcesList?.Clear();
            VisibleResourcesList = null;
        }
    }
}
