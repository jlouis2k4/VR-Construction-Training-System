using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    /// <summary>
    /// Identifies different types reasons why an object was selected. 
    /// </summary>
    public enum ObjectSelectReason
    {
        /// <summary>
        /// Useful in some situations to mark the fact that no object(s) was/were selected.
        /// </summary>
        None = 0,
        /// <summary>
        /// The object was selected via a click.
        /// </summary>
        Click,
        /// <summary>
        /// The object was selected via a click when the append shortcut key was pressed.
        /// </summary>
        ClickAppend,
        /// <summary>
        /// The object was selected via the multi-select shape.
        /// </summary>
        MultiSelect,
        /// <summary>
        /// The object was selected via tge multi-select shape while the append to selection
        /// hotkey was active.
        /// </summary>
        MultiSelectAppend,
        /// <summary>
        /// The object was selected because the user has performed an Undo.
        /// </summary>
        Undo,
        /// <summary>
        /// The object was selected because the user has performed an Redo.
        /// </summary>
        Redo,
        /// <summary>
        /// The object was selected because the client code made a call to a selection_append function.
        /// </summary>
        AppendToSelectionCall,
        /// <summary>
        /// The object was selected because the client code made a call to a set_selected function.
        /// </summary>
        SetSelectedCall
    }

    /// <summary>
    /// Identifies different types reasons why an object was deselected. 
    /// </summary>
    public enum ObjectDeselectReason
    {
        /// <summary>
        /// Useful in some situations to mark the fact that no object(s) was/were deselected.
        /// </summary>
        None = 0,
        /// <summary>
        /// The object was deselected because the user clicked on another object
        /// to select that one instead.
        /// </summary>
        ClickSelectOther,
        /// <summary>
        /// The object was deselected because the user clicked on it while the append
        /// key was active and the object was already selected.
        /// </summary>
        CickAppendAlreadySelected,
        /// <summary>
        /// The object was deselected because the user click in thin air.
        /// </summary>
        ClickAir,
        /// <summary>
        /// The object was deselected via the multi-select shape.
        /// </summary>
        MultiDeselect,
        /// <summary>
        /// The user is selecting objcts using the multi-select shape, but the object no longer
        /// falls inside the shape area.
        /// </summary>
        MultiSelectNotOverlapped,
        /// <summary>
        /// The object was deselected because the user has performed an Undo.
        /// </summary>
        Undo,
        /// <summary>
        /// The object was deselected because the user has performed an Redo.
        /// </summary>
        Redo,
        /// <summary>
        /// The object was deselected because the client code made a call to a remove_from_selection function.
        /// </summary>
        RemoveFromSelectionCall,
        /// <summary>
        /// The object was deselected because the client code made a call to a clear_selection function.
        /// </summary>
        ClearSelectionCall,
        /// <summary>
        /// The object was deselected because the client code made a call to a set_selected function.
        /// </summary>
        SetSelectedCall,
        /// <summary>
        /// The object was deselected because it has been detected to be inactive.
        /// </summary>
        Inactive,
        /// <summary>
        /// The object was deselected because it is about to be deleted.
        /// </summary>
        WillBeDeleted
    }

    /// <summary>
    /// Holds data for an object selection changed event.
    /// </summary>
    public class ObjectSelectionChangedEventArgs
    {
        /// <summary>
        /// If there were any objects which were selected, this specifies the selection reason.
        /// </summary>
        private ObjectSelectReason _selectReason = ObjectSelectReason.None;
        /// <summary>
        /// The objects which were selected. If the select reason is 'None', this list is empty.
        /// </summary>
        private List<GameObject> _objectsWhichWereSelected = new List<GameObject>();
        /// <summary>
        /// If there were any objects which were deselected, this secifies the reason why they 
        /// were deselected.
        /// </summary>
        private ObjectDeselectReason _deselectReason = ObjectDeselectReason.None;
        /// <summary>
        /// The objects which were deselected. If the select reason is 'None', this list is empty.
        /// </summary>
        private List<GameObject> _objectsWhichWereDeselected = new List<GameObject>();
        /// <summary>
        /// If the object selection was changed because of an Undo/Redo operation, this has to point
        /// to a snapshot instance which represents the state of the object selection before being
        /// modified. Otherwise, this will be null.
        /// </summary>
        private ObjectSelectionSnapshot _undoRedoSnapshot;

        public ObjectSelectReason SelectReason { get { return _selectReason; } }
        public int NumObjectsSelected { get { return _objectsWhichWereSelected.Count; } }
        public List<GameObject> ObjectsWhichWereSelected { get { return new List<GameObject>(_objectsWhichWereSelected); } }
        public ObjectDeselectReason DeselectReason { get { return _deselectReason; } }
        public int NumObjectsDeselected { get { return _objectsWhichWereDeselected.Count; } }
        public List<GameObject> ObjectsWhichWereDeselected { get { return new List<GameObject>(_objectsWhichWereDeselected); } }
        public ObjectSelectionSnapshot UndoRedoSnapshot { get { return _undoRedoSnapshot; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectReason">
        /// If there were any objects which were selected, this specifies the selection reason.
        /// </param>
        /// <param name="objectsWhichWereSelected">
        /// The objects which were selected. If the select reason is 'None', this list is empty.
        /// This can be null or empty when the select reason is 'None'.
        /// </param>
        /// <param name="deselectReason">
        /// If there were any objects which were deselected, this secifies the reason why they 
        /// were deselected.
        /// </param>
        /// <param name="objectsWhichWereDeselected">
        /// The objects which were deselected. If the select reason is 'None', this list is empty.
        /// This can be null or empty when the deselect reason is 'None'.
        /// </param>
        /// <param name="undoRedoSnapshot">
        /// If the object selection was changed because of an Undo/Redo operation, this has to point
        /// to a snapshot instance which represents the state of the object selection before being
        /// modified. Otherwise, this has to be null.
        /// </param>
        public ObjectSelectionChangedEventArgs(ObjectSelectReason selectReason, List<GameObject> objectsWhichWereSelected,
                                               ObjectDeselectReason deselectReason, List<GameObject> objectsWhichWereDeselected,
                                               ObjectSelectionSnapshot undoRedoSnapshot = null)
        {
            _selectReason = selectReason;
            if (objectsWhichWereSelected != null) _objectsWhichWereSelected = new List<GameObject>(objectsWhichWereSelected);
            else _objectsWhichWereSelected = new List<GameObject>();

            _deselectReason = deselectReason;
            if (objectsWhichWereDeselected != null) _objectsWhichWereDeselected = new List<GameObject>(objectsWhichWereDeselected);
            else _objectsWhichWereDeselected = new List<GameObject>();

            if (_selectReason == ObjectSelectReason.Undo || _selectReason == ObjectSelectReason.Redo ||
                _deselectReason == ObjectDeselectReason.Undo || _deselectReason == ObjectDeselectReason.Redo) _undoRedoSnapshot = undoRedoSnapshot;
        }
    }

    public class ObjectSelectEventArgs
    {
        private ObjectSelectReason _selectReason;
        public ObjectSelectReason SelectReason { get { return _selectReason; } }

        public ObjectSelectEventArgs(ObjectSelectReason selectReason)
        {
            _selectReason = selectReason;
        }
    }

    public class ObjectDeselectEventArgs
    {
        private ObjectDeselectReason _deselectReason;
        public ObjectDeselectReason DeselectReason { get { return _deselectReason; } }

        public ObjectDeselectEventArgs(ObjectDeselectReason deselectReason)
        {
            _deselectReason = deselectReason;
        }
    }

    public class ObjectSelectionSnapshot
    {
        private List<GameObject> _snapshotObjects = new List<GameObject>();
        private ObjectSelectionGizmosSnapshot _gizmosSnapshot = new ObjectSelectionGizmosSnapshot();

        public int NumObjects { get { return _snapshotObjects.Count; } }
        public List<GameObject> SnapshotObjects { get { return new List<GameObject>(_snapshotObjects); } }
        public ObjectSelectionGizmosSnapshot GizmosSnapshot { get { return new ObjectSelectionGizmosSnapshot(_gizmosSnapshot); } }

        public ObjectSelectionSnapshot()
        {
        }

        public ObjectSelectionSnapshot(ObjectSelectionSnapshot copy)
        {
            _snapshotObjects = copy.SnapshotObjects;
            _gizmosSnapshot = copy.GizmosSnapshot;
        }

        public void Snapshot()
        {
            _snapshotObjects = new List<GameObject>(RTObjectSelection.Get.SelectedObjects);
            _gizmosSnapshot.Snapshot();
        }
    }

    public class ObjectSelectionGizmosSnapshot
    {
        private GameObject _pivotObject;

        public GameObject PivotObject { get { return _pivotObject; } }

        public ObjectSelectionGizmosSnapshot()
        {
        }

        public ObjectSelectionGizmosSnapshot(ObjectSelectionGizmosSnapshot copy)
        {
            _pivotObject = copy.PivotObject;
        }

        public void Snapshot()
        {
            // TODO: Remove RTObjectSelectionGizmos dependency 
            if (RTObjectSelectionGizmos.Get != null)
                _pivotObject = RTObjectSelectionGizmos.Get.PivotObject;
        }
    }

    public class ObjectSelectionDuplicationResult
    {
        private List<GameObject> _duplicateParents;

        public List<GameObject> DuplicateParents { get { return new List<GameObject>(_duplicateParents); } }
        public int NumDuplicateParents { get { return _duplicateParents.Count; } }

        public ObjectSelectionDuplicationResult(List<GameObject> duplicatedParents)
        {
            _duplicateParents = new List<GameObject>(duplicatedParents);
        }

        public GameObject GetParentByIndex(int index)
        {
            return _duplicateParents[index];
        }
    }
}