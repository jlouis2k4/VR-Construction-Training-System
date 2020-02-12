using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    public delegate void ObjectGridSnapSessionBeginHandler();
    public delegate void ObjectGridSnapSessionEndHandler();

    public class ObjectGridSnapSession
    {
        public event ObjectGridSnapSessionBeginHandler SessionBegin;
        public event ObjectGridSnapSessionEndHandler SessionEnd;

        private enum State
        {
            Inactive = 0,
            SelectPivot,
            Snap
        }

        private List<GameObject> _targetParents = new List<GameObject>();
        private List<GameObject> _targetObjects = new List<GameObject>();
        private List<LocalTransformSnapshot> _preTargetTransformSnapshots;
        private Vector3 _snapPivotPoint;

        private State _state = State.Inactive;

        private ObjectGridSnapHotkeys _sharedHotkeys;
        private ObjectGridSnapLookAndFeel _sharedLookAndFeel;

        public bool IsActive { get { return _state != State.Inactive; } }
        public ObjectGridSnapLookAndFeel SharedLookAndFeel { get { return _sharedLookAndFeel; } set { if (value != null) _sharedLookAndFeel = value; } }
        public ObjectGridSnapHotkeys SharedHotkeys { get { return _sharedHotkeys; } set { if (value != null) _sharedHotkeys = value; } }

        public void Render()
        {
            if (_sharedLookAndFeel == null) return;

            if(IsActive)
            {
                Material material = MaterialPool.Get.SimpleColor;

                if(_sharedLookAndFeel.DrawBoxes)
                {
                    material.SetColor(_sharedLookAndFeel.BoxLineColor);
                    material.SetZTestEnabled(true);
                    material.SetPass(0);

                    var boundsQConfig = GetObjectBoundsQConfig();
                    foreach (var targetObject in _targetObjects)
                    {
                        if (targetObject == null) continue;

                        OBB worldOBB = ObjectBounds.CalcWorldOBB(targetObject, boundsQConfig);
                        if (worldOBB.IsValid) GraphicsEx.DrawWireBox(worldOBB);
                    }
                }

                Camera camera = Camera.current;
                Vector2 screenSnapPivot = camera.WorldToScreenPoint(_snapPivotPoint);
                if (_sharedLookAndFeel.PivotShapeType == PivotPointShapeType.Circle)
                {
                    material.SetZTestEnabled(false);
                    material.SetColor(_sharedLookAndFeel.PivotPointFillColor);
                    material.SetPass(0);

                    const int numCirclePoints = 100;
                    List<Vector2> pivotCirclePoints = PrimitiveFactory.Generate2DCircleBorderPointsCW(screenSnapPivot, _sharedLookAndFeel.PivotCircleRadius, numCirclePoints);
                    GLRenderer.DrawTriangleFan2D(screenSnapPivot, pivotCirclePoints, camera);

                    if(_sharedLookAndFeel.DrawPivotBorder)
                    {
                        material.SetColor(_sharedLookAndFeel.PivotPointBorderColor);
                        material.SetPass(0);
                        GLRenderer.DrawLineLoop2D(pivotCirclePoints, camera);
                    }
                }
                else
                if(_sharedLookAndFeel.PivotShapeType == PivotPointShapeType.Square)
                {
                    material.SetZTestEnabled(false);
                    material.SetColor(_sharedLookAndFeel.PivotPointFillColor);
                    material.SetPass(0);

                    Rect pivotRect = RectEx.FromCenterAndSize(screenSnapPivot, Vector2Ex.FromValue(_sharedLookAndFeel.PivotSquareSideLength));
                    GLRenderer.DrawRect2D(pivotRect, camera);

                    if (_sharedLookAndFeel.DrawPivotBorder)
                    {
                        material.SetColor(_sharedLookAndFeel.PivotPointBorderColor);
                        material.SetPass(0);
                        GLRenderer.DrawRectBorder2D(pivotRect, camera);
                    }
                }
            }
        }

        public void Update(IEnumerable<GameObject> targetObjects)
        {
            if (_sharedHotkeys == null) return;

            if (_state == State.Inactive)
            {
                if (_sharedHotkeys.BeginGridSnap.IsActive()) Begin(targetObjects);
            }
            else if (!_sharedHotkeys.BeginGridSnap.IsActive()) End();

            if(_state != State.Inactive)
            {
                if (RTInputDevice.Get.Device.IsButtonPressed(0)) _state = State.Snap;
                else _state = State.SelectPivot;

                if (_state == State.SelectPivot) SelectPivot();
                else if (_state == State.Snap) Snap();
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
            if (_state != State.Inactive || _sharedHotkeys == null || targetObjects == null) return false;

            if (!IdentifyTargetParents(targetObjects) ||
                !IdentifyTargetObjects(targetObjects))
            {
                _targetParents.Clear();
                _targetObjects.Clear();
                return false;
            }

            _state = State.SelectPivot;
            _preTargetTransformSnapshots = LocalTransformSnapshot.GetSnapshotCollection(_targetParents);

            if (SessionBegin != null) SessionBegin();
            return true;
        }

        private bool IdentifyTargetParents(IEnumerable<GameObject> targetObjects)
        {
            _targetParents = GameObjectEx.FilterParentsOnly(targetObjects);
            if (_targetParents == null || _targetParents.Count == 0) return false;

            return true;
        }

        private bool IdentifyTargetObjects(IEnumerable<GameObject> targetObjects)
        {
            _targetObjects.Clear();

            foreach(var targetObject in targetObjects)
            {
                if (targetObject == null) continue;

                Mesh mesh = targetObject.GetMesh();
                if (mesh == null)
                {
                    Sprite sprite = targetObject.GetSprite();
                    if (sprite == null) continue;
                }

                _targetObjects.Add(targetObject);
            }

            return _targetObjects.Count != 0;
        }

        private void SelectPivot()
        {
            if (!RTInputDevice.Get.Device.HasPointer()) return;

            var boundsQConfig = GetObjectBoundsQConfig();

            Vector2 inputDevicePos = RTInputDevice.Get.Device.GetPositionYAxisUp();
            float minDistFromDevice = float.MaxValue;
            foreach(var targetObject in _targetObjects)
            {
                if (targetObject == null) continue;

                OBB worldOBB = ObjectBounds.CalcWorldOBB(targetObject, boundsQConfig);
                if (worldOBB.IsValid)
                {
                    Camera camera = RTFocusCamera.Get.TargetCamera;
                    List<Vector3> centerAndCorners = worldOBB.GetCenterAndCornerPoints();
                    List<Vector2> screenCenterAndCorners = camera.ConvertWorldToScreenPoints(centerAndCorners);
                    for(int ptIndex = 0; ptIndex < screenCenterAndCorners.Count; ++ptIndex)
                    {
                        float distance = (inputDevicePos - screenCenterAndCorners[ptIndex]).magnitude;
                        if(distance < minDistFromDevice)
                        {
                            minDistFromDevice = distance;
                            _snapPivotPoint = centerAndCorners[ptIndex];
                        }
                    }
                }
            }
        }

        private ObjectBounds.QueryConfig GetObjectBoundsQConfig()
        {
            var boundsQConfig = new ObjectBounds.QueryConfig();
            boundsQConfig.ObjectTypes = GameObjectType.Mesh | GameObjectType.Sprite;
            return boundsQConfig;
        }

        private void Snap()
        {
            Camera camera = RTFocusCamera.Get.TargetCamera;
            XZGridRayHit sceneGridHit = RTScene.Get.RaycastSceneGridIfVisible(RTInputDevice.Get.Device.GetRay(camera));
            if (sceneGridHit == null) return;

            List<Vector3> hitCellPoints = sceneGridHit.HitCell.GetCenterAndCorners();
            int destPtIndex = Vector3Ex.GetPointClosestToPoint(hitCellPoints, sceneGridHit.HitPoint);
            if (destPtIndex < 0) return;

            Vector3 snapDestination = hitCellPoints[destPtIndex];
            Vector3 snapVector = (snapDestination - _snapPivotPoint);
            foreach (var targetParent in _targetParents)
            {
                if (targetParent == null) continue;

                Transform targetParentTransform = targetParent.transform;
                targetParentTransform.position += snapVector;
            }

            _snapPivotPoint += snapVector;
        }
    }
}
