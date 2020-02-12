#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RLD
{
    public class PrefabDropEventHandler : DragAndDropHandler
    {
        public RTPrefabLibDb PrefabLibDb { get; set; }

        protected override void PerformDrop()
        {
            if (PrefabLibDb == null || PrefabLibDb.ActiveLib == null) return;

            EditorUndoEx.Record(PrefabLibDb);

            PrefabLibDb.EditorPrefabPreviewGen.BeginGenSession(PrefabLibDb.PrefabPreviewLookAndFeel);
            var objectRefs = DragAndDrop.objectReferences;
            foreach(var objectRef in objectRefs)
            {
                GameObject prefab = objectRef as GameObject;
                if (prefab == null || prefab.IsSceneObject()) continue;

                Texture2D prefabPreview = PrefabLibDb.EditorPrefabPreviewGen.Generate(prefab);
                PrefabLibDb.ActiveLib.CreatePrefab(prefab, prefabPreview);
            }

            var paths = DragAndDrop.paths;
            var allFolders = FileSystem.GetFoldersAndChildFolderPaths(paths);

            RTPrefabLib lastCreatedLib = null;
            for (int folderIndex = 0; folderIndex < allFolders.Count; ++folderIndex)
            {
                string folderPath = allFolders[folderIndex];
                EditorUtility.DisplayProgressBar("Processing dropped folders", "Please wait... (" + folderPath + ")", (float)folderIndex / allFolders.Count);
                RTPrefabLib newPrefabLib = PrefabLibDb.CreateLib(FileSystem.GetLastFolderNameInPath(folderPath));
                lastCreatedLib = newPrefabLib;

                var prefabsInFolder = AssetDatabaseEx.LoadPrefabsInFolder(folderPath, false, false);
                foreach(var prefab in prefabsInFolder)
                {
                    Texture2D prefabPreview = PrefabLibDb.EditorPrefabPreviewGen.Generate(prefab);
                    newPrefabLib.CreatePrefab(prefab, prefabPreview);
                }
            }
            EditorUtility.ClearProgressBar();
            PrefabLibDb.EditorPrefabPreviewGen.EndGenSession();

            if (lastCreatedLib != null) PrefabLibDb.SetActiveLib(lastCreatedLib);
        }
    }
}
#endif