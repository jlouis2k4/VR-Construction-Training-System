using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class ObjectSelectionGizmosHotkeys : Settings
    {
        [SerializeField]
        private Hotkeys _activateMoveGizmo = new Hotkeys("Activate move gizmo", new HotkeysStaticData() { CanHaveMouseButtons = false } )
        {
            UseStrictMouseCheck = true, // Needed to avoid camera keys collision
            Key = KeyCode.W
        };
        [SerializeField]
        private Hotkeys _activateRotationGizmo = new Hotkeys("Activate rotation gizmo", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            UseStrictMouseCheck = true, // Needed to avoid camera keys collision
            Key = KeyCode.E
        };
        [SerializeField]
        private Hotkeys _activateScaleGizmo = new Hotkeys("Activate scale gizmo", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            UseStrictMouseCheck = true, // Needed to avoid camera keys collision
            Key = KeyCode.R
        };
        [SerializeField]
        private Hotkeys _activateBoxScaleGizmo = new Hotkeys("Activate box scale gizmo", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            UseStrictMouseCheck = true, // Needed to avoid camera keys collision
            Key = KeyCode.T
        };
        [SerializeField]
        private Hotkeys _activateUniversalGizmo = new Hotkeys("Activate universal gizmo", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            UseStrictMouseCheck = true, // Needed to avoid camera keys collision
            Key = KeyCode.U
        };
        [SerializeField]
        private Hotkeys _activateExtrudeGizmo = new Hotkeys("Activate extrude gizmo", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            UseStrictMouseCheck = true, // Needed to avoid camera keys collision
            Key = KeyCode.Q
        };
        [SerializeField]
        private Hotkeys _toggleTransformSpace = new Hotkeys("Toggle transform space (global/local)", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            Key = KeyCode.L
        };

        public Hotkeys ActivateMoveGizmo { get { return _activateMoveGizmo; } }
        public Hotkeys ActivateRotationGizmo { get { return _activateRotationGizmo; } }
        public Hotkeys ActivateScaleGizmo { get { return _activateScaleGizmo; } }
        public Hotkeys ActivateBoxScaleGizmo { get { return _activateBoxScaleGizmo; } }
        public Hotkeys ActivateUniversalGizmo { get { return _activateUniversalGizmo; } }
        public Hotkeys ActivateExtrudeGizmo { get { return _activateExtrudeGizmo; } }
        public Hotkeys ToggleTransformSpace { get { return _toggleTransformSpace; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            ActivateMoveGizmo.RenderEditorGUI(undoRecordObject);
            ActivateRotationGizmo.RenderEditorGUI(undoRecordObject);
            ActivateScaleGizmo.RenderEditorGUI(undoRecordObject);
            ActivateBoxScaleGizmo.RenderEditorGUI(undoRecordObject);
            ActivateUniversalGizmo.RenderEditorGUI(undoRecordObject);
            ActivateExtrudeGizmo.RenderEditorGUI(undoRecordObject);
            ToggleTransformSpace.RenderEditorGUI(undoRecordObject);
        }
        #endif
    }
}
