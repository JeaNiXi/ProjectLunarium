using Mono.Cecil;
using SO;
using State;
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
        private bool listUpdated;
        private List<float> spriteTimers;
        private List<int> spriteFrames;
        private float frameTime = 0f;
        private float globalFrameTime = 0.2f;
        private Dictionary<int, VisualElement> visibleElements;
        private ResourceStateSO resourceStateSO;
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            this.page = page;
            this.data = data as ResourceManagerSO;
            if (this.data == null)
                Debug.Log("NO DATA SO FOUND");
            listView = page.Q<ListView>("mainListView");

            resourcePanelAsset = Resources.Load<VisualTreeAsset>("UI/Panel/ResourcePanelAsset");
            resourceStateSO = Resources.Load<ResourceStateSO>("SO/ResourceState");
            visibleElements = new Dictionary<int, VisualElement>();
            InitializeData(this.data);
            InitializeListView();
        }
        private void InitializeData(ResourceManagerSO data)
        {
            AllResourcesList = new List<ResourceSO>();
            foreach (var resource in data.AllResourcesList)
                AllResourcesList.Add(resource);
            spriteTimers = new List<float>(AllResourcesList.Count);
            spriteFrames = new List<int>(AllResourcesList.Count);
            for (int i = 0; i < AllResourcesList.Count; i++)
            {
                spriteTimers.Add(0f);
                spriteFrames.Add(0);
            }
        }
        private void InitializeListView()
        {
            listView.itemsSource = AllResourcesList;
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
                Label currentAmount = element.Q<Label>("currentAmount");

                resourceNameLabel.text = AllResourcesList[index].NameKey;
                resourceImage.sprite = AllResourcesList[index].AnimationSprites[0];
                currentAmount.text = resourceStateSO.resourcesAmounts[index].ToString();

                listUpdated = true;
            };
            listView.unbindItem = (element, index) =>
            {
                if (visibleElements.ContainsKey(index))
                    visibleElements.Remove(index);
            };
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
            if (!listUpdated)
                return;
            UpdateSpriteAnimations();
        }
        private void UpdateSpriteAnimations()
        {
            if (visibleElements == null || visibleElements.Count == 0)
                return;
            frameTime += Time.deltaTime;
            if (frameTime < globalFrameTime)
                return;
            foreach (var kv in visibleElements)
            {
                int index = kv.Key;
                VisualElement element = kv.Value;

                spriteFrames[index]++;
                if (spriteFrames[index] >= AllResourcesList[index].AnimationSprites.Count)
                    spriteFrames[index] = 0;

                var image = element.Q<Image>("resourceImage");
                if (image != null)
                    image.sprite = AllResourcesList[index].AnimationSprites[spriteFrames[index]];

                UpdateAmounts(element, index);

                frameTime = 0;
            }
        }
        private void UpdateAmounts(VisualElement element, int index)
        {
            if (visibleElements == null || visibleElements.Count == 0)
                return;
            var amountLabel = element.Q<Label>("currentAmount");
            if (amountLabel != null)
                amountLabel.text = resourceStateSO.resourcesAmounts[index].ToString();
        }
    }
}
