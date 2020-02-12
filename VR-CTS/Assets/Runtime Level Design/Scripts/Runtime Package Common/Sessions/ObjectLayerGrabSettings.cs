using UnityEngine;
using System;

namespace RLD
{
    [Serializable]
    public class ObjectLayerGrabSettings
    {
        [SerializeField]
        private int _layer;
        [SerializeField]
        private bool _isActive;
        [SerializeField]
        private bool _alignAxis = true;
        [SerializeField]
        private TransformAxis _alignmentAxis = TransformAxis.PositiveY;
        [SerializeField]
        private float _defaultOffsetFromSurface = 0.0f;

        public int Layer { get { return _layer; } }
        public bool IsActive { get { return _isActive; } set { _isActive = value; } }
        public bool AlignAxis { get { return _alignAxis; } set { _alignAxis = value; } }
        public TransformAxis AlignmentAxis { get { return _alignmentAxis; } set { _alignmentAxis = value; } }
        public float DefaultOffsetFromSurface { get { return _defaultOffsetFromSurface; } set { _defaultOffsetFromSurface = value; } }

        public ObjectLayerGrabSettings(int layer)
        {
            _layer = layer;
        }
    }
}
