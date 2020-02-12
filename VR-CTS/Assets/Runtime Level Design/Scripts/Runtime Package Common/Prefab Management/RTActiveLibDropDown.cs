using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace RLD
{
    public class RTActiveLibDropDown : MonoBehaviour
    {
        private Dropdown _dropDown;
        private List<UnityAction<int>> _valueChangedListeners = new List<UnityAction<int>>();

        public int ActiveLibIndex { get { return _dropDown.value; } }

        public void AddValueChangedListener(UnityAction<int> listener)
        {
            _dropDown.onValueChanged.AddListener(listener);
            _valueChangedListeners.Add(listener);
        }

        public void SetActiveLibIndex(int activeLibIndex)
        {
            _dropDown.onValueChanged.RemoveAllListeners();
            _dropDown.value = activeLibIndex;

            foreach (var listener in _valueChangedListeners)
                _dropDown.onValueChanged.AddListener(listener);
        }

        public void ClearLibs()
        {
            _dropDown.ClearOptions();
        }

        public void SyncWithLibDb()
        {
            ClearLibs();

            if (RTPrefabLibDb.Get.NumLibs != 0)
            {
                var allLibNames = RTPrefabLibDb.Get.GetAllLibNames();
                _dropDown.AddOptions(allLibNames);

                SetActiveLibIndex(RTPrefabLibDb.Get.ActiveLibIndex);
            }
        }

        private void Awake()
        {
            _dropDown = gameObject.GetComponent<Dropdown>();
        }
    }
}
