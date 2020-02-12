#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace RLD
{
    public static class AssetDatabaseEx
    {
        public enum AssetType
        {
            Prefab = 1,
        }

        public static List<GameObject> LoadPrefabsInFolder(string folderPath, bool includeSubfolders = true, bool showProgressBar = false)
        {
            if (string.IsNullOrEmpty(folderPath)) return new List<GameObject>();

            if (includeSubfolders)
            {
                string progressTitle = "Loading assets";
                string progressMessage = "Please wait...";

                if (showProgressBar) EditorUtility.DisplayProgressBar(progressTitle, progressMessage, 0.0f);
                List<UnityEngine.Object> assets = LoadAssetsInFolder(folderPath, AssetType.Prefab);
                if (showProgressBar) EditorUtility.DisplayProgressBar(progressTitle, progressMessage, 0.5f);
                List<GameObject> unityPrefabs = ConvertAssetsToPrefabs(assets);

                if (showProgressBar) EditorUtility.ClearProgressBar();
                return unityPrefabs;
            }
            else
            {
                List<string> assetPaths = FileSystem.GetAllFilesInFolder(folderPath);
                return GetPrefabsInAssets(assetPaths, showProgressBar);
            }
        }

        public static bool IsAssetPrefab(string assetFilePath)
        {
            string prefabExtension = ".prefab";
            if (assetFilePath.Length <= prefabExtension.Length) return false;
            else return assetFilePath.EndsWith(prefabExtension);
        }

        public static List<UnityEngine.Object> LoadAssetsInFolder(string folderPath, AssetType assetType)
        {
            string assetTypeFilter = "t:";
            if(assetType == AssetType.Prefab) assetTypeFilter += "GameObject";

            var assetGUIDs = AssetDatabase.FindAssets(assetTypeFilter, new string[] { folderPath });
            return ConvertGUIDsToAssets(assetGUIDs);
        }

        public static List<UnityEngine.Object> ConvertGUIDsToAssets(string[] assetGUIDs)
        {
            var assets = new List<UnityEngine.Object>();
            foreach (var guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

                if (asset != null) assets.Add(asset);
            }

            return assets;
        }

        public static List<GameObject> ConvertAssetsToPrefabs(List<UnityEngine.Object> assets)
        {
            if (assets == null || assets.Count == 0) return new List<GameObject>();

            var prefabs = new List<GameObject>(assets.Count);
            foreach(var asset in assets)
            {
                GameObject prefab = asset as GameObject;
                if (prefab != null) prefabs.Add(prefab);
            }

            return prefabs;
        }

        public static List<GameObject> GetPrefabsInAssets(List<string> assetPaths, bool showProgressBar = false)
        {
            var prefabs = new List<GameObject>();
            if (showProgressBar)
            {
                for (int pathIndex = 0; pathIndex < assetPaths.Count; ++pathIndex)
                {
                    string assetPath = assetPaths[pathIndex];
                    EditorUtility.DisplayProgressBar("Loading assets", "Please wait... (" + assetPath + ")", (pathIndex + 1) / (float)assetPaths.Count);
                    if (IsAssetPrefab(assetPath))
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                        if (prefab != null) prefabs.Add(prefab);
                    }
                }
                EditorUtility.ClearProgressBar();
            }
            else
            {
                foreach (string assetPath in assetPaths)
                {
                    if (IsAssetPrefab(assetPath))
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                        if (prefab != null) prefabs.Add(prefab);
                    }
                }
            }

            return prefabs;
        }
    }
}
#endif