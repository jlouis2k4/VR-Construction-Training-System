using UnityEngine;

namespace RLD
{
    public interface ISceneGizmo
    {
        Gizmo OwnerGizmo { get; }
        Camera SceneCamera { get; }
    }
}
