using UnityEngine;

namespace RLD
{
    public interface IGizmoCircle3DBorderController
    {
        void UpdateHandles();
        void UpdateEpsilons(float zoomFactor);
        void UpdateTransforms(float zoomFactor);
    }
}
