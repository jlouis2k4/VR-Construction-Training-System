using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    [Serializable]
    public class UniversalGizmoConfig : Settings
    {
        [SerializeField]
        private UniversalGizmoSettingsCategory _inheritCategory = UniversalGizmoSettingsCategory.Move;
        [SerializeField]
        private UniversalGizmoSettingsType _inheritType = UniversalGizmoSettingsType.LookAndFeel3D;
        [SerializeField]
        private UniversalGizmoSettingsCategory _displayCategory = UniversalGizmoSettingsCategory.Move;

        public UniversalGizmoSettingsCategory InheritCategory { get { return _inheritCategory; } set { _inheritCategory = value; } }
        public UniversalGizmoSettingsType InheritType { get { return _inheritType; } set { _inheritType = value; } }
        public UniversalGizmoSettingsCategory DisplayCategory { get { return _displayCategory; } set { _displayCategory = value; } }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            RTObjectSelectionGizmos gizmos = RTObjectSelectionGizmos.Get;
            var content = new GUIContent();

            EditorGUILayout.BeginHorizontal();
            content.text = "Inherit";
            content.tooltip = "Inherit different category of settings from the other gizmos (move, rotate or scale).";
            if (GUILayout.Button(content))
            {
                EditorUndoEx.Record(undoRecordObject);
                if (InheritCategory == UniversalGizmoSettingsCategory.Move)
                {
                    if (InheritType == UniversalGizmoSettingsType.Settings2D)
                    {
                        gizmos.UniversalGizmoSettings2D.Inherit(gizmos.MoveGizmoSettings2D);
                    }
                    else
                    if (InheritType == UniversalGizmoSettingsType.Settings3D)
                    {
                        gizmos.UniversalGizmoSettings3D.Inherit(gizmos.MoveGizmoSettings3D);
                    }
                    else
                    if (InheritType == UniversalGizmoSettingsType.LookAndFeel2D)
                    {
                        gizmos.UniversalGizmoLookAndFeel2D.Inherit(gizmos.MoveGizmoLookAndFeel2D);
                    }
                    else
                    if (InheritType == UniversalGizmoSettingsType.LookAndFeel3D)
                    {
                        gizmos.UniversalGizmoLookAndFeel3D.Inherit(gizmos.MoveGizmoLookAndFeel3D);
                    }
                }
                else
                if (InheritCategory == UniversalGizmoSettingsCategory.Rotate)
                {
                    if (InheritType == UniversalGizmoSettingsType.Settings3D)
                    {
                        gizmos.UniversalGizmoSettings3D.Inherit(gizmos.RotationGizmoSettings3D);
                    }
                    else
                    if (InheritType == UniversalGizmoSettingsType.LookAndFeel3D)
                    {
                        gizmos.UniversalGizmoLookAndFeel3D.Inherit(gizmos.RotationGizmoLookAndFeel3D);
                    }
                }
                else
                if (InheritCategory == UniversalGizmoSettingsCategory.Scale)
                {
                    if (InheritType == UniversalGizmoSettingsType.Settings3D)
                    {
                        gizmos.UniversalGizmoSettings3D.Inherit(gizmos.ScaleGizmoSettings3D);
                    }
                    else
                    if (InheritType == UniversalGizmoSettingsType.LookAndFeel3D)
                    {
                        gizmos.UniversalGizmoLookAndFeel3D.Inherit(gizmos.ScaleGizmoLookAndFeel3D);
                    }
                }
            }

            UniversalGizmoSettingsCategory newCategory;
            UniversalGizmoSettingsType newInheritType;

            newCategory = (UniversalGizmoSettingsCategory)EditorGUILayout.EnumPopup(InheritCategory);
            if (newCategory != InheritCategory)
            {
                EditorUndoEx.Record(undoRecordObject);
                InheritCategory = newCategory;
            }
            newInheritType = (UniversalGizmoSettingsType)EditorGUILayout.EnumPopup(InheritType);
            if (newInheritType != InheritType)
            {
                EditorUndoEx.Record(undoRecordObject);
                InheritType = newInheritType;
            }
            EditorGUILayout.EndHorizontal();

            content.text = "Display category";
            content.tooltip = "Specifies what category of settings are currently displayed for modification.";
            newCategory = (UniversalGizmoSettingsCategory)EditorGUILayout.EnumPopup(content, DisplayCategory);
            if (newCategory != DisplayCategory)
            {
                EditorUndoEx.Record(undoRecordObject);
                DisplayCategory = newCategory;
            }

            gizmos.UniversalGizmoSettings2D.DisplayCategory = DisplayCategory;
            gizmos.UniversalGizmoSettings3D.DisplayCategory = DisplayCategory;
            gizmos.UniversalGizmoLookAndFeel2D.DisplayCategory = DisplayCategory;
            gizmos.UniversalGizmoLookAndFeel3D.DisplayCategory = DisplayCategory;

            gizmos.UniversalGizmoSettings2D.CanBeDisplayed = true;
            gizmos.UniversalGizmoLookAndFeel2D.CanBeDisplayed = true;
            gizmos.UniversalGizmoSettings3D.CanBeDisplayed = true;
            gizmos.UniversalGizmoLookAndFeel3D.CanBeDisplayed = true;

            if(DisplayCategory != UniversalGizmoSettingsCategory.Move)
            {
                gizmos.UniversalGizmoSettings2D.CanBeDisplayed = false;
                gizmos.UniversalGizmoLookAndFeel2D.CanBeDisplayed = false;
            }
        }
        #endif
    }
}
