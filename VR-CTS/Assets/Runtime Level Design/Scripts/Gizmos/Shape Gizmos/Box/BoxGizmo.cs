using UnityEngine;
using System;
using System.Collections.Generic;

namespace RLD
{
    [Serializable]
    public class BoxGizmo : GizmoBehaviour
    {
        public enum Usage
        {
            Generic = 0,
            ObjectScale
        }

        private Usage _usage = Usage.Generic;
        private bool _isUsagePermanent;
        private Vector3 _boxSize;
        private GameObject _targetHierarchy;
        private Transform _targetHierarchyTransform;
        private LocalTransformSnapshot _dragBeginTargetTransformSnapshot = new LocalTransformSnapshot();

        private GizmoCap2D _rightTick;
        private GizmoCap2D _topTick;
        private GizmoCap2D _backTick;
        private GizmoCap2D _leftTick;
        private GizmoCap2D _bottomTick;
        private GizmoCap2D _frontTick;
        private GizmoCap2DCollection _ticks = new GizmoCap2DCollection();

        private bool _scaleFromCenter;
        private Vector3 _scalePivot;
        private GizmoSglAxisScaleDrag3D.WorkData _scaleDragWorkData = new GizmoSglAxisScaleDrag3D.WorkData();
        private GizmoSglAxisScaleDrag3D _scaleDrag = new GizmoSglAxisScaleDrag3D();

        [SerializeField]
        private BoxGizmoSettings3D _settings3D = new BoxGizmoSettings3D();
        private BoxGizmoSettings3D _sharedSettings3D;

        [SerializeField]
        private BoxGizmoLookAndFeel3D _lookAndFeel3D = new BoxGizmoLookAndFeel3D();
        private BoxGizmoLookAndFeel3D _sharedLookAndFeel3D;

        private BoxGizmoHotkeys _hotkeys = new BoxGizmoHotkeys();
        private BoxGizmoHotkeys _sharedHotkeys;

        public BoxGizmoSettings3D Settings3D { get { return _sharedSettings3D == null ? _settings3D : _sharedSettings3D; } }
        public BoxGizmoSettings3D SharedSettings3D
        {
            get { return _sharedSettings3D; }
            set
            {
                _sharedSettings3D = value;
                SetupSharedSettings();
            }
        }
        public BoxGizmoLookAndFeel3D LookAndFeel3D { get { return _sharedLookAndFeel3D == null ? _lookAndFeel3D : _sharedLookAndFeel3D; } }
        public BoxGizmoLookAndFeel3D SharedLookAndFeel3D
        {
            get { return _sharedLookAndFeel3D; }
            set
            {
                _sharedLookAndFeel3D = value;
                SetupSharedLookAndFeel();
            }
        }
        public BoxGizmoHotkeys Hotkeys { get { return _sharedHotkeys == null ? _hotkeys : _sharedHotkeys; } }
        public BoxGizmoHotkeys SharedHotkeys { get { return _sharedHotkeys; } set { _sharedHotkeys = value; } }
        public BoxGizmo.Usage BoxUsage { get { return _usage; } }
        public bool IsUsagePermanent { get { return _isUsagePermanent; } }
        public Vector3 BoxCenter { get { return Gizmo.Transform.Position3D; } }
        public Quaternion BoxRotation { get { return Gizmo.Transform.Rotation3D; } }
        public Vector3 BoxRight { get { return BoxRotation * Vector3.right; } }
        public Vector3 BoxUp { get { return BoxRotation * Vector3.up; } }
        public Vector3 BoxLook { get { return BoxRotation * Vector3.forward; } }

        public override void OnDetached()
        {
            Gizmo.Transform.Changed -= OnGizmoTransformChanged;
            RTUndoRedo.Get.UndoEnd -= OnUndoRedoEnd;
            RTUndoRedo.Get.RedoEnd -= OnUndoRedoEnd;
        }

        public override void OnEnabled()
        {
            Gizmo.Transform.Changed += OnGizmoTransformChanged;
        }

        public override void OnDisabled()
        {
            Gizmo.Transform.Changed -= OnGizmoTransformChanged;
        }

        public override void OnGizmoEnabled()
        {
            OnGizmoUpdateBegin();
        }

        public void MakeUsagePermanent()
        {
            _isUsagePermanent = true;
        }

        public bool OwnsHandle(int handleId)
        {
            return _ticks.Contains(handleId);
        }

        public bool IsXTick(int handleId)
        {
            return handleId == _leftTick.HandleId || handleId == _rightTick.HandleId;
        }

        public bool IsYTick(int handleId)
        {
            return handleId == _topTick.HandleId || handleId == _bottomTick.HandleId;
        }

