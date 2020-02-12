using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class ObjectExtrudeGizmoHotkeys : Settings
    {
        [SerializeField]
        private Hotkeys _enableOverlapTest = new Hotkeys("Enable overlap test", new HotkeysStaticData() { CanHaveMouseButtons = false })
        {
            Key = KeyCode.None,
            LShift = true
        };

        public Hotkeys EnableOverlapTest { get { return _enableOverlapTest; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            EnableOverlapTest.RenderEditorGUI(undoRecordObject);
        }
        #endif
    }
}
