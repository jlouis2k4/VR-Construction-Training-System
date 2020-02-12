using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RLD
{
    [Serializable]
    public class ObjectSelectionRotationHotkeys : Settings
    {
        [SerializeField]
        private Hotkeys _rotateAroundX = new Hotkeys("Rotate around X", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            Key = KeyCode.X,
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = false
        };
        [SerializeField]
        private Hotkeys _rotateAroundY = new Hotkeys("Rotate around Y", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            Key = KeyCode.Y,
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = false
        };
        [SerializeField]
        private Hotkeys _rotateAroundZ = new Hotkeys("Rotate around Z", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            Key = KeyCode.Z,
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = false
        };
        [SerializeField]
        private Hotkeys _setRotationToIdentity = new Hotkeys("Set rotation to identity", new HotkeysStaticData { CanHaveMouseButtons = false })
        {
            Key = KeyCode.I,
            UseStrictModifierCheck = true,
            UseStrictMouseCheck = false
        };

        public Hotkeys RotateAroundX { get { return _rotateAroundX; } }
        public Hotkeys RotateAroundY { get { return _rotateAroundY; } }
        public Hotkeys RotateAroundZ { get { return _rotateAroundZ; } }
        public Hotkeys SetRotationToIdentity { get { return _setRotationToIdentity; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            RotateAroundX.RenderEditorGUI(undoRecordObject);
            RotateAroundY.RenderEditorGUI(undoRecordObject);
            RotateAroundZ.RenderEditorGUI(undoRecordObject);
            SetRotationToIdentity.RenderEditorGUI(undoRecordObject);
        }
        #endif
    }
}
