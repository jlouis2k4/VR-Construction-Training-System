using UnityEngine;
using UnityEditor;

namespace RLD
{
    [CustomEditor(typeof(RTPrefabLibDb))]
    public class RTPrefabLibDbInspector : Editor
    {
        private RTPrefabLibDb _libDb;
        private PrefabDropEventHandler _prefabDropHandler = new PrefabDropEventHandler();

        public override void OnInspectorGUI()
        {
            const float createBtnWidth = 95.0f;
            int newInt; string newString;
            var content = new GUIContent();

            if (!_libDb.HasRuntimeUI)
                EditorGUILayout.HelpBox("No runtime UI is associated with the lib database. You can use the \'Create RT UI\' button to create a UI or connect to an already existing one.", MessageType.Info);

            // Create runtime UI
            EditorGUILayout.BeginHorizontal();
            content.text = "Create RT UI";
            content.tooltip = "Creates a UI canvas which allows you to interact with the prefab library DB at runtime. If a UI already exists in the scene, that will be used instead.";
            if (GUILayout.Button(content, GUILayout.Width(createBtnWidth)))
            {
                _libDb.CreateRuntimeUI();
            }

            // Refresh previews
            content.text = "Refresh previews";
            content.tooltip = "Regenerates ALL prefab previews in ALL prefab libraries.";
            if (GUILayout.Button(content, GUILayout.Width(createBtnWidth + 20.0f)))
            {
                _libDb.RefreshEditorPrefabPreviews();
            }
            EditorGUILayout.EndHorizontal();

            // Create lib
            content.text = "Create lib";
            content.tooltip = "Creates a new prefab library with the specified name. Note: If a library with the same name already exists, a number suffix will be added.";
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(content, GUILayout.Width(createBtnWidth)))
            {
                EditorUndoEx.Record(_libDb);
                _libDb.SetActiveLib(_libDb.CreateLib(_libDb.NewLibName));
            }
            newString = EditorGUILayout.TextField(_libDb.NewLibName);
            if (newString != _libDb.NewLibName)
            {
                EditorUndoEx.Record(_libDb);
                _libDb.NewLibName = newString;
            }
            EditorGUILayout.EndHorizontal();

            // Remove ###
            EditorGUILayout.BeginHorizontal();
            content.text = "Remove active";
            content.tooltip = "Removes the active library.";
            if (GUILayout.Button(content, GUILayout.Width(createBtnWidth)))
            {
                if (_libDb.ActiveLib != null)
                {
                    EditorUndoEx.Record(_libDb);
                    _libDb.Remove(_libDb.ActiveLibIndex);
                }
            }
            content.text = "Remove all";
            content.tooltip = "Removes all libraries.";
            if (GUILayout.Button(content, GUILayout.Width(createBtnWidth)))
            {
                EditorUndoEx.Record(_libDb);
                _libDb.Clear();
            }

            content.text = "Remove empty";
            content.tooltip = "Removes all empty libraries.";
            if (GUILayout.Button(content, GUILayout.Width(createBtnWidth + 10.0f)))
            {
                EditorUndoEx.Record(_libDb);
                _libDb.RemoveEmptyLibs();
            }
            EditorGUILayout.EndHorizontal();

            // Misc
            EditorGUILayout.BeginHorizontal();
            content.text = "Sort libs";
            content.tooltip = "Sorts all prefab libraries alphabetically.";
            if (GUILayout.Button(content, GUILayout.Width(createBtnWidth)))
            {
                _libDb.SortLibsByName();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            if (_libDb.NumLibs == 0) EditorGUILayout.HelpBox("There are no libraries currently available. Use the \'Create lib\' button to create a new prefab library.", MessageType.Info);
            else
            {
                // Active lib selection
                var libNames = _libDb.GetAllLibNames();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Active library", GUILayout.Width(createBtnWidth));
                newInt = EditorGUILayout.Popup(_libDb.ActiveLibIndex, libNames.ToArray());
                EditorGUILayout.EndHorizontal();
                if (newInt != _libDb.ActiveLibIndex)
                {
                    EditorUndoEx.Record(_libDb);
                    _libDb.SetActiveLib(newInt);
                }

                // Active lib name
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name", GUILayout.Width(createBtnWidth));
                newString = EditorGUILayout.DelayedTextField(_libDb.ActiveLib.Name);
                EditorGUILayout.EndHorizontal();
                if (newString != _libDb.ActiveLib.Name)
                {
                    EditorUndoEx.Record(_libDb);
                    _libDb.SetLibName(_libDb.ActiveLib, newString);
                }

                // Active lib scroll view
                if (_libDb.ActiveLib != null)
                {
                    float previewWidth = _libDb.PrefabPreviewLookAndFeel.PreviewWidth;
                    float previewHeight = _libDb.PrefabPreviewLookAndFeel.PreviewHeight;

                    var prefabPreviewContent = new GUIContent();
                    prefabPreviewContent.text = "";

                    content.text = "Remove";
                    content.tooltip = "Remove this prefab from the library.";

                    _libDb.PrefabScrollPos = EditorGUILayout.BeginScrollView(_libDb.PrefabScrollPos, "Box", GUILayout.Height(350.0f));
                    EditorGUILayout.BeginHorizontal();
                    for (int prefabIndex = 0; prefabIndex < _libDb.ActiveLib.NumPrefabs; ++prefabIndex)
                    {
                        RTPrefab prefab = _libDb.ActiveLib.GetPrefab(prefabIndex);
                        prefabPreviewContent.tooltip = prefab.UnityPrefab.name;
                        prefabPreviewContent.image = prefab.PreviewTexture;

                        EditorGUILayout.BeginVertical(GUILayout.Width(previewWidth));
                        GUILayout.Button(prefabPreviewContent, "Box", GUILayout.Width(previewWidth), GUILayout.Height(previewHeight));
                        if (GUILayout.Button(content, GUILayout.Width(previewWidth)))
                        {
                            EditorUndoEx.Record(_libDb);
                            _libDb.ActiveLib.Remove(prefabIndex);
                        }
                        EditorGUILayout.EndVertical();

                        if (prefabIndex != 0 && ((prefabIndex + 1) % _libDb.NumPrefabsPerRow == 0))
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();

                    _prefabDropHandler.Handle(Event.current, GUILayoutUtility.GetLastRect());

                    content.text = "Clear lib";
                    content.tooltip = "Removes all the prefabs from the active library.";
                    if (GUILayout.Button(content, GUILayout.Width(createBtnWidth)))
                    {
                        EditorUndoEx.Record(_libDb);
                        _libDb.ActiveLib.Clear();
                    }

                    _libDb.PrefabPreviewLookAndFeel.RenderEditorGUI(_libDb);
                }
            }
        }

        private void OnEnable()
        {
            _libDb = target as RTPrefabLibDb;
            _libDb.PrefabPreviewLookAndFeel.UsesFoldout = true;
            _libDb.PrefabPreviewLookAndFeel.FoldoutLabel = "Prefab preview look & feel";

            _prefabDropHandler.PrefabLibDb = _libDb;
        }
    }
}
