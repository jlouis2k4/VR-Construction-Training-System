using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RLD
{
    [Serializable]
    public class ObjectSelectionHotkeys : Settings
    {
        [SerializeField]
        private Hotkeys _appendToSelection = new Hotkeys("Append to selection", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            LCtrl = true,
        };
        [SerializeField]
        private Hotkeys _multiDeselect = new Hotkeys("Multi deselect", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            LShift = true,
        };
        [SerializeField]
        private Hotkeys _deleteSelected = new Hotkeys("Delete selected", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            Key = KeyCode.Delete
        };
        [SerializeField]
        private Hotkeys _focusCameraOnSelection = new Hotkeys("Focus camera on selection", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            Key = KeyCode.F
        };
        [SerializeField]
        private Hotkeys _duplicateSelection = new Hotkeys("Duplicate selected", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            UseStrictMouseCheck = true,
            UseStrictModifierCheck = true,
            LCtrl = true,
            Key = KeyCode.D
        };

        public Hotkeys AppendToSelection { get { return _appendToSelection; } }
        public Hotkeys MultiDeselect { get { return _multiDeselect; } }
        public Hotkeys DeleteSelected { get { return _deleteSelected; } }
        public Hotkeys FocusCameraOnSelection { get { return _focusCameraOnSelection; } }
        public Hotkeys DuplicateSelection { get { return _duplicateSelection; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            AppendToSelection.RenderEditorGUI(undoRecordObject);
            MultiDeselect.RenderEditorGUI(undoRecordObject);
            DeleteSelected.RenderEditorGUI(undoRecordObject);
            FocusCameraOnSelection.RenderEditorGUI(undoRecordObject);
            DuplicateSelection.RenderEditorGUI(undoRecordObject);
        }
        #endif
    }
}
