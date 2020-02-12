using UnityEngine;
using System;
using System.Collections.Generic;

namespace RLD
{
    public delegate void ObjectSelectionManipSessionBeginHandler(ObjectSelectionManipSession session);
    public delegate void ObjectSelectionManipSessionEndHandler(ObjectSelectionManipSession session);
    public delegate void ObjectSelectionCanClickSelectDeselectHandler(YesNoAnswer answer);
    public delegate void ObjectSelectionCanMultiSelectDeselectHandler(YesNoAnswer answer);
    public delegate void ObjectSelectionChangedHandler(ObjectSelectionChangedEventArgs args);
    public delegate void ObjectSelectionWillBeDeletedHandler();
    public delegate void ObjectSelectionDeletedHandler();
    public delegate void ObjectSelectionWillBeDuplicatedHandler();
    public delegate void ObjectSelectionDuplicatedHandler(ObjectSelectionDuplicationResult result);
    public delegate void ObjectSelectionRotatedHandler();
    public delegate void ObjectSelectionPreSelectCustomizeHandler(ObjectPreSelectCustomizeInfo customizeInfo, List<GameObject> toBeSelected);
    public delegate void ObjectSelectionPreDeselectCustomizeHandler(ObjectPreDeselectCustomizeInfo customizeInfo, List<GameObject> toBeDeselected);
    public delegate void ObjectSelectionEnabled();
    public delegate void ObjectSelectionDisabled();

    public enum ObjectSelectionManipSession
    {
        None = 0,
        Grab,
        GridSnap,
        Object2ObjectSnap
    }

    [Serializable]
    public class RTObjectSelection : MonoSingleton<RTObjectSelection>
    {
        private static readonly int _objectPickDeviceBtnIndex = 0;

        public event ObjectSelectionManipSessionBeginHandler ManipSessionBegin;
        public event ObjectSelectionManipSessionEndHandler ManipSessionEnd;
        public event ObjectSelectionCanClickSelectDeselectHandler CanClickSelectDeselect;
        public event ObjectSelectionCanMultiSelectDeselectHandler CanMultiSelectDeselect;
        public event ObjectSelectionChangedHandler Changed;
        public event ObjectSelectionWillBeDeletedHandler WillBeDeleted;
        public event ObjectSelectionDeletedHandler Deleted;
        public event ObjectSelectionWillBeDuplicatedHandler WillBeDuplicated;
        public event ObjectSelectionDuplicatedHandler Duplicated;
        public event ObjectSelectionRotatedHandler Rotated;
        public event ObjectSelectionPreSelectCustomizeHandler PreSelectCustomize;
        public event ObjectSelectionPreDeselectCustomizeHandler PreDeselectCustomize;
        public event ObjectSelectionEnabled Enabled;
        public event ObjectSelectionDisabled Disabled;

        [Flags]
        private enum SelectRestrictFlags
        {
            None = 0,
            ObjectLayer = 1,
            ObjectType = 2,
            Object = 4,
            SelectionListener = 8,
            All = ObjectLayer | ObjectType | Object | SelectionListener
        }

        private struct CyclicalClickSelectInfo
        {
            public int LastSelectedIndex;
            public GameObject LastPickedObject;
        }

        [SerializeField]
        private bool _isEnabled = true;

        private List<Camera> _renderIgnoreCameras = new List<Camera>();
        private List<GameObject> _selectedObjects = new List<GameObject>();

        private MultiSelectShape _multiSelectShape = new MultiSelectShape();
        private ObjectSelectionSnapshot _multiSelectPreChangeSnapshot = new ObjectSelectionSnapshot();

        private bool _wasSelectionChangedViaMultiSelectShape;
        private bool _willBeDeleted;
        private bool _doingPreSelectCustomize;
        private bool _doingPreDeselectCustomize;
        private bool _firingSelectionChanged;

        private ObjectSelectionManipSession _activeManipSession = ObjectSelectionManipSession.None;
        private CyclicalClickSelectInfo _cyclicalClickSelectInfo = new CyclicalClickSelectInfo() { LastSelectedIndex = -1 };

        [SerializeField]
        private ObjectSelectionHotkeys _hotkeys = new ObjectSelectionHotkeys();
        [SerializeField]
        private ObjectSelectionSettings _settings = new ObjectSelectionSettings();
        [SerializeField]
        private ObjectSelectionLookAndFeel _lookAndFeel = new ObjectSelectionLookAndFeel();

        [SerializeField]
        private ObjectSelectionRotationSettings _rotationSettings = new ObjectSelectionRotationSettings();
        [SerializeField]
        private ObjectSelectionRotationHotkeys _rotationHotkeys = new ObjectSelectionRotationHotkeys();

        private DeviceObjectGrabSession _grabSession = new DeviceObjectGrabSession();
        [SerializeField]
        private ObjectGrabSettings _grabSettings = new ObjectGrabSettings();
        [SerializeField]
        private ObjectGrabLookAndFeel _grabLookAndFeel = new ObjectGrabLookAndFeel();
        [SerializeField]
        private ObjectGrabHotkeys _grabHotkeys = new ObjectGrabHotkeys();

        private ObjectGridSnapSession _gridSnapSession = new ObjectGridSnapSession();
        [SerializeField]
        private ObjectGridSnapLookAndFeel _gridSnapLookAndFeel = new ObjectGridSnapLookAndFeel();
        [SerializeField]
        private ObjectGridSnapHotkeys _gridSnapHotkeys = new ObjectGridSnapHotkeys();

        private Object2ObjectSnapSession _object2ObjectSnapSession = new Object2ObjectSnapSession();
        [SerializeField]
        private Object2ObjectSnapSettings _object2ObjectSnapSettings = new Object2ObjectSnapSettings();
        [SerializeField]
        private Object2ObjectSnapHotkeys _object2ObjectSnapHotkeys = new Object2ObjectSnapHotkeys();

        [SerializeField]
        private EditorToolbar _settingsToolbar = new EditorToolbar
        (
            new EditorToolbarTab[]
            {
                new EditorToolbarTab("General", "General selection settings."),
                new EditorToolbarTab("Selection grab", "Selection grab settings."),
                new EditorToolbarTab("Object2Object snap", "Selection object-to-object snap settings."),
                new EditorToolbarTab("Grid snap", "Selection grid snap settings."),
                new EditorToolbarTab("Rotation", "Selection rotation settings.")
            },
            5, Color.green
        );

