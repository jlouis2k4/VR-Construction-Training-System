using UnityEngine;

namespace RLD
{
    public interface IGizmoQuad3DBorderController
    {
        void UpdateHandles();
        void UpdateEpsilons(float zoomFactor);
        void UpdateTransforms(float zoomFactor);
    }
}
