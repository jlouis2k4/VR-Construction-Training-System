using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class Object2ObjectSnapHotkeys : Settings
    {
        [SerializeField]
        private Hotkeys _toggleSnap = new Hotkeys("Toggle on/off", new HotkeysStaticData { CanHaveMouseButtons = false} )
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            Key = KeyCode.S
        };
        [SerializeField]
        private Hotkeys _toggleSitBelowSurface = new Hotkeys("Toggle sit below surface", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            Key = KeyCode.N
        };
        [SerializeField]
        private Hotkeys _enableMoreControl = new Hotkeys("Enable more control", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            LShift = true
        };
        [SerializeField]
        private Hotkeys _enableFlexiSnap = new Hotkeys("Enable flexi-snap", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            LCtrl = true
        };

        public Hotkeys ToggleSnap { get { return _toggleSnap; } }
        public Hotkeys ToggleSitBelowSurface { get { return _toggleSitBelowSurface; } }
        public Hotkeys EnableMoreControl { get { return _enableMoreControl; } }
        public Hotkeys EnableFlexiSnap { get { return _enableFlexiSnap; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            ToggleSnap.RenderEditorGUI(undoRecordObject);
            ToggleSitBelowSurface.RenderEditorGUI(undoRecordObject);
            EnableMoreControl.RenderEditorGUI(undoRecordObject);
            EnableFlexiSnap.RenderEditorGUI(undoRecordObject);
        } 
        #endif
    }
}
