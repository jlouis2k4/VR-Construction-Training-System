using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class BoxGizmoLookAndFeel3D : Settings
    {
        [SerializeField]
        private Color _boxWireColor = new Color(1.0f, 1.0f, 1.0f, RTSystemValues.AxisAlpha);
        [SerializeField]
        private GizmoCap2DLookAndFeel[] _tickLookAndFeel = new GizmoCap2DLookAndFeel[6];

        public Color BoxWireColor { get { return _boxWireColor; } }
        public Color XTickColor { get { return GetTickLookAndFeel(0, AxisSign.Positive).Color; } }
        public Color YTickColor { get { return GetTickLookAndFeel(1, AxisSign.Positive).Color; } }
        public Color ZTickColor { get { return GetTickLookAndFeel(2, AxisSign.Positive).Color; } }
        public Color TickBorderColor { get { return GetTickLookAndFeel(0, AxisSign.Positive).BorderColor; } }
        public Color TickHoveredColor { get { return GetTickLookAndFeel(0, AxisSign.Positive).HoveredColor; } }
        public Color TickHoveredBorderColor { get { return GetTickLookAndFeel(0, AxisSign.Positive).HoveredBorderColor; } }
        public GizmoCap2DType TickType { get { return GetTickLookAndFeel(0, AxisSign.Positive).CapType; } }
        public float TickQuadWidth { get { return GetTickLookAndFeel(0, AxisSign.Positive).QuadWidth; } }
        public float TickQuadHeight { get { return GetTickLookAndFeel(0, AxisSign.Positive).QuadHeight; } }
        public float TickCircleRadius { get { return GetTickLookAndFeel(0, AxisSign.Positive).CircleRadius; } }

        public BoxGizmoLookAndFeel3D()
        {
            for(int tickIndex = 0; tickIndex < _tickLookAndFeel.Length; ++tickIndex)
            {
                _tickLookAndFeel[tickIndex] = new GizmoCap2DLookAndFeel();
            }

            SetAxisTickColor(0, RTSystemValues.XAxisColor);
            SetAxisTickColor(1, RTSystemValues.YAxisColor);
            SetAxisTickColor(2, RTSystemValues.ZAxisColor);
            SetTickHoveredColor(RTSystemValues.HoveredAxisColor);
            SetTickBorderColor(ColorEx.KeepAllButAlpha(Color.black, 0.0f));
            SetTickHoveredBorderColor(ColorEx.KeepAllButAlpha(Color.black, 0.0f));

            SetTickQuadWidth(10.0f);
            SetTickQuadHeight(10.0f);
            SetTickCircleRadius(6.0f);
            SetTickType(GizmoCap2DType.Quad);
        }

        public List<Enum> GetAllowedTickTypes()
        {
            return new List<Enum>() { GizmoCap2DType.Circle, GizmoCap2DType.Quad };
        }

        public bool IsTickTypeAllowed(GizmoCap2DType tickType)
        {
            return tickType == GizmoCap2DType.Circle || tickType == GizmoCap2DType.Quad;
        }

        public void SetBoxWireColor(Color color)
        {
            _boxWireColor = color;
        }

        public void SetAxisTickColor(int axisIndex, Color color)
        {
            GetTickLookAndFeel(axisIndex, AxisSign.Positive).Color = color;
            GetTickLookAndFeel(axisIndex, AxisSign.Negative).Color = color;
        }

        public void SetTickBorderColor(Color color)
        {
            foreach (var lookAndFeel in _tickLookAndFeel)
                lookAndFeel.BorderColor = color;
        }

        public void SetTickHoveredColor(Color color)
        {
            foreach (var lookAndFeel in _tickLookAndFeel)
                lookAndFeel.HoveredColor = color;
        }

        public void SetTickHoveredBorderColor(Color color)
        {
            foreach (var lookAndFeel in _tickLookAndFeel)
                lookAndFeel.HoveredBorderColor = color;
        }

        public void SetTickType(GizmoCap2DType tickType)
        {
            foreach (var lookAndFeel in _tickLookAndFeel)
                lookAndFeel.CapType = tickType;
        }

        public void SetTickQuadWidth(float width)
        {
            foreach (var lookAndFeel in _tickLookAndFeel)
                lookAndFeel.QuadWidth = width;
        }

        public void SetTickQuadHeight(float height)
        {
            foreach (var lookAndFeel in _tickLookAndFeel)
                lookAndFeel.QuadHeight = height;
        }

        public void SetTickCircleRadius(float radius)
        {
            foreach (var lookAndFeel in _tickLookAndFeel)
                lookAndFeel.CircleRadius = radius;
        }

        public void ConnectTickLookAndFeel(GizmoCap2D tick, int axisIndex, AxisSign axisSign)
        {
            tick.SharedLookAndFeel = GetTickLookAndFeel(axisIndex, axisSign);
        }

        private GizmoCap2DLookAndFeel GetTickLookAndFeel(int axisIndex, AxisSign axisSign)
        {
            if (axisSign == AxisSign.Positive) return _tickLookAndFeel[axisIndex];
            else return _tickLookAndFeel[axisIndex + 3];
        }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            Color newColor; float newFloat;
            GizmoCap2DType newTickType;

            var content = new GUIContent();
            EditorGUILayoutEx.SectionHeader("Tick shape");
            content.text = "Tick type";
            content.tooltip = "The type of shape which is used to draw the ticks.";
            newTickType = (GizmoCap2DType)EditorGUILayoutEx.SelectiveEnumPopup(content, TickType, GetAllowedTickTypes());
            if (newTickType != TickType)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetTickType(newTickType);
            }

            if (TickType == GizmoCap2DType.Quad)
            {
                content.text = "Quad width";
                content.tooltip = "The tick quad width.";
                newFloat = EditorGUILayout.FloatField(content, TickQuadWidth);
                if (newFloat != TickQuadWidth)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetTickQuadWidth(newFloat);
                }

                content.text = "Quad height";
                content.tooltip = "The tick quad height.";
                newFloat = EditorGUILayout.FloatField(content, TickQuadHeight);
                if (newFloat != TickQuadHeight)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetTickQuadHeight(newFloat);
                }
            }
            else
            if (TickType == GizmoCap2DType.Circle)
            {
                content.text = "Circle radius";
                content.tooltip = "The tick circle radius.";
                newFloat = EditorGUILayout.FloatField(content, TickCircleRadius);
                if (newFloat != TickCircleRadius)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetTickCircleRadius(newFloat);
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Colors");
            content.text = "Box wire color";
            content.tooltip = "The color used to draw the box wireframe.";
            newColor = EditorGUILayout.ColorField(content, BoxWireColor);
            if (newColor != BoxWireColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetBoxWireColor(newColor);
            }

            content.text = "X ticks";
            content.tooltip = "The color of the ticks which are associated with the X axis.";
            newColor = EditorGUILayout.ColorField(content, XTickColor);
            if (newColor != XTickColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetAxisTickColor(0, newColor);
            }

            content.text = "Y ticks";
            content.tooltip = "The color of the ticks which are associated with the Y axis.";
            newColor = EditorGUILayout.ColorField(content, YTickColor);
            if (newColor != YTickColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetAxisTickColor(1, newColor);
            }

            content.text = "Z ticks";
            content.tooltip = "The color of the ticks which are associated with the Z axis.";
            newColor = EditorGUILayout.ColorField(content, ZTickColor);
            if (newColor != ZTickColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetAxisTickColor(2, newColor);
            }

            content.text = "Hovered ticks";
            content.tooltip = "The color used to draw hovered ticks.";
            newColor = EditorGUILayout.ColorField(content, TickHoveredColor);
            if (newColor != TickHoveredColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetTickHoveredColor(newColor);
            }

            content.text = "Tick border";
            content.tooltip = "The tick border color.";
            newColor = EditorGUILayout.ColorField(content, TickBorderColor);
            if (newColor != TickBorderColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetTickBorderColor(newColor);
            }

            content.text = "Tick hovered border";
            content.tooltip = "The tick hovered border color.";
            newColor = EditorGUILayout.ColorField(content, TickHoveredBorderColor);
            if (newColor != TickHoveredBorderColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetTickHoveredBorderColor(newColor);
            }
        }
        #endif
    }
}
