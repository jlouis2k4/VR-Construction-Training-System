using UnityEngine;

namespace RLD
{
    public interface IGizmoQuad2DBorderController
    {
        void UpdateHandles();
        void UpdateEpsilons();
        void UpdateTransforms();
    }
}
