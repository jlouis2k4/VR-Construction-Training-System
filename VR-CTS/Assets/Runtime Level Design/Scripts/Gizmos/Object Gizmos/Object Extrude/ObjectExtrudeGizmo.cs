using UnityEngine;
using System;
using System.Collections.Generic;

namespace RLD
{
    public delegate void ObjectExtrudeGizmoExtrudeUpdateHandler(List<GameObject> clones);

    [Serializable]
    public class ObjectExtrudeGizmo : GizmoBehaviour
    {
        public event ObjectExtrudeGizmoExtrudeUpdateHandler ExtrudeUpdate;

        private struct HandleDragExtrudeData
        {
            public Vector3 ExtrudeDir;
            public Vector3 ExtrudeCenter;
            public int AxisIndex;
        }

        private Vector3 _boxSize = Vector3Ex.FromValue(5.0f);
        private GizmoSpace _extrudeSpace = GizmoSpace.Local;

        private List<GameObject> _targetParents = new List<GameObject>();
        private HashSet<GameObject> _ignoredParentObjects = new HashSet<GameObject>();
        private ObjectBounds.QueryConfig _boundsQConfig = new ObjectBounds.QueryConfig();
        private SceneOverlapFilter _sceneOverlapFilter = new SceneOverlapFilter();
        private ObjectExtrudeGizmoDragEnd _dragEndAction;

        private HandleDragExtrudeData _handleDragExtrData = new HandleDragExtrudeData();

        private GizmoLineSlider3D _rightExtrude;
        private GizmoLineSlider3D _upExtrude;
        private GizmoLineSlider3D _frontExtrude;
        private GizmoLineSlider3D _leftExtrude;
        private GizmoLineSlider3D _bottomExtrude;
        private GizmoLineSlider3D _backExtrude;
        private GizmoLineSlider3DCollection _extrudeSliders = new GizmoLineSlider3DCollection();

        [SerializeField]
        private ObjectExtrudeGizmoLookAndFeel3D _lookAndFeel3D = new ObjectExtrudeGizmoLookAndFeel3D();
        private ObjectExtrudeGizmoLookAndFeel3D _sharedLookAndFeel3D;

        [SerializeField]
        private ObjectExtrudeGizmoHotkeys _hotkeys = new ObjectExtrudeGizmoHotkeys();
        private ObjectExtrudeGizmoHotkeys _sharedHotkeys;

        public ObjectExtrudeGizmoLookAndFeel3D LookAndFeel3D { get { return _sharedLookAndFeel3D != null ? _sharedLookAndFeel3D : _lookAndFeel3D; } }
        public ObjectExtrudeGizmoLookAndFeel3D SharedLookAndFeel3D
        {
            get { return _sharedLookAndFeel3D; }
            set
            {
                _sharedLookAndFeel3D = value;
                SetupSharedLookAndFeel();
            }
        }
        public ObjectExtrudeGizmoHotkeys Hotkeys { get { return _sharedHotkeys == null ? _hotkeys : _sharedHotkeys; } }
        public ObjectExtrudeGizmoHotkeys SharedHotkeys { get { return _sharedHotkeys; } set { _sharedHotkeys = value; } }

        public Vector3 BoxCenter { get { return Gizmo.Transform.Position3D; } }
        public Quaternion BoxRotation { get { return Gizmo.Transform.Rotation3D; } }
        public Vector3 BoxSize { get { return _boxSize; } }
        public Vector3 BoxRight { get { return BoxRotation * Vector3.right; } }
        public Vector3 BoxUp { get { return BoxRotation * Vector3.up; } }
        public Vector3 BoxLook { get { return BoxRotation * Vector3.forward; } }
        public OBB OBB { get { return new OBB(BoxCenter, BoxSize, BoxRotation); } }
        public GizmoSpace ExtrudeSpace { get { return _extrudeSpace; } }
        public int NumTargetParents { get { return _targetParents.Count; } }

        public bool OwnsHandle(int handleId)
        {
            return _extrudeSliders.Contains(handleId) || _extrudeSliders.ContainsCapId(handleId);
        }

        public bool IsRightExtrudeHandle(int handleId)
        {
            return _rightExtrude.HandleId == handleId || _rightExtrude.Cap3DHandleId == handleId;
        }

        public bool IsLeftExtrudeHandle(int handleId)
        {
            return _leftExtrude.HandleId == handleId || _leftExtrude.Cap3DHandleId == handleId;
        }

