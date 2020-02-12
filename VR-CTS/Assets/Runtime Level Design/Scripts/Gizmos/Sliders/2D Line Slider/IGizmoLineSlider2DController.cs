using UnityEngine;

namespace RLD
{
    public interface IGizmoLineSlider2DController
    {
        void UpdateHandles();
        void UpdateTransforms();
        void UpdateEpsilons();
    }
}