        public bool IsZTick(int handleId)
        {
            return handleId == _frontTick.HandleId || handleId == _backTick.HandleId;
        }

        public void SetSnapEnabled(bool isEnabled)
        {
            _scaleDrag.IsSnapEnabled = isEnabled;
        }

        public void SetSize(Vector3 size)
        {
            if (!Gizmo.IsDragged && BoxUsage == Usage.Generic)
            {
                _boxSize = size;
                UpdateTickPositions();
            }
        }

        public void SetUsage(BoxGizmo.Usage usage)
        {
            if (Gizmo.IsDragged || IsUsagePermanent || usage == _usage) return;

            _usage = usage;
            if (_usage == Usage.Generic)
            {
                _ticks.SetVisible(true);
                _ticks.SetHoverable(true);
            }
        }

        public bool SetTargetHierarchy(GameObject targetHierarchy)
        {
            if (BoxUsage == Usage.ObjectScale && !Gizmo.IsDragged && targetHierarchy != null)
            {
                _targetHierarchy = targetHierarchy;
                _targetHierarchyTransform = _targetHierarchy.transform;

                if(!FitBoxToTargetHierarchy())
                {
                    _targetHierarchy = null;
                    _targetHierarchyTransform = null;
                    return false;
                }

                return true;
            }

            if (BoxUsage == Usage.ObjectScale)
            {
                if(_targetHierarchy != null)
                {
                    _ticks.SetVisible(true);
                    _ticks.SetHoverable(true);
                }
                else
                {
                    _ticks.SetVisible(false);
                    _ticks.SetHoverable(false);
                }
            }

            return false;
        }

        public bool FitBoxToTargetHierarchy()
        {
            if (BoxUsage == Usage.ObjectScale)
            {
                if (_targetHierarchy == null)
                {
                    _boxSize = Vector3.zero;
                    return false;
                }

                var obb = CalcTargetRootOBB(_targetHierarchy);
                if (!obb.IsValid)
                {
                    _boxSize = Vector3.zero;
                    return false;
                }

                _boxSize = obb.Size;
                Gizmo.Transform.Position3D = obb.Center;
                Gizmo.Transform.Rotation3D = _targetHierarchy.transform.rotation;

                return true;
            }

            _boxSize = Vector3.zero;
            return false;
        }

        public override void OnAttached()
        {
            RTUndoRedo.Get.UndoEnd += OnUndoRedoEnd;
            RTUndoRedo.Get.RedoEnd += OnUndoRedoEnd;

            _leftTick = new GizmoCap2D(Gizmo, GizmoHandleId.BoxTickLeftCenter);
            _rightTick = new GizmoCap2D(Gizmo, GizmoHandleId.BoxTickRightCenter);
            _topTick = new GizmoCap2D(Gizmo, GizmoHandleId.BoxTickTopCenter);
            _bottomTick = new GizmoCap2D(Gizmo, GizmoHandleId.BoxTickBottomCenter);
            _backTick = new GizmoCap2D(Gizmo, GizmoHandleId.BoxTickBackCenter);
            _frontTick = new GizmoCap2D(Gizmo, GizmoHandleId.BoxTickFrontCenter);

            _ticks.Add(_leftTick);
            _ticks.Add(_rightTick);
            _ticks.Add(_topTick);
            _ticks.Add(_bottomTick);
            _ticks.Add(_backTick);
            _ticks.Add(_frontTick);
            _ticks.SetDragSession(_scaleDrag);

            SetupSharedLookAndFeel();
            SetupSharedSettings();
        }

        public override bool OnGizmoCanBeginDrag(int handleId)
        {
            if (BoxUsage == Usage.ObjectScale && _targetHierarchy != null)
            {
                IRTTransformGizmoListener transformGizmoListener = _targetHierarchy.GetComponent<IRTTransformGizmoListener>();
                if (transformGizmoListener != null) return transformGizmoListener.OnCanBeTransformed(Gizmo);
            }

            return true;
        }

        public override void OnGizmoUpdateBegin()
        {
            SetSnapEnabled(Hotkeys.EnableSnapping.IsActive());

            _scaleDrag.Sensitivity = Settings3D.DragSensitivity;
            UpdateTickPositions();
            ValidateBoxSize();
        }

