using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class ObjectGrabLookAndFeel : Settings
    {
        [SerializeField]
        private bool _drawAnchorLines = true;
        [SerializeField]
        private Color _anchorLineColor = Color.green;
        [SerializeField]
        private bool _drawObjectPosTicks = true;
        [SerializeField]
        private bool _drawAnchorPosTick = true;
        [SerializeField]
        private Color _objectPosTickColor = Color.white;
        [SerializeField]
        private Color _anchorPosTickColor = ColorEx.FromByteValues(255, 140, 0, 255);
        [SerializeField]
        private float _objectPosTickSize = 10.0f;
        [SerializeField]
        private float _anchorPosTickSize = 10.0f;
        [SerializeField]
        private bool _drawObjectBoxes = true;
        [SerializeField]
        private Color _objectBoxWireColor = ColorEx.KeepAllButAlpha(Color.white, 0.3f);

        public bool DrawAnchorLines { get { return _drawAnchorLines; } set { _drawAnchorLines = value; } }
        public Color AnchorLineColor { get { return _anchorLineColor; } set { _anchorLineColor = value; } }
        public bool DrawObjectPosTicks { get { return _drawObjectPosTicks; } set { _drawObjectPosTicks = value; } }
        public bool DrawAnchorPosTick { get { return _drawAnchorPosTick; } set { _drawAnchorPosTick = value; } }
        public Color ObjectPosTickColor { get { return _objectPosTickColor; } set { _objectPosTickColor = value; } }
        public float ObjectPosTickSize { get { return _objectPosTickSize; } set { _objectPosTickSize = Mathf.Max(2.0f, value); } }
        public Color AnchorPosTickColor { get { return _anchorPosTickColor; } set { _anchorPosTickColor = value; } }
        public float AnchorPosTickSize { get { return _anchorPosTickSize; } set { _anchorPosTickSize = Mathf.Max(2.0f, value); } }
        public bool DrawObjectBoxes { get { return _drawObjectBoxes; } set { _drawObjectBoxes = value; } }
        public Color ObjectBoxWireColor { get { return _objectBoxWireColor; } set { _objectBoxWireColor = value; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            bool newBool; Color newColor; float newFloat;

            EditorGUILayoutEx.SectionHeader("Anchor lines");
            var content = new GUIContent();
            content.text = "Visible";
            content.tooltip = "If this is checked, a line will be drawn between each object's position and the anchor position.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawAnchorLines);
            if(newBool != DrawAnchorLines)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawAnchorLines = newBool;
            }

            content.text = "Anchor line color";
            content.tooltip = "Allows you to change the color of the anchor lines.";
            newColor = EditorGUILayout.ColorField(content, AnchorLineColor);
            if (newColor != AnchorLineColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                AnchorLineColor = newColor;
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Object position ticks");
            content.text = "Visible";
            content.tooltip = "If this is checked, ticks will be drawn at the position of each object that is controlled by the grab session.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawObjectPosTicks);
            if (newBool != DrawObjectPosTicks)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawObjectPosTicks = newBool;
            }

            content.text = "Tick color";
            content.tooltip = "The color which is used to draw the object position ticks.";
            newColor = EditorGUILayout.ColorField(content, ObjectPosTickColor);
            if (newColor != ObjectPosTickColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                ObjectPosTickColor = newColor;
            }

            content.text = "Tick size";
            content.tooltip = "The size of the object ticks.";
            newFloat = EditorGUILayout.FloatField(content, ObjectPosTickSize);
            if (newFloat != ObjectPosTickSize)
            {
                EditorUndoEx.Record(undoRecordObject);
                ObjectPosTickSize = newFloat;
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Anchor position ticks");
            content.text = "Visible";
            content.tooltip = "If this is checked, a tick will be drawn at the anchor position.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawAnchorPosTick);
            if (newBool != DrawAnchorPosTick)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawAnchorPosTick = newBool;
            }

            content.text = "Tick color";
            content.tooltip = "The color which is used to draw the anchor position tick.";
            newColor = EditorGUILayout.ColorField(content, AnchorPosTickColor);
            if (newColor != AnchorPosTickColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                AnchorPosTickColor = newColor;
            }

            content.text = "Tick size";
            content.tooltip = "The size of the anchor position tick.";
            newFloat = EditorGUILayout.FloatField(content, AnchorPosTickSize);
            if (newFloat != AnchorPosTickSize)
            {
                EditorUndoEx.Record(undoRecordObject);
                AnchorPosTickSize = newFloat;
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Object boxes");
            content.text = "Visible";
            content.tooltip = "If this is checked, a box will be drawn for each object that is controlled by the grab session.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawObjectBoxes);
            if (newBool != DrawObjectBoxes)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawObjectBoxes = newBool;
            }

            content.text = "Wire color";
            content.tooltip = "The color which is used to draw the object wire boxes.";
            newColor = EditorGUILayout.ColorField(content, ObjectBoxWireColor);
            if (newColor != ObjectBoxWireColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                ObjectBoxWireColor = newColor;
            }
        }
        #endif
    }
}