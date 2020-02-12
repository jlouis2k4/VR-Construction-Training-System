using UnityEngine;

namespace RLD
{
    public class TexturePool : Singleton<TexturePool>
    {
        private Texture2D _mainLightIcon;
        private Texture2D _mainParticleSystemIcon;
        private Texture2D _cameraIcon;
        private Texture2D _xAxisLabel;
        private Texture2D _yAxisLabel;
        private Texture2D _zAxisLabel;
        private Texture2D _camPerspMode;
        private Texture2D _camOrthoMode;

        public Texture2D MainLightIcon
        {
            get
            {
                if (_mainLightIcon == null) _mainLightIcon = Resources.Load("Textures/MainLightIcon") as Texture2D;
                return _mainLightIcon;
            }
        }
        public Texture2D MainParticleSystemIcon
        {
            get
            {
                if (_mainParticleSystemIcon == null) _mainParticleSystemIcon = Resources.Load("Textures/MainParticleSystemIcon") as Texture2D;
                return _mainParticleSystemIcon;
            }
        }
        public Texture2D CameraIcon
        {
            get
            {
                if (_cameraIcon == null) _cameraIcon = Resources.Load("Textures/CameraIcon") as Texture2D;
                return _cameraIcon;
            }
        }
        public Texture2D XAxisLabel
        {
            get
            {
                if (_xAxisLabel == null) _xAxisLabel = Resources.Load("Textures/XAxisLabel") as Texture2D;
                return _xAxisLabel;
            }
        }
        public Texture2D YAxisLabel
        {
            get
            {
                if (_yAxisLabel == null) _yAxisLabel = Resources.Load("Textures/YAxisLabel") as Texture2D;
                return _yAxisLabel;
            }
        }
        public Texture2D ZAxisLabel
        {
            get
            {
                if (_zAxisLabel == null) _zAxisLabel = Resources.Load("Textures/ZAxisLabel") as Texture2D;
                return _zAxisLabel;
            }
        }
        public Texture2D CamPerspMode
        {
            get
            {
                if (_camPerspMode == null) _camPerspMode = Resources.Load("Textures/CamPerspMode") as Texture2D;
                return _camPerspMode;
            }
        }
        public Texture2D CamOrthoMode
        {
            get
            {
                if (_camOrthoMode == null) _camOrthoMode = Resources.Load("Textures/CamOrthoMode") as Texture2D;
                return _camOrthoMode;
            }
        }
    }
}