        public bool IsTopExtrudeHandle(int handleId)
        {
            return _upExtrude.HandleId == handleId || _upExtrude.Cap3DHandleId == handleId;
        }

        public bool IsBottomExtrudeHandle(int handleId)
        {
            return _bottomExtrude.HandleId == handleId || _bottomExtrude.Cap3DHandleId == handleId;
        }

        public bool IsFrontExtrudeHandle(int handleId)
        {
            return _backExtrude.HandleId == handleId || _backExtrude.Cap3DHandleId == handleId;
        }

        public bool IsBackExtrudeHandle(int handleId)
        {
            return _frontExtrude.HandleId == handleId || _frontExtrude.Cap3DHandleId == handleId;
        }

        public void SetIgnoredParentObjects(IEnumerable<GameObject> ignoredParentObjects)
        {
            _ignoredParentObjects.Clear();
            foreach (var ignoredParent in ignoredParentObjects)
                _ignoredParentObjects.Add(ignoredParent);
        }

        public void SetExtrudeSpace(GizmoSpace extrudeSpace)
        {
            if (extrudeSpace == _extrudeSpace || Gizmo.IsDragged) return;

            _extrudeSpace = extrudeSpace;
            FitBoxToTargets();
        }

        public void SetExtrudeTargets(IEnumerable<GameObject> extrudeTargets)
        {
            if (extrudeTargets == null)
            {
                _targetParents.Clear();
                _boxSize = Vector3.zero;
                return;
            }

            _targetParents = GameObjectEx.FilterParentsOnly(extrudeTargets);
            FitBoxToTargets();
        }

        public void FitBoxToTargets()
        {
            if (NumTargetParents == 0)
            {
                _boxSize = Vector3.zero;
                return;
            }

            if (ExtrudeSpace == GizmoSpace.Global)
            {
                AABB worldAABB = AABB.GetInvalid();
                foreach(var parent in _targetParents)
                {
                    if (_ignoredParentObjects.Contains(parent)) continue;

                    AABB aabb = ObjectBounds.CalcHierarchyWorldAABB(parent, _boundsQConfig);
                    if(aabb.IsValid)
                    {
                        if (worldAABB.IsValid) worldAABB.Encapsulate(aabb);
                        else worldAABB = aabb;
                    }
                }

                SetAABB(worldAABB);
                UpdateSnapSteps();
            }
            else
            if (ExtrudeSpace == GizmoSpace.Local)
            {
                int firstParentIndex = 0;
                while (firstParentIndex < NumTargetParents)
                {
                    if (_ignoredParentObjects.Contains(_targetParents[firstParentIndex])) ++firstParentIndex;
                    else break;
                }

                if (firstParentIndex == NumTargetParents)
                {
                    SetOBB(OBB.GetInvalid());
                    UpdateSnapSteps();
                    return;
                }

                OBB worldOBB = ObjectBounds.CalcHierarchyWorldOBB(_targetParents[firstParentIndex], _boundsQConfig);
                for (int parentIndex = firstParentIndex; parentIndex < NumTargetParents; ++parentIndex )
                {
                    GameObject parent = _targetParents[parentIndex];
                    if (_ignoredParentObjects.Contains(parent)) continue;

                    OBB obb = ObjectBounds.CalcHierarchyWorldOBB(parent, _boundsQConfig);
                    if (obb.IsValid)
                    {
                        if (worldOBB.IsValid) worldOBB.Encapsulate(obb);
                        else worldOBB = obb;
                    }
                }

                SetOBB(worldOBB);
                UpdateSnapSteps();
            }
        }

