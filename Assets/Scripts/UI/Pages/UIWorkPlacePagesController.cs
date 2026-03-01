using Managers;
using SO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIWPPViewHolder
    {
        public Label NameLabel;
        public Label AmountLabel;
        public Label ValueLabel;
        public DropdownField TypeDropdown;
        public WorkPlaceSO BoundWorkPlace;
        public Button AddWorkerButton, RemoveWorkerButton;
        public List<WorkPlaceTypeSO> CurrentAvailableTypes;

        public UIWPPViewHolder(VisualElement root)
        {
            NameLabel = root.Q<Label>("workPlaceNK");
            AmountLabel = root.Q<Label>("currentWorkersAmountLabel");
            ValueLabel = root.Q<Label>("currentWorkersAmountValue");
            TypeDropdown = root.Q<DropdownField>("wpTypeDropDownfield");

            AddWorkerButton = root.Q<Button>("addWorkerButton");
            RemoveWorkerButton = root.Q<Button>("removeWorkerButton");
        }
    }
    public class UIWorkPlacePagesController : IUIPageController
    {
        private VisualElement RootVE;
        private WorkPlaceCategorySO MainCategorySO;
        private ListView MainListView;
        private VisualTreeAsset wpDetailListAsset;
        private List<WorkPlaceSO> allWorkPlacesInCategoryList;
        private List<WorkPlaceSO> visibleWorkPlacesList;
        private Dictionary<WorkPlaceSO, VisualElement> visibleWorkPlaceElements;

        private WorkPlaceManager WPM;
        private LocalizationManager LM;
        private UIWorkPlaceLocalizationSO UIWPLocalization;

        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            RootVE = page;
            MainCategorySO = data as WorkPlaceCategorySO;

            InitializeData();
            InitializeListView();
        }
        private void InitializeData()
        {
            WPM = WorkPlaceManager.Instance;
            LM = LocalizationManager.Instance;
            UIWPLocalization = WPM.GetWorkPlaceLocalizationSO();

            MainListView = RootVE.Q<ListView>("mainWPListView");
            wpDetailListAsset = Resources.Load<VisualTreeAsset>("UI/Panel/WPDetailPanelAsset");
            allWorkPlacesInCategoryList = new List<WorkPlaceSO>();
            visibleWorkPlacesList = new List<WorkPlaceSO>();
            visibleWorkPlaceElements = new Dictionary<WorkPlaceSO, VisualElement>();
            FillLists();
        }
        private void FillLists()
        {
            foreach (var workPlace in MainCategorySO.WorkPlaces)
            {
                allWorkPlacesInCategoryList.Add(workPlace);
                //Debug.Log($"Added WorkPlace: {workPlace} from Category: {MainCategorySO.CategoryID}");
            }
            UpdateVisibleWorkPlacesList();
        }
        private void UpdateVisibleWorkPlacesList()
        {
            visibleWorkPlacesList?.Clear();
            foreach (var workPlace in allWorkPlacesInCategoryList)
            {
                if (WorkPlaceManager.Instance.IsWorkPlaceAvailable(workPlace.ID))
                    visibleWorkPlacesList.Add(workPlace);
            }
        }
        private void InitializeListView()
        {
            MainListView.itemsSource = visibleWorkPlacesList;
            UpdateListView();
        }
        private void UpdateListView()
        {
            MainListView.makeItem = () =>
            {
                TemplateContainer ve = wpDetailListAsset.CloneTree();
                var holder = new UIWPPViewHolder(ve);
                ve.userData = holder;

                holder.AddWorkerButton.clicked += () =>
                {
                    if (holder.BoundWorkPlace != null)
                    {
                        WPM.TryMoveWorkerToWorkPlace(holder.BoundWorkPlace.ID, 1);
                        UpdateNumbersUI(holder);
                    }
                };
                holder.RemoveWorkerButton.clicked += () =>
                {
                    if (holder.BoundWorkPlace != null)
                    {
                        WPM.TryRemoveWorkersFromWorkPlace(holder.BoundWorkPlace.ID, 1);
                        UpdateNumbersUI(holder);
                    }
                };

                holder.TypeDropdown.RegisterValueChangedCallback(evt =>
                {
                    if (holder.BoundWorkPlace != null && holder.CurrentAvailableTypes != null)
                    {
                        int newIndex = holder.TypeDropdown.index;
                        if (newIndex >= 0 && newIndex < holder.CurrentAvailableTypes.Count)
                        {
                            var selectedType = holder.CurrentAvailableTypes[newIndex];
                            WPM.SetWorkPlaceProductionTypeMode(holder.BoundWorkPlace.ID, selectedType);
                        }
                    }
                });
                return ve;
            };
            MainListView.bindItem = (element, index) =>
            {
                var workPlace = visibleWorkPlacesList[index];
                var holder = element.userData as UIWPPViewHolder;

                holder.BoundWorkPlace = workPlace;
                visibleWorkPlaceElements[workPlace] = element;

                holder.NameLabel.text = LM.GetLocalizedWorkPlaceSOData(workPlace.NameKey);
                holder.AmountLabel.text = LM.GetLocalizedUIWorkPlaceData(UIWPLocalization.currentWorkersAmountKey);

                var availableTypes = workPlace.WorkPlaceTypes
                    .Where(t => t.TechNeeded.All(tech => TechnologyManager.Instance.IsTechnologyResearched(tech)))
                    .ToList();

                holder.CurrentAvailableTypes = availableTypes;

                holder.TypeDropdown.choices = availableTypes
                    .Select(t => LM.GetLocalizedWorkPlaceTypeSOData(t.NameKey, out var value) ? value : t.NameKey)
                    .ToList();

                WorkPlaceTypeSO currentState = WPM.GetWorkPlaceProductionTypeMode(workPlace.ID);
                int activeIndex = availableTypes.IndexOf(currentState);
                holder.TypeDropdown.index = activeIndex >= 0 ? activeIndex : 0;

                UpdateNumbersUI(holder);
            };
            MainListView.unbindItem = (element, index) =>
            {
                var holder = element.userData as UIWPPViewHolder;
                if (holder.BoundWorkPlace != null)
                    visibleWorkPlaceElements.Remove(holder.BoundWorkPlace);
                holder.BoundWorkPlace = null;
            };
        }

        public void ShowPage()
        {
            RootVE.style.display = DisplayStyle.Flex;
        }
        public void HidePage()
        {
            RootVE.style.display = DisplayStyle.None;
        }
        private void UpdateNumbersUI(UIWPPViewHolder holder)
        {
            Helper.SetLabelCurrentXMaxText(
                holder.ValueLabel,
                WPM.GetCurrentWorkersAmount(holder.BoundWorkPlace),
                WPM.GetMaxCapacity(holder.BoundWorkPlace));
        }
        public void UpdatePage()
        {
            foreach (var entry in visibleWorkPlaceElements)
            {
                var element = entry.Value;
                if (element.userData is UIWPPViewHolder holder)
                    UpdateNumbersUI(holder);
            }
        }
    }
}