using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class ObjectGrabSettings : Settings
    {
        [SerializeField]
        private bool _alignAxis = true;
        [SerializeField]
        private TransformAxis _alignmentAxis = TransformAxis.PositiveY;
        [SerializeField]
        private float _rotationSensitivity = 1.0f;
        [SerializeField]
        private float _scaleSensitivity = 0.03f;
        [SerializeField]
        private float _offsetFromSurfaceSensitivity = 0.03f;
        [SerializeField]
        private float _offsetFromAnchorSensitivity = 0.03f;
        [SerializeField]
        private ObjectGrabSurfaceFlags _surfaceFlags = ObjectGrabSurfaceFlags.Grid | ObjectGrabSurfaceFlags.Mesh | ObjectGrabSurfaceFlags.Terrain;
        [SerializeField]
        private float _defaultOffsetFromSurface = 0.0f;
        [SerializeField]
        private int _surfaceLayers = ~0;
        [SerializeField]
        private ObjectLayerGrabSettings[] _layerGrabSettings = new ObjectLayerGrabSettings[LayerEx.GetMaxLayer() + 1];
        [SerializeField]
        private int _sphericalMeshLayers = 0;
        [SerializeField]
        private int _terrainMeshLayers = 0;

        #if UNITY_EDITOR
        [SerializeField]
        private int _newLayer = 0;
        #endif

        public bool AlignAxis { get { return _alignAxis; } set { _alignAxis = value; } }
        public TransformAxis AlignmentAxis { get { return _alignmentAxis; } set { _alignmentAxis = value; } }
        public float RotationSensitivity { get { return _rotationSensitivity; } set { _rotationSensitivity = Mathf.Clamp(value, 1e-2f, 1.0f); } }
        public float ScaleSensitivity { get { return _scaleSensitivity; } set { _scaleSensitivity = Mathf.Clamp(value, 1e-2f, 1.0f); } }
        public float OffsetFromSurfaceSensitivity { get { return _offsetFromSurfaceSensitivity; } set { _offsetFromSurfaceSensitivity = Mathf.Clamp(value, 1e-2f, 1.0f); } }
        public float OffsetFromAnchorSensitivity { get { return _offsetFromAnchorSensitivity; } set { _offsetFromAnchorSensitivity = Mathf.Clamp(value, 1e-2f, 1.0f); } }
        public ObjectGrabSurfaceFlags SurfaceFlags { get { return _surfaceFlags; } set { _surfaceFlags = value; } }
        public float DefaultOffsetFromSurface { get { return _defaultOffsetFromSurface; } set { _defaultOffsetFromSurface = value; } }
        public int SurfaceLayers { get { return _surfaceLayers; } set { _surfaceLayers = value; } }
        public int SphericalMeshLayers { get { return _sphericalMeshLayers; } set { _sphericalMeshLayers = value; } }
        public int TerrainMeshLayers { get { return _terrainMeshLayers; } set { _terrainMeshLayers = value; } }

        public ObjectGrabSettings()
        {
            for (int index = 0; index < _layerGrabSettings.Length; ++index )
            {
                _layerGrabSettings[index] = new ObjectLayerGrabSettings(index);
            }
        }

        public ObjectLayerGrabSettings GetLayerGrabSettings(int layer)
        {
            return _layerGrabSettings[layer];
        }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            bool newBool; float newFloat; int newInt;
            TransformAxis newTransformAxis;

            EditorGUILayoutEx.SectionHeader("Alignment");
            var content = new GUIContent();
            content.text = "Align axis";
            content.tooltip = "If this is checked, the grabbed objects will have their local axes aligned with the grab surface normal.";
            newBool = EditorGUILayout.ToggleLeft(content, AlignAxis);
            if(newBool != AlignAxis)
            {
                EditorUndoEx.Record(undoRecordObject);
                AlignAxis = newBool;
            }

            content.text = "Alignment axis";
            content.tooltip = "When axis alignment is turned on, this is the axis that will be aligned to the surface normal.";
            newTransformAxis = (TransformAxis)EditorGUILayout.EnumPopup(content, AlignmentAxis);
            if(newTransformAxis != AlignmentAxis)
            {
                EditorUndoEx.Record(undoRecordObject);
                AlignmentAxis = newTransformAxis;
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Sensitivity");
            content.text = "Rotation";
            content.tooltip = "Allows you to control how sensitive object rotation is to the input device.";
            newFloat = EditorGUILayout.FloatField(content, RotationSensitivity);
            if(newFloat != RotationSensitivity)
            {
                EditorUndoEx.Record(undoRecordObject);
                RotationSensitivity = newFloat;
            }

            content.text = "Scale";
            content.tooltip = "Allows you to control how sensitive object scaling is to the input device.";
            newFloat = EditorGUILayout.FloatField(content, ScaleSensitivity);
            if (newFloat != ScaleSensitivity)
            {
                EditorUndoEx.Record(undoRecordObject);
                ScaleSensitivity = newFloat;
            }

            content.text = "Offset from surface";
            content.tooltip = "Sensitivity value used when offseting objects from the surface on which they are sitting.";
            newFloat = EditorGUILayout.FloatField(content, OffsetFromSurfaceSensitivity);
            if (newFloat != OffsetFromSurfaceSensitivity)
            {
                EditorUndoEx.Record(undoRecordObject);
                OffsetFromSurfaceSensitivity = newFloat;
            }

            content.text = "Offset from anchor";
            content.tooltip = "Sensitivity value used when offseting objects from the anchor point.";
            newFloat = EditorGUILayout.FloatField(content, OffsetFromAnchorSensitivity);
            if (newFloat != OffsetFromAnchorSensitivity)
            {
                EditorUndoEx.Record(undoRecordObject);
                OffsetFromAnchorSensitivity = newFloat;
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Surface");
            content.text = "Default offset";
            content.tooltip = "A default offset value which controls the distance between the objects and the surface they are sitting on. Negative values can be used to embed objects in the surface.";
            newFloat = EditorGUILayout.FloatField(content, DefaultOffsetFromSurface);
            if (newFloat != DefaultOffsetFromSurface)
            {
                EditorUndoEx.Record(undoRecordObject);
                DefaultOffsetFromSurface = newFloat;
            }

            content.text = "Surface flags";
            content.tooltip = "Allows you to specify the types of surfaces that can be used during a grab session.";
            newInt = (int)((ObjectGrabSurfaceFlags)EditorGUILayout.EnumMaskPopup(content, (ObjectGrabSurfaceFlags)SurfaceFlags));
            if (newInt != (int)SurfaceFlags)
            {
                EditorUndoEx.Record(undoRecordObject);
                SurfaceFlags = (ObjectGrabSurfaceFlags)newInt;
            }

            content.text = "Surface layers";
            content.tooltip = "Allows you to specify which layers can be used as snap surface during a grab session.";
            newInt = EditorGUILayoutEx.LayerMaskField(content, SurfaceLayers);
            if (newInt != SurfaceLayers)
            {
                EditorUndoEx.Record(undoRecordObject);
                SurfaceLayers = newInt;
            }

            content.text = "Spherical mesh layers";
            content.tooltip = "Objects that belong to these layers will be treated as spherical meshes (spheres). This allows the system to make " + 
                              "certain assumptions about the surface geometry in order to produce the correct results.";
            newInt = EditorGUILayoutEx.LayerMaskField(content, SphericalMeshLayers);
            if (newInt != SphericalMeshLayers)
            {
                EditorUndoEx.Record(undoRecordObject);
                SphericalMeshLayers = newInt;
            }

            content.text = "Terrain mesh layers";
            content.tooltip = "Objects that belong to these layers will be treated as terrain meshes (meshes that resemble a terrain but are NOT Unity Terrains). This allows the system to make " +
                              "certain assumptions about the surface geometry in order to produce the correct results.";
            newInt = EditorGUILayoutEx.LayerMaskField(content, TerrainMeshLayers);
            if (newInt != TerrainMeshLayers)
            {
                EditorUndoEx.Record(undoRecordObject);
                TerrainMeshLayers = newInt;
            }

            EditorGUILayout.Separator();
            EditorGUILayoutEx.SectionHeader("Per layer settings");
            EditorGUILayout.BeginHorizontal();
            content.text = "Add";
            content.tooltip = "Adds grab settings which apply only to the chosen layer. Objects which belong to that layer will use these settings during a grab session.";
            if(GUILayout.Button(content, GUILayout.Width(70.0f)))
            {
                EditorUndoEx.Record(undoRecordObject);
                GetLayerGrabSettings(_newLayer).IsActive = true;
            }

            newInt = EditorGUILayout.LayerField(_newLayer);
            if (newInt != _newLayer)
            {
                EditorUndoEx.Record(undoRecordObject);
                _newLayer = newInt;
            }
            EditorGUILayout.EndHorizontal();

            foreach(var layerGrabSettings in _layerGrabSettings)
            {
                if (layerGrabSettings.IsActive)
                {
                    EditorGUILayout.BeginVertical("Box");
                    EditorGUILayout.HelpBox(LayerMask.LayerToName(layerGrabSettings.Layer), MessageType.None);

                    content.text = "Align axis";
                    content.tooltip = "If this is checked, the grabbed objects that belong to this layer will have their local axes aligned with the grab surface normal.";
                    newBool = EditorGUILayout.ToggleLeft(content, layerGrabSettings.AlignAxis);
                    if (newBool != layerGrabSettings.AlignAxis)
                    {
                        EditorUndoEx.Record(undoRecordObject);
                        layerGrabSettings.AlignAxis = newBool;
                    }

                    content.text = "Alignment axis";
                    content.tooltip = "When axis alignment is turned on, this is the axis that will be aligned to the surface normal.";
                    newTransformAxis = (TransformAxis)EditorGUILayout.EnumPopup(content, layerGrabSettings.AlignmentAxis);
                    if (newTransformAxis != layerGrabSettings.AlignmentAxis)
                    {
                        EditorUndoEx.Record(undoRecordObject);
                        layerGrabSettings.AlignmentAxis = newTransformAxis;
                    }

                    content.text = "Default offset from surface";
                    content.tooltip = "A default offset value which controls the distance between the objects and the surface they are sitting on. Negative values can be used to embed objects in the surface.";
                    newFloat = EditorGUILayout.FloatField(content, layerGrabSettings.DefaultOffsetFromSurface);
                    if (newFloat != layerGrabSettings.DefaultOffsetFromSurface)
                    {
                        EditorUndoEx.Record(undoRecordObject);
                        layerGrabSettings.DefaultOffsetFromSurface = newFloat;
                    }

                    content.text = "Remove";
                    content.tooltip = "Removes the settings for this layer.";
                    if (GUILayout.Button(content, GUILayout.Width(70.0f)))
                    {
                        EditorUndoEx.Record(undoRecordObject);
                        layerGrabSettings.IsActive = false;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }
        #endif
    }
}
