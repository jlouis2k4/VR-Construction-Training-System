using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;

namespace RLD
{
    [Serializable]
    public class ObjectSelectionSettings : Settings
    {
        [SerializeField]
        private MultiSelectOverlapMode _multiSelectOverlapMode = MultiSelectOverlapMode.Partial;
        [SerializeField]
        private GameObjectType _selectableObjectTypes = GameObjectType.Mesh | GameObjectType.Sprite | GameObjectType.Light | GameObjectType.ParticleSystem;
        [SerializeField]
        private int _selectableLayers = ~0;
        [SerializeField]
        private int _duplicatableLayers = ~0;
        [SerializeField]
        private int _deletableLayers = ~0;

        private HashSet<GameObject> _nonSelectableObjects = new HashSet<GameObject>();
        private HashSet<Camera> _nonSelectableCameras = new HashSet<Camera>();

        [SerializeField]
        private bool _canClickSelect = true;
        [SerializeField]
        private bool _enableCyclicalClickSelect = false;
        [SerializeField]
        private bool _canMultiSelect = true;

        [SerializeField]
        private int _minMultiSelectSize = 3;

        public MultiSelectOverlapMode MultiSelectOverlapMode { get { return _multiSelectOverlapMode; } set { _multiSelectOverlapMode = value; } }
        public bool CanClickSelect { get { return _canClickSelect; } set { _canClickSelect = value; } }
        public bool EnableCyclicalClickSelect { get { return _enableCyclicalClickSelect; } set { _enableCyclicalClickSelect = value; } }
        public bool CanMultiSelect { get { return _canMultiSelect; } set { _canMultiSelect = value; } }
        public int SelectableLayers { get { return _selectableLayers; } set { _selectableLayers = value; } }
        public int DuplicatableLayers { get { return _duplicatableLayers; } set { _duplicatableLayers = value; } }
        public int DeletableLayers { get { return _deletableLayers; } set { _deletableLayers = value; } }
        public int MinMultiSelectSize { get { return _minMultiSelectSize; } set { _minMultiSelectSize = Mathf.Max(1, value); } }

        public bool IsCameraSelectable(Camera camera)
        {
            return !_nonSelectableCameras.Contains(camera);
        }

        public void SetCameraSelectable(Camera camera, bool isSelectable)
        {
            if (camera == null) return;

            if (isSelectable) _nonSelectableCameras.Remove(camera);
            else _nonSelectableCameras.Add(camera);
        }

        public void SetCameraCollectionSelectable(List<Camera> cameraCollection, bool areSelectable)
        {
            foreach (var camera in cameraCollection)
            {
                SetCameraSelectable(camera, areSelectable);
            }
        }

        public bool IsObjectTypeSelectable(GameObjectType gameObjectType)
        {
            return (_selectableObjectTypes & gameObjectType) != 0;
        }

        public void SetObjectTypeSelectable(GameObjectType gameObjectType, bool isSelectable)
        {
            _selectableObjectTypes |= gameObjectType;
        }

        public bool IsObjectLayerSelectable(int objectLayer)
        {
            return LayerEx.IsLayerBitSet(_selectableLayers, objectLayer);
        }

        public void SetObjectLayerSelectable(int objectLayer, bool isSelectable)
        {
            if (isSelectable) _selectableLayers = LayerEx.SetLayerBit(_selectableLayers, objectLayer);
            else _selectableLayers = LayerEx.ClearLayerBit(_selectableLayers, objectLayer);
        }

        public bool IsObjectSelectable(GameObject gameObject)
        {
            if (gameObject == null) return false;
            return !_nonSelectableObjects.Contains(gameObject);
        }

        public void SetObjectSelectable(GameObject gameObject, bool isSelectable)
        {
            if (gameObject == null) return;

            if (isSelectable) _nonSelectableObjects.Remove(gameObject);
            else _nonSelectableObjects.Add(gameObject);
        }

        public void SetObjectCollectionSelectable(List<GameObject> gameObjectCollection, bool areSelectable)
        {
            foreach(var gameObject in gameObjectCollection)
            {
                SetObjectSelectable(gameObject, areSelectable);
            }
        }

        public void RemoveNullObjectRefs()
        {
            _nonSelectableObjects.RemoveWhere(item => item == null);
        }

        public bool IsObjectLayerDuplicatable(int objectLayer)
        {
            return LayerEx.IsLayerBitSet(_duplicatableLayers, objectLayer);
        }

