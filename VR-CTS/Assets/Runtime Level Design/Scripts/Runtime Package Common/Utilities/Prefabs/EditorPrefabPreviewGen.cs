using UnityEngine;
using System;
using System.Collections.Generic;

namespace RLD
{
    public class EditorPrefabPreviewGen
    {
        private enum PreviewObjectType
        {
            Mesh = 1,
            Sprite,
            Light,
            ParticleSystem,
            Other
        }

        private PrefabPreviewLookAndFeel _previewLookAndFeel;
        private ObjectBounds.QueryConfig _boundsQConfig = new ObjectBounds.QueryConfig();
        private Light _previewLight;
        private Camera _renderCamera;
        private bool _isGenSessionActive;
        private Dictionary<Light, bool> _lightToState = new Dictionary<Light, bool>();
        private GameObject _nonMeshPreviewObject;

        public EditorPrefabPreviewGen()
        {
            _boundsQConfig.ObjectTypes = GameObjectTypeHelper.AllCombined & (~GameObjectType.Terrain);
            _boundsQConfig.NoVolumeSize = Vector3Ex.FromValue(1.0f);
        }

        public bool BeginGenSession(PrefabPreviewLookAndFeel previewLookAndFeel)
        {
            if (_isGenSessionActive || previewLookAndFeel == null) return false;

            DisableSceneLights();

            _previewLookAndFeel = previewLookAndFeel;
            if (!CreateRenderCamera() ||
                !CreatePreviewLight())
            {
                RestoreSceneLights();
                return false;
            }
            CreateNonMeshPreviewObject();

            _isGenSessionActive = true;
            return true;
        }

        public void EndGenSession()
        {
            if (!_isGenSessionActive) return;

            if (_renderCamera != null)
            {
                if (_renderCamera.targetTexture != null) _renderCamera.targetTexture.Release();
                GameObject.DestroyImmediate(_renderCamera.gameObject);
            }

            if (_previewLight != null) GameObject.DestroyImmediate(_previewLight.gameObject);
            if (_nonMeshPreviewObject != null) GameObject.DestroyImmediate(_nonMeshPreviewObject);

            RestoreSceneLights();
            _isGenSessionActive = false;
        }

