using UnityEngine;

namespace RLD
{
    public static class ObjectSpawnUtil
    {
        private static Config _defaultConfig = new Config()
        {
            ObjectSize = 0.3f
        };

        public struct Config
        {
            public float ObjectSize;
        }

        public static Config DefaultConfig { get { return _defaultConfig; } }

        public static GameObject SpawnInFrontOfCamera(GameObject sourceObject, Camera camera, Config config)
        {
            float halfSize = config.ObjectSize * 0.5f;

            var boundsQConfig = new ObjectBounds.QueryConfig();
            boundsQConfig.ObjectTypes = GameObjectTypeHelper.AllCombined;
            boundsQConfig.NoVolumeSize = Vector3Ex.FromValue(1.0f);

            Transform cameraTransform = camera.transform;
            AABB aabb = ObjectBounds.CalcHierarchyWorldAABB(sourceObject, boundsQConfig);
            if (!aabb.IsValid) return null;

            Sphere sphere = new Sphere(aabb);
            Vector3 fromCenterToPos = sourceObject.transform.position - sphere.Center;
            float zOffset = Mathf.Max(camera.nearClipPlane + sphere.Radius, sphere.Radius / halfSize);
            Vector3 spherePos = cameraTransform.position + cameraTransform.forward * zOffset;

            GameObject spawned = GameObject.Instantiate(sourceObject, spherePos + fromCenterToPos, sourceObject.transform.rotation) as GameObject;
            spawned.SetActive(true);
            OBB spawnedOBB = ObjectBounds.CalcHierarchyWorldOBB(spawned, boundsQConfig);
            Ray ray = new Ray(camera.transform.position, (spawnedOBB.Center - camera.transform.position).normalized);
            var raycastFilter = new SceneRaycastFilter();
            raycastFilter.AllowedObjectTypes.Add(GameObjectType.Mesh);
            raycastFilter.AllowedObjectTypes.Add(GameObjectType.Terrain);
            raycastFilter.AllowedObjectTypes.Add(GameObjectType.Sprite);

            var rayHit = RTScene.Get.Raycast(ray, SceneRaycastPrecision.BestFit, raycastFilter);
            if (rayHit.WasAnObjectHit)
            {
                Vector3 oldCenter = spawnedOBB.Center;
                spawnedOBB.Center = rayHit.ObjectHit.HitPoint;
                Vector3 offsetVector = spawnedOBB.Center - oldCenter;
                offsetVector += ObjectSurfaceSnap.CalculateSitOnSurfaceOffset(spawnedOBB, rayHit.ObjectHit.HitPlane, 0.0f);

                spawned.transform.position += offsetVector;
            }

            return spawned;
        }
    }
}
