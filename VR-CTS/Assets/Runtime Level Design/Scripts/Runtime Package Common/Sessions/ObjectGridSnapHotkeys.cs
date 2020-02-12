using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RLD
{
    [Serializable]
    public class ObjectGridSnapHotkeys : Settings
    {
        [SerializeField]
        private Hotkeys _beginGridSnap = new Hotkeys("Begin grid snap", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = false,
            Key = KeyCode.B
        };

        public Hotkeys BeginGridSnap { get { return _beginGridSnap; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            BeginGridSnap.RenderEditorGUI(undoRecordObject);
        }
        #endif
    }
}
