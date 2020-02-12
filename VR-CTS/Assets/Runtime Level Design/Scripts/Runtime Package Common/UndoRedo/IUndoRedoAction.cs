using UnityEngine;

namespace RLD
{
    public interface IUndoRedoAction
    {
        void Execute();
        void Undo();
        void Redo();
        void OnRemovedFromUndoRedoStack();
    }
}
