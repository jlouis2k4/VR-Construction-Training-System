using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    public delegate void ObjectGrabSessionBeginHandler();
    public delegate void ObjectGrabSessionEndHandler();

    public class DeviceObjectGrabSession
    {
        public event ObjectGrabSessionBeginHandler SessionBegin;
        public event ObjectGrabSessionEndHandler SessionEnd;

        private enum State
        {
            Inactive = 0,
            ActiveSnapToSurface,
            ActiveRotate,
            ActiveRotateAroundAnchor,
            ActiveScale,
            ActiveOffsetFromSurface,
            ActiveAnchorAdjust,
            ActiveOffsetFromAnchor
        }

        public enum GrabSurfaceType
        {
            Invalid = 0,
            Mesh,
            SphericalMesh,
            UnityTerrain,
            TerrainMesh,
            Grid
        }

        private struct GrabSurfaceInfo
        {
            public GrabSurfaceType SurfaceType;
            public Vector3 AnchorPoint;
            public Vector3 AnchorNormal;
            public Plane AnchorPlane;
            public SceneRaycastHit SceneRaycastHit;
        }

        private class GrabTarget
        {
            private GameObject _gameObject;
            private Transform _transform;

            public GameObject GameObject { get { return _gameObject; } }
            public Transform Transform { get { return _transform; } }
            public Vector3 AnchorVector;
            public Vector3 WorldScaleSnapshot;
            public Vector3 AnchorVectorSnapshot;
            public Plane SittingPlane;
            public Vector3 SittingPoint;
            public float OffsetFromSurface;

            public GrabTarget(GameObject parentObject)
            {
                _gameObject = parentObject;
                _transform = _gameObject.transform;
            }
        }

        private State _state = State.Inactive;

        private ObjectGrabSettings _sharedSettings;
        private ObjectGrabHotkeys _sharedHotkeys;
        private ObjectGrabLookAndFeel _sharedLookAndFeel;

        private List<GameObject> _targetParents = new List<GameObject>();
        private List<GrabTarget> _grabTargets = new List<GrabTarget>();
        private GrabSurfaceInfo _grabSurfaceInfo = new GrabSurfaceInfo();
        private int _deltaCaptureId;

        private TransformAxis[] _possibleAlignmentAxes = new TransformAxis[] 
        {   
            TransformAxis.PositiveX,
            TransformAxis.NegativeX,
            TransformAxis.PositiveY,
            TransformAxis.NegativeY,
            TransformAxis.PositiveZ,
            TransformAxis.NegativeZ
        };

        private List<LocalTransformSnapshot> _preTargetTransformSnapshots;

        public ObjectGrabSettings SharedSettings { get { return _sharedSettings; } set { if (value != null) _sharedSettings = value; } }
        public ObjectGrabHotkeys SharedHotkeys { get { return _sharedHotkeys; } set { if (value != null) _sharedHotkeys = value; } }
        public ObjectGrabLookAndFeel SharedLookAndFeel { get { return _sharedLookAndFeel; } set { if (value != null) _sharedLookAndFeel = value; } }
        public bool IsActive { get { return _state != State.Inactive; } }

        public void Render()
        {
            if (SharedLookAndFeel == null) return;

            if (IsActive && _grabSurfaceInfo.SurfaceType != GrabSurfaceType.Invalid)
            {
                Material material = MaterialPool.Get.SimpleColor;
                if (SharedLookAndFeel.DrawAnchorLines)
                {
                    var linePoints = new List<Vector3>(_grabTargets.Count * 2);
                    foreach (var grabTarget in _grabTargets)
                    {
                        linePoints.Add(grabTarget.Transform.position);
                        linePoints.Add(_grabSurfaceInfo.AnchorPoint);
                    }

                    material.SetZTestAlways();
                    material.SetColor(_sharedLookAndFeel.AnchorLineColor);
                    material.SetPass(0);
                    GLRenderer.DrawLines3D(linePoints);
                }

                if (SharedLookAndFeel.DrawObjectBoxes)
                {
                    material.SetZTestLess();
                    material.SetColor(SharedLookAndFeel.ObjectBoxWireColor);
                    material.SetPass(0);

                    var boundsQConfig = GetObjectBoundsQConfig();
                    foreach (var grabTarget in _grabTargets)
                    {
                        OBB obb = ObjectBounds.CalcHierarchyWorldOBB(grabTarget.GameObject, boundsQConfig);
                        if (obb.IsValid) GraphicsEx.DrawWireBox(obb);
                    }
                }

                if (SharedLookAndFeel.DrawObjectPosTicks)
                {
                    material.SetColor(SharedLookAndFeel.ObjectPosTickColor);
                    material.SetPass(0);

                    foreach (var grabTarget in _grabTargets)
                    {
                        Vector2 screenPos = Camera.current.WorldToScreenPoint(grabTarget.Transform.position);
                        GLRenderer.DrawRect2D(RectEx.FromCenterAndSize(screenPos, Vector2Ex.FromValue(SharedLookAndFeel.ObjectPosTickSize)), Camera.current);
                    }
                }

                if (SharedLookAndFeel.DrawAnchorPosTick)
                {
                    material.SetColor(SharedLookAndFeel.AnchorPosTickColor);
                    material.SetPass(0);

                    Vector2 anchorScreenPos = Camera.current.WorldToScreenPoint(_grabSurfaceInfo.AnchorPoint);
                    GLRenderer.DrawRect2D(RectEx.FromCenterAndSize(anchorScreenPos, Vector2Ex.FromValue(SharedLookAndFeel.AnchorPosTickSize)), Camera.current);
                }
            }
        }

        public void Update(IEnumerable<GameObject> targetObjects)
        {
            if (SharedHotkeys == null || SharedSettings == null) return;

            if (_state == State.Inactive)
            {
                if (_sharedHotkeys.ToggleGrab.IsActiveInFrame()) Begin(targetObjects);
            }
            else
            if (_sharedHotkeys.ToggleGrab.IsActiveInFrame()) End();

            if (IsActive)
            {
                State oldState = _state;

                IInputDevice inputDevice = RTInputDevice.Get.Device;
                if (SharedHotkeys.EnableOffsetFromAnchor.IsActive())
                {
                    if (_state != State.ActiveOffsetFromAnchor &&
                        inputDevice.CreateDeltaCapture(inputDevice.GetPositionYAxisUp(), out _deltaCaptureId))
                    {
                        StoreGrabTargetsAnchorVectorSnapshots();
                        _state = State.ActiveOffsetFromAnchor;
                    }
                }
                else if (SharedHotkeys.EnableAnchorAdjust.IsActive()) _state = State.ActiveAnchorAdjust;
                else if (SharedHotkeys.EnableRotation.IsActive()) _state = State.ActiveRotate;
                else if (SharedHotkeys.EnableRotationAroundAnchor.IsActive()) _state = State.ActiveRotateAroundAnchor;
                else if (SharedHotkeys.EnableScaling.IsActive())
                {
                    if (_state != State.ActiveScale &&
                        inputDevice.CreateDeltaCapture(inputDevice.GetPositionYAxisUp(), out _deltaCaptureId))
                    {
                        StoreGrabTargetsWorldScaleSnapshots();
                        _state = State.ActiveScale;
                    }
                }
                else if (SharedHotkeys.EnableOffsetFromSurface.IsActive()) _state = State.ActiveOffsetFromSurface;
                else _state = State.ActiveSnapToSurface;

                if (_state != State.ActiveScale && 
                    _state != State.ActiveOffsetFromAnchor) inputDevice.RemoveDeltaCapture(_deltaCaptureId);

                if (_state != State.ActiveOffsetFromAnchor && 
                    _state != State.ActiveRotateAroundAnchor)
                {
                    if (!IdentifyGrabSurface()) return;
                }

                if ((oldState == State.ActiveOffsetFromAnchor && _state != State.ActiveOffsetFromAnchor) ||
                    (oldState == State.ActiveRotateAroundAnchor && _state != State.ActiveRotateAroundAnchor))
                {
                    CalculateGrabTargetsAnchorVectors();
                }


                if (_state == State.ActiveSnapToSurface && 
                    SharedHotkeys.NextAlignmentAxis.IsActiveInFrame())
                {
                    SwitchToNextAlignmentAxis();
                }

                if (RTInputDevice.Get.Device.WasMoved())
                {
                    if (_state == State.ActiveOffsetFromAnchor) OffsetTargetsFromAnchor();
                    else if (_state == State.ActiveAnchorAdjust) CalculateGrabTargetsAnchorVectors();
                    else if (_state == State.ActiveSnapToSurface) SnapTargetsToSurface();
                    else if (_state == State.ActiveRotate) RotateTargets();
                    else if (_state == State.ActiveRotateAroundAnchor)  RotateTargetsAroundAnchor();
                    else if (_state == State.ActiveScale) ScaleTargets();
                    else if (_state == State.ActiveOffsetFromSurface) OffsetTargetsFromSurface();
                }
            }
        }

        public void End()
        {
            if (_state == State.Inactive) return;

            _grabTargets.Clear();
            _state = State.Inactive;
            _grabSurfaceInfo.SurfaceType = GrabSurfaceType.Invalid;

            var postObjectTransformChangedAction = new PostObjectTransformsChangedAction(_preTargetTransformSnapshots, LocalTransformSnapshot.GetSnapshotCollection(_targetParents));
            postObjectTransformChangedAction.Execute();
            _targetParents.Clear();

            if (SessionEnd != null) SessionEnd();
        }

        private bool Begin(IEnumerable<GameObject> targetObjects)
        {
            if (_state != State.Inactive || SharedSettings == null || _sharedHotkeys == null || targetObjects == null) return false;
            if ((int)SharedSettings.SurfaceFlags == 0) return false;

            if (!IdentifyGrabTargets(targetObjects)) return false;
            if (!IdentifyGrabSurface())
            {
                _grabTargets.Clear();
                return false;
            }
            CalculateGrabTargetsAnchorVectors();
     
            _state = State.ActiveSnapToSurface;
            _preTargetTransformSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_targetParents);

            SnapTargetsToSurface();
            CalculateGrabTargetsAnchorVectors();

            if (SessionBegin != null) SessionBegin();
            return true;
        }

        private void SnapTargetsToSurface()
        {
            if (_grabSurfaceInfo.SurfaceType == GrabSurfaceType.Invalid) return;

            ObjectSurfaceSnap.SnapConfig snapConfig = new ObjectSurfaceSnap.SnapConfig();
            snapConfig.SurfaceHitPoint = _grabSurfaceInfo.AnchorPoint;
            snapConfig.SurfaceHitNormal = _grabSurfaceInfo.AnchorNormal;
            snapConfig.SurfaceHitPlane = _grabSurfaceInfo.AnchorPlane;
            snapConfig.SurfaceObject = _grabSurfaceInfo.SceneRaycastHit.WasAnObjectHit ? _grabSurfaceInfo.SceneRaycastHit.ObjectHit.HitObject : null;

            snapConfig.SurfaceType = ObjectSurfaceSnap.Type.UnityTerrain;
            if (_grabSurfaceInfo.SurfaceType == GrabSurfaceType.Mesh) snapConfig.SurfaceType = ObjectSurfaceSnap.Type.Mesh;
            else if (_grabSurfaceInfo.SurfaceType == GrabSurfaceType.Grid) snapConfig.SurfaceType = ObjectSurfaceSnap.Type.SceneGrid;
            else if (_grabSurfaceInfo.SurfaceType == GrabSurfaceType.SphericalMesh) snapConfig.SurfaceType = ObjectSurfaceSnap.Type.SphericalMesh;
            else if (_grabSurfaceInfo.SurfaceType == GrabSurfaceType.TerrainMesh) snapConfig.SurfaceType = ObjectSurfaceSnap.Type.TerrainMesh;

            foreach (var grabTarget in _grabTargets)
            {
                if (grabTarget.GameObject == null) continue;
                grabTarget.Transform.position = _grabSurfaceInfo.AnchorPoint + grabTarget.AnchorVector;

                var layerGrabSettings = SharedSettings.GetLayerGrabSettings(grabTarget.GameObject.layer);
                if (layerGrabSettings.IsActive)
                {
                    snapConfig.AlignAxis = layerGrabSettings.AlignAxis;
                    snapConfig.AlignmentAxis = layerGrabSettings.AlignmentAxis;
                    snapConfig.OffsetFromSurface = layerGrabSettings.DefaultOffsetFromSurface + grabTarget.OffsetFromSurface;
                }
                else
                {
                    snapConfig.AlignAxis = SharedSettings.AlignAxis;
                    snapConfig.AlignmentAxis = SharedSettings.AlignmentAxis;
                    snapConfig.OffsetFromSurface = SharedSettings.DefaultOffsetFromSurface + grabTarget.OffsetFromSurface;
                }

                var snapResult = ObjectSurfaceSnap.SnapHierarchy(grabTarget.GameObject, snapConfig);
                if (snapResult.Success)
                {
                    grabTarget.SittingPlane = snapResult.SittingPlane;
                    grabTarget.SittingPoint = snapResult.SittingPoint;
                }
            }
        }

        private void RotateTargets()
        {
            IInputDevice inputDevice = RTInputDevice.Get.Device;
            if (!inputDevice.WasMoved()) return;

            float rotationAmount = inputDevice.GetFrameDelta().x * SharedSettings.RotationSensitivity;
            foreach (var grabTarget in _grabTargets)
            {
                if (grabTarget == null) continue;

                var layerGrabSettings = SharedSettings.GetLayerGrabSettings(grabTarget.GameObject.layer);
                if (layerGrabSettings.IsActive)
                {
                    if (layerGrabSettings.AlignAxis) grabTarget.Transform.Rotate(grabTarget.SittingPlane.normal, rotationAmount, Space.World);
                    else grabTarget.Transform.Rotate(Vector3.up, rotationAmount, Space.World);
                }
                else
                {
                    if (SharedSettings.AlignAxis) grabTarget.Transform.Rotate(grabTarget.SittingPlane.normal, rotationAmount, Space.World);
                    else grabTarget.Transform.Rotate(Vector3.up, rotationAmount, Space.World);
                }
            }

            CalculateGrabTargetsAnchorVectors();
        }

        private void RotateTargetsAroundAnchor()
        {
            IInputDevice inputDevice = RTInputDevice.Get.Device;
            if (!inputDevice.WasMoved()) return;

            float rotationAmount = inputDevice.GetFrameDelta().x * SharedSettings.RotationSensitivity;
            foreach(var grabTarget in _grabTargets)
            {
                if (grabTarget == null) continue;

                var layerGrabSettings = SharedSettings.GetLayerGrabSettings(grabTarget.GameObject.layer);
                if (layerGrabSettings.IsActive)
                {
                    if (layerGrabSettings.AlignAxis)
                    {
                        Quaternion rotation = Quaternion.AngleAxis(rotationAmount, _grabSurfaceInfo.AnchorNormal);
                        grabTarget.Transform.RotateAroundPivot(rotation, _grabSurfaceInfo.AnchorPoint);
                        grabTarget.AnchorVector = rotation * grabTarget.AnchorVector;
                    }
                    else
                    {
                        Quaternion rotation = Quaternion.AngleAxis(rotationAmount, Vector3.up);
                        grabTarget.Transform.RotateAroundPivot(rotation, _grabSurfaceInfo.AnchorPoint);
                        grabTarget.AnchorVector = rotation * grabTarget.AnchorVector;
                    }
                }
                else
                {
                    if (SharedSettings.AlignAxis)
                    {
                        Quaternion rotation = Quaternion.AngleAxis(rotationAmount, _grabSurfaceInfo.AnchorNormal);
                        grabTarget.Transform.RotateAroundPivot(rotation, _grabSurfaceInfo.AnchorPoint);
                        grabTarget.AnchorVector = rotation * grabTarget.AnchorVector;
                    }
                    else
                    {
                        Quaternion rotation = Quaternion.AngleAxis(rotationAmount, Vector3.up);
                        grabTarget.Transform.RotateAroundPivot(rotation, _grabSurfaceInfo.AnchorPoint);
                        grabTarget.AnchorVector = rotation * grabTarget.AnchorVector;
                    }
                }
            }

            SnapTargetsToSurface();
        }

        private void ScaleTargets()
        {
            IInputDevice inputDevice = RTInputDevice.Get.Device;
            if (!inputDevice.WasMoved()) return;

            foreach (var grabTarget in _grabTargets)
            {
                if (grabTarget == null) continue;

                float scaleFactor = 1.0f + inputDevice.GetCaptureDelta(_deltaCaptureId).x * SharedSettings.ScaleSensitivity;
                Vector3 newScale = grabTarget.WorldScaleSnapshot * scaleFactor;
                grabTarget.GameObject.SetHierarchyWorldScaleByPivot(newScale, grabTarget.SittingPoint + grabTarget.SittingPlane.normal * grabTarget.OffsetFromSurface);
            }

            CalculateGrabTargetsAnchorVectors();
        }

        private void OffsetTargetsFromSurface()
        {
            IInputDevice inputDevice = RTInputDevice.Get.Device;
            if (!inputDevice.WasMoved()) return;

            float offsetAmount = inputDevice.GetFrameDelta().x * SharedSettings.OffsetFromSurfaceSensitivity;
            foreach (var grabTarget in _grabTargets)
            {
                if (grabTarget == null) continue;

                var layerGrabSettings = SharedSettings.GetLayerGrabSettings(grabTarget.GameObject.layer);
                if (layerGrabSettings.IsActive)
                {
                    if (layerGrabSettings.AlignAxis)
                    {
                        grabTarget.Transform.position += grabTarget.SittingPlane.normal * offsetAmount;
                        grabTarget.OffsetFromSurface += offsetAmount;
                    }
                    else
                    {
                        grabTarget.Transform.position += Vector3.up * offsetAmount;
                        grabTarget.OffsetFromSurface += offsetAmount;
                    }
                }
                else
                {
                    if (SharedSettings.AlignAxis)
                    {
                        grabTarget.Transform.position += grabTarget.SittingPlane.normal * offsetAmount;
                        grabTarget.OffsetFromSurface += offsetAmount;
                    }
                    else
                    {
                        grabTarget.Transform.position += Vector3.up * offsetAmount;
                        grabTarget.OffsetFromSurface += offsetAmount;
                    }
                }
            }

            CalculateGrabTargetsAnchorVectors();
        }

        private void OffsetTargetsFromAnchor()
        {
            IInputDevice inputDevice = RTInputDevice.Get.Device;
            if (!inputDevice.WasMoved()) return;

            float scaleFactor = 1.0f + inputDevice.GetCaptureDelta(_deltaCaptureId).x * SharedSettings.OffsetFromAnchorSensitivity;
            foreach (var grabTarget in _grabTargets)
            {
                if (grabTarget == null) continue;
                grabTarget.Transform.position = (_grabSurfaceInfo.AnchorPoint + grabTarget.AnchorVectorSnapshot * scaleFactor);
            }

            CalculateGrabTargetsAnchorVectors();
            SnapTargetsToSurface();
        }

        private bool IdentifyGrabTargets(IEnumerable<GameObject> targetObjects)
        {
            _targetParents = GameObjectEx.FilterParentsOnly(targetObjects);
            if (_targetParents == null || _targetParents.Count == 0) return false;

            _grabTargets.Clear();
            foreach (var targetObject in _targetParents)
            {
                if (targetObject.HierarchyHasObjectsOfType(GameObjectType.Terrain)) continue;
                _grabTargets.Add(new GrabTarget(targetObject));
            }

            return _grabTargets.Count != 0;
        }

        private void CalculateGrabTargetsAnchorVectors()
        {
            foreach (var grabTarget in _grabTargets)
                grabTarget.AnchorVector = grabTarget.Transform.position - _grabSurfaceInfo.AnchorPoint;
        }

        private void StoreGrabTargetsWorldScaleSnapshots()
        {
            foreach (var grabTarget in _grabTargets)
                grabTarget.WorldScaleSnapshot = grabTarget.Transform.lossyScale;
        }

        private void StoreGrabTargetsAnchorVectorSnapshots()
        {
            foreach (var grabTarget in _grabTargets)
                grabTarget.AnchorVectorSnapshot = grabTarget.AnchorVector;
        }

        private bool IdentifyGrabSurface()
        {
            _grabSurfaceInfo.SurfaceType = GrabSurfaceType.Invalid;

            SceneRaycastFilter raycastFilter = new SceneRaycastFilter();
            raycastFilter.LayerMask = SharedSettings.SurfaceLayers;
            if ((SharedSettings.SurfaceFlags & ObjectGrabSurfaceFlags.Mesh) != 0) raycastFilter.AllowedObjectTypes.Add(GameObjectType.Mesh);
            if ((SharedSettings.SurfaceFlags & ObjectGrabSurfaceFlags.Terrain) != 0) raycastFilter.AllowedObjectTypes.Add(GameObjectType.Terrain);
            foreach (var grabTarget in _grabTargets)
                raycastFilter.IgnoreObjects.AddRange(grabTarget.GameObject.GetAllChildrenAndSelf());

            IInputDevice inputDevice = RTInputDevice.Get.Device;
            SceneRaycastHit raycastHit = RTScene.Get.Raycast(inputDevice.GetRay(RTFocusCamera.Get.TargetCamera), SceneRaycastPrecision.BestFit, raycastFilter);
            if (!raycastHit.WasAnythingHit) return false;

            _grabSurfaceInfo.SceneRaycastHit = raycastHit;
            if (raycastHit.WasAnObjectHit)
            {
                _grabSurfaceInfo.AnchorNormal = raycastHit.ObjectHit.HitNormal;
                _grabSurfaceInfo.AnchorPoint = raycastHit.ObjectHit.HitPoint;
                _grabSurfaceInfo.AnchorPlane = raycastHit.ObjectHit.HitPlane;
         
                GameObjectType hitObjectType = raycastHit.ObjectHit.HitObject.GetGameObjectType();
                if (hitObjectType == GameObjectType.Mesh)
                {
                    _grabSurfaceInfo.SurfaceType = GrabSurfaceType.Mesh;

                    int objectLayer = raycastHit.ObjectHit.HitObject.layer;
                    if (LayerEx.IsLayerBitSet(SharedSettings.SphericalMeshLayers, objectLayer)) _grabSurfaceInfo.SurfaceType = GrabSurfaceType.SphericalMesh;
                    else if (LayerEx.IsLayerBitSet(SharedSettings.TerrainMeshLayers, objectLayer)) _grabSurfaceInfo.SurfaceType = GrabSurfaceType.TerrainMesh;
                }
                else _grabSurfaceInfo.SurfaceType = GrabSurfaceType.UnityTerrain;
            }
            else
            if (raycastHit.WasGridHit && (SharedSettings.SurfaceFlags & ObjectGrabSurfaceFlags.Grid) != 0)
            {
                _grabSurfaceInfo.AnchorNormal = raycastHit.GridHit.HitNormal;
                _grabSurfaceInfo.AnchorPoint = raycastHit.GridHit.HitPoint;
                _grabSurfaceInfo.AnchorPlane = raycastHit.GridHit.HitPlane;
                _grabSurfaceInfo.SurfaceType = GrabSurfaceType.Grid;
            }

            return true;
        }

        private void SwitchToNextAlignmentAxis()
        {
            if (!SharedSettings.AlignAxis) return;

            int axisIndex = (int)SharedSettings.AlignmentAxis;
            if (axisIndex + 1 == _possibleAlignmentAxes.Length) SharedSettings.AlignmentAxis = _possibleAlignmentAxes[0];
            else
            {
                ++axisIndex;
                SharedSettings.AlignmentAxis = _possibleAlignmentAxes[axisIndex];
            }

            foreach (var grabTarget in _grabTargets)
            {
                if (grabTarget.GameObject != null)
                    grabTarget.Transform.rotation = Quaternion.identity;
            }

            SnapTargetsToSurface();
            CalculateGrabTargetsAnchorVectors();
        }

        private ObjectBounds.QueryConfig GetObjectBoundsQConfig()
        {
            var boundsQConfig = new ObjectBounds.QueryConfig();
            boundsQConfig.NoVolumeSize = Vector3.zero;
            boundsQConfig.ObjectTypes = GameObjectType.Mesh | GameObjectType.Sprite;

            return boundsQConfig;
        }
    }
}
