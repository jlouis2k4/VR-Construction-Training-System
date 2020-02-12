using UnityEngine;
using UnityEditor;

namespace RLD
{
    [CustomEditor(typeof(RTScene))]
    public class RTSceneInspector : Editor
    {
        private RTScene _scene;

        public override void OnInspectorGUI()
        {
            _scene.Settings.RenderEditorGUI(_scene);
            _scene.LookAndFeel.RenderEditorGUI(_scene);
        }

        private void OnEnable()
        {
            _scene = target as RTScene;

            _scene.Settings.FoldoutLabel = "Settings";
            _scene.Settings.UsesFoldout = true;

            _scene.LookAndFeel.FoldoutLabel = "Look & feel";
            _scene.LookAndFeel.UsesFoldout = true;
        }
    }
}
