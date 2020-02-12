using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class BoxGizmoHotkeys : Settings
    {
        [SerializeField]
        private Hotkeys _enableSnapping = new Hotkeys("Enable snapping", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            Key = KeyCode.None,
            LCtrl = true
        };

        [SerializeField]
        private Hotkeys _enableCenterPivot = new Hotkeys("Enable center pivot", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            Key = KeyCode.None,
            LShift = true
        };

        public Hotkeys EnableSnapping { get { return _enableSnapping; } }
        public Hotkeys EnableCenterPivot { get { return _enableCenterPivot; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            EnableSnapping.RenderEditorGUI(undoRecordObject);
            EnableCenterPivot.RenderEditorGUI(undoRecordObject);
        }
        #endif
    }
}