        public override void OnDetached()
        {
            RTUndoRedo.Get.UndoEnd -= OnUndoRedoEnd;
            RTUndoRedo.Get.RedoEnd -= OnUndoRedoEnd;
            Gizmo.Transform.Changed -= OnGizmoTransformChanged;
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

        public override void OnAttached()
        {
            RTUndoRedo.Get.UndoEnd += OnUndoRedoEnd;
            RTUndoRedo.Get.RedoEnd += OnUndoRedoEnd;

            _rightExtrude = new GizmoLineSlider3D(Gizmo, GizmoHandleId.ExtrudeSliderRight, GizmoHandleId.ExtrudeCapRight);
            _rightExtrude.SetDragChannel(GizmoDragChannel.Offset);

            _leftExtrude = new GizmoLineSlider3D(Gizmo, GizmoHandleId.ExtrudeSliderLeft, GizmoHandleId.ExtrudeCapLeft);
            _leftExtrude.SetDragChannel(GizmoDragChannel.Offset);

            _upExtrude = new GizmoLineSlider3D(Gizmo, GizmoHandleId.ExtrudeSliderTop, GizmoHandleId.ExtrudeCapTop);
            _upExtrude.SetDragChannel(GizmoDragChannel.Offset);

            _bottomExtrude = new GizmoLineSlider3D(Gizmo, GizmoHandleId.ExtrudeSliderBottom, GizmoHandleId.ExtrudeCapBottom);
            _bottomExtrude.SetDragChannel(GizmoDragChannel.Offset);

            _frontExtrude = new GizmoLineSlider3D(Gizmo, GizmoHandleId.ExtrudeSliderBack, GizmoHandleId.ExtrudeCapBack);
            _frontExtrude.SetDragChannel(GizmoDragChannel.Offset);

            _backExtrude = new GizmoLineSlider3D(Gizmo, GizmoHandleId.ExtrudeSliderFront, GizmoHandleId.ExtrudeCapFront);
            _backExtrude.SetDragChannel(GizmoDragChannel.Offset);

            _extrudeSliders.Add(_rightExtrude);
            _extrudeSliders.Add(_upExtrude);
            _extrudeSliders.Add(_frontExtrude);
            _extrudeSliders.Add(_leftExtrude);
            _extrudeSliders.Add(_bottomExtrude);
            _extrudeSliders.Add(_backExtrude);
            _extrudeSliders.SetSnapEnabled(true);

            _boundsQConfig.ObjectTypes = GameObjectType.Sprite | GameObjectType.Mesh;
            _boundsQConfig.NoVolumeSize = Vector3.zero;
            _sceneOverlapFilter.AllowedObjectTypes.Add(GameObjectType.Mesh);
            _sceneOverlapFilter.AllowedObjectTypes.Add(GameObjectType.Sprite);

            ValidateBoxSize();
            SetupSharedLookAndFeel();
        }

        public override void OnGizmoUpdateBegin()
        {
            ValidateBoxSize();
            UpdateExtrudeSliderTransforms();

            if (!LookAndFeel3D.IsExtrudeSliderVisible(0, AxisSign.Positive))
                _rightExtrude.Set3DCapVisible(false);
            if (!LookAndFeel3D.IsExtrudeSliderVisible(1, AxisSign.Positive))
                _upExtrude.Set3DCapVisible(false);
            if (!LookAndFeel3D.IsExtrudeSliderVisible(2, AxisSign.Positive))
                _frontExtrude.Set3DCapVisible(false);
            if (!LookAndFeel3D.IsExtrudeSliderVisible(0, AxisSign.Negative))
                _leftExtrude.Set3DCapVisible(false);
            if (!LookAndFeel3D.IsExtrudeSliderVisible(1, AxisSign.Negative))
                _bottomExtrude.Set3DCapVisible(false);
            if (!LookAndFeel3D.IsExtrudeSliderVisible(2, AxisSign.Negative))
                _backExtrude.Set3DCapVisible(false);
        }

        public override void OnGizmoRender(Camera camera)
        {
            var boxWireMaterial = GizmoLineMaterial.Get;
            boxWireMaterial.ResetValuesToSensibleDefaults();
            boxWireMaterial.SetColor(LookAndFeel3D.BoxWireColor);
            boxWireMaterial.SetPass(0);
            GraphicsEx.DrawWireBox(OBB);

            bool multipleRenderCams = RTGizmosEngine.Get.NumRenderCameras > 1;
            if (multipleRenderCams)
            {
                _extrudeSliders.ApplyZoomFactor(camera);
                UpdateExtrudeSliderTransforms();
            }

            var sortedSliders = _extrudeSliders.GetRenderSortedSliders(camera);
            foreach (var slider in sortedSliders) slider.Render(camera);
        }

        public override void OnGizmoDragBegin(int handleId)
        {
            if (OwnsHandle(handleId))
            {
                _sceneOverlapFilter.IgnoreObjects.Clear();
                foreach (var targetParent in _targetParents)
                    _sceneOverlapFilter.IgnoreObjects.AddRange(targetParent.GetAllChildrenAndSelf());

                _dragEndAction = new ObjectExtrudeGizmoDragEnd();
                _dragEndAction.SetTargetParents(_targetParents);
                _dragEndAction.TakeUndoTargetSnapshots();

                _handleDragExtrData.ExtrudeCenter = BoxCenter;
                if (IsLeftExtrudeHandle(handleId))
                {
                    _handleDragExtrData.AxisIndex = 0;
                    _handleDragExtrData.ExtrudeDir = -BoxRight;
                }
                else
                if (IsRightExtrudeHandle(handleId))
                {
                    _handleDragExtrData.AxisIndex = 0;
                    _handleDragExtrData.ExtrudeDir = BoxRight;
                }
                else
                if (IsTopExtrudeHandle(handleId))
                {
                    _handleDragExtrData.AxisIndex = 1;
                    _handleDragExtrData.ExtrudeDir = BoxUp;
                }
                else
                if (IsBottomExtrudeHandle(handleId))
                {
                    _handleDragExtrData.AxisIndex = 1;
                    _handleDragExtrData.ExtrudeDir = -BoxUp;
                }
                else
                if (IsFrontExtrudeHandle(handleId))
                {
                    _handleDragExtrData.AxisIndex = 2;
                    _handleDragExtrData.ExtrudeDir = -BoxLook;
                }
                else
                if (IsBackExtrudeHandle(handleId))
                {
                    _handleDragExtrData.AxisIndex = 2;
                    _handleDragExtrData.ExtrudeDir = BoxLook;
                }
            }
        }

        public override void OnGizmoDragUpdate(int handleId)
        {
            if (OwnsHandle(handleId) && Gizmo.RelativeDragOffset.magnitude != 0.0f)
            {
                float extrAmount = Gizmo.RelativeDragOffset.magnitude;
                float boxSize = _boxSize[_handleDragExtrData.AxisIndex];
                float fNumClones = boxSize != 0.0f ? (extrAmount / boxSize) : 0.0f;
                int iNumClones = (int)fNumClones;

                float absFractional = Mathf.Abs(fNumClones - (int)fNumClones);
                if (iNumClones == 0 && Mathf.Abs(absFractional - 1.0f) < 1e-5f) ++iNumClones;

                var createdClones = new List<GameObject>(10);
                var cloneConfig = ObjectCloning.DefaultConfig;
                var cloneOffset = _handleDragExtrData.ExtrudeDir * boxSize;
                for(int cloneIndex = 0; cloneIndex < iNumClones; ++cloneIndex)
                {
                    foreach(var targetParent in _targetParents)
                    {
                        if (_ignoredParentObjects.Contains(targetParent)) continue;

                        if (!Hotkeys.EnableOverlapTest.IsActive())
                        {
                            cloneConfig.Parent = targetParent.transform.parent;
                            var clonedHierarchy = ObjectCloning.CloneHierarchy(targetParent, cloneConfig);
                            if (clonedHierarchy != null)
                            {
                                _sceneOverlapFilter.IgnoreObjects.AddRange(clonedHierarchy.GetAllChildrenAndSelf());
                                _dragEndAction.AddExtrudeClone(clonedHierarchy);
                                createdClones.Add(clonedHierarchy);
                            }
                            clonedHierarchy.transform.position += cloneIndex * cloneOffset;
                        }
                        else
                        {
                            OBB targetOBB = ObjectBounds.CalcHierarchyWorldOBB(targetParent, _boundsQConfig);
                            if (!targetOBB.IsValid) continue;
                            targetOBB.Center += cloneIndex * cloneOffset;

                            // Bring the size down a tad. Otherwise, we can get false positives when objects are really close to 
                            // each other even if they do not intersect.
                            targetOBB.Inflate(-1e-2f);
                            var overlappedObjects = RTScene.Get.OverlapBox(targetOBB, _sceneOverlapFilter);
                            if (overlappedObjects.Count != 0) continue;

                            cloneConfig.Parent = targetParent.transform.parent;
                            var clonedHierarchy = ObjectCloning.CloneHierarchy(targetParent, cloneConfig);
                            if (clonedHierarchy != null)
                            {
                                _sceneOverlapFilter.IgnoreObjects.AddRange(clonedHierarchy.GetAllChildrenAndSelf());
                                _dragEndAction.AddExtrudeClone(clonedHierarchy);
                                createdClones.Add(clonedHierarchy);
                            }
                            clonedHierarchy.transform.position += cloneIndex * cloneOffset;
                        }
                    }
                }
                _handleDragExtrData.ExtrudeCenter += Gizmo.RelativeDragOffset;

                foreach (var targetParent in _targetParents)
                {
                    if (!_ignoredParentObjects.Contains(targetParent))
                        targetParent.transform.position += Gizmo.RelativeDragOffset;
                }

                if (ExtrudeUpdate != null) ExtrudeUpdate(createdClones);
            }
        }

        public override void OnGizmoDragEnd(int handleId)
        {
            if (OwnsHandle(handleId))
            {
                _dragEndAction.TakeRedoTargetSnapshots();
                _dragEndAction.Execute();
            }
        }

        private void UpdateExtrudeSliderTransforms()
        {
            Vector3 center = BoxCenter;
            Quaternion rotation = BoxRotation;

            _leftExtrude.StartPosition = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Left);
            _leftExtrude.SetDirection(-BoxRight);

            _rightExtrude.StartPosition = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Right);
            _rightExtrude.SetDirection(BoxRight);

