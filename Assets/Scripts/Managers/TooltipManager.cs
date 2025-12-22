using UnityEngine;
using UnityEngine.UIElements;
namespace Managers
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance { get; private set; }

        [SerializeField] private UIDocument TooltipUIDocument;
        [SerializeField] private VisualTreeAsset TooltipUIAsset;

        private VisualElement tooltipRoot;
        private Label tooltipLabel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeTooltip();
        }
        private void InitializeTooltip()
        {
            var root = TooltipUIDocument.rootVisualElement;
            tooltipRoot = TooltipUIAsset.CloneTree();
            tooltipLabel = tooltipRoot.Q<Label>("tooltipLabel");

            tooltipRoot.style.display = DisplayStyle.None;

            root.Add(tooltipRoot);
        }
        public void Show(string text, Vector2 screenPosition)
        {
            tooltipLabel.text = text;
            tooltipRoot.style.left = screenPosition.x + 12;
            tooltipRoot.style.top = screenPosition.y + 12;
            tooltipRoot.style.display = DisplayStyle.Flex;
        }
        public void Hide()
        {
            tooltipRoot.style.display = DisplayStyle.None;
        }
        public void Move(Vector2 screenPosition)
        {
            tooltipRoot.style.left = screenPosition.x + 12;
            tooltipRoot.style.top = screenPosition.y + 12;
        }
    }
}