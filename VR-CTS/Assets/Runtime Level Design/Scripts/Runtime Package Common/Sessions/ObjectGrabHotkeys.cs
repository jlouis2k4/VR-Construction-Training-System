using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class ObjectGrabHotkeys : Settings
    {
        [SerializeField]
        private Hotkeys _toggleGrab = new Hotkeys("Toggle on/off", new HotkeysStaticData { CanHaveMouseButtons = false } )
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            Key = KeyCode.C
        };
        [SerializeField]
        private Hotkeys _enableRotation = new Hotkeys("Enable rotation", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            LShift = true
        };
        [SerializeField]
        private Hotkeys _enableRotationAroundAnchor = new Hotkeys("Enable rotation around anchor", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            LShift = true,
            LCtrl = true
        };
        [SerializeField]
        private Hotkeys _enableScaling = new Hotkeys("Enable scaling", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            LCtrl = true
        };
        [SerializeField]
        private Hotkeys _enableOffsetFromSurface = new Hotkeys("Enable offset from surface", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            Key = KeyCode.Q
        };
        [SerializeField]
        private Hotkeys _enableAnchorAdjust = new Hotkeys("Enable anchor adjust", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            LAlt = true
        };
        [SerializeField]
        private Hotkeys _enableOffsetFromAnchor = new Hotkeys("Enable offset from anchor", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            Key = KeyCode.Space
        };
        [SerializeField]
        private Hotkeys _nextAlignmentAxis = new Hotkeys("Next alignment axis", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = true,
            Key = KeyCode.B
        };

        public Hotkeys ToggleGrab { get { return _toggleGrab; } }
        public Hotkeys EnableRotation { get { return _enableRotation; } }
        public Hotkeys EnableRotationAroundAnchor { get { return _enableRotationAroundAnchor; } }
        public Hotkeys EnableScaling { get { return _enableScaling; } }
        public Hotkeys EnableOffsetFromSurface { get { return _enableOffsetFromSurface; } }
        public Hotkeys EnableAnchorAdjust { get { return _enableAnchorAdjust; } }
        public Hotkeys EnableOffsetFromAnchor { get { return _enableOffsetFromAnchor; } }
        public Hotkeys NextAlignmentAxis { get { return _nextAlignmentAxis; } }

        public ObjectGrabHotkeys()
        {
            EstablishPotentialOverlaps();
        }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            ToggleGrab.RenderEditorGUI(undoRecordObject);
            EnableRotation.RenderEditorGUI(undoRecordObject);
            EnableRotationAroundAnchor.RenderEditorGUI(undoRecordObject);
            EnableScaling.RenderEditorGUI(undoRecordObject);
            EnableOffsetFromSurface.RenderEditorGUI(undoRecordObject);
            EnableAnchorAdjust.RenderEditorGUI(undoRecordObject);
            EnableOffsetFromAnchor.RenderEditorGUI(undoRecordObject);
            NextAlignmentAxis.RenderEditorGUI(undoRecordObject);
        }
        #endif

        private void EstablishPotentialOverlaps()
        {
            var list = new List<Hotkeys>()
            {
                EnableRotation, EnableRotationAroundAnchor, EnableScaling,
                EnableOffsetFromSurface, EnableAnchorAdjust, EnableOffsetFromAnchor
            };
            Hotkeys.EstablishPotentialOverlaps(list);
        }
    }
}