        public Texture2D Generate(GameObject unityPrefab)
        {
            if (!_isGenSessionActive || _renderCamera.targetTexture == null) return null;

            RenderTexture oldRenderTexture = UnityEngine.RenderTexture.active;
            RenderTexture.active = _renderCamera.targetTexture;
            GL.Clear(true, true, _previewLookAndFeel.BkColor);

            bool hasMesh = unityPrefab.HierarchyHasMesh();
            bool hasSprite = unityPrefab.HierarchyHasSprite();

            PreviewObjectType previewObjectType = PreviewObjectType.Mesh;
            if (!hasMesh && hasSprite) previewObjectType = PreviewObjectType.Sprite;
            else if (!hasMesh && !hasSprite)
            {
                if (unityPrefab.HierarchyHasObjectsOfType(GameObjectType.Light)) previewObjectType = PreviewObjectType.Light;
                else if (unityPrefab.HierarchyHasObjectsOfType(GameObjectType.ParticleSystem)) previewObjectType = PreviewObjectType.ParticleSystem;
                else previewObjectType = PreviewObjectType.Other;
            }

            GameObject previewObject = null;
            if (previewObjectType == PreviewObjectType.Mesh || previewObjectType == PreviewObjectType.Sprite) 
                previewObject = GameObject.Instantiate(unityPrefab);
            else previewObject = _nonMeshPreviewObject;

            Transform previewObjectTransform = previewObject.transform;
            previewObjectTransform.position = Vector3.zero;
            previewObjectTransform.rotation = Quaternion.identity;
            previewObjectTransform.localScale = unityPrefab.transform.lossyScale;

            AABB sceneAABB = RTScene.Get.CalculateBounds();
            Sphere sceneSphere = new Sphere(sceneAABB);
            AABB previewAABB = new AABB();
            previewAABB = ObjectBounds.CalcHierarchyWorldAABB(previewObject, _boundsQConfig);
            Sphere previewSphere = new Sphere(previewAABB);
            
            Vector3 previewSphereCenter = sceneSphere.Center - Vector3.right * (sceneSphere.Radius + previewSphere.Radius + 90.0f);
            previewObjectTransform.position += (previewSphereCenter - previewSphere.Center);
            previewAABB = ObjectBounds.CalcHierarchyWorldAABB(previewObject, _boundsQConfig);
            previewSphere.Center = previewSphereCenter;

            Transform camTransform = _renderCamera.transform;
            if (previewObjectType == PreviewObjectType.Mesh || previewObjectType == PreviewObjectType.Sprite)
            {
                camTransform.rotation = Quaternion.identity;
                if (previewObjectType != PreviewObjectType.Sprite) 
                    camTransform.rotation = Quaternion.AngleAxis(-45.0f, Vector3.up) * Quaternion.AngleAxis(35.0f, camTransform.right);
                camTransform.position = previewSphere.Center - camTransform.forward * (previewSphere.Radius * 2.0f + _renderCamera.nearClipPlane);             
            }
            else
            {
                camTransform.rotation = previewObjectTransform.rotation;
                camTransform.position = previewSphere.Center - camTransform.forward * (previewSphere.Radius * 2.0f + _renderCamera.nearClipPlane);

                Texture2D previewIcon = previewObjectType == PreviewObjectType.Light ? RTScene.Get.LookAndFeel.LightIcon : RTScene.Get.LookAndFeel.ParticleSystemIcon;
                _nonMeshPreviewObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", previewIcon);
            }

            _previewLight.transform.forward = camTransform.forward;
            _renderCamera.Render();
            if (previewObject != _nonMeshPreviewObject) GameObject.DestroyImmediate(previewObject);

            Texture2D previewTexture = new Texture2D(_previewLookAndFeel.PreviewWidth, _previewLookAndFeel.PreviewHeight, TextureFormat.ARGB32, true, true);
            previewTexture.ReadPixels(new Rect(0, 0, _previewLookAndFeel.PreviewWidth, _previewLookAndFeel.PreviewHeight), 0, 0);
            previewTexture.Apply();
            UnityEngine.RenderTexture.active = oldRenderTexture;

            return previewTexture;
        }

        private bool CreateRenderCamera()
        {
            RenderTexture renderTexture = new RenderTexture(_previewLookAndFeel.PreviewWidth, _previewLookAndFeel.PreviewHeight, 24);
            if (renderTexture == null || !renderTexture.Create()) return false;

            GameObject renderCameraObject = new GameObject("Render Camera");
            Camera renderCam = renderCameraObject.AddComponent<Camera>();

            renderCam.backgroundColor = _previewLookAndFeel.BkColor;
            renderCam.orthographic = false;
            renderCam.fieldOfView = 65.0f;
            renderCam.clearFlags = CameraClearFlags.Color;
            renderCam.nearClipPlane = 0.0001f;
            renderCam.targetTexture = renderTexture;

            _renderCamera = renderCam;
            return true;
        }

        private bool CreatePreviewLight()
        {
            GameObject lightObject = new GameObject("Preview light");
            _previewLight = lightObject.AddComponent<Light>();
            _previewLight.type = LightType.Directional;
            _previewLight.intensity = _previewLookAndFeel.LightIntensity;

            return true;
        }

        private void CreateNonMeshPreviewObject()
        {
            _nonMeshPreviewObject = new GameObject("Non-mesh preview object");
            var meshRenderer = _nonMeshPreviewObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = MaterialPool.Get.TintedTexture;
            var meshFilter = _nonMeshPreviewObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = MeshPool.Get.UnitQuadXY;
        }

        private void DisableSceneLights()
        {
            _lightToState.Clear();
            var sceneLights = GameObject.FindObjectsOfType<Light>();
            foreach (var light in sceneLights)
            {
                _lightToState.Add(light, light.enabled);
                light.enabled = false;
            }
        }

        private void RestoreSceneLights()
        {
            foreach(var pair in _lightToState)
            {
                Light light = pair.Key;
                if (light == null) continue;

                light.enabled = pair.Value;
            }
        }
    }
}