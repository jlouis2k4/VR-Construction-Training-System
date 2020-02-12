using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class SceneLookAndFeel : Settings
    {
        [SerializeField]
        private bool _drawLightIcons = true;
        [SerializeField]
        private bool _drawParticleSystemIcons = true;
        [SerializeField]
        private bool _drawCameraIcons = true;

        [SerializeField]
        private float _lightIconAlpha = 0.7f;
        [SerializeField]
        private float _particleSystemIconAlpha = 0.7f;
        [SerializeField]
        private float _cameraIconAlpha = 0.7f;

        [SerializeField]
        private Texture2D _lightIcon;
        [SerializeField]
        private Texture2D _particleSystemIcon;
        [SerializeField]
        private Texture2D _cameraIcon;

        public bool DrawLightIcons { get { return _drawLightIcons; } set { _drawLightIcons = value; } }
        public bool DrawParticleSystemIcons { get { return _drawParticleSystemIcons; } set { _drawParticleSystemIcons = value; } }
        public bool DrawCameraIcons { get { return _drawCameraIcons; } set { _drawCameraIcons = value; } }
        public float LightIconAlpha { get { return _lightIconAlpha; } set { _lightIconAlpha = Mathf.Clamp(value, 0.0f, 1.0f); } }
        public float ParticleSystemIconAlpha { get { return _particleSystemIconAlpha; } set { _particleSystemIconAlpha = Mathf.Clamp(value, 0.0f, 1.0f); } }
        public float CameraIconAlpha { get { return _cameraIconAlpha; } set { _cameraIconAlpha = Mathf.Clamp(value, 0.0f, 1.0f); } }
        public Texture2D LightIcon { get { return _lightIcon; } set { _lightIcon = value; } }
        public Texture2D ParticleSystemIcon { get { return _particleSystemIcon; } set { _particleSystemIcon = value; } }
        public Texture2D CameraIcon { get { return _cameraIcon; } set { _cameraIcon = value; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            bool newBool; Texture2D newTexture; float newFloat;
            var content = new GUIContent();

            content.text = "Draw light icons";
            content.tooltip = "If this is checked, there will be an icon drawn in the scene for each light object.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawLightIcons);
            if (newBool != DrawLightIcons)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawLightIcons = newBool;
            }

            content.text = "Icon alpha";
            content.tooltip = "Alpha values used to control the transparency for light icons.";
            newFloat = EditorGUILayout.FloatField(content, LightIconAlpha);
            if (newFloat != LightIconAlpha)
            {
                EditorUndoEx.Record(undoRecordObject);
                LightIconAlpha = newFloat;
            }

            content.text = "Light icon";
            content.tooltip = "The texture to use when drawing light icons.";
            newTexture = EditorGUILayout.ObjectField(content, LightIcon, typeof(Texture2D), false) as Texture2D;
            if (newTexture != LightIcon)
            {
                EditorUndoEx.Record(undoRecordObject);
                LightIcon = newTexture;
            }

            EditorGUILayout.Separator();
            content.text = "Draw particle system icons";
            content.tooltip = "If this is checked, there will be an icon drawn in the scene for each particle system object.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawParticleSystemIcons);
            if (newBool != DrawParticleSystemIcons)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawParticleSystemIcons = newBool;
            }

            content.text = "Icon alpha";
            content.tooltip = "Alpha values used to control the transparency for particle system icons.";
            newFloat = EditorGUILayout.FloatField(content, ParticleSystemIconAlpha);
            if (newFloat != ParticleSystemIconAlpha)
            {
                EditorUndoEx.Record(undoRecordObject);
                ParticleSystemIconAlpha = newFloat;
            }

            content.text = "Particle system icon";
            content.tooltip = "The texture to use when drawing particle system icons.";
            newTexture = EditorGUILayout.ObjectField(content, ParticleSystemIcon, typeof(Texture2D), false) as Texture2D;
            if (newTexture != ParticleSystemIcon)
            {
                EditorUndoEx.Record(undoRecordObject);
                ParticleSystemIcon = newTexture;
            }

            EditorGUILayout.Separator();
            content.text = "Draw camera icons";
            content.tooltip = "If this is checked, there will be an icon drawn in the scene for each camera object.";
            newBool = EditorGUILayout.ToggleLeft(content, DrawCameraIcons);
            if (newBool != DrawCameraIcons)
            {
                EditorUndoEx.Record(undoRecordObject);
                DrawCameraIcons = newBool;
            }

            content.text = "Icon alpha";
            content.tooltip = "Alpha values used to control the transparency for camera icons.";
            newFloat = EditorGUILayout.FloatField(content, CameraIconAlpha);
            if (newFloat != CameraIconAlpha)
            {
                EditorUndoEx.Record(undoRecordObject);
                CameraIconAlpha = newFloat;
            }

            content.text = "Camera icon";
            content.tooltip = "The texture to use when drawing camera icons.";
            newTexture = EditorGUILayout.ObjectField(content, CameraIcon, typeof(Texture2D), false) as Texture2D;
            if (newTexture != CameraIcon)
            {
                EditorUndoEx.Record(undoRecordObject);
                CameraIcon = newTexture;
            }
        }
        #endif
    }
}
