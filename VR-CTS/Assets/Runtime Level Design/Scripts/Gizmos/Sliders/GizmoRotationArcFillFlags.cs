using System;

namespace RLD
{
    [Flags]
    public enum GizmoRotationArcFillFlags
    {
        None = 0,
        Area = 1,
        ExtremitiesBorder = 2,
        ArcBorder = 4,
        All = Area | ExtremitiesBorder | ArcBorder
    }
}
