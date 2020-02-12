using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class PrefabPreviewLookAndFeel : Settings
    {
        private static readonly float _minBkAlpha = 0.004f;

        [SerializeField]
        private Color _bkColor = new Color(0.321568638f, 0.321568638f, 0.321568638f, 0.2588f);
        [SerializeField]
        private int _previewWidth = 90;
        [SerializeField]
        private int _previewHeight = 90;
        [SerializeField]
        private float _lightIntensity = 1.0f;

        public Color BkColor { get { return _bkColor; } set { _bkColor = value; _bkColor.a = Mathf.Max(_minBkAlpha, _bkColor.a); } }
        public int PreviewWidth { get { return _previewWidth; } set { _previewWidth = Mathf.Max(4, value); } }
        public int PreviewHeight { get { return _previewHeight; } set { _previewHeight = Mathf.Max(4, value); } }
        public float LightIntensity { get { return _lightIntensity; } set { _lightIntensity = Mathf.Max(1e-4f, value); } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            Color newColor; int newInt; float newFloat;

            var content = new GUIContent();
            content.text = "Bk color";
            content.tooltip = "The background color of the prefab previews.";
            newColor = EditorGUILayout.ColorField(content, BkColor);
            if (newColor != BkColor)
            {
                EditorUndoEx.Record(undoRecordObject);
                BkColor = newColor;
            }

            content.text = "Preview width";
            content.tooltip = "The width of the prefab previews.";
            newInt = EditorGUILayout.IntField(content, PreviewWidth);
            if (newInt != PreviewWidth)
            {
                EditorUndoEx.Record(undoRecordObject);
                PreviewWidth = newInt;
            }

            content.text = "Preview height";
            content.tooltip = "The height of the prefab previews.";
            newInt = EditorGUILayout.IntField(content, PreviewHeight);
            if (newInt != PreviewHeight)
            {
                EditorUndoEx.Record(undoRecordObject);
                PreviewHeight = newInt;
            }

            content.text = "Light intensity";
            content.tooltip = "The intensity of the light which lights the preview objects.";
            newFloat = EditorGUILayout.FloatField(content, LightIntensity);
            if (newFloat != LightIntensity)
            {
                EditorUndoEx.Record(undoRecordObject);
                LightIntensity = newFloat;
            }
        }
        #endif
    }
}
