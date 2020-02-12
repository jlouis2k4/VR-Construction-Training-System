using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RLD
{
    public enum SelectionBoxBorderStyle
    {
        FullWire = 0,
        WireCorners
    }

    public enum SelectionBoxRenderMode
    {
        PerObject = 0,
        FromParentToBottom,
        SelectionVolume
    }

    [Serializable]
    public class ObjectSelectionLookAndFeel : Settings
    {
        [SerializeField]
        private bool _drawHighlight = true;
        [SerializeField]
        private SelectionBoxBorderStyle _selectionBoxBorderStyle = SelectionBoxBorderStyle.WireCorners;
        [SerializeField]
        private float _wireCornerLinePercentage = 0.5f;
        [SerializeField]
        private SelectionBoxRenderMode _selectionBoxRenderMode = SelectionBoxRenderMode.FromParentToBottom;
        [SerializeField]
        private Color _selectionBoxBorderColor = Color.green;
        [SerializeField]
        private float _selectionBoxInflateAmount = 0.005f;
        [SerializeField]
        private Color _selectionRectBorderColor = Color.white;
        [SerializeField]
        private Color _selectionRectFillColor = ColorEx.FromByteValues(95, 109, 130, 128);

        public bool DrawHighlight { get { return _drawHighlight; } set { _drawHighlight = value; } }
        public SelectionBoxBorderStyle SelBoxBorderStyle { get { return _selectionBoxBorderStyle; } set { _selectionBoxBorderStyle = value; } }
        public float WireCornerLinePercentage { get { return _wireCornerLinePercentage; } set { _wireCornerLinePercentage = Mathf.Clamp(value, 1e-2f, 1.0f); } }
        public SelectionBoxRenderMode SelBoxRenderMode { get { return _selectionBoxRenderMode; } set { _selectionBoxRenderMode = value; } }
        public Color SelectionBoxBorderColor { get { return _selectionBoxBorderColor; } set { _selectionBoxBorderColor = value; } }
        public float SelectionBoxInflateAmount { get { return _selectionBoxInflateAmount; } set { _selectionBoxInflateAmount = Mathf.Max(value, 0.0f); } }
        public Color SelectionRectBorderColor { get { return _selectionRectBorderColor; } set { _selectionRectBorderColor = value; } }
        public Color SelectionRectFillColor { get { return _selectionRectFillColor; } set { _selectionRectFillColor = value; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            bool newBool; Color newColor; 
            float newFloat; 
            SelectionBoxBorderStyle newBoxBorderStyle;
            SelectionBoxRenderMode newBoxRenderMode;

            // Can draw?
            var content = new GUIContent();
            content.text = "Draw highlight";
            content.tooltip = "Allows you to toggle selection highlight drawing on/off.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawHighlight);
            if(newBool != DrawHighlight)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawHighlight = newBool;
            }

            // Selection box border style
            content.text = "Box border style";
            content.tooltip = "Allows you to control the style of the selection box borders.";
            newBoxBorderStyle = (SelectionBoxBorderStyle)EditorGUILayout.EnumPopup(content, SelBoxBorderStyle);
            if (newBoxBorderStyle != SelBoxBorderStyle)
            {
                EditorUndoEx.Record(undoRecordObject);
                SelBoxBorderStyle = newBoxBorderStyle;
            }

            if (SelBoxBorderStyle == SelectionBoxBorderStyle.WireCorners)
            {
                // Wire corner line percentage
                content.text = "Corner line percentage";
                content.tooltip = "When the border style is set to \'WireCorners\', this controls the length of the corner lines as a percentage of half the box edge along which they extend.";
                newFloat = EditorGUILayout.FloatField(content, WireCornerLinePercentage);
                if(newFloat != WireCornerLinePercentage)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    WireCornerLinePercentage = newFloat;
                }
            }

            // Selection box render mode
            content.text = "Box render mode";
            content.tooltip = "Allows you to control the selection box render mode (e.g. per object, from parent to bottom, selection volume).";
            newBoxRenderMode = (SelectionBoxRenderMode)EditorGUILayout.EnumPopup(content, SelBoxRenderMode);
            if (newBoxRenderMode != SelBoxRenderMode)
            {
                EditorUndoEx.Record(undoRecordObject);
                SelBoxRenderMode = newBoxRenderMode;
            }

            // Selection box inflate amount
            content.text = "Box inflate amount";
            content.tooltip = "Allows you to inflate the selection boxes to avoid Z wars with the object's volume.";
            newFloat = EditorGUILayout.FloatField(content, SelectionBoxInflateAmount);
            if(newFloat != SelectionBoxInflateAmount)
            {
                EditorUndoEx.Record(undoRecordObject);
                SelectionBoxInflateAmount = newFloat;
            }

            // Selection box border color
            content.text = "Box border color";
            content.tooltip = "Allows you to modify the color of the selection box border lines.";
            newColor = EditorGUILayout.ColorField(content, SelectionBoxBorderColor);
            if(newColor != SelectionBoxBorderColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SelectionBoxBorderColor = newColor;
            }

            // Selection rectangle border color
            content.text = "Select rect border color";
            content.tooltip = "Allows you to modify the color of the selection rectangle border.";
            newColor = EditorGUILayout.ColorField(content, SelectionRectBorderColor);
            if (newColor != SelectionRectBorderColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SelectionRectBorderColor = newColor;
            }

            // Selection rectangle fill color
            content.text = "Select rect fill color";
            content.tooltip = "Allows you to modify the fill color of the selection rectangle.";
            newColor = EditorGUILayout.ColorField(content, SelectionRectFillColor);
            if (newColor != SelectionRectFillColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SelectionRectFillColor = newColor;
            }
        }
        #endif
    }
}