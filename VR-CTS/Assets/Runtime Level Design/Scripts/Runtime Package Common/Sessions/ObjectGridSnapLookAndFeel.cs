using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RLD
{
    [Serializable]
    public class ObjectGridSnapLookAndFeel : Settings
    {
        [SerializeField]
        private PivotPointShapeType _pivotShapeType = PivotPointShapeType.Circle;
        [SerializeField]
        private Color _pivotPointFillColor = Color.green;
        [SerializeField]
        private Color _pivotPointBorderColor = Color.black;
        [SerializeField]
        private float _pivotCircleRadius = 5.0f;
        [SerializeField]
        private float _pivotSquareSideLength = 10.0f;
        [SerializeField]
        private bool _drawPivotBorder = true;

        [SerializeField]
        private Color _boxLineColor = Color.yellow;
        [SerializeField]
        private bool _drawBoxes = true;

        public PivotPointShapeType PivotShapeType { get { return _pivotShapeType; } set { _pivotShapeType = value; } }
        public Color PivotPointFillColor { get { return _pivotPointFillColor; } set { _pivotPointFillColor = value; } }
        public Color PivotPointBorderColor { get { return _pivotPointBorderColor; } set { _pivotPointBorderColor = value; } }
        public float PivotCircleRadius { get { return _pivotCircleRadius; } set { _pivotCircleRadius = Mathf.Max(2.0f, value); } }
        public float PivotSquareSideLength { get { return _pivotSquareSideLength; } set { _pivotSquareSideLength = Mathf.Max(2.0f, value); } }
        public bool DrawPivotBorder { get { return _drawPivotBorder; } set { _drawPivotBorder = value; } }
        public Color BoxLineColor { get { return _boxLineColor; } set { _boxLineColor = value; } }
        public bool DrawBoxes { get { return _drawBoxes; } set { _drawBoxes = value; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            bool newBool; Color newColor; float newFloat;
            PivotPointShapeType newPivotShapeType;

            var content = new GUIContent();
            content.text = "Pivot shape type";
            content.tooltip = "Allows you to choose the shape that is drawn for the snap pivot point.";
            newPivotShapeType = (PivotPointShapeType)EditorGUILayout.EnumPopup(content, PivotShapeType);
            if(newPivotShapeType != PivotShapeType)
            {
                EditorUndoEx.Record(undoRecordObject);
                PivotShapeType = newPivotShapeType;
            }

            content.text = "Pivot fill color";
            content.tooltip = "Allows you to choose the fill color for the snap pivot point.";
            newColor = EditorGUILayout.ColorField(content, PivotPointFillColor);
            if(newColor != PivotPointFillColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                PivotPointFillColor = newColor;
            }

            content.text = "Pivot border color";
            content.tooltip = "Allows you to choose the border color for the snap pivot point.";
            newColor = EditorGUILayout.ColorField(content, PivotPointBorderColor);
            if (newColor != PivotPointBorderColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                PivotPointBorderColor = newColor;
            }

            if(PivotShapeType == PivotPointShapeType.Circle)
            {
                content.text = "Pivot circle radius";
                content.tooltip = "Allows you to control the radius of the pivot circle.";
                newFloat = EditorGUILayout.FloatField(content, PivotCircleRadius);
                if(newFloat != PivotCircleRadius)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    PivotCircleRadius = newFloat;
                }
            }
            else
            if(PivotShapeType == PivotPointShapeType.Square)
            {
                content.text = "Pivot square side length";
                content.tooltip = "Allows you to control the length of the pivot square side.";
                newFloat = EditorGUILayout.FloatField(content, PivotSquareSideLength);
                if (newFloat != PivotSquareSideLength)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    PivotSquareSideLength = newFloat;
                }
            }

            content.text = "Draw pivot border";
            content.tooltip = "If this is checked, the pivot will be drawn with a border.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawPivotBorder);
            if(newBool != DrawPivotBorder)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawPivotBorder = newBool;
            }

            EditorGUILayout.Separator();
            content.text = "Box line color";
            content.tooltip = "Allows you to choose the line color for the boxes that are drawn for each mesh object involved in the snap session.";
            newColor = EditorGUILayout.ColorField(content, BoxLineColor);
            if(newColor != BoxLineColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                BoxLineColor = newColor;
            }

            content.text = "Draw boxes";
            content.tooltip = "If this is checked, a box will be drawn for each mesh object involved in the snap session.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawBoxes);
            if (newBool != DrawBoxes)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawBoxes = newBool;
            }
        }
        #endif
    }
}
