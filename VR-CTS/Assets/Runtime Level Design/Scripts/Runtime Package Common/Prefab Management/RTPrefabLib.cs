using UnityEngine;
using System;
using System.Collections.Generic;

namespace RLD
{
    public delegate void PrefabCreatedInLibHandler(RTPrefabLib prefabLib, RTPrefab prefab);
    public delegate void PrefabRemovedFromLibHandler(RTPrefabLib prefabLib, RTPrefab prefab);
    public delegate void PrefabLibClearedHandler(RTPrefabLib prefabLib);

    [Serializable]
    public class RTPrefabLib
    {
        public event PrefabCreatedInLibHandler PrefabCreated;
        public event PrefabRemovedFromLibHandler PrefabRemoved;
        public event PrefabLibClearedHandler Cleared;

        [SerializeField]
        private string _name = string.Empty;
        [SerializeField]
        private List<RTPrefab> _prefabs = new List<RTPrefab>();

        public int NumPrefabs { get { return _prefabs.Count; } }
        public string Name { get { return _name; } set { _name = value; } }

        public RTPrefab CreatePrefab(GameObject unityPrefab, Texture2D prefabPreview)
        {
            if (Contains(unityPrefab) || unityPrefab == null) return null;

            var prefab = new RTPrefab();
            prefab.UnityPrefab = unityPrefab;
            prefab.PreviewTexture = prefabPreview;

            _prefabs.Add(prefab);
            if (PrefabCreated != null) PrefabCreated(this, prefab);

            return prefab;
        }

        public RTPrefab CreatePrefabFromSceneObject(GameObject sceneObject)
        {
            GameObject unityPrefab = GameObject.Instantiate(sceneObject);

            RTPrefabLibDb.Get.EditorPrefabPreviewGen.BeginGenSession(RTPrefabLibDb.Get.PrefabPreviewLookAndFeel);
            Texture2D prefabPreview = RTPrefabLibDb.Get.EditorPrefabPreviewGen.Generate(unityPrefab);
            RTPrefabLibDb.Get.EditorPrefabPreviewGen.EndGenSession();
            unityPrefab.SetActive(false);

            var prefab = new RTPrefab();
            prefab.UnityPrefab = unityPrefab;
            prefab.PreviewTexture = prefabPreview;

            _prefabs.Add(prefab);
            if (PrefabCreated != null) PrefabCreated(this, prefab);

            return prefab;
        }

        public List<RTPrefab> CreatePrefabsFromSceneObjects(List<GameObject> sceneObjects)
        {
            RTPrefabLibDb.Get.EditorPrefabPreviewGen.BeginGenSession(RTPrefabLibDb.Get.PrefabPreviewLookAndFeel);
            var createdPrefabs = new List<RTPrefab>();
            foreach(var sceneObject in sceneObjects)
            {
                GameObject unityPrefab = GameObject.Instantiate(sceneObject);
                Texture2D prefabPreview = RTPrefabLibDb.Get.EditorPrefabPreviewGen.Generate(unityPrefab);
                unityPrefab.SetActive(false);

                var prefab = new RTPrefab();
                prefab.UnityPrefab = unityPrefab;
                prefab.PreviewTexture = prefabPreview;

                _prefabs.Add(prefab);
                createdPrefabs.Add(prefab);
                if (PrefabCreated != null) PrefabCreated(this, prefab);
            }

            RTPrefabLibDb.Get.EditorPrefabPreviewGen.EndGenSession();
            return createdPrefabs;
        }

        public void Remove(int prefabIndex)
        {
            if (prefabIndex >= 0 && prefabIndex < NumPrefabs)
            {
                var removedPrefab = _prefabs[prefabIndex];

                _prefabs.RemoveAt(prefabIndex);
                if (PrefabRemoved != null) PrefabRemoved(this, removedPrefab);
            }
        }

        public void Remove(RTPrefab prefab)
        {
            int prefabIndex = GetPrefabIndex(prefab);
            if (prefabIndex >= 0) Remove(prefabIndex);
        }

        public void Clear()
        {
            _prefabs.Clear();
            if (Cleared != null) Cleared(this);
        }

        public bool Contains(GameObject unityPrefab)
        {
            return _prefabs.FindAll(item => item.UnityPrefab == unityPrefab).Count != 0;
        }

        public bool Contains(RTPrefab prefab)
        {
            return _prefabs.Contains(prefab);
        }

        public int GetPrefabIndex(RTPrefab prefab)
        {
            return _prefabs.IndexOf(prefab);
        }

        public RTPrefab GetPrefab(int prefabIndex)
        {
            return _prefabs[prefabIndex];
        }

        public RTPrefab GetPrefab(GameObject unityPrefab)
        {
            var prefabs = _prefabs.FindAll(item => item.UnityPrefab == unityPrefab);
            return prefabs.Count != 0 ? prefabs[0] : null;
        }
    }
}
