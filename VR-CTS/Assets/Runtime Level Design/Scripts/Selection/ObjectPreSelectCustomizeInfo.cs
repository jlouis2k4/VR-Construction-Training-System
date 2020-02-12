using System.Collections.Generic;
using UnityEngine;

namespace RLD
{
    public class ObjectPreSelectCustomizeInfo
    {
        private List<GameObject> _toBeSelected;
        private ObjectSelectReason _selectRason;

        public ObjectSelectReason SelectReason { get { return _selectRason; } }
        public int ToBeSelectedCount { get { return _toBeSelected.Count; } }
        public List<GameObject> ToBeSelected { get { return new List<GameObject>(_toBeSelected); } }

        public ObjectPreSelectCustomizeInfo(List<GameObject> toBeSelected, ObjectSelectReason selectReason)
        {
            _toBeSelected = new List<GameObject>(toBeSelected);
            _selectRason = selectReason;
        }

        public void SelectThese(IEnumerable<GameObject> toBeSelected)
        {
            if (toBeSelected == null) _toBeSelected = new List<GameObject>();
            else _toBeSelected = new List<GameObject>(toBeSelected);
        }

        public void IgnoreThese(IEnumerable<GameObject> toBeIgnored)
        {
            if (toBeIgnored == null) return;

            foreach (var ignoreObject in toBeIgnored)
            {
                _toBeSelected.Remove(ignoreObject);
            }
        }
    }
}
