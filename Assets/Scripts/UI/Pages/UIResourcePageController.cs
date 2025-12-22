using Managers;
using Mono.Cecil;
using SO;
using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<ResourceSO, float> spriteTimers;
        private Dictionary<ResourceSO, int> spriteFrames;
        private float frameTime = 0f;
        private float globalFrameTime = 0.2f;
        //private Dictionary<int, VisualElement> visibleElements;
        private Dictionary<ResourceSO, VisualElement> visibleResourceElements;
        //private Dictionary<int, ResourceSO> visibleResourceIndexes;
        private ResourceStateSO resourceStateSO;
        private TechnologyStateSO technologyStateSO;
        private WorkersStateSO workersStateSO;
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            this.page = page;
            this.data = data as ResourceManagerSO;
            if (this.data == null)
                Debug.LogError($"NO DATA SO FOUND. Current data type is {data.GetType()}, but expected {typeof(ResourceManagerSO)}");
            listView = page.Q<ListView>("mainListView");

            resourcePanelAsset = Resources.Load<VisualTreeAsset>("UI/Panel/ResourcePanelAsset");
            resourceStateSO = Resources.Load<ResourceStateSO>("SO/ResourceState");
            technologyStateSO = Resources.Load<TechnologyStateSO>("SO/TechnologyState");
            workersStateSO = Resources.Load<WorkersStateSO>("SO/WorkersState");

            //visibleElements = new Dictionary<int, VisualElement>();
            visibleResourceElements = new Dictionary<ResourceSO, VisualElement>();
            //visibleResourceIndexes = new Dictionary<int, ResourceSO>();
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
            spriteTimers = new Dictionary<ResourceSO, float>();
            spriteFrames = new Dictionary<ResourceSO, int>();
            for (int i = 0; i < AllResourcesList.Count; i++)
            {
                spriteTimers.Add(AllResourcesList[i], 0f);
                spriteFrames.Add(AllResourcesList[i], 0);
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
            if (GameManager.Instance.IsVisibleResourcesUpdateNeeded)
                GameManager.Instance.SetIsVisibleResourcesUpdateNeeded(false);
        }
        private void RefreshListView()
        {
            listView.Rebuild();
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
                var resource = VisibleResourcesList[index];
                visibleResourceElements[resource] = element;
                element.userData = resource;

                //visibleElements[index] = element;
                //visibleResourceIndexes[index] = VisibleResourcesList[index];
                //private Dictionary<ResourceSO, VisualElement> visibleResourceElements;
                //private Dictionary<int, ResourceSO> visibleResourceIndexes;


                Label resourceNameLabel = element.Q<Label>("nameLabel");
                Image resourceImage = element.Q<Image>("resourceImage");
                Button addWorkerButton = element.Q<Button>("addWorkerButton");
                Label currenWorkers = element.Q<Label>("currentWorkers");
                Label currentAmount = element.Q<Label>("currentAmount");

                resourceNameLabel.text = LocalizationManager.Instance.GetLocalizedResourceName(resource.NameKey);
                resourceNameLabel.RegisterCallback<MouseEnterEvent>(evt
                    => TooltipManager.Instance.Show(LocalizationManager.Instance.GetLocalizedResourceDescription(resource.DescriptionKey), evt.mousePosition));
                resourceNameLabel.RegisterCallback<MouseMoveEvent>(evt
                    => TooltipManager.Instance.Move(evt.mousePosition));
                resourceNameLabel.RegisterCallback<MouseLeaveEvent>(evt
                    => TooltipManager.Instance.Hide());
                resourceImage.sprite = resource.AnimationSprites[0];
                addWorkerButton.RegisterCallback<ClickEvent, ResourceSO>(OnAddWorkerButtonClicked, VisibleResourcesList[index]);
                currenWorkers.text = workersStateSO.GetWorkersAmount(VisibleResourcesList[index]).ToString();
                currentAmount.text = resourceStateSO.GetResourceAmount(VisibleResourcesList[index]).ToString();

                listUpdated = true;
            };
            listView.unbindItem = (element, index) =>
            {
                if (element.userData is ResourceSO resource)
                    visibleResourceElements.Remove(resource);
                element.userData = null;
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
            if (visibleResourceElements == null || visibleResourceElements.Count == 0)
                return;
            frameTime += Time.deltaTime;
            if (!isSwitchingToPage && frameTime < globalFrameTime)
                return;
            isUpdating = true;
            if (GameManager.Instance.IsVisibleResourcesUpdateNeeded)
            {
                UpdateVisibleResources(data);
                RefreshListView();
            }
            foreach (var kv in visibleResourceElements)
            {
                ResourceSO resource = kv.Key;
                VisualElement element = kv.Value;

                UpdateSprites(element, resource);
                UpdateAmounts(element, resource);
                UpdateWorkersAmounts(element, resource);

                frameTime = 0;
            }
            if (isSwitchingToPage)
                isSwitchingToPage = false;
            isUpdating = false;
        }
        private void UpdateSprites(VisualElement element, ResourceSO resource)
        {
            if (!spriteFrames.ContainsKey(resource))
                spriteFrames[resource] = 0;
            spriteFrames[resource]++;
            if (spriteFrames[resource] >= resource.AnimationSprites.Count)
                spriteFrames[resource] = 0;
            element.Q<Image>("resourceImage").sprite = resource.AnimationSprites[spriteFrames[resource]];
        }
        private void UpdateAmounts(VisualElement element, ResourceSO resource)
        {
            if (visibleResourceElements == null || visibleResourceElements.Count == 0)
                return;
            var amountLabel = element.Q<Label>("currentAmount");
            if (amountLabel != null)
                amountLabel.text = resourceStateSO.GetResourceAmount(resource).ToString();
        }
        private void UpdateWorkersAmounts(VisualElement element, ResourceSO resource)
        {
            if (visibleResourceElements == null || visibleResourceElements.Count == 0)
                return;
            if (workersStateSO.currentWorkersAmountStateList.Count == 0)
                return;
            var workerLabel = element.Q<Label>("currentWorkers");
            if (workerLabel != null)
                workerLabel.text = workersStateSO.GetWorkersAmount(resource).ToString();
        }
    }
}
