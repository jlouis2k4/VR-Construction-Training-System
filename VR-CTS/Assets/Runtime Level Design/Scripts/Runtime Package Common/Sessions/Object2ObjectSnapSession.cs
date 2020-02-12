using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    public delegate void Object2ObjectSnapSessionBeginHandler();
    public delegate void Object2ObjectSnapSessionEndHandler();

    public class Object2ObjectSnapSession
    {
        public event Object2ObjectSnapSessionBeginHandler SessionBegin;
        public event Object2ObjectSnapSessionEndHandler SessionEnd;

        private enum State
        {
            Inactive = 0,
            Active
        }

        private enum SitSurfaceType
        {
            Invalid = 0,
            Grid,
            Object
        }

        private struct SitSurface
        {
            public SitSurfaceType SurfaceType;
            public Vector3 SitPoint;
            public Plane SitPlane;
        }

        private State _state = State.Inactive;

        private List<GameObject> _targetObjects;
        private List<GameObject> _targetParents;
        private AABB _targetAABB;
        private SitSurface _sitSurface = new SitSurface();
        private bool _sitBelowSurface;

        private Object2ObjectSnapSettings _sharedSettings;
        private Object2ObjectSnapHotkeys _sharedHotkeys;

        private List<LocalTransformSnapshot> _preTargetTransformSnapshots;

        public Object2ObjectSnapSettings SharedSettings { get { return _sharedSettings; } set { if (value != null) _sharedSettings = value; } }
        public Object2ObjectSnapHotkeys SharedHotkeys { get { return _sharedHotkeys; } set { if (value != null) _sharedHotkeys = value; } }
        public bool IsActive { get { return _state != State.Inactive; } }

        public void Update(IEnumerable<GameObject> targetObjects)
        {
            if (SharedHotkeys == null || SharedSettings == null) return;

            if (_state == State.Inactive)
            {
                if (SharedHotkeys.ToggleSnap.IsActiveInFrame()) Begin(targetObjects);
            }
            else
            if (SharedHotkeys.ToggleSnap.IsActiveInFrame()) End();

            if (IsActive)
            {
                if (SharedHotkeys.ToggleSitBelowSurface.IsActiveInFrame()) _sitBelowSurface = !_sitBelowSurface;

                IInputDevice inputDevice = RTInputDevice.Get.Device;
                if (inputDevice.WasMoved())
                {
                    if (!IdentifySitSurface()) return;
                    SnapTargets();
                }
            }
        }

        public void End()
        {
            if (_state == State.Inactive) return;

            _targetObjects.Clear();
            _state = State.Inactive;

            var postObjectTransformChangedAction = new PostObjectTransformsChangedAction(_preTargetTransformSnapshots, LocalTransformSnapshot.GetSnapshotCollection(_targetParents));
            postObjectTransformChangedAction.Execute();
            _targetParents.Clear();

            if (SessionEnd != null) SessionEnd();
        }

        private bool Begin(IEnumerable<GameObject> targetObjects)
        {
            if (_state != State.Inactive || SharedSettings == null || SharedHotkeys == null || targetObjects == null) return false;

            _targetObjects = new List<GameObject>(targetObjects);
            if (_targetObjects.Count == 0) return false;
            _targetParents = GameObjectEx.FilterParentsOnly(_targetObjects);

            if (!CalculateTargetAABB() || !IdentifySitSurface())
            {
                _targetObjects.Clear();
                _targetParents.Clear();
                return false;
            }

            _state = State.Active;
            _preTargetTransformSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_targetParents);

            if (SessionBegin != null) SessionBegin();
            return true;
        }

        private void SnapTargets()
        {
            if (!CalculateTargetAABB()) return;

            Vector3 oldCenter = _targetAABB.Center;
            _targetAABB.Center = _sitSurface.SitPoint;

            Plane sitSurfacePlane = _sitSurface.SitPlane;
            if (_sitBelowSurface) sitSurfacePlane = sitSurfacePlane.InvertNormal();

            Vector3 sitOnPlaneOffset = ObjectSurfaceSnap.CalculateSitOnSurfaceOffset(_targetAABB, sitSurfacePlane, 0.0f);
            _targetAABB.Center += sitOnPlaneOffset;

            Vector3 parentMove = _targetAABB.Center - oldCenter;
            foreach (var parent in _targetParents)
                parent.transform.position += parentMove;

            if (!SharedHotkeys.EnableFlexiSnap.IsActive())
            {
                var snapConfig = new Object2ObjectSnap.Config();
                snapConfig.Prefs = SharedHotkeys.EnableMoreControl.IsActive() ? Object2ObjectSnap.Prefs.None : Object2ObjectSnap.Prefs.TryMatchArea;
                snapConfig.AreaMatchEps = 1e-2f;
                snapConfig.SnapRadius = SharedSettings.SnapRadius;
                snapConfig.DestinationLayers = SharedSettings.SnapDestinationLayers;
                snapConfig.IgnoreDestObjects = new List<GameObject>();
                snapConfig.IgnoreDestObjects.AddRange(_targetParents);
                foreach (var parent in _targetParents)
                    snapConfig.IgnoreDestObjects.AddRange(parent.GetAllChildren());

                Object2ObjectSnap.Snap(_targetParents, snapConfig);
            }
        }

        private bool CalculateTargetAABB()
        {
            var boundsQConfig = new ObjectBounds.QueryConfig();
            boundsQConfig.ObjectTypes = GameObjectType.Mesh | GameObjectType.Sprite;

            _targetAABB = ObjectBounds.CalcHierarchyCollectionWorldAABB(_targetParents, boundsQConfig);
            return _targetAABB.IsValid;
        }

        private bool IdentifySitSurface()
        {
            _sitSurface.SurfaceType = SitSurfaceType.Invalid;

            SceneRaycastFilter raycastFilter = new SceneRaycastFilter();
            if (SharedSettings.CanClimbObjects) raycastFilter.AllowedObjectTypes.Add(GameObjectType.Mesh);
            foreach (var target in _targetParents)
                raycastFilter.IgnoreObjects.AddRange(target.GetAllChildrenAndSelf());

            IInputDevice inputDevice = RTInputDevice.Get.Device;
            SceneRaycastHit raycastHit = RTScene.Get.Raycast(inputDevice.GetRay(RTFocusCamera.Get.TargetCamera), SceneRaycastPrecision.BestFit, raycastFilter);
            if (!raycastHit.WasAnythingHit) return false;

            if (raycastHit.WasAnObjectHit && raycastHit.WasGridHit)
            {
                if (raycastHit.ObjectHit.HitEnter < raycastHit.GridHit.HitEnter)
                {
                    _sitSurface.SitPlane = raycastHit.ObjectHit.HitPlane;
                    _sitSurface.SitPoint = raycastHit.ObjectHit.HitPoint;
                    _sitSurface.SurfaceType = SitSurfaceType.Object;
                }
                else
                {
                    _sitSurface.SitPlane = raycastHit.GridHit.HitPlane;
                    _sitSurface.SitPoint = raycastHit.GridHit.HitPoint;
                    _sitSurface.SurfaceType = SitSurfaceType.Grid;
                }
            }
            else
            if (raycastHit.WasAnObjectHit)
            {
                _sitSurface.SitPlane = raycastHit.ObjectHit.HitPlane;
                _sitSurface.SitPoint = raycastHit.ObjectHit.HitPoint;
                _sitSurface.SurfaceType = SitSurfaceType.Object;
            }
            else
            if (raycastHit.WasGridHit)
            {
                _sitSurface.SitPlane = raycastHit.GridHit.HitPlane;
                _sitSurface.SitPoint = raycastHit.GridHit.HitPoint;
                _sitSurface.SurfaceType = SitSurfaceType.Grid;
            }

            return true;
        }
    } 
}
