using UnityEngine;
using UnityEditor;

namespace RLD
{
    [CustomEditor(typeof(RTObjectSelectionGizmos))]
    public class RTObjectSelectionGizmosInspector : Editor
    {
        private const int _generalTab = 0;
        private const int _moveGizmoTab = _generalTab + 1;
        private const int _rotationGizmoTab = _moveGizmoTab + 1;
        private const int _scaleGizmoTab = _rotationGizmoTab + 1;
        private const int _boxScaleGizmoTab = _scaleGizmoTab + 1;
        private const int _universalGizmoTab = _boxScaleGizmoTab + 1;
        private const int _extrudeGizmoTab = _universalGizmoTab + 1;

        private RTObjectSelectionGizmos _gizmos;

        public override void OnInspectorGUI()
        {
            _gizmos.MainToolbar.RenderEditorGUI(_gizmos);
        }

        private void OnEnable()
        {
            _gizmos = target as RTObjectSelectionGizmos;

            _gizmos.Hotkeys.FoldoutLabel = "Hotkeys";
            _gizmos.Hotkeys.UsesFoldout = true;

            var tab = _gizmos.MainToolbar.GetTabByIndex(_generalTab);
            tab.AddTargetSettings(_gizmos.Hotkeys);

            _gizmos.MoveGizmoSettings2D.FoldoutLabel = "2D Mode settings";
            _gizmos.MoveGizmoSettings2D.UsesFoldout = true;
            _gizmos.MoveGizmoSettings3D.FoldoutLabel = "3D Mode settings";
            _gizmos.MoveGizmoSettings3D.UsesFoldout = true;
            _gizmos.MoveGizmoLookAndFeel2D.FoldoutLabel = "2D Mode look & feel";
            _gizmos.MoveGizmoLookAndFeel2D.UsesFoldout = true;
            _gizmos.MoveGizmoLookAndFeel3D.FoldoutLabel = "3D Mode look & feel";
            _gizmos.MoveGizmoLookAndFeel3D.UsesFoldout = true;
            _gizmos.MoveGizmoHotkeys.FoldoutLabel = "Hotkeys";
            _gizmos.MoveGizmoHotkeys.UsesFoldout = true;
            _gizmos.ObjectMoveGizmoSettings.FoldoutLabel = "Object settings";
            _gizmos.ObjectMoveGizmoSettings.UsesFoldout = true;

            _gizmos.RotationGizmoSettings3D.FoldoutLabel = "Settings";
            _gizmos.RotationGizmoSettings3D.UsesFoldout = true;
            _gizmos.RotationGizmoLookAndFeel3D.FoldoutLabel = "Look & feel";
            _gizmos.RotationGizmoLookAndFeel3D.UsesFoldout = true;
            _gizmos.RotationGizmoHotkeys.FoldoutLabel = "Hotkeys";
            _gizmos.RotationGizmoHotkeys.UsesFoldout = true;
            _gizmos.ObjectRotationGizmoSettings.FoldoutLabel = "Object settings";
            _gizmos.ObjectRotationGizmoSettings.UsesFoldout = true;

            _gizmos.ScaleGizmoSettings3D.FoldoutLabel = "Settings";
            _gizmos.ScaleGizmoSettings3D.UsesFoldout = true;
            _gizmos.ScaleGizmoLookAndFeel3D.FoldoutLabel = "Look & feel";
            _gizmos.ScaleGizmoLookAndFeel3D.UsesFoldout = true;
            _gizmos.ScaleGizmoHotkeys.FoldoutLabel = "Hotkeys";
            _gizmos.ScaleGizmoHotkeys.UsesFoldout = true;
            _gizmos.ObjectScaleGizmoSettings.FoldoutLabel = "Object settings";
            _gizmos.ObjectScaleGizmoSettings.UsesFoldout = true;

            _gizmos.UniversalGizmoSettings2D.FoldoutLabel = "2D Mode settings";
            _gizmos.UniversalGizmoSettings2D.UsesFoldout = true;
            _gizmos.UniversalGizmoSettings3D.FoldoutLabel = "3D Mode settings";
            _gizmos.UniversalGizmoSettings3D.UsesFoldout = true;
            _gizmos.UniversalGizmoLookAndFeel2D.FoldoutLabel = "2D Mode look & feel";
            _gizmos.UniversalGizmoLookAndFeel2D.UsesFoldout = true;
            _gizmos.UniversalGizmoLookAndFeel3D.FoldoutLabel = "3D Mode look & feel";
            _gizmos.UniversalGizmoLookAndFeel3D.UsesFoldout = true;
            _gizmos.UniversalGizmoHotkeys.FoldoutLabel = "Hotkeys";
            _gizmos.UniversalGizmoHotkeys.UsesFoldout = true;
            _gizmos.ObjectUniversalGizmoSettings.FoldoutLabel = "Object settings";
            _gizmos.ObjectUniversalGizmoSettings.UsesFoldout = true;

            _gizmos.BoxScaleGizmoSettings3D.FoldoutLabel = "Settings";
            _gizmos.BoxScaleGizmoSettings3D.UsesFoldout = true;
            _gizmos.BoxScaleGizmoLookAndFeel3D.FoldoutLabel = "Look & feel";
            _gizmos.BoxScaleGizmoLookAndFeel3D.UsesFoldout = true;
            _gizmos.BoxScaleGizmoHotkeys.FoldoutLabel = "Hotkeys";
            _gizmos.BoxScaleGizmoHotkeys.UsesFoldout = true;

            _gizmos.ExtrudeGizmoLookAndFeel3D.FoldoutLabel = "Look & feel";
            _gizmos.ExtrudeGizmoLookAndFeel3D.UsesFoldout = true;
            _gizmos.ExtrudeGozmoHotkeys.FoldoutLabel = "Hotkeys";
            _gizmos.ExtrudeGozmoHotkeys.UsesFoldout = true;

            tab = _gizmos.MainToolbar.GetTabByIndex(_moveGizmoTab);
            tab.AddTargetSettings(_gizmos.ObjectMoveGizmoSettings);
            tab.AddTargetSettings(_gizmos.MoveGizmoSettings3D);
            tab.AddTargetSettings(_gizmos.MoveGizmoSettings2D);
            tab.AddTargetSettings(_gizmos.MoveGizmoLookAndFeel3D);
            tab.AddTargetSettings(_gizmos.MoveGizmoLookAndFeel2D);
            tab.AddTargetSettings(_gizmos.MoveGizmoHotkeys);

            tab = _gizmos.MainToolbar.GetTabByIndex(_rotationGizmoTab);
            tab.AddTargetSettings(_gizmos.ObjectRotationGizmoSettings);
            tab.AddTargetSettings(_gizmos.RotationGizmoSettings3D);
            tab.AddTargetSettings(_gizmos.RotationGizmoLookAndFeel3D);
            tab.AddTargetSettings(_gizmos.RotationGizmoHotkeys);

            tab = _gizmos.MainToolbar.GetTabByIndex(_scaleGizmoTab);
            tab.AddTargetSettings(_gizmos.ObjectScaleGizmoSettings);
            tab.AddTargetSettings(_gizmos.ScaleGizmoSettings3D);
            tab.AddTargetSettings(_gizmos.ScaleGizmoLookAndFeel3D);
            tab.AddTargetSettings(_gizmos.ScaleGizmoHotkeys);

            tab = _gizmos.MainToolbar.GetTabByIndex(_boxScaleGizmoTab);
            tab.AddTargetSettings(_gizmos.BoxScaleGizmoSettings3D);
            tab.AddTargetSettings(_gizmos.BoxScaleGizmoLookAndFeel3D);
            tab.AddTargetSettings(_gizmos.BoxScaleGizmoHotkeys);

            tab = _gizmos.MainToolbar.GetTabByIndex(_universalGizmoTab);
            tab.AddTargetSettings(_gizmos.UniversalGizmoConfig);
            tab.AddTargetSettings(_gizmos.ObjectUniversalGizmoSettings);
            tab.AddTargetSettings(_gizmos.UniversalGizmoSettings2D);
            tab.AddTargetSettings(_gizmos.UniversalGizmoSettings3D);
            tab.AddTargetSettings(_gizmos.UniversalGizmoLookAndFeel2D);
            tab.AddTargetSettings(_gizmos.UniversalGizmoLookAndFeel3D);
            tab.AddTargetSettings(_gizmos.UniversalGizmoHotkeys);

            tab = _gizmos.MainToolbar.GetTabByIndex(_extrudeGizmoTab);
            tab.AddTargetSettings(_gizmos.ExtrudeGizmoLookAndFeel3D);
            tab.AddTargetSettings(_gizmos.ExtrudeGozmoHotkeys);
        }
    }
}
