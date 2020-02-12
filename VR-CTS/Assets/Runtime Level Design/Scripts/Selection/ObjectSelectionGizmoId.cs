using UnityEngine;

namespace RLD
{
    public static class ObjectSelectionGizmoId
    {
        public static int None { get { return 0; } }
        public static int MoveGizmo { get { return 1; } }
        public static int RotationGizmo { get { return 2; } }
        public static int ScaleGizmo { get { return 3; } }
        public static int BoxScaleGizmo { get { return 4; } }
        public static int UniversalGizmo { get { return 5; } }
        public static int ExtrudeGizmo { get { return 6; } }

        public static int SafeClientId { get { return 10000; } }
    }
}
