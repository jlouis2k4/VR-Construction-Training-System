using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class ObjectExtrudeGizmoLookAndFeel3D : Settings
    {
        [SerializeField]
        private Color _boxWireColor = new Color(1.0f, 1.0f, 1.0f, RTSystemValues.AxisAlpha);
        [SerializeField]
        private GizmoLineSlider3DLookAndFeel[] _sglSlidersLookAndFeel = new GizmoLineSlider3DLookAndFeel[6];
        [SerializeField]
        private bool[] _extrudeSliderVis = new bool[6];

        public bool UseZoomFactor { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.UseZoomFactor; } }
        public Color BoxWireColor { get { return _boxWireColor; } }
        public GizmoCap3DType SliderCapType { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.CapType; } }
        public GizmoShadeMode SliderCapShadeMode { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.ShadeMode; } }
        public GizmoFillMode3D SliderCapFillMode { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.FillMode; } }
        public Color XColor { get { return GetSglSliderLookAndFeel(0, AxisSign.Positive).Color; } }
        public Color YColor { get { return GetSglSliderLookAndFeel(1, AxisSign.Positive).Color; } }
        public Color ZColor { get { return GetSglSliderLookAndFeel(2, AxisSign.Positive).Color; } }
        public Color HoveredColor { get { return _sglSlidersLookAndFeel[0].HoveredColor; } }
        public float SliderBoxCapWidth { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.BoxWidth; } }
        public float SliderBoxCapHeight { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.BoxHeight; } }
        public float SliderBoxCapDepth { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.BoxDepth; } }
        public float SliderConeCapHeight { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.ConeHeight; } }
        public float SliderConeCapBaseRadius { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.ConeRadius; } }
        public float SliderPyramidCapWidth { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.PyramidWidth; } }
        public float SliderPyramidCapHeight { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.PyramidHeight; } }
        public float SliderPyramidCapDepth { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.PyramidDepth; } }
        public float SliderTriPrismCapWidth { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.TrPrismWidth; } }
        public float SliderTriPrismCapHeight { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.TrPrismHeight; } }
        public float SliderTriPrismCapDepth { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.TrPrismDepth; } }
        public float SliderSphereCapRadius { get { return _sglSlidersLookAndFeel[0].CapLookAndFeel.SphereRadius; } }

        public ObjectExtrudeGizmoLookAndFeel3D()
        {
            for (int axisIndex = 0; axisIndex < _sglSlidersLookAndFeel.Length; ++axisIndex)
            {
                _sglSlidersLookAndFeel[axisIndex] = new GizmoLineSlider3DLookAndFeel();
                _sglSlidersLookAndFeel[axisIndex].Length = 0.0f;
            }

            SetAxisColor(0, RTSystemValues.XAxisColor);
            SetAxisColor(1, RTSystemValues.YAxisColor);
            SetAxisColor(2, RTSystemValues.ZAxisColor);
            SetSliderCapType(GizmoCap3DType.Pyramid);

            SetExtrudeSliderVisible(0, AxisSign.Positive, true);
            SetExtrudeSliderVisible(1, AxisSign.Positive, true);
            SetExtrudeSliderVisible(2, AxisSign.Positive, true);

            SetExtrudeSliderVisible(0, AxisSign.Negative, true);
            SetExtrudeSliderVisible(1, AxisSign.Negative, true);
            SetExtrudeSliderVisible(2, AxisSign.Negative, true);
        }

        public bool IsExtrudeSliderVisible(int axisIndex, AxisSign axisSign)
        {
            if (axisSign == AxisSign.Positive) return _extrudeSliderVis[axisIndex];
            else return _extrudeSliderVis[3 + axisIndex];
        }

        public void SetExtrudeSliderVisible(int axisIndex, AxisSign axisSign, bool isVisible)
        {
            if (axisSign == AxisSign.Positive) _extrudeSliderVis[axisIndex] = isVisible;
            else _extrudeSliderVis[3 + axisIndex] = isVisible;
        }

        public void SetBoxWireColor(Color color)
        {
            _boxWireColor = color;
        }

        public void SetSliderCapType(GizmoCap3DType capType)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.CapType = capType;
        }

        public void SetSliderBoxCapWidth(float width)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.BoxWidth = width;
        }

        public void SetSliderBoxCapHeight(float height)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.BoxHeight = height;
        }

        public void SetSliderBoxCapDepth(float depth)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.BoxDepth = depth;
        }

        public void SetSliderConeCapHeight(float height)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.ConeHeight = height;
        }

        public void SetSliderConeCapBaseRadius(float radius)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.ConeRadius = radius;
        }

        public void SetSliderPyramidCapWidth(float width)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.PyramidWidth = width;
        }

        public void SetSliderPyramidCapHeight(float height)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.PyramidHeight = height;
        }

        public void SetSliderPyramidCapDepth(float depth)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.PyramidDepth = depth;
        }

        public void SetSliderTriPrismCapWidth(float width)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.TrPrismWidth = width;
        }

        public void SetSliderTriPrismCapHeight(float height)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.TrPrismHeight = height;
        }

        public void SetSliderTriPrismCapDepth(float depth)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.TrPrismDepth = depth;
        }

        public void SetSliderSphereCapRadius(float radius)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.SphereRadius = radius;
        }

        public void SetUseZoomFactor(bool useZoomFactor)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
            {
                lookAndFeel.CapLookAndFeel.UseZoomFactor = useZoomFactor;
            }
        }

        public void SetSliderCapShadeMode(GizmoShadeMode shadeMode)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.ShadeMode = shadeMode;
        }

        public void SetSliderCapFillMode(GizmoFillMode3D fillMode)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
                lookAndFeel.CapLookAndFeel.FillMode = fillMode;
        }

        public void SetAxisColor(int axisIndex, Color color)
        {
            GetSglSliderLookAndFeel(axisIndex, AxisSign.Positive).Color = color;
            GetSglSliderLookAndFeel(axisIndex, AxisSign.Positive).CapLookAndFeel.Color = color;
            GetSglSliderLookAndFeel(axisIndex, AxisSign.Negative).Color = color;
            GetSglSliderLookAndFeel(axisIndex, AxisSign.Negative).CapLookAndFeel.Color = color;
        }

        public void SetHoveredColor(Color hoveredColor)
        {
            foreach (var lookAndFeel in _sglSlidersLookAndFeel)
            {
                lookAndFeel.HoveredColor = hoveredColor;
                lookAndFeel.CapLookAndFeel.HoveredColor = hoveredColor;
            }
        }

        public void ConnectSliderLookAndFeel(GizmoLineSlider3D slider, int axisIndex, AxisSign axisSign)
        {
            slider.SharedLookAndFeel = GetSglSliderLookAndFeel(axisIndex, axisSign);
        }

        private GizmoLineSlider3DLookAndFeel GetSglSliderLookAndFeel(int axisIndex, AxisSign axisSign)
        {
            if (axisSign == AxisSign.Positive) return _sglSlidersLookAndFeel[axisIndex];
            else return _sglSlidersLookAndFeel[3 + axisIndex];
        }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            float newFloat; bool newBool;  Color newColor;
            GizmoShadeMode newShadeMode;
            GizmoFillMode3D newFillMode3D;
            GizmoCap3DType newCap3DType;

            EditorGUILayoutEx.SectionHeader("Scale");
            var content = new GUIContent();
            content.text = "Use zoom factor";
            content.tooltip = "If this is checked, the drag handles will maintain a constant size regardless of their distance from the camera.";
            newBool = EditorGUILayout.ToggleLeft(content, UseZoomFactor);
            if (newBool != UseZoomFactor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetUseZoomFactor(newBool);
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Cap shape");
            content.text = "Cap type";
            content.tooltip = "The type of shape which is used to draw the extrude caps.";
            newCap3DType = (GizmoCap3DType)EditorGUILayout.EnumPopup(content, SliderCapType);
            if (newCap3DType != SliderCapType)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetSliderCapType(newCap3DType);
            }

            if (SliderCapType == GizmoCap3DType.Box)
            {
                content.text = "Box width";
                content.tooltip = "Extrude caps box width when the cap type is set to \'Box\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderBoxCapWidth);
                if (newFloat != SliderBoxCapWidth)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderBoxCapWidth(newFloat);
                }

                content.text = "Box height";
                content.tooltip = "Extrude caps box height when the cap type is set to \'Box\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderBoxCapHeight);
                if (newFloat != SliderBoxCapHeight)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderBoxCapHeight(newFloat);
                }

                content.text = "Box depth";
                content.tooltip = "Extrude caps box depth when the cap type is set to \'Box\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderBoxCapDepth);
                if (newFloat != SliderBoxCapDepth)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderBoxCapDepth(newFloat);
                }
            }
            else
            if (SliderCapType == GizmoCap3DType.Cone)
            {
                content.text = "Cone height";
                content.tooltip = "Extrude caps cone height when the cap type is set to \'Cone\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderConeCapHeight);
                if (newFloat != SliderConeCapHeight)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderConeCapHeight(newFloat);
                }

                content.text = "Cone radius";
                content.tooltip = "Extrude caps cone radius when the cap type is set to \'Cone\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderConeCapBaseRadius);
                if (newFloat != SliderConeCapBaseRadius)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderConeCapBaseRadius(newFloat);
                }
            }
            else
            if (SliderCapType == GizmoCap3DType.Pyramid)
            {
                content.text = "Pyramid width";
                content.tooltip = "Extrude caps pyramid width when the cap type is set to \'Pyramid\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderPyramidCapWidth);
                if (newFloat != SliderPyramidCapWidth)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderPyramidCapWidth(newFloat);
                }

                content.text = "Pyramid height";
                content.tooltip = "Extrude caps pyramid height when the cap type is set to \'Pyramid\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderPyramidCapHeight);
                if (newFloat != SliderPyramidCapHeight)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderPyramidCapHeight(newFloat);
                }

                content.text = "Pyramid depth";
                content.tooltip = "Extrude caps pyramid depth when the cap type is set to \'Pyramid\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderPyramidCapDepth);
                if (newFloat != SliderPyramidCapDepth)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderPyramidCapDepth(newFloat);
                }
            }
            else
            if (SliderCapType == GizmoCap3DType.TriangPrism)
            {
                content.text = "Triang prism width";
                content.tooltip = "Extrude caps prism width when the cap type is set to \'TriangPrism\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderTriPrismCapWidth);
                if (newFloat != SliderTriPrismCapWidth)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderTriPrismCapWidth(newFloat);
                }

                content.text = "Triang prism height";
                content.tooltip = "Extrude caps prism height when the cap type is set to \'TriangPrism\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderTriPrismCapHeight);
                if (newFloat != SliderTriPrismCapHeight)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderTriPrismCapHeight(newFloat);
                }

                content.text = "Triang prism depth";
                content.tooltip = "Extrude caps prism depth when the cap type is set to \'TriangPrism\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderTriPrismCapDepth);
                if (newFloat != SliderTriPrismCapDepth)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderTriPrismCapDepth(newFloat);
                }
            }
            else
            if (SliderCapType == GizmoCap3DType.Sphere)
            {
                content.text = "Sphere radius";
                content.tooltip = "Extrude caps sphere radius when the cap type is set to \'Sphere\'.";
                newFloat = EditorGUILayout.FloatField(content, SliderSphereCapRadius);
                if (newFloat != SliderSphereCapRadius)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetSliderSphereCapRadius(newFloat);
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

            string[] axesLabels = new string[] { "X", "Y", "Z" };
            for (int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                GizmoLineSlider3DLookAndFeel sliderLookAndFeel = GetSglSliderLookAndFeel(axisIndex, AxisSign.Positive);

                content.text = axesLabels[axisIndex];
                content.tooltip = "The color of the " + axesLabels[axisIndex] + " axis when it is not hovered.";
                newColor = EditorGUILayout.ColorField(content, sliderLookAndFeel.Color);
                if (newColor != sliderLookAndFeel.Color)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetAxisColor(axisIndex, newColor);
                }
            }

            content.text = "Hovered";
            content.tooltip = "The gizmo hovered color.";
            newColor = EditorGUILayout.ColorField(content, HoveredColor);
            if (newColor != HoveredColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetHoveredColor(newColor);
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Shading");
            content.text = "Extrude caps";
            content.tooltip = "The type of shading that is applied to the extrude caps.";
            newShadeMode = (GizmoShadeMode)EditorGUILayout.EnumPopup(content, SliderCapShadeMode);
            if (newShadeMode != SliderCapShadeMode)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetSliderCapShadeMode(newShadeMode);
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Fill");
            content.text = "Extrude caps";
            content.tooltip = "Fill mode for extrude caps.";
            newFillMode3D = (GizmoFillMode3D)EditorGUILayout.EnumPopup(content, SliderCapFillMode);
            if (newFillMode3D != SliderCapFillMode)
            {
                EditorUndoEx.Record(undoRecordObject);
                SetSliderCapFillMode(newFillMode3D);
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Slider visibility");
            DrawSliderVisibilityControls(AxisSign.Positive, undoRecordObject);
            DrawSliderVisibilityControls(AxisSign.Negative, undoRecordObject);
            DrawCheckUncheckAllSlidersVisButtons(undoRecordObject);
        }

        private void DrawSliderVisibilityControls(AxisSign axisSign, UnityEngine.Object undoRecordObject)
        {
            var content = new GUIContent();
            EditorGUILayout.BeginHorizontal();
            string[] sliderLabels = axisSign == AxisSign.Positive ? new string[] { "+X", "+Y", "+Z" } : new string[] { "-X", "-Y", "-Z" };
            for (int sliderIndex = 0; sliderIndex < 3; ++sliderIndex)
            {
                content.text = sliderLabels[sliderIndex];
                content.tooltip = "Toggle visibility for the " + sliderLabels[sliderIndex] + " slider.";

                bool isVisible = IsExtrudeSliderVisible(sliderIndex, axisSign);
                bool newBool = EditorGUILayout.ToggleLeft(content, isVisible, GUILayout.Width(60.0f));
                if (newBool != isVisible)
                {
                    EditorUndoEx.Record(undoRecordObject);
                    SetExtrudeSliderVisible(sliderIndex, axisSign, newBool);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCheckUncheckAllSlidersVisButtons(UnityEngine.Object undoRecordObject)
        {
            EditorGUILayout.BeginHorizontal();
            var content = new GUIContent();
            content.text = "Show all";
            content.tooltip = "Show all sliders.";
            if (GUILayout.Button(content, GUILayout.Width(80.0f)))
            {
                EditorUndoEx.Record(undoRecordObject);
                for (int index = 0; index < _extrudeSliderVis.Length; ++index) _extrudeSliderVis[index] = true;
            }

            content.text = "Hide all";
            content.tooltip = "Hide all sliders.";
            if (GUILayout.Button(content, GUILayout.Width(80.0f)))
            {
                EditorUndoEx.Record(undoRecordObject);
                for (int index = 0; index < _extrudeSliderVis.Length; ++index) _extrudeSliderVis[index] = false;
            }
            EditorGUILayout.EndHorizontal();
        }
        #endif
    }
}
