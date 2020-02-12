using System.Collections.Generic;
using UnityEngine;

namespace RLD
{
    public class ObjectSelectEntireHierarchy : Singleton<ObjectSelectEntireHierarchy>
    {
        private bool _isActive;
        private bool _ignoreObjectGroups = false;

        public bool IgnoreObjectGroups { get { return _ignoreObjectGroups; } set { _ignoreObjectGroups = value; } }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive) return;

            if (isActive)
            {
                _isActive = true;
                RTObjectSelection.Get.PreSelectCustomize += OnPreSelectCustomize;
                RTObjectSelection.Get.PreDeselectCustomize += OnPreDeselectCustomize;
            }
            else
            {
                _isActive = false;
                RTObjectSelection.Get.PreSelectCustomize -= OnPreSelectCustomize;
                RTObjectSelection.Get.PreDeselectCustomize -= OnPreDeselectCustomize;
            }
        }

        private void OnPreSelectCustomize(ObjectPreSelectCustomizeInfo customizeInfo, List<GameObject> toBeSelected)
        {
            if (IgnoreObjectGroups)
            {
                List<GameObject> roots = GameObjectEx.GetRoots(toBeSelected);
                if (roots.Count == 0) return;

                List<GameObject> selectThese = new List<GameObject>(roots.Count * 10);
                foreach (var root in roots)
                {
                    selectThese.AddRange(root.GetAllChildrenAndSelf());
                }

                customizeInfo.SelectThese(selectThese);
            }
            else
            {
                HashSet<GameObject> selectedObjects = new HashSet<GameObject>();
                foreach(var gameObj in toBeSelected)
                {
                    // If this is an object group, ignore it
                    if (RTObjectGroupDb.Get.IsGroup(gameObj)) continue;

                    // Now move to the furthest parent up the hierarchy which is not an object group and 
                    // select it and all its children.
                    Transform parentTransform = GetFurthestParentNotGroup(gameObj);

                    // Store the parent and all its children
                    var allObjectsInHierarchy = parentTransform.gameObject.GetAllChildrenAndSelf();
                    foreach (var hierarchyObject in allObjectsInHierarchy) selectedObjects.Add(hierarchyObject);
                }

                // Select the objects
                customizeInfo.SelectThese(selectedObjects);
            }
        }

        private  void OnPreDeselectCustomize(ObjectPreDeselectCustomizeInfo customizeInfo, List<GameObject> toBeDeselected)
        {
            if (IgnoreObjectGroups)
            {
                List<GameObject> roots = GameObjectEx.GetRoots(toBeDeselected);
                if (roots.Count == 0) return;

                List<GameObject> deselectThese = new List<GameObject>(roots.Count * 10);
                foreach (var root in roots)
                {
                    deselectThese.AddRange(root.GetAllChildrenAndSelf());
                }
                customizeInfo.DeselectThese(deselectThese);
            }
            else
            {
                HashSet<GameObject> deselectedObjects = new HashSet<GameObject>();
                foreach (var gameObj in toBeDeselected)
                {
                    if (RTObjectGroupDb.Get.IsGroup(gameObj)) continue;

                    Transform parentTransform = GetFurthestParentNotGroup(gameObj);

                    var allObjectsInHierarchy = parentTransform.gameObject.GetAllChildrenAndSelf();
                    foreach (var hierarchyObject in allObjectsInHierarchy) deselectedObjects.Add(hierarchyObject);
                }

                customizeInfo.DeselectThese(deselectedObjects);
            }
        }

        private Transform GetFurthestParentNotGroup(GameObject gameObj)
        {
            // Move to the last parent which is not an object group and select it and all its children.
            // Note: Start from the object itself just in case the first parent we encounter is group.
            Transform parentTransform = gameObj.transform;
            while (true)
            {
                // If the next parent is an obejct group, we break out of the loop because it
                // means we can't got any further. We also exit the loop if the parent is null.
                Transform nextParent = parentTransform.parent;
                if (nextParent == null) break;
                if (RTObjectGroupDb.Get.IsGroup(nextParent.gameObject)) break;

                // The next parent is not a group and is not null, so we go up and start again
                parentTransform = nextParent;
            }

            return parentTransform;
        }
    }
}
