using UnityEngine;

namespace RLD
{
    public class GizmoOverrideColor
    {
        private bool _isActive;
        private Color _color;

        public bool IsActive { get { return _isActive; } set { _isActive = value; } }
        public Color Color { get { return _color; } set { _color = value; } }
    }
}
