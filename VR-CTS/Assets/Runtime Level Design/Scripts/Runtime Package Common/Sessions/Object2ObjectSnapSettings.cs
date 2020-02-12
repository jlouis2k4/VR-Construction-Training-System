using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class Object2ObjectSnapSettings : Settings
    {
        [SerializeField]
        private int _snapDestinationLayers = ~0;
        [SerializeField]
        private bool _canClimbObjects = true;
        [SerializeField]
        private float _snapRadius = 0.7f;

        public int SnapDestinationLayers { get { return _snapDestinationLayers; } set { _snapDestinationLayers = value; } }
        public bool CanClimbObjects { get { return _canClimbObjects; } set { _canClimbObjects = value; } }
        public float SnapRadius { get { return _snapRadius; } set { _snapRadius = Mathf.Max(0.0f, value); } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            int newInt; bool newBool; float newFloat;

            var content = new GUIContent();
            content.text = "Snap destination layers";
            content.tooltip = "When snapping to nearby obejcts, only the objects which belong to one of these layers will be taken into account.";
            newInt = EditorGUILayoutEx.LayerMaskField(content, SnapDestinationLayers);
            if (newInt != SnapDestinationLayers)
            {
                EditorUndoEx.Record(undoRecordObject);
                SnapDestinationLayers = newInt;
            }

            content.text = "Can climb objects";
            content.tooltip = "This toggle controls what happens when the mouse cursor hovers other objects. If checked, the target obejcts " + 
                              "will climb the hovered objects. Otherwise, they will sit on the scene grid.";
            newBool = EditorGUILayout.ToggleLeft(content, CanClimbObjects);
            if (newBool != CanClimbObjects)
            {
                EditorUndoEx.Record(undoRecordObject);
                CanClimbObjects = newBool;
            }

            content.text = "Snap radius";
            content.tooltip = "This is a distance value that is used to gather nearby destination objects.";
            newFloat = EditorGUILayout.FloatField(content, SnapRadius);
            if (newFloat != SnapRadius)
            {
                EditorUndoEx.Record(undoRecordObject);
                SnapRadius = newFloat;
            }
        }
        #endif
    }
}
