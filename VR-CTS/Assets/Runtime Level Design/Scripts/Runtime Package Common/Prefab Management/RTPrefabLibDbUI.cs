using UnityEngine;
using UnityEngine.UI;

namespace RLD
{
    public class RTPrefabLibDbUI : MonoBehaviour
    {
        private RTActiveLibDropDown _activeLibDropDown;
        private RTPrefabScrollView _prefabScrollView;
        private RTHoveredPrefabNameLabel _hoveredPrefabNameLabel;

        public RTActiveLibDropDown ActiveLibDropDown { get { return _activeLibDropDown; } }
        public RTPrefabScrollView PrefabScrollView { get { return _prefabScrollView; } }
        public RTHoveredPrefabNameLabel HoveredPrefabNameLabel { get { return _hoveredPrefabNameLabel; } }

        private void Awake()
        {
            _activeLibDropDown = gameObject.GetComponentInChildren<RTActiveLibDropDown>();
            _prefabScrollView = gameObject.GetComponentInChildren<RTPrefabScrollView>();
            _hoveredPrefabNameLabel = gameObject.GetComponentInChildren<RTHoveredPrefabNameLabel>();

            _prefabScrollView.PrefabPreviewHoverEnter += OnPrefabPreviewHoverEnter;
            _prefabScrollView.PrefabPreviewHoverExit += OnPrefabPreviewHoverExit;
        }

        private void OnPrefabPreviewHoverEnter(RTPrefab prefab)
        {
            HoveredPrefabNameLabel.PrefabName = prefab.UnityPrefab != null ? prefab.UnityPrefab.name : string.Empty;
        }

        private void OnPrefabPreviewHoverExit(RTPrefab prefab)
        {
            HoveredPrefabNameLabel.PrefabName = string.Empty;
        }
    }
}