        public bool IsEnabled { get { return _isEnabled; } }
        public bool IsMultiSelectShapeVisible { get { return _multiSelectShape.IsVisible; } }
        public int NumSelectedObjects { get { return _selectedObjects.Count; } }
        public ObjectSelectionHotkeys Hotkeys { get { return _hotkeys; } }
        public ObjectSelectionSettings Settings { get { return _settings; } }
        public ObjectSelectionLookAndFeel LookAndFeel { get { return _lookAndFeel; } }
        public ObjectSelectionRotationSettings RotationSettings { get { return _rotationSettings; } }
        public ObjectSelectionRotationHotkeys RotationHotkeys { get { return _rotationHotkeys; } }
        public ObjectGrabSettings GrabSettings { get { return _grabSettings; } }
        public ObjectGrabHotkeys GrabHotkeys { get { return _grabHotkeys; } }
        public ObjectGrabLookAndFeel GrabLookAndFeel { get { return _grabLookAndFeel; } }
        public ObjectGridSnapLookAndFeel GridSnapLookAndFeel { get { return _gridSnapLookAndFeel; } }
        public ObjectGridSnapHotkeys GridSnapHotkeys { get { return _gridSnapHotkeys; } }
        public Object2ObjectSnapSettings Object2ObjectSnapSettings { get { return _object2ObjectSnapSettings; } }
        public Object2ObjectSnapHotkeys Object2ObjectSnapHotkeys { get { return _object2ObjectSnapHotkeys; } }
        public bool IsManipSessionActive { get { return _activeManipSession != ObjectSelectionManipSession.None; } }
        public ObjectSelectionManipSession ActiveManipSession { get { return _activeManipSession; } }
        public bool IsGrabSessionActive { get { return ActiveManipSession == ObjectSelectionManipSession.Grab; } }
        public bool IsGridSnapSessionActive { get { return ActiveManipSession == ObjectSelectionManipSession.GridSnap; } }
        public bool IsObject2ObjectSnapSessionActive { get { return ActiveManipSession == ObjectSelectionManipSession.Object2ObjectSnap; } }
        public List<GameObject> SelectedObjects { get { return new List<GameObject>(_selectedObjects); } }

        #if UNITY_EDITOR
        public EditorToolbar SettingsToolbar { get { return _settingsToolbar; } }
        #endif

        public void Initialize_SystemCall()
        {
            _grabSession.SharedSettings = GrabSettings;
            _grabSession.SharedHotkeys = GrabHotkeys;
            _grabSession.SharedLookAndFeel = GrabLookAndFeel;
            _grabSession.SessionBegin += OnGrabSessionBegin;
            _grabSession.SessionEnd += OnGrabSessionEnd;

            _gridSnapSession.SharedHotkeys = GridSnapHotkeys;
            _gridSnapSession.SharedLookAndFeel = GridSnapLookAndFeel;
            _gridSnapSession.SessionBegin += OnGridSnapSessionBegin;
            _gridSnapSession.SessionEnd += OnGridSnapSessionEnd;

            _object2ObjectSnapSession.SharedSettings = Object2ObjectSnapSettings;
            _object2ObjectSnapSession.SharedHotkeys = Object2ObjectSnapHotkeys;
            _object2ObjectSnapSession.SessionBegin += OnObject2ObjectSnapSessionBegin;
            _object2ObjectSnapSession.SessionEnd += OnObject2ObjectSnapSessionEnd;

            RTUndoRedo.Get.UndoEnd += OnUndoEnd;
            RTUndoRedo.Get.RedoEnd += OnRedoEnd;
        }

        public void AttachGizmoController(IObjectCollectionGizmoController gizmoController)
        {
            gizmoController.SetTargetObjectCollection(_selectedObjects);
        }

        public bool IsRenderIgnoreCamera(Camera camera)
        {
            return _renderIgnoreCameras.Contains(camera);
        }

        public void AddRenderIgnoreCamera(Camera camera)
        {
            if (!IsRenderIgnoreCamera(camera)) _renderIgnoreCameras.Add(camera);
        }

        public void RemoveRenderIgnoreCamera(Camera camera)
        {
            _renderIgnoreCameras.Remove(camera);
        }

        public void SetEnabled(bool isEnabled)
        {
            if (isEnabled == _isEnabled) return;

            _isEnabled = isEnabled;
            if (!_isEnabled)
            {
                _gridSnapSession.End();
                _grabSession.End();
                _object2ObjectSnapSession.End();

                if (Disabled != null) Disabled();
            }
            else
            {
                if (Enabled != null) Enabled();
            }
        }

        public void SetRotation(Quaternion rotation)
        {
            var preChangeSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_selectedObjects);

            var parents = GameObjectEx.FilterParentsOnly(_selectedObjects);
            foreach (var parent in parents)
            {
                parent.transform.rotation = rotation;
            }

            var postChangeSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_selectedObjects);
            var postTransformAction = new PostObjectTransformsChangedAction(preChangeSnapshots, postChangeSnapshots);
            postTransformAction.Execute();

