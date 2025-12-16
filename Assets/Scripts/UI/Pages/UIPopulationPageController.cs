using SO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI
{
    public class UIPopulationPageController : IUIPageController
    {
        private VisualElement page;
        private PopulationManagerSO data;
        private PopulationStateSO populationStateSO;
        public void InitializePage(VisualElement page, ScriptableObject data)
        {
            this.page = page;
            this.data = data as PopulationManagerSO;
            if (this.data == null)
                Debug.Log("NO DATA SO FOUND");
            populationStateSO = Resources.Load<PopulationStateSO>("SO/PopulationState");
            InitializeData();
        }
        private void InitializeData()
        {
            UpdatePopulationPage();
        }
        private void UpdatePopulationPage()
        {
            Label currentPopulationLabel = page.Q<Label>("currentPopulation");
            currentPopulationLabel.text = populationStateSO.CurrentPopulation.ToString();
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