        public override void OnGizmoRender(Camera camera)
        {
            var boxWireMaterial = GizmoLineMaterial.Get;
            boxWireMaterial.ResetValuesToSensibleDefaults();
            boxWireMaterial.SetColor(LookAndFeel3D.BoxWireColor);
            boxWireMaterial.SetPass(0);
            GraphicsEx.DrawWireBox(new OBB(BoxCenter, _boxSize, BoxRotation));

            if (RTGizmosEngine.Get.NumRenderCameras > 1)
            {
                UpdateTickPositions();
            }

            _leftTick.Render(camera);
            _rightTick.Render(camera);
            _topTick.Render(camera);
            _bottomTick.Render(camera);
            _frontTick.Render(camera);
            _backTick.Render(camera);
        }

        public override void OnGizmoAttemptHandleDragBegin(int handleId)
        {
            if (OwnsHandle(handleId))
            {
                _scaleFromCenter = Hotkeys.EnableCenterPivot.IsActive();
                _scaleDragWorkData.DragOrigin = BoxCenter;

                if (handleId == _leftTick.HandleId)
                {
                    _scaleDragWorkData.Axis = -BoxRight;
                    _scaleDragWorkData.AxisIndex = 0;
                    _scaleDragWorkData.SnapStep = Settings3D.XSnapStep;
                    _scaleDragWorkData.EntityScale = _targetHierarchyTransform.lossyScale.x;
                    _scalePivot = BoxMath.CalcBoxFaceCenter(BoxCenter, _boxSize, BoxRotation, BoxFace.Right);
                }
                else
                if (handleId == _rightTick.HandleId)
                {
                    _scaleDragWorkData.Axis = BoxRight;
                    _scaleDragWorkData.AxisIndex = 0;
                    _scaleDragWorkData.SnapStep = Settings3D.XSnapStep;
                    _scaleDragWorkData.EntityScale = _targetHierarchyTransform.lossyScale.x;
                    _scalePivot = BoxMath.CalcBoxFaceCenter(BoxCenter, _boxSize, BoxRotation, BoxFace.Left);
                }
                else
                if (handleId == _topTick.HandleId)
                {
                    _scaleDragWorkData.Axis = BoxUp;
                    _scaleDragWorkData.AxisIndex = 1;
                    _scaleDragWorkData.SnapStep = Settings3D.YSnapStep;
                    _scaleDragWorkData.EntityScale = _targetHierarchyTransform.lossyScale.y;
                    _scalePivot = BoxMath.CalcBoxFaceCenter(BoxCenter, _boxSize, BoxRotation, BoxFace.Bottom);
                }
                else
                if (handleId == _bottomTick.HandleId)
                {
                    _scaleDragWorkData.Axis = -BoxUp;
                    _scaleDragWorkData.AxisIndex = 1;
                    _scaleDragWorkData.SnapStep = Settings3D.YSnapStep;
                    _scaleDragWorkData.EntityScale = _targetHierarchyTransform.lossyScale.y;
                    _scalePivot = BoxMath.CalcBoxFaceCenter(BoxCenter, _boxSize, BoxRotation, BoxFace.Top);
                }
                else
                if (handleId == _frontTick.HandleId)
                {
                    _scaleDragWorkData.Axis = -BoxLook;
                    _scaleDragWorkData.AxisIndex = 2;
                    _scaleDragWorkData.SnapStep = Settings3D.ZSnapStep;
                    _scaleDragWorkData.EntityScale = _targetHierarchyTransform.lossyScale.z;
                    _scalePivot = BoxMath.CalcBoxFaceCenter(BoxCenter, _boxSize, BoxRotation, BoxFace.Back);
                }
                else
                if (handleId == _backTick.HandleId)
                {
                    _scaleDragWorkData.Axis = BoxLook;
                    _scaleDragWorkData.AxisIndex = 2;
                    _scaleDragWorkData.SnapStep = Settings3D.ZSnapStep;
                    _scaleDragWorkData.EntityScale = _targetHierarchyTransform.lossyScale.z;
                    _scalePivot = BoxMath.CalcBoxFaceCenter(BoxCenter, _boxSize, BoxRotation, BoxFace.Front);
                }

                if (_scaleFromCenter) _scalePivot = BoxCenter;
                _scaleDrag.SetWorkData(_scaleDragWorkData);

                if (BoxUsage == Usage.ObjectScale && _targetHierarchyTransform != null) 
                    _dragBeginTargetTransformSnapshot.Snapshot(_targetHierarchyTransform);
            }
        }

