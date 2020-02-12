using UnityEngine;

namespace RLD
{
    public interface IRTObjectSelectionListener
    {
        bool OnCanBeSelected(ObjectSelectEventArgs selectArgs);
        void OnSelected(ObjectSelectEventArgs selectArgs);
        void OnDeselected(ObjectDeselectEventArgs deselectArgs);
    }
}
