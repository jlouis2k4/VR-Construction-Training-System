using UnityEngine;
using UnityEditor;

namespace RLD
{
    [CustomEditor(typeof(RTObjectSelection))]
    public class RTSelectionInspector : Editor
    {
        private RTObjectSelection _objectSelection;

        public override void OnInspectorGUI()
        {
            _objectSelection.SettingsToolbar.RenderEditorGUI(_objectSelection);
        }

        private void OnEnable()
        {
            _objectSelection = target as RTObjectSelection;
            EditorToolbar settingsToolbar = _objectSelection.SettingsToolbar;

            _objectSelection.Settings.UsesFoldout = true;
            _objectSelection.Settings.FoldoutLabel = "Settings";

            _objectSelection.LookAndFeel.UsesFoldout = true;
            _objectSelection.LookAndFeel.FoldoutLabel = "Look & feel";

            _objectSelection.Hotkeys.UsesFoldout = true;
            _objectSelection.Hotkeys.FoldoutLabel = "Hotkeys";

            _objectSelection.GrabSettings.UsesFoldout = true;
            _objectSelection.GrabSettings.FoldoutLabel = "Settings";

            _objectSelection.GrabHotkeys.UsesFoldout = true;
            _objectSelection.GrabHotkeys.FoldoutLabel = "Hotkeys";

            _objectSelection.GrabLookAndFeel.UsesFoldout = true;
            _objectSelection.GrabLookAndFeel.FoldoutLabel = "Look & feel";

            _objectSelection.GridSnapLookAndFeel.UsesFoldout = true;
            _objectSelection.GridSnapLookAndFeel.FoldoutLabel = "Look & feel";

            _objectSelection.GridSnapHotkeys.UsesFoldout = true;
            _objectSelection.GridSnapHotkeys.FoldoutLabel = "Hotkeys";

            _objectSelection.RotationSettings.UsesFoldout = true;
            _objectSelection.RotationSettings.FoldoutLabel = "Settings";

            _objectSelection.RotationHotkeys.UsesFoldout = true;
            _objectSelection.RotationHotkeys.FoldoutLabel = "Hotkeys";

            _objectSelection.Object2ObjectSnapSettings.UsesFoldout = true;
            _objectSelection.Object2ObjectSnapSettings.FoldoutLabel = "Settings";

            _objectSelection.Object2ObjectSnapHotkeys.UsesFoldout = true;
            _objectSelection.Object2ObjectSnapHotkeys.FoldoutLabel = "Hotkeys";

            settingsToolbar.GetTabByIndex(0).AddTargetSettings(_objectSelection.Settings);
            settingsToolbar.GetTabByIndex(0).AddTargetSettings(_objectSelection.LookAndFeel);
            settingsToolbar.GetTabByIndex(0).AddTargetSettings(_objectSelection.Hotkeys);
            settingsToolbar.GetTabByIndex(1).AddTargetSettings(_objectSelection.GrabSettings);
            settingsToolbar.GetTabByIndex(1).AddTargetSettings(_objectSelection.GrabLookAndFeel);
            settingsToolbar.GetTabByIndex(1).AddTargetSettings(_objectSelection.GrabHotkeys);
            settingsToolbar.GetTabByIndex(2).AddTargetSettings(_objectSelection.Object2ObjectSnapSettings);
            settingsToolbar.GetTabByIndex(2).AddTargetSettings(_objectSelection.Object2ObjectSnapHotkeys);
            settingsToolbar.GetTabByIndex(3).AddTargetSettings(_objectSelection.GridSnapLookAndFeel);
            settingsToolbar.GetTabByIndex(3).AddTargetSettings(_objectSelection.GridSnapHotkeys);
            settingsToolbar.GetTabByIndex(4).AddTargetSettings(_objectSelection.RotationSettings);
            settingsToolbar.GetTabByIndex(4).AddTargetSettings(_objectSelection.RotationHotkeys);
        }
    }
}