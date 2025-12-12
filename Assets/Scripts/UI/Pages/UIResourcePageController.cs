using Mono.Cecil;
using SO;
using System.Collections.Generic;
using System.Resources;
using Unity.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;
namespace UI
{
    public class UIResourcePageController : IUIPageController
    {
        private VisualElement page;
        private ResourceManagerSO data;
        private VisualTreeAsset resourcePanelAsset;
        private ListView listView;
        private List<ResourceSO> AllResourcesList;
        private List<ResourceSO> VisibleResourcesList;
        private bool listUpdated;
        private bool isUpdating;
        private bool isSwitchingToPage;
        private List<float> spriteTimers;
        private List<int> spriteFrames;
        private float frameTime = 0f;
        private float globalFrameTime = 0.2f;
        private Dictionary<int, VisualElement> visibleElements;
        private ResourceStateSO resourceStateSO;
        private TechnologyStateSO technologyStateSO;
        private WorkersStateSO workersStateSO;
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            this.page = page;
            this.data = data as ResourceManagerSO;
            if (this.data == null)
                Debug.Log("NO DATA SO FOUND");
            listView = page.Q<ListView>("mainListView");

            resourcePanelAsset = Resources.Load<VisualTreeAsset>("UI/Panel/ResourcePanelAsset");
            resourceStateSO = Resources.Load<ResourceStateSO>("SO/ResourceState");
            technologyStateSO = Resources.Load<TechnologyStateSO>("SO/TechnologyState");
            workersStateSO = Resources.Load<WorkersStateSO>("SO/WorkersState");

            visibleElements = new Dictionary<int, VisualElement>();
            InitializeData(this.data);
            InitializeListView();
        }
        private void InitializeData(ResourceManagerSO data)
        {
            AllResourcesList = new List<ResourceSO>();
            VisibleResourcesList = new List<ResourceSO>();
            foreach (var resource in data.AllResourcesList)
                AllResourcesList.Add(resource);
            UpdateVisibleResources(data);
            spriteTimers = new List<float>(AllResourcesList.Count);
            spriteFrames = new List<int>(AllResourcesList.Count);
            for (int i = 0; i < AllResourcesList.Count; i++)
            {
                spriteTimers.Add(0f);
                spriteFrames.Add(0);
            }
        }
        private void UpdateVisibleResources(ResourceManagerSO data)
        {
            VisibleResourcesList?.Clear();
            foreach (var resource in AllResourcesList)
            {
                if (data.IsResourceVisible(resource, technologyStateSO) == true)
                {
                    VisibleResourcesList.Add(resource);
                    Debug.Log($"Adding to Visible: {resource.ID}");
                }
            }

        }
        private void InitializeListView()
        {
            listView.itemsSource = VisibleResourcesList;
            UpdateListView();
        }
        private void UpdateListView()
        {
            listView.makeItem = () =>
            {
                TemplateContainer ve = resourcePanelAsset.CloneTree();
                return ve;
            };
            listView.bindItem = (element, index) =>
            {
                visibleElements[index] = element;

                Label resourceNameLabel = element.Q<Label>("nameLabel");
                Image resourceImage = element.Q<Image>("resourceImage");
                Button addWorkerButton = element.Q<Button>("addWorkerButton");
                Label currenWorkers = element.Q<Label>("currentWorkers");
                Label currentAmount = element.Q<Label>("currentAmount");

                resourceNameLabel.text = VisibleResourcesList[index].NameKey;
                resourceImage.sprite = VisibleResourcesList[index].AnimationSprites[0];
                addWorkerButton.RegisterCallback<ClickEvent, ResourceSO>(OnAddWorkerButtonClicked, VisibleResourcesList[index]);
                currenWorkers.text = workersStateSO.GetWorkersAmount(VisibleResourcesList[index]).ToString();
                currentAmount.text = resourceStateSO.GetResourceAmount(VisibleResourcesList[index]).ToString();

                listUpdated = true;
            };
            listView.unbindItem = (element, index) =>
            {
                if (visibleElements.ContainsKey(index))
                    visibleElements.Remove(index);
            };
        }
        private void OnAddWorkerButtonClicked(ClickEvent evt, ResourceSO resource)
        {
            Managers.WorkersManager.Instance.AddWorkerToResource(resource);
        }
        public void ShowPage()
        {
            page.style.display = DisplayStyle.Flex;
            isSwitchingToPage = true;
        }
        public void HidePage()
        {
            page.style.display = DisplayStyle.None;
        }
        public void UpdatePage()
        {
            if (!listUpdated)
                return;
            if (!isUpdating)
                UpdateUIResourceData();
        }
        private void UpdateUIResourceData()
        {
            if (visibleElements == null || visibleElements.Count == 0)
                return;
            frameTime += Time.deltaTime;
            if (!isSwitchingToPage && frameTime < globalFrameTime)
                return;
            isUpdating = true;
            foreach (var kv in visibleElements)
            {
                int index = kv.Key;
                VisualElement element = kv.Value;

                UpdateSprites(element, index);
                UpdateAmounts(element, index);
                UpdateWorkersAmounts(element, index);

                frameTime = 0;
            }
            if (isSwitchingToPage)
                isSwitchingToPage = false;
            isUpdating = false;
        }
        private void UpdateSprites(VisualElement element, int index)
        {
            spriteFrames[index]++;
            if (spriteFrames[index] >= VisibleResourcesList[index].AnimationSprites.Count)
                spriteFrames[index] = 0;

            var image = element.Q<Image>("resourceImage");
            if (image != null)
                image.sprite = VisibleResourcesList[index].AnimationSprites[spriteFrames[index]];
        }
        private void UpdateAmounts(VisualElement element, int index)
        {
            if (visibleElements == null || visibleElements.Count == 0)
                return;
            var amountLabel = element.Q<Label>("currentAmount");
            if (amountLabel != null)
                amountLabel.text = resourceStateSO.resourcesAmountsList[index].ToString();
        }
        private void UpdateWorkersAmounts(VisualElement element, int index)
        {
            if (visibleElements == null || visibleElements.Count == 0)
                return;
            var workersLabel = element.Q<Label>("currentWorkers");
            if (workersLabel != null)
                workersLabel.text = workersStateSO.currentWorkersStateList[index].ToString();
        }
    }
}
