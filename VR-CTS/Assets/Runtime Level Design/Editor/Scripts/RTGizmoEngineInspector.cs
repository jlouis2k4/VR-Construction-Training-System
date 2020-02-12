using UnityEngine;
using UnityEditor;

namespace RLD
{
    [CustomEditor(typeof(RTGizmosEngine))]
    public class RTGizmoEngineInspector : Editor
    {
        private RTGizmosEngine _gizmoEngine;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            _gizmoEngine.MainToolbar.GetTabByIndex(0).AddTargetSettings(_gizmoEngine.Settings);
            _gizmoEngine.MainToolbar.GetTabByIndex(1).AddTargetSettings(_gizmoEngine.SharedSceneGizmoLookAndFeel);
            _gizmoEngine.MainToolbar.RenderEditorGUI(_gizmoEngine);
        }

        private void OnEnable()
        {
            _gizmoEngine = target as RTGizmosEngine;
        }
    }
}
