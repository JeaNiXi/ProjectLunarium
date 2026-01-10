using SO;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
namespace Managers
{
    public class LedgerManager
    {
        private readonly VisualElement root; //Readonly
        private readonly Dictionary<string, VisualElement> elements; //Func
        private readonly Dictionary<LedgerEntryType, VisualElement> groups;
        public enum LedgerEntryType
        {
            None,
            Resource,
            Population,
            Building,
            Worker,
            Technology
        }
        public LedgerManager(VisualElement ledgerRoot)
        {
            root = ledgerRoot;
            elements = new Dictionary<string, VisualElement>();
            groups = new Dictionary<LedgerEntryType, VisualElement>();
        }
        public void AddOrUpdate(LedgerViewDescriptor descriptor)
        {
            if (elements.TryGetValue(descriptor.ID, out var existingElement))
            {
                descriptor.Bind?.Invoke(existingElement);
                return;
            }
            var group = GetOrCreateGroup(descriptor.Type);
            var ve = new VisualElement();

            descriptor.Asset.CloneTree(ve);
            descriptor.Bind?.Invoke(ve);
            elements[descriptor.ID] = ve;
            group.Add(ve);
        }
        private VisualElement GetOrCreateGroup(LedgerEntryType type)
        {
            if (groups.TryGetValue(type, out var existingGroup))
                return existingGroup;

            existingGroup = new VisualElement();
            existingGroup.AddToClassList("ledger-group");

            var title = new Label(type.ToString());
            title.AddToClassList("ledger-group-title");

            existingGroup.Add(title);
            root.Add(existingGroup);

            groups[type] = existingGroup;
            return existingGroup;
        }
    }
    public class LedgerEntry
    {
        public string ID;
        public LedgerManager.LedgerEntryType Type;
        public Func<string> GetTitle;
        public Func<string> GetValue;
        public Func<string> GetExtraInfo;

        public Action OnClick;
    }
    public class LedgerViewDescriptor
    {
        public string ID;
        public LedgerManager.LedgerEntryType Type;

        public VisualTreeAsset Asset;
        public Action<VisualElement> Bind;
    }
}