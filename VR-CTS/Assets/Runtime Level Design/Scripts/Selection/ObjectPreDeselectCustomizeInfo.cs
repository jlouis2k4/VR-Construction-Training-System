using System.Collections.Generic;
using UnityEngine;

namespace RLD
{
    public class ObjectPreDeselectCustomizeInfo
    {
        private List<GameObject> _toBeDeselected;
        private ObjectDeselectReason _deselectReason;

        public ObjectDeselectReason DeselectReason { get { return _deselectReason; } }
        public int ToBeDeselectedCount { get { return _toBeDeselected.Count; } }
        public List<GameObject> ToBeDeselected { get { return new List<GameObject>(_toBeDeselected); } }

        public ObjectPreDeselectCustomizeInfo(List<GameObject> toBeDeselected, ObjectDeselectReason deselectReason)
        {
            _toBeDeselected = new List<GameObject>(toBeDeselected);
            _deselectReason = deselectReason;
        }

        public void DeselectThese(IEnumerable<GameObject> toBeDeselected)
        {
            if (toBeDeselected == null) _toBeDeselected = new List<GameObject>();
            else _toBeDeselected = new List<GameObject>(toBeDeselected);
        }

        public void IgnoreThese(IEnumerable<GameObject> toBeIgnored)
        {
            if (toBeIgnored == null) return;

            foreach (var ignoreObject in toBeIgnored)
            {
                _toBeDeselected.Remove(ignoreObject);
            }
        }
    }
}
