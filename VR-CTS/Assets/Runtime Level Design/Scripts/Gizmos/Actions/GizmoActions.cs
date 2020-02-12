using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    public class ObjectExtrudeGizmoDragEnd : IUndoRedoAction
    {
        private bool _wasExecuted;
        private bool _destroyClones;
        private List<GameObject> _targetParents = new List<GameObject>();
        private List<LocalTransformSnapshot> _undoTargetSnapshots = new List<LocalTransformSnapshot>();
        private List<LocalTransformSnapshot> _redoTargetSnapshots = new List<LocalTransformSnapshot>();

        private List<GameObject> _extrudeClones = new List<GameObject>();

        public int NumTargets { get { return _targetParents.Count; } }

        public void SetTargetParents(IEnumerable<GameObject> targetParents)
        {
            if (_wasExecuted) return;
            _targetParents = new List<GameObject>(targetParents);
        }

        public void TakeUndoTargetSnapshots()
        {
            if (_wasExecuted) return;
            _undoTargetSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_targetParents);
        }

        public void TakeRedoTargetSnapshots()
        {
            if (_wasExecuted) return;
            _redoTargetSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_targetParents);
        }

        public void AddExtrudeClones(List<GameObject> extrudeClones)
        {
            if (_wasExecuted) return;
            foreach (var clone in extrudeClones)
                AddExtrudeClone(clone);
        }

        public void AddExtrudeClone(GameObject extrudeClone)
        {
            if (_wasExecuted) return;
            if (extrudeClone != null) _extrudeClones.Add(extrudeClone);
        }

        public void Execute()
        {
            RTUndoRedo.Get.RecordAction(this);
            _wasExecuted = true;
        }

        public void Undo()
        {
            foreach (var snapshot in _undoTargetSnapshots)
                snapshot.Apply();

            foreach (var clone in _extrudeClones)
                if (clone != null) clone.SetActive(false);

            _destroyClones = true;
        }

        public void Redo()
        {
            foreach (var snapshot in _redoTargetSnapshots)
                snapshot.Apply();

            foreach (var clone in _extrudeClones)
                if (clone != null) clone.SetActive(true);

            _destroyClones = false;
        }

        public void OnRemovedFromUndoRedoStack()
        {
            if (_destroyClones && _extrudeClones.Count != 0)
            {
                foreach (var clone in _extrudeClones) GameObject.Destroy(clone);
                _extrudeClones.Clear();
            }
        }
    }
}
