using UnityEngine;

namespace RLD
{
    public interface IGizmoPolygon2DBorderController
    {
        void UpdateHandles();
        void UpdateEpsilons();
        void UpdateTransforms();
    }
}