        public override void OnGizmoDragUpdate(int handleId)
        {
            if (OwnsHandle(handleId))
            {
                if (BoxUsage == Usage.Generic)
                {
                    _boxSize = Vector3.Scale(_boxSize, Gizmo.RelativeDragScale);
                    if (!_scaleFromCenter) Gizmo.Transform.Position3D = _scalePivot + _scaleDragWorkData.Axis * _boxSize[_scaleDragWorkData.AxisIndex] * 0.5f;
                }
                else
                if (BoxUsage == Usage.ObjectScale && _targetHierarchy != null)
                {
                    // Bug: When scale pivot overlaps object position, ScaleFromPivot will produce incorrect results.
                    _targetHierarchyTransform.ScaleFromPivot(Gizmo.RelativeDragScale, _scalePivot);
                    FitBoxToTargetHierarchy();

                    IRTTransformGizmoListener transformGizmoListener = _targetHierarchy.GetComponent<IRTTransformGizmoListener>();
                    if (transformGizmoListener != null) transformGizmoListener.OnTransformed(Gizmo);
                }

                UpdateTickPositions();
                ValidateBoxSize();
            }
        }

        public override void OnGizmoDragEnd(int handleId)
        {
            if (OwnsHandle(handleId))
            {
                if (BoxUsage == Usage.ObjectScale && _targetHierarchyTransform != null)
                {
                    var postSnapshot = new LocalTransformSnapshot();
                    postSnapshot.Snapshot(_targetHierarchyTransform);
                    var action = new PostObjectTransformsChangedAction(new List<LocalTransformSnapshot>() { _dragBeginTargetTransformSnapshot },
                        new List<LocalTransformSnapshot>() { postSnapshot });
                    action.Execute();
                }
            }
        }

        private void OnUndoRedoEnd(IUndoRedoAction action)
        {
            if (action is PostObjectTransformsChangedAction)
                FitBoxToTargetHierarchy();
        }

        private void UpdateTickPositions()
        {
            Camera camera = Gizmo.GetWorkCamera();
            Vector3 center = BoxCenter;
            Quaternion rotation = BoxRotation;

            Vector3 faceCenter = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Left);
            _leftTick.Position = camera.WorldToScreenPoint(faceCenter);

            faceCenter = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Right);
            _rightTick.Position = camera.WorldToScreenPoint(faceCenter);

            faceCenter = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Top);
            _topTick.Position = camera.WorldToScreenPoint(faceCenter);

            faceCenter = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Bottom);
            _bottomTick.Position = camera.WorldToScreenPoint(faceCenter);

            faceCenter = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Front);
            _frontTick.Position = camera.WorldToScreenPoint(faceCenter);

            faceCenter = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Back);
            _backTick.Position = camera.WorldToScreenPoint(faceCenter);
        }

        private void ValidateBoxSize()
        {
            const float minSize = 1e-5f;
            Vector3 boxSize = _boxSize;

            if (Mathf.Abs(boxSize.x) < minSize)
            {
                _leftTick.SetVisible(false);
                _rightTick.SetVisible(false);
            }
            else
            {
                _leftTick.SetVisible(true);
                _rightTick.SetVisible(true);
            }

            if (Mathf.Abs(boxSize.y) < minSize)
            {
                _topTick.SetVisible(false);
                _bottomTick.SetVisible(false);
            }
            else
            {
                _topTick.SetVisible(true);
                _bottomTick.SetVisible(true);
            }

            if (Mathf.Abs(boxSize.z) < minSize)
            {
                _backTick.SetVisible(false);
                _frontTick.SetVisible(false);
            }
            else
            {
                _backTick.SetVisible(true);
                _frontTick.SetVisible(true);
            }
        }

        private void SetupSharedLookAndFeel()
        {
            LookAndFeel3D.ConnectTickLookAndFeel(_rightTick, 0, AxisSign.Positive);
            LookAndFeel3D.ConnectTickLookAndFeel(_topTick, 1, AxisSign.Positive);
            LookAndFeel3D.ConnectTickLookAndFeel(_backTick, 2, AxisSign.Positive);
            LookAndFeel3D.ConnectTickLookAndFeel(_leftTick, 0, AxisSign.Negative);
            LookAndFeel3D.ConnectTickLookAndFeel(_bottomTick, 1, AxisSign.Negative);
            LookAndFeel3D.ConnectTickLookAndFeel(_frontTick, 2, AxisSign.Negative);
        }

        private void SetupSharedSettings()
        {
        }

        private void OnGizmoTransformChanged(GizmoTransform gizmoTransform, GizmoTransform.ChangeData changeData)
        {
            UpdateTickPositions();
        }

        private OBB CalcTargetRootOBB(GameObject targetRoot)
        {
            var boundsQConfig = new ObjectBounds.QueryConfig();
            boundsQConfig.ObjectTypes = GameObjectType.Mesh | GameObjectType.Sprite;
            return ObjectBounds.CalcHierarchyWorldOBB(targetRoot, boundsQConfig);
        }
    }
}
