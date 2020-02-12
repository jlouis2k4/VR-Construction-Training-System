using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class DynamicConvertSettings : Settings
    {
        private Rect _prefabFolderDropRect;

        [SerializeField]
        private GameObjectType _convertableObjectTypes = GameObjectTypeHelper.AllCombined;
        [SerializeField]
        private string _prefabFolder = string.Empty;
        [SerializeField]
        private bool _processPrefabSubfolders = true;

        public GameObjectType ConvertableObjectTypes { get { return _convertableObjectTypes; } set { _convertableObjectTypes = value; } }
        public string PrefabFolder { get { return _prefabFolder; } set { if (value != null) _prefabFolder = value; } }
        public bool ProcessPrefabSubfolders { get { return _processPrefabSubfolders; } set { _processPrefabSubfolders = value; } }
        public Rect PrefabFolderDropRect { get { return _prefabFolderDropRect; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            int newInt; string newString; bool newBool;

            var content = new GUIContent();
            content.text = "Object types";
            content.tooltip = "The types of objects that can participate in the conversion operation. Applies to both scene objects and prefabs.";
            newInt = (int)((GameObjectType)EditorGUILayout.EnumMaskPopup(content, (GameObjectType)_convertableObjectTypes));
            if (newInt != (int)_convertableObjectTypes)
            {
                EditorUndoEx.Record(undoRecordObject);
                _convertableObjectTypes = (GameObjectType)newInt;
            }

            content.text = "Prefab folder";
            content.tooltip = "Allows you to specify a folder which contains prefabs that need to be converted. You can either write the path manually or " + 
                              "simply drag and drop a folder onto the text field to have the path automatically extracted for you.";
            newString = EditorGUILayout.TextField(content, PrefabFolder);
            if (newString != PrefabFolder)
            {
                EditorUndoEx.Record(undoRecordObject);
                PrefabFolder = newString;
            }
            _prefabFolderDropRect = GUILayoutUtility.GetLastRect();

            content.text = "Process prefab subfolders";
            content.tooltip = "If this is checked, the conversion utility will also convert prefabs which reside in any subfolders that reside in the specified folder. Otherwise, " + 
                              "subfolders are ignored.";
            newBool = EditorGUILayout.ToggleLeft(content, ProcessPrefabSubfolders);
            if (newBool != ProcessPrefabSubfolders)
            {
                EditorUndoEx.Record(undoRecordObject);
                ProcessPrefabSubfolders = newBool;
            }
        }
        #endif
    }
}