            _upExtrude.StartPosition = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Top);
            _upExtrude.SetDirection(BoxUp);

            _bottomExtrude.StartPosition = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Bottom);
            _bottomExtrude.SetDirection(-BoxUp);

            _backExtrude.StartPosition = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Front);
            _backExtrude.SetDirection(-BoxLook);

            _frontExtrude.StartPosition = BoxMath.CalcBoxFaceCenter(center, _boxSize, rotation, BoxFace.Back);
            _frontExtrude.SetDirection(BoxLook);
        }

        private void OnGizmoTransformChanged(GizmoTransform gizmoTransform, GizmoTransform.ChangeData changeData)
        {
            UpdateExtrudeSliderTransforms();
        }

        private void SetAABB(AABB aabb)
        {
            if (!aabb.IsValid)
            {
                _boxSize = Vector3.zero;
                return;
            }

            _boxSize = aabb.Size.Abs();
            Gizmo.Transform.Rotation3D = Quaternion.identity;
            Gizmo.Transform.Position3D = aabb.Center;
        }

        private void SetOBB(OBB obb)
        {
            if (!obb.IsValid)
            {
                _boxSize = Vector3.zero;
                return;
            }

            _boxSize = obb.Size.Abs();
            Gizmo.Transform.Rotation3D = obb.Rotation;
            Gizmo.Transform.Position3D = obb.Center;
        }

        private void UpdateSnapSteps()
        {
            _rightExtrude.Settings.OffsetSnapStep = _boxSize.x;
            _leftExtrude.Settings.OffsetSnapStep = _boxSize.x;

            _upExtrude.Settings.OffsetSnapStep = _boxSize.y;
            _bottomExtrude.Settings.OffsetSnapStep = _boxSize.y;

            _frontExtrude.Settings.OffsetSnapStep = _boxSize.z;
            _backExtrude.Settings.OffsetSnapStep = _boxSize.z;
        }

        private void ValidateBoxSize()
        {
            const float minSize = 1e-5f;
            Vector3 boxSize = _boxSize;

            if (Mathf.Abs(boxSize.x) < minSize)
            {
                _rightExtrude.Set3DCapVisible(false);
                _leftExtrude.Set3DCapVisible(false);
            }
            else
            {
                _rightExtrude.Set3DCapVisible(true);
                _leftExtrude.Set3DCapVisible(true);
            }

            if (Mathf.Abs(boxSize.y) < minSize)
            {
                _upExtrude.Set3DCapVisible(false);
                _bottomExtrude.Set3DCapVisible(false);
            }
            else
            {
                _upExtrude.Set3DCapVisible(true);
                _bottomExtrude.Set3DCapVisible(true);
            }

            if (Mathf.Abs(boxSize.z) < minSize)
            {
                _frontExtrude.Set3DCapVisible(false);
                _backExtrude.Set3DCapVisible(false);
            }
            else
            {
                _frontExtrude.Set3DCapVisible(true);
                _backExtrude.Set3DCapVisible(true);
            }
        }

        private void SetupSharedLookAndFeel()
        {
            LookAndFeel3D.ConnectSliderLookAndFeel(_rightExtrude, 0, AxisSign.Positive);
            LookAndFeel3D.ConnectSliderLookAndFeel(_upExtrude, 1, AxisSign.Positive);
            LookAndFeel3D.ConnectSliderLookAndFeel(_frontExtrude, 2, AxisSign.Positive);
            LookAndFeel3D.ConnectSliderLookAndFeel(_leftExtrude, 0, AxisSign.Negative);
            LookAndFeel3D.ConnectSliderLookAndFeel(_bottomExtrude, 1, AxisSign.Negative);
            LookAndFeel3D.ConnectSliderLookAndFeel(_backExtrude, 2, AxisSign.Negative);
        }

        private void OnUndoRedoEnd(IUndoRedoAction action)
        {
            if (action is ObjectExtrudeGizmoDragEnd)
                FitBoxToTargets();
        }
    }
}