        public void SetObjectLayerDuplicatable(int objectLayer, bool isDuplicatable)
        {
            if (isDuplicatable) _duplicatableLayers = LayerEx.SetLayerBit(_duplicatableLayers, objectLayer);
            else _duplicatableLayers = LayerEx.ClearLayerBit(_duplicatableLayers, objectLayer);
        }

        public bool IsObjectLayerDeletable(int objectLayer)
        {
            return LayerEx.IsLayerBitSet(_deletableLayers, objectLayer);
        }

        #if UNITY_EDITOR
        protected override void RenderContent(UnityEngine.Object undoRecordObject)
        {
            bool newBool; int newInt;
            MultiSelectOverlapMode newMultiSelectOverlapMode;

            // Can click select
            var content = new GUIContent();
            content.text = "Can click select";
            content.tooltip = "Allows you to specify if objects can be selected via mouse clicks.";
            newBool = EditorGUILayout.ToggleLeft(content, CanClickSelect);
            if(newBool != CanClickSelect)
            {
                EditorUndoEx.Record(undoRecordObject);
                CanClickSelect = newBool;
            }

            // Can cyclical click select
            content.text = "Enable cyclical click select";
            content.tooltip = "If this is checked, each successive click will select an object which lies behind the previously selected one. Useful especially when dealing with overlapping sprites.";
            newBool = EditorGUILayout.ToggleLeft(content, EnableCyclicalClickSelect);
            if (newBool != EnableCyclicalClickSelect)
            {
                EditorUndoEx.Record(undoRecordObject);
                EnableCyclicalClickSelect = newBool;
            }

            // Can multi select
            content.text = "Can multi-select";
            content.tooltip = "Allows you to specify if objects can be selected using the multi-select shape.";
            newBool = EditorGUILayout.ToggleLeft(content, CanMultiSelect);
            if (newBool != CanMultiSelect)
            {
                EditorUndoEx.Record(undoRecordObject);
                CanMultiSelect = newBool;
            }

            content.text = "Multi-select overlap mode";
            content.tooltip = "Allows you to specify the way in which objects are selected via the multi-select shape.";
            newMultiSelectOverlapMode = (MultiSelectOverlapMode)EditorGUILayout.EnumPopup(content, MultiSelectOverlapMode);
            if (newMultiSelectOverlapMode != MultiSelectOverlapMode)
            {
                EditorUndoEx.Record(undoRecordObject);
                MultiSelectOverlapMode = newMultiSelectOverlapMode;
            }

            // Selectable object types
            content.text = "Selectable object types";
            content.tooltip = "Allows you to specify which object types can be selected.";
            newInt = (int)((GameObjectType)EditorGUILayout.EnumMaskPopup(content, (GameObjectType)_selectableObjectTypes));
            if (newInt != (int)_selectableObjectTypes)
            {
                EditorUndoEx.Record(undoRecordObject);
                _selectableObjectTypes = (GameObjectType)newInt;
            }

            // Selectable layers
            content.text = "Selectable layers";
            content.tooltip = "Allows you to specify which layers can be selected.";
            newInt = EditorGUILayoutEx.LayerMaskField(content, _selectableLayers);
            if(newInt != _selectableLayers)
            {
                EditorUndoEx.Record(undoRecordObject);
                _selectableLayers = newInt;
            }

            // Duplicatable layers
            content.text = "Duplicatable layers";
            content.tooltip = "Allows you to specify which layers can be duplicated.";
            newInt = EditorGUILayoutEx.LayerMaskField(content, _duplicatableLayers);
            if (newInt != _duplicatableLayers)
            {
                EditorUndoEx.Record(undoRecordObject);
                _duplicatableLayers = newInt;
            }

            // Deletable layers
            content.text = "Deletable layers";
            content.tooltip = "Allows you to specify which layers can be deleted.";
            newInt = EditorGUILayoutEx.LayerMaskField(content, _deletableLayers);
            if (newInt != _deletableLayers)
            {
                EditorUndoEx.Record(undoRecordObject);
                _deletableLayers = newInt;
            }

            // Multi-select shape
            content.text = "Min multi-select size";
            content.tooltip = "The minimum size (width and height) the multi-select shape must have to be able to overlap objects. The value is expressed in pixels.";
            newInt = EditorGUILayout.IntField(content, MinMultiSelectSize);
            if (newInt != MinMultiSelectSize)
            {
                EditorUndoEx.Record(undoRecordObject);
                MinMultiSelectSize = newInt;
            }
        }
        #endif
    }
}
