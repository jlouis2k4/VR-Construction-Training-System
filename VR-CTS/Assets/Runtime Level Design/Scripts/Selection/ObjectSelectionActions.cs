using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    public class DeleteSelectedObjectsAction : IUndoRedoAction
    {
        private List<GameObject> _selectedObjects;
        private List<GameObject> _deletedObjects;
        private ObjectSelectionSnapshot _preDeleteSnapshot;
        private ObjectSelectionSnapshot _postDeleteSnapshot;
        private bool _canDestroyObjects;

        public ObjectSelectionSnapshot PreDeleteSnapshot { get { return _preDeleteSnapshot; } }
        public ObjectSelectionSnapshot PostDeleteSnapshot { get { return _postDeleteSnapshot; } }

        public DeleteSelectedObjectsAction(List<GameObject> selectedObjects, ObjectSelectionSnapshot preDeleteSnapshot)
        {
            _selectedObjects = new List<GameObject>(selectedObjects);
            _preDeleteSnapshot = preDeleteSnapshot;
        }

        public void Execute()
        {
            // Only delete if we haven't deleted already
            if (_postDeleteSnapshot == null && _selectedObjects.Count != 0)
            {
                // Loop through each selected object
                _deletedObjects = new List<GameObject>(_selectedObjects.Count);
                foreach (var selectedObject in _selectedObjects)
                {
                    // Add the object to the deleted list and make it inactive
                    _deletedObjects.Add(selectedObject);
                    selectedObject.SetActive(false);
                }

                // Take a post delete snapshot to allow us to redo
                _postDeleteSnapshot = new ObjectSelectionSnapshot();
                _postDeleteSnapshot.Snapshot();

                // Record th action with the Undo/Redo system
                RTUndoRedo.Get.RecordAction(this);
                _canDestroyObjects = true;
            }
        }

        public void Undo()
        {
            // Only undo if we deleted anything
            if(_deletedObjects != null)
            {
                // Enable the objects
                foreach (var deletedObject in _deletedObjects)
                {
                    deletedObject.SetActive(true);
                }
                _canDestroyObjects = false;
            }
        }

        public void Redo()
        {
            // Only redo if we deleted anything
            if (_deletedObjects != null)
            {
                // Disable the objects
                foreach (var deletedObject in _deletedObjects)
                {
                    deletedObject.SetActive(false);
                }
                _canDestroyObjects = true;
            }
        }

        public void OnRemovedFromUndoRedoStack()
        {
            if(_deletedObjects != null && _deletedObjects.Count != 0)
            { 
                if (_canDestroyObjects)
                {
                    // Delete the objects
                    foreach (var gameObject in _deletedObjects) GameObject.Destroy(gameObject);
                    _deletedObjects.Clear();
                    _deletedObjects = null;
                }
            }
        }
    }

    public class PostObjectSelectionChangedAction : IUndoRedoAction
    {
        private ObjectSelectionSnapshot _preChangeSnapshot;
        private ObjectSelectionSnapshot _postChangeSnapshot;

        public ObjectSelectionSnapshot PreChangeSnapshot { get { return _preChangeSnapshot; } }
        public ObjectSelectionSnapshot PostChangeSnapshot { get { return _postChangeSnapshot; } }

        public PostObjectSelectionChangedAction(ObjectSelectionSnapshot preChangeSnapshot, ObjectSelectionSnapshot postChangeSnapshot)
        {
            _preChangeSnapshot = preChangeSnapshot;
            _postChangeSnapshot = postChangeSnapshot;
        }

        public void Execute()
        {
            if(_preChangeSnapshot != null && _postChangeSnapshot != null)
                RTUndoRedo.Get.RecordAction(this);
        }

        public void Undo()
        {
        }

        public void Redo()
        {
        }

        public void OnRemovedFromUndoRedoStack()
        {
        }
    }
}