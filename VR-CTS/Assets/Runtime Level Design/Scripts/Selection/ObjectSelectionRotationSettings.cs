using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class ObjectSelectionRotationSettings : Settings
    {
        [SerializeField]
        private ObjectRotationPivot _rotationPivot = ObjectRotationPivot.GroupCenter;
        [SerializeField]
        private ObjectKeyRotationSettings _keyRotationSettings = new ObjectKeyRotationSettings();

        public ObjectRotationPivot RotationPivot { get { return _rotationPivot; } set { _rotationPivot = value; } }
        public ObjectKeyRotationSettings KeyRotationSettings { get { return _keyRotationSettings; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            ObjectRotationPivot newRotationPivot;

            var content = new GUIContent();
            content.text = "Rotation pivot";
            content.tooltip = "Allows you to specify the rotation pivot.";
            newRotationPivot = (ObjectRotationPivot)EditorGUILayout.EnumPopup(content, RotationPivot);
            if (newRotationPivot != RotationPivot)
            {
                EditorUndoEx.Record(undoRecordObject);
                RotationPivot = newRotationPivot;
            }

            KeyRotationSettings.RenderEditorGUI(undoRecordObject);
        }
        #endif
    }
}
