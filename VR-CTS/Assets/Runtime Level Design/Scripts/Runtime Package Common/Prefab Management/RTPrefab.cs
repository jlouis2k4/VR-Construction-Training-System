using UnityEngine;
using System;

namespace RLD
{
    [Serializable]
    public class RTPrefab
    {
        [SerializeField]
        private GameObject _unityPrefab;
        [SerializeField]
        private Texture2D _previewTexture;
        [NonSerialized]
        private Sprite _previewSprite;

        public GameObject UnityPrefab { get { return _unityPrefab; } set { _unityPrefab = value; } }
        public Texture2D PreviewTexture { get { return _previewTexture; } set { _previewTexture = value; } }
        public Sprite PreviewSprite 
        { 
            get 
            {
                if (_previewTexture == null) return null;
                if (_previewSprite == null) _previewSprite = Sprite.Create(_previewTexture, new Rect(0.0f, 0.0f, _previewTexture.width, 
                    _previewTexture.height), new Vector2(_previewTexture.width, _previewTexture.height));

                return _previewSprite; 
            } 
        }

        public GameObject Instantiate()
        {
            if (UnityPrefab == null) return null;
            return GameObject.Instantiate(UnityPrefab) as GameObject;
        }

        public GameObject Instantiate(Vector3 worldPos, Quaternion worldRotation)
        {
            if (UnityPrefab == null) return null;
            return GameObject.Instantiate(UnityPrefab, worldPos, worldRotation);
        }

        public GameObject Instantiate(Vector3 worldPos, Quaternion worldRotation, Vector3 worldScale)
        {
            if (UnityPrefab == null) return null;

            GameObject gameObject = GameObject.Instantiate(UnityPrefab, worldPos, worldRotation);
            gameObject.transform.SetWorldScale(worldScale);
            return gameObject;
        }
    }
}
