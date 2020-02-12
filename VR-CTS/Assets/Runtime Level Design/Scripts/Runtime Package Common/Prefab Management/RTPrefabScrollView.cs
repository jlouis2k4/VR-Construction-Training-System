using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RLD
{
    public class RTPrefabScrollView : MonoBehaviour
    {
        public delegate void PrefabPreviewClickedHandler(RTPrefab prefab);
        public delegate void PrefabPreviewHoverEnterHandler(RTPrefab prefab);
        public delegate void PrefabPreviewHoverExitHandler(RTPrefab prefab);

        public event PrefabPreviewClickedHandler PrefabPreviewClicked;
        public event PrefabPreviewHoverEnterHandler PrefabPreviewHoverEnter;
        public event PrefabPreviewHoverExitHandler PrefabPreviewHoverExit;

        private ObjectPool _previewButtonPool;
        private GameObject _gridObject;
        private GridLayoutGroup _gridLayoutGroup;

        public void AddPrefabPreview(RTPrefab prefab)
        {
            GameObject previewButton = _previewButtonPool.GetPooledObject();
            previewButton.name = "Preview_" + prefab.UnityPrefab.name;

            Image image = previewButton.GetComponent<Image>();
            if (image != null) image.sprite = prefab.PreviewSprite;
        
            var previewBtnScript = previewButton.GetComponent<RTPrefabPreviewButton>();
            previewBtnScript.Prefab = prefab;
            previewBtnScript.Text = prefab.UnityPrefab.name;
            previewBtnScript.HoverEnter -= OnPrefabPreviewHoverEnter;
            previewBtnScript.HoverEnter += OnPrefabPreviewHoverEnter;
            previewBtnScript.HoverExit -= OnPrefabPreviewHoverExit;
            previewBtnScript.HoverExit += OnPrefabPreviewHoverExit;

            Button button = previewButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveListener(OnPreviewButtonClicked);
                button.onClick.AddListener(OnPreviewButtonClicked);
            }
        }

        public void ClearPreviews()
        {
            _previewButtonPool.MarkAllAsUnused();
        }

        public void SyncWithLib(RTPrefabLib prefabLib)
        {
            ClearPreviews();
            if (prefabLib != null)
            {
                for (int prefabIndex = 0; prefabIndex < prefabLib.NumPrefabs; ++prefabIndex)
                {
                    RTPrefab prefab = prefabLib.GetPrefab(prefabIndex);
                    AddPrefabPreview(prefab);
                }
            }
        }

        private void Awake()
        {
            _gridLayoutGroup = gameObject.GetComponentInChildren<GridLayoutGroup>();
            _gridObject = _gridLayoutGroup.gameObject;

            GameObject previewButtonPrefab = Resources.Load("Prefabs/RTPrefabPreviewButton") as GameObject;
            _previewButtonPool = new ObjectPool(previewButtonPrefab, 100, ObjectPool.GrowMode.ByAmount);
            _previewButtonPool.GrowAmount = 30;
            _previewButtonPool.SetPooledObjectsParent(_gridObject.transform);
        }

        private void OnPreviewButtonClicked()
        {
            var hoveredUIElements = RTScene.Get.GetHoveredUIElements();
            if (hoveredUIElements.Count != 0)
            {
                foreach(var uiElement in hoveredUIElements)
                {
                    RTPrefabPreviewButton prefabPreviewBtn = uiElement.gameObject.GetComponent<RTPrefabPreviewButton>();
                    if (prefabPreviewBtn != null)
                    {
                        if (PrefabPreviewClicked != null) PrefabPreviewClicked(prefabPreviewBtn.Prefab);
                        break;
                    }
                }
            }
        }

        private void OnPrefabPreviewHoverEnter(RTPrefab prefab)
        {
            if (PrefabPreviewHoverEnter != null) PrefabPreviewHoverEnter(prefab);
        }

        private void OnPrefabPreviewHoverExit(RTPrefab prefab)
        {
            if (PrefabPreviewHoverExit != null) PrefabPreviewHoverExit(prefab);
        }
    }
}
