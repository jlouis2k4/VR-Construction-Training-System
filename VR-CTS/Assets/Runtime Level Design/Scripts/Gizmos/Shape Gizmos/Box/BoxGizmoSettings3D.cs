using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class BoxGizmoSettings3D : Settings
    {
        [SerializeField]
        private float _xSnapStep = 0.1f;
        [SerializeField]
        private float _ySnapStep = 0.1f;
        [SerializeField]
        private float _zSnapStep = 0.1f;
        [SerializeField]
        private float _dragSensitivity = 1.0f;

        public float XSnapStep { get { return _xSnapStep; } }
        public float YSnapStep { get { return _ySnapStep; } }
        public float ZSnapStep { get { return _zSnapStep; } }
        public float DragSensitivity { get { return _dragSensitivity; } }

        public void SetXSnapStep(float snapStep)
        {
            _xSnapStep = Mathf.Max(1e-4f, snapStep);
        }

        public void SetYSnapStep(float snapStep)
        {
            _ySnapStep = Mathf.Max(1e-4f, snapStep);
        }

        public void SetZSnapStep(float snapStep)
        {
            _zSnapStep = Mathf.Max(1e-4f, snapStep);
        }

        public void SetDragSensitivity(float sensitivity)
        {
            _dragSensitivity = Mathf.Max(1e-4f, sensitivity);
        }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            float newFloat;

            EditorGUILayoutEx.SectionHeader("Snapping");
            var content = new GUIContent();
            content.text = "X";
            content.tooltip = "The snap step for the X axis.";
            newFloat = EditorGUILayout.FloatField(content, XSnapStep);
            if (newFloat != XSnapStep)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetXSnapStep(newFloat);
            }

            content.text = "Y";
            content.tooltip = "The snap step for the Y axis.";
            newFloat = EditorGUILayout.FloatField(content, YSnapStep);
            if (newFloat != YSnapStep)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetYSnapStep(newFloat);
            }

            content.text = "Z";
            content.tooltip = "The snap step for the Z axis.";
            newFloat = EditorGUILayout.FloatField(content, ZSnapStep);
            if (newFloat != ZSnapStep)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetZSnapStep(newFloat);
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Drag sensitivity");
            content.text = "Sensitivity";
            content.tooltip = "This value allows you to scale the drag speed.";
            newFloat = EditorGUILayout.FloatField(content, DragSensitivity);
            if (newFloat != DragSensitivity)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetDragSensitivity(newFloat);
            }
        }
        #endif
    }
}
