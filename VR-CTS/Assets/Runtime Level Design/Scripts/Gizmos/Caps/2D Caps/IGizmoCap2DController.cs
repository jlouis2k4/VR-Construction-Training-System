using UnityEngine;

namespace RLD
{
    public interface IGizmoCap2DController
    {
        void UpdateHandles();
        void UpdateTransforms();
        void CapSlider2D(Vector2 sliderDirection, Vector2 sliderEndPt);
        void CapSlider2DInvert(Vector2 sliderDirection, Vector2 sliderEndPt);
        float GetSliderAlignedRealLength();
    }
}