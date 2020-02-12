using UnityEngine;

namespace RLD
{
    public interface IGizmoRATriangle3DBorderController
    {
        void UpdateHandles();
        void UpdateEpsilons(float zoomFactor);
        void UpdateTransforms(float zoomFactor);
    }
}