            if (Rotated != null) Rotated();
        }

        public void Rotate(Axis axis, float rotationAngle, ObjectRotationPivot rotationPivot)
        {
            var preChangeSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_selectedObjects);

            Vector3 rotationAxis = Vector3.right;
            if (axis == Axis.Y) rotationAxis = Vector3.up;
            else if (axis == Axis.Z) rotationAxis = Vector3.forward;

            Quaternion rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);
            if (rotationPivot == ObjectRotationPivot.GroupCenter)
            {
                AABB aabb = GetWorldAABB();
                var parents = GameObjectEx.FilterParentsOnly(_selectedObjects);

                foreach(var parent in parents)
                {
                    parent.transform.RotateAroundPivot(rotation, aabb.Center);
                }
            }
            else
            if (rotationPivot == ObjectRotationPivot.IndividualCenter)
            {
                var boundsQConfig = new ObjectBounds.QueryConfig();
                boundsQConfig.NoVolumeSize = Vector3Ex.FromValue(1e-2f);
                boundsQConfig.ObjectTypes = GameObjectTypeHelper.AllCombined;

                var parents = GameObjectEx.FilterParentsOnly(_selectedObjects);
                foreach (var parent in parents)
                {
                    OBB hierarchyOBB = ObjectBounds.CalcHierarchyWorldOBB(parent, boundsQConfig);
                    parent.transform.RotateAroundPivot(rotation, hierarchyOBB.Center);
                }
            }
            else
            if(rotationPivot == ObjectRotationPivot.IndividualPivot)
            {
                var parents = GameObjectEx.FilterParentsOnly(_selectedObjects);
                foreach (var parent in parents)
                {
                    parent.transform.rotation = rotation * parent.transform.rotation;
                }
            }

            var postChangeSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_selectedObjects);
            var postTransformAction = new PostObjectTransformsChangedAction(preChangeSnapshots, postChangeSnapshots);
            postTransformAction.Execute();

            if (Rotated != null) Rotated();
        }

        public void AppendObjects(List<GameObject> gameObjects, bool allowUndoRedo)
        {
            if (!CanBeModifiedByAPI()) return;
            if (gameObjects == null || gameObjects.Count == 0) return;

            // Take a pre change snapshot if necessary
            var preChangeSnapshot = new ObjectSelectionSnapshot();
            if (allowUndoRedo) preChangeSnapshot.Snapshot();

            // We will need these later to fire a selection changed event
            bool selectionWasChanged = false;
            ObjectSelectReason selectReason = ObjectSelectReason.AppendToSelectionCall;
            List<GameObject> objectsWhichWereSelected = new List<GameObject>();

            // Loop through each object and select it (if possible)           
            foreach(var gameObject in gameObjects)
            {
                // Select the object if it can be selected
                if (!IsObjectSelected(gameObject) && CanSelectObject(gameObject, SelectRestrictFlags.None, selectReason))
                {
                    SelectObject(gameObject, selectReason);
                    objectsWhichWereSelected.Add(gameObject);
                    selectionWasChanged = true;
                }
            }

            // Was the selection changed?
            if(selectionWasChanged)
            {
                // Fire a selection changed event
                OnSelectionChanged(new ObjectSelectionChangedEventArgs(selectReason, objectsWhichWereSelected, ObjectDeselectReason.None, null));

                // Execute a post selection changed action if Undo/Redo is allowed
                if(allowUndoRedo)
                {
                    var postChangeSnapshot = new ObjectSelectionSnapshot();
                    postChangeSnapshot.Snapshot();
                    var postSelectionChangedAction = new PostObjectSelectionChangedAction(preChangeSnapshot, postChangeSnapshot);
                    postSelectionChangedAction.Execute();
                }
            }
        }

        public void RemoveObjects(List<GameObject> gameObjects, bool allowUndoRedo)
        {
            if (!CanBeModifiedByAPI()) return;
            if (gameObjects == null || gameObjects.Count == 0) return;

            // Take a pre change snapshot if necessary
            var preChangeSnapshot = new ObjectSelectionSnapshot();
            if (allowUndoRedo) preChangeSnapshot.Snapshot();

            // We will need these later to fire a selection changed event
            bool selectionWasChanged = false;
            ObjectDeselectReason deselectReason = ObjectDeselectReason.RemoveFromSelectionCall;
            List<GameObject> objectsWhichWereDeselected = new List<GameObject>();

            // Loop through each object and deselect it       
            foreach (var gameObject in gameObjects)
            {
                // Deselect the object if it is selected
                if (IsObjectSelected(gameObject))
                {
                    DeselectObject(gameObject, deselectReason);
                    objectsWhichWereDeselected.Add(gameObject);
                    selectionWasChanged = true;
                }
            }

            // Was the selection changed?
            if (selectionWasChanged)
            {
                // Fire a selection changed event
                OnSelectionChanged(new ObjectSelectionChangedEventArgs(ObjectSelectReason.None, null, deselectReason, objectsWhichWereDeselected));

                // Execute a post selection changed action if Undo/Redo is allowed
                if (allowUndoRedo)
                {
                    var postChangeSnapshot = new ObjectSelectionSnapshot();
                    postChangeSnapshot.Snapshot();
                    var postSelectionChangedAction = new PostObjectSelectionChangedAction(preChangeSnapshot, postChangeSnapshot);
                    postSelectionChangedAction.Execute();
                }
            }
        }

        public void SetSelectedObjects(List<GameObject> gameObjects, bool allowUndoRedo)
        {
            if (!CanBeModifiedByAPI()) return;
            if (gameObjects == null) gameObjects = new List<GameObject>();

            // Ignore this call if the specified object collection matches the current selection
            if (IsSelectionExactMatch(gameObjects)) return;

            // Take a pre change snapshot if necessary
            var preChangeSnapshot = new ObjectSelectionSnapshot();
            if (allowUndoRedo) preChangeSnapshot.Snapshot();

            // We will need these later to fire a selection changed event
            bool selectionWasChanged = false;
            ObjectSelectReason selectReason = ObjectSelectReason.None;
            ObjectDeselectReason deselectReason = ObjectDeselectReason.None;
            List<GameObject> objectsWhichWereSelected = new List<GameObject>();
            List<GameObject> objectsWhichWereDeselected = new List<GameObject>();

            // First we need to deselect the objects which can not be found inside 'gameObjects'
            List<GameObject> selectedObjects = SelectedObjects;
            foreach(var selectedObject in selectedObjects)
            {
                // If the object does not reside in the specified list, we must deselect it
                if(!gameObjects.Contains(selectedObject))
                {
                    DeselectObject(selectedObject, deselectReason);
                    objectsWhichWereDeselected.Add(selectedObject);
                    selectionWasChanged = true;
                    deselectReason = ObjectDeselectReason.SetSelectedCall;
                }
            }

            // Now we need to select the objects inside 'gameObjects' which are not currently selected
            foreach(var objectToSelect in gameObjects)
            {
                // Check if the game object can be selected
                if (!IsObjectSelected(objectToSelect) && CanSelectObject(objectToSelect, SelectRestrictFlags.None, selectReason))
                {
                    SelectObject(objectToSelect, selectReason);
                    objectsWhichWereSelected.Add(objectToSelect);
                    selectionWasChanged = true;
                    selectReason = ObjectSelectReason.SetSelectedCall;
                }
            }

            // Was the selection changed?
            if (selectionWasChanged)
            {
                // Fire a selection changed event
                OnSelectionChanged(new ObjectSelectionChangedEventArgs(selectReason, objectsWhichWereSelected, deselectReason, objectsWhichWereDeselected));

                // Execute a post selection changed action if Undo/Redo is allowed
                if (allowUndoRedo)
                {
                    var postChangeSnapshot = new ObjectSelectionSnapshot();
                    postChangeSnapshot.Snapshot();
                    var postSelectionChangedAction = new PostObjectSelectionChangedAction(preChangeSnapshot, postChangeSnapshot);
                    postSelectionChangedAction.Execute();
                }
            }
        }

        public void ClearSelection(bool allowUndoRedo)
        {
            if (!CanBeModifiedByAPI()) return;
            if (NumSelectedObjects == 0) return;

            // Take a pre change snapshot if necessary
            var preChangeSnapshot = new ObjectSelectionSnapshot();
            if (allowUndoRedo) preChangeSnapshot.Snapshot();

            // We will need these later to fire a selection changed event
            ObjectDeselectReason deselectReason = ObjectDeselectReason.ClearSelectionCall;
            List<GameObject> previosulySelectedObjects = SelectedObjects;

            // Loop through each selected object and deselect it    
            foreach (var gameObject in previosulySelectedObjects)
            {
                DeselectObject(gameObject, deselectReason);
            }

            // Fire a selection changed event
            OnSelectionChanged(new ObjectSelectionChangedEventArgs(ObjectSelectReason.None, null, deselectReason, previosulySelectedObjects));

            // Execute a post selection changed action if Undo/Redo is allowed
            if (allowUndoRedo)
            {
                var postChangeSnapshot = new ObjectSelectionSnapshot();
                postChangeSnapshot.Snapshot();
                var postSelectionChangedAction = new PostObjectSelectionChangedAction(preChangeSnapshot, postChangeSnapshot);
                postSelectionChangedAction.Execute();
            }
        }

        public void Delete()
        {
            if (!CanBeDeleted()) return;

            // First, retrieve a list of objects which can be deleted
            var objectsToDelete = new List<GameObject>(NumSelectedObjects);
            var selectedParents = GameObjectEx.FilterParentsOnly(_selectedObjects);
            foreach (var parent in selectedParents)
            {
                if (Settings.IsObjectLayerDeletable(parent.layer)) objectsToDelete.AddRange(parent.GetAllChildrenAndSelf());
            }

            // Nothing to delete?
            if (objectsToDelete.Count == 0) return;

            // Take a pre delete snapshot needed for the delete action
            var preDeleteSnapshot = new ObjectSelectionSnapshot();
            preDeleteSnapshot.Snapshot();

            // Deselect the objects which are about to be deleted
            ObjectDeselectReason deselectReason = ObjectDeselectReason.WillBeDeleted;
            foreach (var objectToDelete in objectsToDelete)
            {
                DeselectObject(objectToDelete, deselectReason);
            }

            // Fire a selection changed event
            OnSelectionChanged(new ObjectSelectionChangedEventArgs(ObjectSelectReason.None, null, deselectReason, objectsToDelete));

            // We are about to delete the selection
            _willBeDeleted = true;
            if (WillBeDeleted != null) WillBeDeleted();

            // Invoke the delete action
            var deleteSelectedAction = new DeleteSelectedObjectsAction(objectsToDelete, preDeleteSnapshot);
            deleteSelectedAction.Execute();

            // The selection was deleted
            _willBeDeleted = false;
            if (Deleted != null) Deleted();
        }

        public void ForceDelete()
        {
            _grabSession.End();
            _gridSnapSession.End();
            _object2ObjectSnapSession.End();

            var objectsToDelete = new List<GameObject>();
            var selectedParents = GameObjectEx.FilterParentsOnly(_selectedObjects);
            foreach (var parent in selectedParents)
            {
                objectsToDelete.AddRange(parent.GetAllChildrenAndSelf());
            }

            ObjectDeselectReason deselectReason = ObjectDeselectReason.WillBeDeleted;
            foreach (var objectToDelete in objectsToDelete)
            {
                DeselectObject(objectToDelete, deselectReason);
            }
            OnSelectionChanged(new ObjectSelectionChangedEventArgs(ObjectSelectReason.None, null, deselectReason, objectsToDelete));

            _willBeDeleted = true;
            if (WillBeDeleted != null) WillBeDeleted();

            foreach (var parent in selectedParents)
                GameObject.Destroy(parent);

            _willBeDeleted = false;
            if (Deleted != null) Deleted();
        }

        public bool CanBeDeleted()
        {
            return _isEnabled && !_willBeDeleted && !IsManipSessionActive &&
                   !_doingPreSelectCustomize && !_doingPreDeselectCustomize && 
                   NumSelectedObjects != 0;
        }

        public bool CanBeDuplicated()
        {
            return _isEnabled && !_willBeDeleted && !IsManipSessionActive &&
                   !_doingPreSelectCustomize & !_doingPreDeselectCustomize && NumSelectedObjects != 0;
        }

        public bool CanBeModifiedByAPI()
        {
            if (!_isEnabled || _willBeDeleted || _doingPreSelectCustomize ||
                _doingPreDeselectCustomize || IsManipSessionActive || _firingSelectionChanged) return false;

            return true;
        }

        public ObjectSelectionDuplicationResult Duplicate()
        {
            var emptyResult = new ObjectSelectionDuplicationResult(new List<GameObject>());
            if (!CanBeDuplicated()) return emptyResult;

            List<GameObject> objectsToDuplicate = new List<GameObject>();
            foreach(var selectedObject in _selectedObjects)
            {
                if (_settings.IsObjectLayerDuplicatable(selectedObject.layer)) objectsToDuplicate.Add(selectedObject);
            }
            if (objectsToDuplicate.Count == 0) return emptyResult;

            if (WillBeDuplicated != null) WillBeDuplicated();
            var duplicateAction = new DuplicateObjectsAction(objectsToDuplicate);
            duplicateAction.Execute();

            var duplicateResult = new ObjectSelectionDuplicationResult(duplicateAction.DuplicateResult);
            if (Duplicated != null) Duplicated(duplicateResult);

            return duplicateResult;
        }

        public bool IsSelectionExactMatch(List<GameObject> gameObjectsToMatch)
        {
            // Handle the null case
            if (gameObjectsToMatch == null)
            {
                // If the specified object collection is null and there are no objects currently
                // selected, we will consider this a match and return true.
                if (NumSelectedObjects == 0) return true;

                // If null but the selection contains objects, we will return false
                return false;
            }

            // If the number of elements don't match, we can return false
            if (NumSelectedObjects != gameObjectsToMatch.Count) return false;

            // Make sure that every game object in 'gameObjectsToMatch' is selected.
            // If we find at least one object which is not selected, we can return false.
            foreach (GameObject objectToMatch in gameObjectsToMatch)
            {
                if (!IsObjectSelected(objectToMatch)) return false;
            }

            // We have a match
            return true;
        }

        public bool IsObjectSelected(GameObject gameObject)
        {
            if (gameObject == null) return false;
            return _selectedObjects.Contains(gameObject);
        }

        public AABB GetWorldAABB()
        {
            if (NumSelectedObjects == 0) return AABB.GetInvalid();

            var boundsQConfig = new ObjectBounds.QueryConfig();
            boundsQConfig.NoVolumeSize = Vector3Ex.FromValue(1e-5f);
            boundsQConfig.ObjectTypes = GameObjectTypeHelper.AllCombined;

            return ObjectBounds.CalcObjectCollectionWorldAABB(_selectedObjects, boundsQConfig);
        }

        public void Update_SystemCall()
        {
            if (!Settings.EnableCyclicalClickSelect)
            {
                _cyclicalClickSelectInfo.LastPickedObject = null;
                _cyclicalClickSelectInfo.LastSelectedIndex = -1;
            }

            RemoveNullAndInactiveObjectRefs();
            if (!_isEnabled) return;

            _multiSelectShape.MinSize = Settings.MinMultiSelectSize;
            if (!_multiSelectShape.IsVisible)
            {
                if (!IsManipSessionActive || IsGrabSessionActive) _grabSession.Update(_selectedObjects);
                if (!IsManipSessionActive || IsGridSnapSessionActive) _gridSnapSession.Update(_selectedObjects);
                if (!IsManipSessionActive || IsObject2ObjectSnapSessionActive) _object2ObjectSnapSession.Update(_selectedObjects);
            }

            if (!IsManipSessionActive)
            {
                IInputDevice inputDevice = RTInputDevice.Get.Device;
                if (inputDevice.WasButtonPressedInCurrentFrame(_objectPickDeviceBtnIndex)) OnInputDevicePickButtonDown();
                else if (inputDevice.WasButtonReleasedInCurrentFrame(_objectPickDeviceBtnIndex)) OnInputDevicePickButtonUp();
                if (inputDevice.WasMoved()) OnInputDeviceWasMoved();

                if (!_multiSelectShape.IsVisible)
                {
                    if (Hotkeys.DeleteSelected.IsActiveInFrame()) Delete();
                    else
                    if (Hotkeys.DuplicateSelection.IsActiveInFrame()) Duplicate();
                    else
                    if (Hotkeys.FocusCameraOnSelection.IsActiveInFrame()) RTFocusCamera.Get.Focus(GetWorldAABB());
                    else
                    if (RotationHotkeys.RotateAroundX.IsActiveInFrame()) Rotate(Axis.X, RotationSettings.KeyRotationSettings.XRotationStep, RotationSettings.RotationPivot);
                    else
                    if (RotationHotkeys.RotateAroundY.IsActiveInFrame()) Rotate(Axis.Y, RotationSettings.KeyRotationSettings.YRotationStep, RotationSettings.RotationPivot);
                    else
                    if (RotationHotkeys.RotateAroundZ.IsActiveInFrame()) Rotate(Axis.Z, RotationSettings.KeyRotationSettings.ZRotationStep, RotationSettings.RotationPivot);
                    else
                    if (RotationHotkeys.SetRotationToIdentity.IsActiveInFrame()) SetRotation(Quaternion.identity);
                }
            }
            else
            {
                if (RTInputDevice.Get.Device.WasButtonPressedInCurrentFrame(_objectPickDeviceBtnIndex))
                {
                    if (ActiveManipSession == ObjectSelectionManipSession.Grab) _grabSession.End();
                    else if (ActiveManipSession == ObjectSelectionManipSession.Object2ObjectSnap) _object2ObjectSnapSession.End();
                }
                else
                {
                    if (ActiveManipSession != ObjectSelectionManipSession.Grab)
                    {
                        if (RotationHotkeys.RotateAroundX.IsActiveInFrame()) Rotate(Axis.X, RotationSettings.KeyRotationSettings.XRotationStep, RotationSettings.RotationPivot);
                        else
                        if (RotationHotkeys.RotateAroundY.IsActiveInFrame()) Rotate(Axis.Y, RotationSettings.KeyRotationSettings.YRotationStep, RotationSettings.RotationPivot);
                        else
                        if (RotationHotkeys.RotateAroundZ.IsActiveInFrame()) Rotate(Axis.Z, RotationSettings.KeyRotationSettings.ZRotationStep, RotationSettings.RotationPivot);
                        else
                        if (RotationHotkeys.SetRotationToIdentity.IsActiveInFrame()) SetRotation(Quaternion.identity);
                    }
                    else
                    {
                        if (RotationHotkeys.SetRotationToIdentity.IsActiveInFrame()) SetRotation(Quaternion.identity);
                    }
                }
            }
        }

        public void Render_SystemCall()
        {
            if (!_isEnabled) return;

            Camera renderCamera = Camera.current;
            if (IsRenderIgnoreCamera(renderCamera)) return;

            // Render the multi-select shape.
            // Note: We only render the multi-select shape if the current camera is the focus camera.
            if (renderCamera == RTFocusCamera.Get.TargetCamera)
                _multiSelectShape.Render(LookAndFeel.SelectionRectFillColor, LookAndFeel.SelectionRectBorderColor, renderCamera);

            // Can we draw the selection boxes?
            if (LookAndFeel.DrawHighlight && !IsManipSessionActive)
            {
                // Setup rendering material
                Material material = MaterialPool.Get.SimpleColor;
                material.SetColor(LookAndFeel.SelectionBoxBorderColor);
                material.SetZWriteEnabled(false);
                material.SetZTestEnabled(true);
                material.SetPass(0);

                var boundsQConfig = new ObjectBounds.QueryConfig();
                boundsQConfig.NoVolumeSize = Vector3Ex.FromValue(0.0f);   // We don't want to render anything for objects with no volume
                boundsQConfig.ObjectTypes = GameObjectType.Mesh | GameObjectType.Sprite | GameObjectType.Terrain;

                if (LookAndFeel.SelBoxRenderMode != SelectionBoxRenderMode.SelectionVolume)
                {
                    bool drawFromParentToBottom = LookAndFeel.SelBoxRenderMode == SelectionBoxRenderMode.FromParentToBottom;
                    List<GameObject> selectedObjects = _selectedObjects;
                    if (drawFromParentToBottom) selectedObjects = GameObjectEx.FilterParentsOnly(selectedObjects);
                    if (selectedObjects.Count == 0) return;

                    foreach (GameObject selectedObject in selectedObjects)
                    {
                        OBB worldOBB = drawFromParentToBottom ? ObjectBounds.CalcHierarchyWorldOBB(selectedObject, boundsQConfig) : ObjectBounds.CalcWorldOBB(selectedObject, boundsQConfig);
                        if (worldOBB.IsValid)
                        {
                            worldOBB.Inflate(LookAndFeel.SelectionBoxInflateAmount);
                            if (LookAndFeel.SelBoxBorderStyle == SelectionBoxBorderStyle.FullWire) GraphicsEx.DrawWireBox(worldOBB);
                            else GraphicsEx.DrawWireCornerBox(worldOBB, LookAndFeel.WireCornerLinePercentage);
                        }
                    }
                }
                else
                {
                    AABB selectionWorldAABB = AABB.GetInvalid();
                    foreach (var selectedObject in _selectedObjects)
                    {
                        AABB worldAABB = ObjectBounds.CalcWorldAABB(selectedObject, boundsQConfig);
                        if (worldAABB.IsValid)
                        {
                            if (selectionWorldAABB.IsValid) selectionWorldAABB.Encapsulate(worldAABB);
                            else selectionWorldAABB = worldAABB;
                        }
                    }

                    if (selectionWorldAABB.IsValid)
                    {
                        selectionWorldAABB.Inflate(LookAndFeel.SelectionBoxInflateAmount);
                        if (LookAndFeel.SelBoxBorderStyle == SelectionBoxBorderStyle.FullWire) GraphicsEx.DrawWireBox(new OBB(selectionWorldAABB));
                        else GraphicsEx.DrawWireCornerBox(new OBB(selectionWorldAABB), LookAndFeel.WireCornerLinePercentage);
                    }
                }
            }

            if (renderCamera == RTFocusCamera.Get.TargetCamera)
            {
                _grabSession.Render();
                _gridSnapSession.Render();
            }
        }

        private void OnInputDevicePickButtonDown()
        {
            // Ignore this if there are any hovered scene elements or if the 
            // device coords are outside the focus camera viewport.
            if (RTScene.Get.IsAnySceneEntityHovered() || !RTFocusCamera.Get.IsViewportHoveredByDevice()) return;

            // Whenever the pick button is pressed, we have to prepare for the possibility
            // that the user might start multi-selecting objects. So we make the multi-select
            // shape visible and initialize its coords to sit at the same position in screen
            // space as indicated by the input device.
            if (Settings.CanMultiSelect)
            {
                var answer = new YesNoAnswer();
                if (CanMultiSelectDeselect != null) CanMultiSelectDeselect(answer);
                if (answer.HasOnlyYes)
                {
                    // Adjust selection shape
                    Vector2 inputDeviceScreenCoords = RTInputDevice.Get.Device.GetPositionYAxisUp();
                    _multiSelectShape.IsVisible = true;
                    _multiSelectShape.SetEnclosingRectTopLeftPoint(inputDeviceScreenCoords);
                    _multiSelectShape.SetEnclosingRectBottomRightPoint(inputDeviceScreenCoords);

                    // Take a pre-change snapshot just in case the selection will be modified using the multi-select shape
                    _multiSelectPreChangeSnapshot.Snapshot();
                    _wasSelectionChangedViaMultiSelectShape = false;
                }
            }
        }

        private void OnInputDevicePickButtonUp()
        {
            // The multi-select shape will be hidden when the pick button is not pressed
            _multiSelectShape.IsVisible = false;

            // Check if the selection was changed via the multi-select shape
            if (_wasSelectionChangedViaMultiSelectShape)
            {
                // The selection was changed; execute a post selection changed action.
                var postChangeSnapshot = new ObjectSelectionSnapshot();
                postChangeSnapshot.Snapshot();

                // Use the pre and post change snapshots to execute a post selection changed action to allow for Undo/Redo
                var postSelectionChangedAction = new PostObjectSelectionChangedAction(_multiSelectPreChangeSnapshot, postChangeSnapshot);
                postSelectionChangedAction.Execute();

                // Reset for next time
                _wasSelectionChangedViaMultiSelectShape = false;
            }
            else
            if (!RTScene.Get.IsAnySceneEntityHovered() && RTFocusCamera.Get.IsViewportHoveredByDevice())
            {
                // Can we click select?
                if (Settings.CanClickSelect && (!Settings.CanMultiSelect || !Hotkeys.MultiDeselect.IsActive()))
                {
                    var answer = new YesNoAnswer();
                    if (CanClickSelectDeselect != null) CanClickSelectDeselect(answer);
                    if (answer.HasOnlyYes) PerformClickSelect();
                }
            }
        }

        private void OnInputDeviceWasMoved()
        {
            // Is the multi-select shape visible and can we multi-select?
            if (_multiSelectShape.IsVisible && Settings.CanMultiSelect)
            {
                // We will only continue if there are no UI elements hovered
                if (!RTScene.Get.IsAnyUIElementHovered())
                {
                    // First adjust the shape's corner point based on the input device's position and then perform multi-select
                    _multiSelectShape.SetEnclosingRectBottomRightPoint(RTInputDevice.Get.Device.GetPositionYAxisUp());
                    PerformMultiSelect();
                }
            }
        }

        private void PerformMultiSelect()
        {
            // Check which objects are overlapped by the shape. This is a 2-step process. First,
            // we need to get a list of all objects which are visible by the camera. Then we feed 
            // those objects to the selection shape and have it return to us only those objects 
            // that are overlapped by the shape's area.
            var boundsQConfig = new ObjectBounds.QueryConfig();
            boundsQConfig.ObjectTypes = GameObjectTypeHelper.AllCombined;
            boundsQConfig.NoVolumeSize = Vector3Ex.FromValue(1e-5f);
            List<GameObject> overlappedObjects = RTFocusCamera.Get.GetVisibleObjects();
            overlappedObjects = _multiSelectShape.GetOverlappedObjects(overlappedObjects, RTFocusCamera.Get.TargetCamera, boundsQConfig, Settings.MultiSelectOverlapMode);

            // We will need all these later to fire a selection changed event
            bool selectionWasChanged = false;
            ObjectSelectReason selectReason = ObjectSelectReason.None;
            ObjectDeselectReason deselectReason = ObjectDeselectReason.None;
            List<GameObject> objectsWhichWereSelected = new List<GameObject>();
            List<GameObject> objectsWhichWereDeselected = new List<GameObject>();

            // Check if we need to multi-deselect, append or just regular multi-select
            if (Hotkeys.MultiDeselect.IsActive())
            {
                overlappedObjects = DoPreDeselectCustomize(overlappedObjects, ObjectDeselectReason.MultiDeselect);

                // Now loop through each overlapped object and deselect it
                foreach (var overlappedObject in overlappedObjects)
                {
                    // Check if the object is selected 
                    if (IsObjectSelected(overlappedObject))
                    {
                        // Deselect the object
                        deselectReason = ObjectDeselectReason.MultiDeselect;
                        DeselectObject(overlappedObject, deselectReason);
                        objectsWhichWereDeselected.Add(overlappedObject);
                        selectionWasChanged = true;
                    }
                }
            }
            else
            if(Hotkeys.AppendToSelection.IsActive())
            {
                var potentialList = FilterByRestrictions(overlappedObjects, SelectRestrictFlags.All, ObjectSelectReason.MultiSelectAppend);
                ObjectPreSelectCustomizeInfo preSelectCustomizeInfo = DoPreSelectCustomize(potentialList, ObjectSelectReason.MultiSelectAppend);
                if (preSelectCustomizeInfo != null) potentialList = preSelectCustomizeInfo.ToBeSelected;

                foreach (var gameObj in potentialList)
                {
                    // Select the object
                    if (!IsObjectSelected(gameObj))
                    {
                        selectReason = ObjectSelectReason.MultiSelectAppend;
                        SelectObject(gameObj, selectReason);
                        objectsWhichWereSelected.Add(gameObj);
                        selectionWasChanged = true;
                    }
                }
            }
            else
            {
                // First we need to identify the currently selected objects which are not overlapped
                // by the multi-select shape and deselect them.
                List<GameObject> toBeDeselected = SelectedObjects;
                toBeDeselected.RemoveAll(item => _multiSelectShape.OverlapsObject(item, RTFocusCamera.Get.TargetCamera, boundsQConfig, Settings.MultiSelectOverlapMode));
                toBeDeselected = DoPreDeselectCustomize(toBeDeselected, ObjectDeselectReason.MultiSelectNotOverlapped);

                foreach (var gameObject in toBeDeselected)
                {
                    if (IsObjectSelected(gameObject))
                    {
                        deselectReason = ObjectDeselectReason.MultiSelectNotOverlapped;
                        DeselectObject(gameObject, deselectReason);
                        objectsWhichWereDeselected.Add(gameObject);
                        selectionWasChanged = true;
                    }
                }

                var potentialList = FilterByRestrictions(overlappedObjects, SelectRestrictFlags.All, ObjectSelectReason.MultiSelect);
                ObjectPreSelectCustomizeInfo preSelectCustomizeInfo = DoPreSelectCustomize(potentialList, ObjectSelectReason.MultiSelect);
                if (preSelectCustomizeInfo != null) potentialList = preSelectCustomizeInfo.ToBeSelected;

                foreach (var gameObj in potentialList)
                {
                    // Select the object
                    if (!IsObjectSelected(gameObj))
                    {
                        selectReason = ObjectSelectReason.MultiSelect;
                        SelectObject(gameObj, selectReason);
                        objectsWhichWereSelected.Add(gameObj);
                        selectionWasChanged = true;
                    }
                }
            }

            if (objectsWhichWereSelected.Count == 0) selectReason = ObjectSelectReason.None;
            if (objectsWhichWereDeselected.Count == 0) deselectReason = ObjectDeselectReason.None;

            // If the selection was changed, we need to fire a selection changed event. Also store this
            // info inside the '_selectionWasChangedViaMultiSelect' because we will need it later when the
            // multi-selec session ends.
            if (selectionWasChanged)
            {
                _wasSelectionChangedViaMultiSelectShape = true;
                OnSelectionChanged(new ObjectSelectionChangedEventArgs(selectReason, objectsWhichWereSelected, deselectReason, objectsWhichWereDeselected));
            }
        }

        private void PerformClickSelect()
        {
            bool selectionWasChanged = false;
            ObjectSelectReason selectReason = ObjectSelectReason.None;
            ObjectDeselectReason deselectReason = ObjectDeselectReason.None;
            List<GameObject> objectsWhichWereSelected = new List<GameObject>();
            List<GameObject> objectsWhichWereDeselected = new List<GameObject>();

            // Store the state of the selection befor it is about to change. We will need this
            // in case the selection changes in some way.
            var preChangeSnapshot = new ObjectSelectionSnapshot();
            preChangeSnapshot.Snapshot();

            // Perform a raycast and choose the closest hit object which can be selected
            Ray ray = RTInputDevice.Get.Device.GetRay(RTFocusCamera.Get.TargetCamera);
            List<GameObjectRayHit> allHits = RTScene.Get.RaycastAllObjectsSorted(ray, SceneRaycastPrecision.BestFit);

            var filteredHits = FilterByRestrictions(allHits, SelectRestrictFlags.All);
            if (filteredHits.Count == 0) _cyclicalClickSelectInfo.LastSelectedIndex = -1;

            if (filteredHits.Count != 0)
            {
                GameObject pickedObject = filteredHits[0].HitObject;

                // Handle cyclical click select
                if (Settings.EnableCyclicalClickSelect)
                {
                    if (_cyclicalClickSelectInfo.LastSelectedIndex < 0 ||
                        _cyclicalClickSelectInfo.LastPickedObject != filteredHits[0].HitObject) _cyclicalClickSelectInfo.LastSelectedIndex = 0;
                    else
                    {
                        _cyclicalClickSelectInfo.LastSelectedIndex++;
                        _cyclicalClickSelectInfo.LastSelectedIndex %= filteredHits.Count;
                    }

                    _cyclicalClickSelectInfo.LastPickedObject = filteredHits[0].HitObject;
                    pickedObject = filteredHits[_cyclicalClickSelectInfo.LastSelectedIndex].HitObject;
                }              

                // Should we append the object to the selection
                if (Hotkeys.AppendToSelection.IsActive())
                {
                    // For click append, always use the first object
                    pickedObject = filteredHits[0].HitObject;

                    if (IsObjectSelected(pickedObject))
                    {
                        deselectReason = ObjectDeselectReason.CickAppendAlreadySelected;
                        List<GameObject> toBeDeselected = DoPreDeselectCustomize(new List<GameObject> { pickedObject }, deselectReason);
                        foreach(var objectToDeselect in toBeDeselected)
                        {
                            if (IsObjectSelected(objectToDeselect))
                            {
                                objectsWhichWereDeselected.Add(objectToDeselect);
                                DeselectObject(objectToDeselect, deselectReason);
                                selectionWasChanged = true;
                            }
                        }
                    }
                    else
                    {
                        var toBeSelected = new List<GameObject> { pickedObject };
                        ObjectPreSelectCustomizeInfo preSelectCustomizeInfo = DoPreSelectCustomize(toBeSelected, ObjectSelectReason.ClickAppend);
                        if (preSelectCustomizeInfo != null) toBeSelected = preSelectCustomizeInfo.ToBeSelected;

                        foreach (var gameObj in toBeSelected)
                        {
                            // Select the object
                            if (!IsObjectSelected(gameObj))
                            {
                                selectReason = ObjectSelectReason.ClickAppend;
                                SelectObject(gameObj, selectReason);
                                objectsWhichWereSelected.Add(gameObj);
                                selectionWasChanged = true;
                            }
                        }
                    }
                }
                else
                {
                    var toBeSelected = new List<GameObject> { pickedObject };
                    ObjectPreSelectCustomizeInfo preSelectCustomizeInfo = DoPreSelectCustomize(toBeSelected, ObjectSelectReason.Click);
                    if (preSelectCustomizeInfo != null) toBeSelected = preSelectCustomizeInfo.ToBeSelected;

                    if (toBeSelected.Count == 0)
                    {
                        selectReason = ObjectSelectReason.None;
                        if (NumSelectedObjects != 0)
                        {
                            selectionWasChanged = true;
                            deselectReason = ObjectDeselectReason.ClickAir;
                            ClearSelection(deselectReason);
                        }
                    }
                    else
                    {
                        selectReason = ObjectSelectReason.None;
                        if (NumSelectedObjects != 0)
                        {
                            selectionWasChanged = true;
                            deselectReason = ObjectDeselectReason.ClickSelectOther;
                            ClearSelection(deselectReason);
                        }

                        foreach (var gameObj in toBeSelected)
                        {
                            // Select the object
                            if (!IsObjectSelected(gameObj))
                            {
                                selectReason = ObjectSelectReason.Click;
                                SelectObject(gameObj, selectReason);
                                objectsWhichWereSelected.Add(gameObj);
                                selectionWasChanged = true;
                            }
                        }
                    }
                }
            }
            else
            // Note: We only prepare to clear the selection if the following hotkeys are not active.
            //       When the user wants to perform these 2 actions it is more intuitive to keep the
            //       selection intact.
            if (!Hotkeys.MultiDeselect.IsActive() && !Hotkeys.AppendToSelection.IsActive())
            {
                if (NumSelectedObjects != 0)
                {
                    objectsWhichWereDeselected = new List<GameObject>(_selectedObjects);
                    deselectReason = ObjectDeselectReason.ClickAir;
                    selectionWasChanged = true;

                    ClearSelection(deselectReason);
                }
            }

            if (selectionWasChanged)
            {
                OnSelectionChanged(new ObjectSelectionChangedEventArgs(selectReason, objectsWhichWereSelected, deselectReason, objectsWhichWereDeselected));

                var postChangeSnapshot = new ObjectSelectionSnapshot();
                postChangeSnapshot.Snapshot();

                var postSelectionChangedAction = new PostObjectSelectionChangedAction(preChangeSnapshot, postChangeSnapshot);
                postSelectionChangedAction.Execute();
            }
        }

        private ObjectPreSelectCustomizeInfo DoPreSelectCustomize(List<GameObject> toBeSelected, ObjectSelectReason selectReason)
        {
            if (PreSelectCustomize == null || toBeSelected == null || toBeSelected.Count == 0) return null;

            _doingPreSelectCustomize = true;
            var customizeInfo = new ObjectPreSelectCustomizeInfo(toBeSelected, selectReason);
            PreSelectCustomize(customizeInfo, toBeSelected);
            _doingPreSelectCustomize = false;

            return customizeInfo;
        }

        private List<GameObject> DoPreDeselectCustomize(List<GameObject> toBeDeselected, ObjectDeselectReason deselectReason)
        {
            if (PreDeselectCustomize == null || toBeDeselected == null || toBeDeselected.Count == 0) return toBeDeselected;

            _doingPreDeselectCustomize = true;
            var customizeInfo = new ObjectPreDeselectCustomizeInfo(toBeDeselected, deselectReason);
            PreDeselectCustomize(customizeInfo, toBeDeselected);
            _doingPreDeselectCustomize = false;

            return customizeInfo.ToBeDeselected;
        }

        private List<GameObject> FilterByRestrictions(IEnumerable<GameObject> gameObjects, SelectRestrictFlags restrictFlags, ObjectSelectReason selectReason)
        {
            var filtered = new List<GameObject>(10);
            foreach(var gameObj in gameObjects)
            {
                if (CanSelectObject(gameObj, restrictFlags, selectReason)) filtered.Add(gameObj);
            }

            return filtered;
        }

        private List<GameObjectRayHit> FilterByRestrictions(List<GameObjectRayHit> objectHits, SelectRestrictFlags restrictFlags)
        {
            var filtered = new List<GameObjectRayHit>(10);
            foreach (var hit in objectHits)
            {
                if (CanSelectObject(hit.HitObject, restrictFlags, ObjectSelectReason.None)) filtered.Add(hit);
            }

            return filtered;
        }

        private bool CanSelectObject(GameObject gameObject, SelectRestrictFlags restrictFlags, ObjectSelectReason selectReason)
        {
            if (gameObject == null || !gameObject.activeInHierarchy) return false;

            // Note: Camera object restrictions will always be applied regardless of 'ignoreRestrictions'.
            Camera objectCam = gameObject.GetComponent<Camera>();
            if (objectCam != null && (!Settings.IsCameraSelectable(objectCam) || RTFocusCamera.Get.TargetCamera == objectCam)) return false;

            if ((restrictFlags & SelectRestrictFlags.ObjectLayer) != 0 && !Settings.IsObjectLayerSelectable(gameObject.layer)) return false;
            if ((restrictFlags & SelectRestrictFlags.ObjectType) != 0 && !Settings.IsObjectTypeSelectable(gameObject.GetGameObjectType())) return false;
            if ((restrictFlags & SelectRestrictFlags.Object) != 0 && !Settings.IsObjectSelectable(gameObject)) return false;

            if ((restrictFlags & SelectRestrictFlags.SelectionListener) != 0)
            {
                IRTObjectSelectionListener selectionListener = gameObject.GetComponent<IRTObjectSelectionListener>();
                if (selectionListener != null && !selectionListener.OnCanBeSelected(new ObjectSelectEventArgs(selectReason))) return false;
            }

            return true;
        }

        private void SelectObject(GameObject gameObject, ObjectSelectReason selectReason)
        {
            _selectedObjects.Add(gameObject);

            // If the object has a selection listener attached, inform the mono that it was selected
            IRTObjectSelectionListener selectionListener = gameObject.GetComponent<IRTObjectSelectionListener>();
            if (selectionListener != null) selectionListener.OnSelected(new ObjectSelectEventArgs(selectReason));
        }

        private void DeselectObject(GameObject gameObject, ObjectDeselectReason deselectReason)
        {
            _selectedObjects.Remove(gameObject);

            // If the object has a selection listener attached, inform the mono that it was deselected
            IRTObjectSelectionListener selectionListener = gameObject.GetComponent<IRTObjectSelectionListener>();
            if (selectionListener != null) selectionListener.OnDeselected(new ObjectDeselectEventArgs(deselectReason));
        }

        private void ClearSelection(ObjectDeselectReason deselectReason)
        {
            if(NumSelectedObjects != 0)
            {
                // Store a copy of the currently selected objects so we can still access
                // these objects after the selection was cleared.
                List<GameObject> selectedObjects = SelectedObjects;
                _selectedObjects.Clear();

                // Now inform the objects that they were deselected
                var deselectEventArgs = new ObjectDeselectEventArgs(deselectReason);
                foreach(var gameObject in selectedObjects)
                {
                    IRTObjectSelectionListener selectionListener = gameObject.GetComponent<IRTObjectSelectionListener>();
                    if (selectionListener != null) selectionListener.OnDeselected(deselectEventArgs);
                }
            }         
        }

        private void OnSelectionChanged(ObjectSelectionChangedEventArgs args)
        {
            if (Changed != null)
            {
                _firingSelectionChanged = true;
                Changed(args);
                _firingSelectionChanged = false;
            }
        }

        private void RemoveNullAndInactiveObjectRefs()
        {
            // Get a list of all inactive objects. We will need this later to fire a selection changed event.
            List<GameObject> inactiveObjects = _selectedObjects.FindAll(item => item != null && !item.activeInHierarchy);

            // Remove null object refs and inactive objects
            _selectedObjects.RemoveAll(item => item == null || !item.activeInHierarchy);
            Settings.RemoveNullObjectRefs();

            // If any inactive objects were found, fire an object selection changed event
            if (inactiveObjects.Count != 0) 
                OnSelectionChanged(new ObjectSelectionChangedEventArgs(ObjectSelectReason.None, null, ObjectDeselectReason.Inactive, inactiveObjects));
        }

        private void OnUndoEnd(IUndoRedoAction action)
        {
            if (action is PostObjectSelectionChangedAction)
            {
                var postChangeAction = action as PostObjectSelectionChangedAction;
                HandleUndoRedo(postChangeAction.PreChangeSnapshot, true);
            }
            else
            if(action is DeleteSelectedObjectsAction)
            {
                var deleteObjectsAction = action as DeleteSelectedObjectsAction;
                HandleUndoRedo(deleteObjectsAction.PreDeleteSnapshot, true);
            }
        }

        private void OnRedoEnd(IUndoRedoAction action)
        {
            if (action is PostObjectSelectionChangedAction)
            {
                var postChangeAction = action as PostObjectSelectionChangedAction;
                HandleUndoRedo(postChangeAction.PostChangeSnapshot, false);
            }
            else
            if (action is DeleteSelectedObjectsAction)
            {
                var deleteObjectsAction = action as DeleteSelectedObjectsAction;
                HandleUndoRedo(deleteObjectsAction.PostDeleteSnapshot, false);
            }
        }

        private void HandleUndoRedo(ObjectSelectionSnapshot undoRedoSnapshot, bool isUndo)
        {
            if (!_isEnabled) return;

            // We will need this data later to fire a selection changed event
            bool selectionWasChanged = false;
            ObjectSelectReason selectReason = isUndo ? ObjectSelectReason.Undo : ObjectSelectReason.Redo;
            ObjectDeselectReason deselectReason = isUndo ? ObjectDeselectReason.Undo : ObjectDeselectReason.Redo;
            List<GameObject> objectsWhichWereSelected = new List<GameObject>();
            List<GameObject> objectsWhichWereDeselected = new List<GameObject>();

            // First, we need to deselect the objects which are currently selected
            List<GameObject> currentlySelectedObjects = SelectedObjects;
            foreach (var selectedObject in currentlySelectedObjects)
            {
                DeselectObject(selectedObject, deselectReason);
                objectsWhichWereDeselected.Add(selectedObject);
                selectionWasChanged = true;
            }

            // Now we need to select those objects which are indicated by the snapshot
            List<GameObject> snapshotObjects = undoRedoSnapshot.SnapshotObjects;
            foreach (var snapshotObject in snapshotObjects)
            {
                SelectObject(snapshotObject, selectReason);
                objectsWhichWereSelected.Add(snapshotObject);
                selectionWasChanged = true;
            }

            // Fire a selection changed event if needed
            if (selectionWasChanged) OnSelectionChanged(new ObjectSelectionChangedEventArgs(selectReason, objectsWhichWereSelected, deselectReason, objectsWhichWereDeselected, undoRedoSnapshot));
        }

        private void OnGrabSessionBegin()
        {
            _activeManipSession = ObjectSelectionManipSession.Grab;
            if (ManipSessionBegin != null) ManipSessionBegin(_activeManipSession);
        }

        private void OnGrabSessionEnd()
        {
            _activeManipSession = ObjectSelectionManipSession.None;
            if (ManipSessionEnd != null) ManipSessionEnd(ObjectSelectionManipSession.Grab);
        }

        private void OnGridSnapSessionBegin()
        {
            _activeManipSession = ObjectSelectionManipSession.GridSnap;
            if (ManipSessionBegin != null) ManipSessionBegin(_activeManipSession);
        }

        private void OnGridSnapSessionEnd()
        {
            _activeManipSession = ObjectSelectionManipSession.None;
            if (ManipSessionEnd != null) ManipSessionEnd(ObjectSelectionManipSession.GridSnap);
        }

        private void OnObject2ObjectSnapSessionBegin()
        {
            _activeManipSession = ObjectSelectionManipSession.Object2ObjectSnap;
            if (ManipSessionBegin != null) ManipSessionBegin(_activeManipSession);
        }

        private void OnObject2ObjectSnapSessionEnd()
        {
            _activeManipSession = ObjectSelectionManipSession.None;
            if (ManipSessionEnd != null) ManipSessionEnd(ObjectSelectionManipSession.Object2ObjectSnap);
        }
    }
}
