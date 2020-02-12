using UnityEngine;
using UnityEditor;

namespace RLD
{
    [CustomEditor(typeof(RTObjectGroupDb))]
    public class RTObjectGroupDbInspector : Editor
    {
        private class GroupDropAndDropHandler : DragAndDropHandler
        {
            private RTObjectGroupDb _groupDb;

            public GroupDropAndDropHandler(RTObjectGroupDb groupDb)
            {
                _groupDb = groupDb;
            }

            protected override void PerformDrop()
            {
                var droppedObjects = DragAndDrop.objectReferences;
                foreach(var droppedObject in droppedObjects)
                {
                    GameObject gameObject = droppedObject as GameObject;
                    if (gameObject == null || !gameObject.IsSceneObject()) continue;

                    _groupDb.Add(gameObject);
                }
            }
        }

        private Vector3 _scrollPos;
        private RTObjectGroupDb _groupDb;

        public override void OnInspectorGUI()
        {
            _groupDb.RemoveNullRefs();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, "Box", GUILayout.Height(300.0f));
            if (_groupDb.NumGroups == 0) EditorGUILayout.HelpBox("There are no groups currently available. If you want to mark objects as groups, you can drag and " + 
                                                                 "drop them from the hierarchy window onto this area.", MessageType.Info);
            else
            {
                var removeGroupContent = new GUIContent();
                removeGroupContent.tooltip = "Delete this group. Note: This does not delete the object from the scene. It will only unregister it as an object group.";
                removeGroupContent.text = "Remove";

                var allGroups = _groupDb.GetAll();
                foreach (var group in allGroups)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(group.name);
                    if (GUILayout.Button(removeGroupContent, GUILayout.Width(60.0f)))
                    {
                        EditorUndoEx.Record(_groupDb);
                        _groupDb.Remove(group);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();

            Rect scrollViewRect = GUILayoutUtility.GetLastRect();
            var dragAndDropHandler = new GroupDropAndDropHandler(_groupDb);
            dragAndDropHandler.Handle(Event.current, scrollViewRect);

            var content = new GUIContent();
            content.text = "Remove all";
            content.tooltip = "Removes all object groups. Note: This does not delete the objects from the scene. It will only remove them from the group list " + 
                              "so that they are no longer treated as object groups.";
            if (GUILayout.Button(content, GUILayout.Width(83.0f)))
            {
                EditorUndoEx.Record(_groupDb);
                _groupDb.Clear();
            }
        }

        private void OnEnable()
        {
            _groupDb = target as RTObjectGroupDb;
        }
    }
}
