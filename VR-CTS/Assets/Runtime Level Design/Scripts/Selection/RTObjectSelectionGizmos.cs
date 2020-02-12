using UnityEngine;
using System;
using System.Collections.Generic;

namespace RLD
{
    [Serializable]
    public class RTObjectSelectionGizmos : MonoSingleton<RTObjectSelectionGizmos>, IObjectCollectionGizmoController
    {
        private class ObjectSelectionGizmo
        {
            private int _id;
            private Gizmo _gizmo;
            private BoxGizmo _boxScaleGizmo;
            private ObjectTransformGizmo _transformGizmo;
            private ObjectExtrudeGizmo _extrudeGizmo;
            private bool _isUsable = true;

            public int Id { get { return _id; } }
            public Gizmo Gizmo { get { return _gizmo; } }
            public BoxGizmo BoxScaleGizmo { get { return _boxScaleGizmo; } }
            public bool IsBoxScaleGizmo { get { return _boxScaleGizmo != null; } }
            public ObjectTransformGizmo TransformGizmo { get { return _transformGizmo; } }
            public bool IsTransformGizmo { get { return _transformGizmo != null; } }
            public ObjectExtrudeGizmo ExtrudeGizmo { get { return _extrudeGizmo; } }
            public bool IsExtrudeGizmo { get { return _extrudeGizmo != null; } }
            public bool IsUsable { get { return _isUsable; } set { _isUsable = value; } }

            public ObjectSelectionGizmo(int id, Gizmo gizmo)
            {
                _id = id;
                _gizmo = gizmo;
                _boxScaleGizmo = gizmo.GetFirstBehaviourOfType<BoxGizmo>();
                _transformGizmo = gizmo.GetFirstBehaviourOfType<ObjectTransformGizmo>();
                _extrudeGizmo = gizmo.GetFirstBehaviourOfType<ObjectExtrudeGizmo>();
                _isUsable = true;
            }
        }

        [SerializeField]
        private EditorToolbar _mainToolbar = new EditorToolbar
        (
            new EditorToolbarTab[]
            {
                new EditorToolbarTab("General", "Allows you to change general gizmo settings."),
                new EditorToolbarTab("Move gizmo", "Allows you to change move gizmo settings."),
                new EditorToolbarTab("Rotation gizmo", "Allows you to change rotation settings."),
                new EditorToolbarTab("Scale gizmo", "Allows you to change scale gizmo settings."),
                new EditorToolbarTab("Box scale gizmo", "Allows you to change box scale gizmo settings."),
                new EditorToolbarTab("Universal gizmo", "Allows you to change universal gizmo settings."),
                new EditorToolbarTab("Extrude gizmo", "Allows you to change extrude gizmo settings.")
            },
            4, Color.green
        );

        [SerializeField]
        private UniversalGizmoConfig _universalGizmoConfig = new UniversalGizmoConfig();

        private GizmoCollectionEnabledStateSnapshot _gizmosEnabledStateSnapshot = new GizmoCollectionEnabledStateSnapshot();
        private List<ObjectSelectionGizmo> _allGizmos = new List<ObjectSelectionGizmo>();
        private List<ObjectTransformGizmo> _objectTransformGizmos = new List<ObjectTransformGizmo>();
        private int _workGizmoId;
        private ObjectSelectionGizmo _workGizmo;

        private bool _areGizmosVisible = true;
        private GizmoSpace _transformSpace = GizmoSpace.Global;

        private GameObject _pivotObject;
        private IEnumerable<GameObject> _targetObjectCollection;

        [SerializeField]
        private ObjectSelectionGizmosHotkeys _hotkeys = new ObjectSelectionGizmosHotkeys();

        [SerializeField]
        private MoveGizmoSettings2D _moveGizmoSettings2D = new MoveGizmoSettings2D() { IsExpanded = false };
        [SerializeField]
        private MoveGizmoSettings3D _moveGizmoSettings3D = new MoveGizmoSettings3D() { IsExpanded = false };
        [SerializeField]
        private MoveGizmoLookAndFeel2D _moveGizmoLookAndFeel2D = new MoveGizmoLookAndFeel2D() { IsExpanded = false };
        [SerializeField]
        private MoveGizmoLookAndFeel3D _moveGizmoLookAndFeel3D = new MoveGizmoLookAndFeel3D() { IsExpanded = false };
        [SerializeField]
        private MoveGizmoHotkeys _moveGizmoHotkeys = new MoveGizmoHotkeys() { IsExpanded = false };
        [SerializeField]
        private ObjectTransformGizmoSettings _objectMoveGizmoSettings = new ObjectTransformGizmoSettings() { IsExpanded = false };

        [SerializeField]
        private RotationGizmoSettings3D _rotationGizmoSettings3D = new RotationGizmoSettings3D() { IsExpanded = false };
        [SerializeField]
        private RotationGizmoLookAndFeel3D _rotationGizmoLookAndFeel3D = new RotationGizmoLookAndFeel3D() { IsExpanded = false };
        [SerializeField]
        private RotationGizmoHotkeys _rotationGizmoHotkeys = new RotationGizmoHotkeys() { IsExpanded = false };
        [SerializeField]
        private ObjectTransformGizmoSettings _objectRotationGizmoSettings = new ObjectTransformGizmoSettings() { IsExpanded = false };

        [SerializeField]
        private ScaleGizmoSettings3D _scaleGizmoSettings3D = new ScaleGizmoSettings3D() { IsExpanded = false };
        [SerializeField]
        private ScaleGizmoLookAndFeel3D _scaleGizmoLookAndFeel3D = new ScaleGizmoLookAndFeel3D() { IsExpanded = false };
        [SerializeField]
        private ScaleGizmoHotkeys _scaleGizmoHotkeys = new ScaleGizmoHotkeys() { IsExpanded = false };
        [SerializeField]
        private ObjectTransformGizmoSettings _objectScaleGizmoSettings = new ObjectTransformGizmoSettings() { IsExpanded = false };

        [SerializeField]
        private UniversalGizmoSettings2D _universalGizmoSettings2D = new UniversalGizmoSettings2D() { IsExpanded = false };
        [SerializeField]
        private UniversalGizmoSettings3D _universalGizmoSettings3D = new UniversalGizmoSettings3D() { IsExpanded = false };
        [SerializeField]
        private UniversalGizmoLookAndFeel2D _universalGizmoLookAndFeel2D = new UniversalGizmoLookAndFeel2D() { IsExpanded = false };
        [SerializeField]
        private UniversalGizmoLookAndFeel3D _universalGizmoLookAndFeel3D = new UniversalGizmoLookAndFeel3D() { IsExpanded = false };
        [SerializeField]
        private UniversalGizmoHotkeys _universalGizmoHotkeys = new UniversalGizmoHotkeys() { IsExpanded = false };
        [SerializeField]
        private ObjectTransformGizmoSettings _objectUniversalGizmoSettings = new ObjectTransformGizmoSettings() { IsExpanded = false };
  
        [SerializeField]
        private BoxGizmoSettings3D _boxScaleGizmoSettings3D = new BoxGizmoSettings3D() { IsExpanded = false };
        [SerializeField]
        private BoxGizmoLookAndFeel3D _boxScaleGizmoLookAndFeel3D = new BoxGizmoLookAndFeel3D() { IsExpanded = false };
        [SerializeField]
        private BoxGizmoHotkeys _boxScaleGizmoHotkeys = new BoxGizmoHotkeys() { IsExpanded = false };

        [SerializeField]
        private ObjectExtrudeGizmoLookAndFeel3D _extrudeGizmoLookAndFeel3D = new ObjectExtrudeGizmoLookAndFeel3D() { IsExpanded = false };
        [SerializeField]
        private ObjectExtrudeGizmoHotkeys _extrudeGizmoHotkeys = new ObjectExtrudeGizmoHotkeys() { IsExpanded = false };

        public bool AreGizmosVisible { get { return _areGizmosVisible; } }
        public GameObject PivotObject { get { return _pivotObject; } }
        public Gizmo WorkGizmo { get { return _workGizmo.Gizmo; } }
        public ObjectSelectionGizmosHotkeys Hotkeys { get { return _hotkeys; } }
        public MoveGizmoSettings2D MoveGizmoSettings2D { get { return _moveGizmoSettings2D; } }
        public MoveGizmoSettings3D MoveGizmoSettings3D { get { return _moveGizmoSettings3D; } }
        public MoveGizmoLookAndFeel2D MoveGizmoLookAndFeel2D { get { return _moveGizmoLookAndFeel2D; } }
        public MoveGizmoLookAndFeel3D MoveGizmoLookAndFeel3D { get { return _moveGizmoLookAndFeel3D; } }
        public MoveGizmoHotkeys MoveGizmoHotkeys { get { return _moveGizmoHotkeys; } }
        public ObjectTransformGizmoSettings ObjectMoveGizmoSettings { get { return _objectMoveGizmoSettings; } }
        public RotationGizmoSettings3D RotationGizmoSettings3D { get { return _rotationGizmoSettings3D; } }
        public RotationGizmoLookAndFeel3D RotationGizmoLookAndFeel3D { get { return _rotationGizmoLookAndFeel3D; } }
        public RotationGizmoHotkeys RotationGizmoHotkeys { get { return _rotationGizmoHotkeys; } }
        public ObjectTransformGizmoSettings ObjectRotationGizmoSettings { get { return _objectRotationGizmoSettings; } }
        public ScaleGizmoSettings3D ScaleGizmoSettings3D { get { return _scaleGizmoSettings3D; } }
        public ScaleGizmoLookAndFeel3D ScaleGizmoLookAndFeel3D { get { return _scaleGizmoLookAndFeel3D; } }
        public ScaleGizmoHotkeys ScaleGizmoHotkeys { get { return _scaleGizmoHotkeys; } }
        public ObjectTransformGizmoSettings ObjectScaleGizmoSettings { get { return _objectScaleGizmoSettings; } }
        public UniversalGizmoSettings2D UniversalGizmoSettings2D { get { return _universalGizmoSettings2D; } }
        public UniversalGizmoSettings3D UniversalGizmoSettings3D { get { return _universalGizmoSettings3D; } }
        public UniversalGizmoLookAndFeel2D UniversalGizmoLookAndFeel2D { get { return _universalGizmoLookAndFeel2D; } }
        public UniversalGizmoLookAndFeel3D UniversalGizmoLookAndFeel3D { get { return _universalGizmoLookAndFeel3D; } }
        public UniversalGizmoHotkeys UniversalGizmoHotkeys { get { return _universalGizmoHotkeys; } }
        public ObjectTransformGizmoSettings ObjectUniversalGizmoSettings { get { return _objectUniversalGizmoSettings; } }
        public BoxGizmoSettings3D BoxScaleGizmoSettings3D { get { return _boxScaleGizmoSettings3D; } }
        public BoxGizmoLookAndFeel3D BoxScaleGizmoLookAndFeel3D { get { return _boxScaleGizmoLookAndFeel3D; } }
        public BoxGizmoHotkeys BoxScaleGizmoHotkeys { get { return _boxScaleGizmoHotkeys; } }
        public ObjectExtrudeGizmoLookAndFeel3D ExtrudeGizmoLookAndFeel3D { get { return _extrudeGizmoLookAndFeel3D; } }
        public ObjectExtrudeGizmoHotkeys ExtrudeGozmoHotkeys { get { return _extrudeGizmoHotkeys; } }
             
        #if UNITY_EDITOR
        public EditorToolbar MainToolbar { get { return _mainToolbar; } }
        public UniversalGizmoConfig UniversalGizmoConfig { get { return _universalGizmoConfig; } }
        #endif

        public void SetTargetObjectCollection(IEnumerable<GameObject> targetObjectCollection)
        {
            _targetObjectCollection = targetObjectCollection;
        }

        public void Initialize_SystemCall()
        {
            var objectMoveGizmo = RTGizmosEngine.Get.CreateObjectMoveGizmo();
            RegisterGizmo(ObjectSelectionGizmoId.MoveGizmo, objectMoveGizmo.Gizmo);
            _workGizmo = GetObjectSelectionGizmo(objectMoveGizmo.Gizmo);
            _workGizmo.Gizmo.SetEnabled(false);
            _workGizmoId = ObjectSelectionGizmoId.MoveGizmo;

            var objectRotationGizmo = RTGizmosEngine.Get.CreateObjectRotationGizmo();
            RegisterGizmo(ObjectSelectionGizmoId.RotationGizmo, objectRotationGizmo.Gizmo);
            objectRotationGizmo.Gizmo.SetEnabled(false);

            var objectScaleGizmo = RTGizmosEngine.Get.CreateObjectScaleGizmo();
            RegisterGizmo(ObjectSelectionGizmoId.ScaleGizmo, objectScaleGizmo.Gizmo);
            objectScaleGizmo.Gizmo.SetEnabled(false);

            var objectBoxScaleGizmo = RTGizmosEngine.Get.CreateObjectBoxScaleGizmo();
            RegisterGizmo(ObjectSelectionGizmoId.BoxScaleGizmo, objectBoxScaleGizmo.Gizmo);
            objectBoxScaleGizmo.Gizmo.SetEnabled(false);

            var objectUniversalGizmo = RTGizmosEngine.Get.CreateObjectUniversalGizmo();
            RegisterGizmo(ObjectSelectionGizmoId.UniversalGizmo, objectUniversalGizmo.Gizmo);
            objectUniversalGizmo.Gizmo.SetEnabled(false);

            var objectExtrudeGizmo = RTGizmosEngine.Get.CreateObjectExtrudeGizmo();
            RegisterGizmo(ObjectSelectionGizmoId.ExtrudeGizmo, objectExtrudeGizmo.Gizmo);
            objectExtrudeGizmo.Gizmo.SetEnabled(false);

            RTUndoRedo.Get.UndoEnd += OnUndoRedo;
            RTUndoRedo.Get.RedoEnd += OnUndoRedo;
            RTObjectSelection.Get.Changed += OnObjectSelectionChanged;
            RTObjectSelection.Get.Rotated += OnObjectSelectionRotated;
            RTObjectSelection.Get.Enabled += OnObjectSelectionEnabled;
            RTObjectSelection.Get.Disabled += OnObjectSelectionDisabled;
            RTObjectSelection.Get.ManipSessionBegin += OnObjectSelectionManipSessionBegin;
            RTObjectSelection.Get.ManipSessionEnd += OnObjectSelectionManipSessionEnd;

            SetTransformSpace(GizmoSpace.Local);
            SetTransformPivot(GizmoObjectTransformPivot.ObjectGroupCenter);
        }

        public void SetGizmoUsable(int gizmoId, bool isUsable)
        {
            var selectionGizmo = GetObjectSelectionGizmo(gizmoId);
            if (selectionGizmo != null)
            {
                if (selectionGizmo.IsUsable != isUsable)
                {
                    selectionGizmo.IsUsable = isUsable;
                    if (!selectionGizmo.IsUsable && _workGizmo.Id == selectionGizmo.Id)
                    {
                        selectionGizmo.Gizmo.SetEnabled(false);
                    }
                }
            }
        }

        public Gizmo GetGizmoById(int gizmoId)
        {
            if (IsGizmoRegistered(gizmoId)) return GetObjectSelectionGizmo(gizmoId).Gizmo;
            return null;
        }

        public List<Gizmo> GetAllGizmos()
        {
            if (_allGizmos.Count == 0) return new List<Gizmo>();

            var allGizmos = new List<Gizmo>(_allGizmos.Count);
            foreach(var selectionGizmo in _allGizmos)
            {
                allGizmos.Add(selectionGizmo.Gizmo);
            }

            return allGizmos;
        }

        public int GetGizmoId(Gizmo gizmo)
        {
            var selectionGizmos = _allGizmos.FindAll(item => item.Gizmo == gizmo);
            if (selectionGizmos.Count == 0) return ObjectSelectionGizmoId.None;

            return selectionGizmos[0].Id;
        }

        public ObjectTransformGizmo GetTransformGizmoById(int id)
        {
            var gizmos = _allGizmos.FindAll(item => item.Id == id);
            if (gizmos.Count == 0) return null;

            return gizmos[0].Gizmo.GetFirstBehaviourOfType<ObjectTransformGizmo>();
        }

        public void SetTransformPivot(GizmoObjectTransformPivot transformPivot)
        {
            foreach (var transformGizmo in _objectTransformGizmos)
                transformGizmo.SetTransformPivot(transformPivot);
        }

        public void SetTransformSpace(GizmoSpace transformSpace)
        {
            if (_transformSpace == transformSpace) return;

            _transformSpace = transformSpace;
            foreach(var gizmo in _allGizmos)
            {
                if (gizmo.IsTransformGizmo) gizmo.TransformGizmo.SetTransformSpace(transformSpace);
                else if (gizmo.IsExtrudeGizmo) gizmo.ExtrudeGizmo.SetExtrudeSpace(transformSpace);
            }
        }

        public void SetWorkGizmo(int gizmoId)
        {
            if (gizmoId != ObjectSelectionGizmoId.None)
            {
                bool wasEnabled = _workGizmo.Gizmo.IsEnabled;
                _workGizmo.Gizmo.SetEnabled(false);
                _workGizmo = GetObjectSelectionGizmo(gizmoId);
                if (_areGizmosVisible && _workGizmo.IsUsable && wasEnabled) _workGizmo.Gizmo.SetEnabled(true);
                else _workGizmo.Gizmo.SetEnabled(false);
            }
            else
            {
                if (_workGizmo != null) _workGizmo.Gizmo.SetEnabled(false);
            }

            _workGizmoId = gizmoId;
        }

        public void SetGizmosVisisble(bool visible)
        {
            if (_areGizmosVisible == visible) return;

            _areGizmosVisible = visible;
            if (!_areGizmosVisible)
            {
                var allGizmos = GetAllGizmos();
                foreach (var gizmo in allGizmos)
                    gizmo.SetEnabled(false);
            }
            OnTargetObjectGroupUpdated();
        }

        public void Update_SystemCall()
        {
            bool isManpiSessionActive = RTObjectSelection.Get.IsManipSessionActive;
            if (!isManpiSessionActive && RTObjectSelection.Get.NumSelectedObjects != 0)
            {
                int gizmoId = ObjectSelectionGizmoId.None;

                if (Hotkeys.ActivateMoveGizmo.IsActiveInFrame()) gizmoId = ObjectSelectionGizmoId.MoveGizmo;
                else
                if (Hotkeys.ActivateRotationGizmo.IsActiveInFrame()) gizmoId = ObjectSelectionGizmoId.RotationGizmo;
                else
                if (Hotkeys.ActivateScaleGizmo.IsActiveInFrame()) gizmoId = ObjectSelectionGizmoId.ScaleGizmo;
                else
                if (Hotkeys.ActivateBoxScaleGizmo.IsActiveInFrame()) gizmoId = ObjectSelectionGizmoId.BoxScaleGizmo;
                else
                if (Hotkeys.ActivateUniversalGizmo.IsActiveInFrame()) gizmoId = ObjectSelectionGizmoId.UniversalGizmo;
                else
                if (Hotkeys.ActivateExtrudeGizmo.IsActiveInFrame()) gizmoId = ObjectSelectionGizmoId.ExtrudeGizmo;

                if (gizmoId != ObjectSelectionGizmoId.None) SetWorkGizmo(gizmoId);
            }

            if (!isManpiSessionActive && 
                 Hotkeys.ToggleTransformSpace.IsActiveInFrame())
            {
                GizmoSpace newSpace = _transformSpace == GizmoSpace.Global ? GizmoSpace.Local : GizmoSpace.Global;
                SetTransformSpace(newSpace);
            }
        }

        private void OnObjectSelectionChanged(ObjectSelectionChangedEventArgs args)
        {
            if (args.SelectReason != ObjectSelectReason.None)
            {
                if(args.SelectReason == ObjectSelectReason.Undo || 
                   args.SelectReason == ObjectSelectReason.Redo)
                {
                    _pivotObject = args.UndoRedoSnapshot.GizmosSnapshot.PivotObject;
                }
                else
                if (args.SelectReason == ObjectSelectReason.MultiSelect ||
                    args.SelectReason == ObjectSelectReason.MultiSelectAppend)
                {
                    if (_pivotObject == null) _pivotObject = GameObjectEx.FilterParentsOnly(args.ObjectsWhichWereSelected)[0];
                }
                else
                {
                    if (args.NumObjectsSelected != 0) _pivotObject = GameObjectEx.FilterParentsOnly(args.ObjectsWhichWereSelected)[0];
                    else _pivotObject = null;
                }
            }
            else if (args.DeselectReason != ObjectDeselectReason.None)
            {
                if (args.DeselectReason == ObjectDeselectReason.Undo ||
                    args.DeselectReason == ObjectDeselectReason.Redo)
                {
                    _pivotObject = args.UndoRedoSnapshot.GizmosSnapshot.PivotObject;
                }
                else
                if (RTObjectSelection.Get.NumSelectedObjects != 0)
                {
                    if (!RTObjectSelection.Get.IsObjectSelected(_pivotObject))
                        _pivotObject = GameObjectEx.FilterParentsOnly(_targetObjectCollection)[0];
                }
                else _pivotObject = null;
            }

            OnTargetObjectGroupUpdated();
        }

        private void OnUndoRedo(IUndoRedoAction action)
        {
            OnTargetObjectGroupUpdated();
        }

        private void OnGizmoPostEnabled(Gizmo gizmo)
        {
            var selectionGizmo = GetObjectSelectionGizmo(gizmo);
            if (selectionGizmo.IsTransformGizmo) selectionGizmo.TransformGizmo.RefreshPositionAndRotation();
            else if (selectionGizmo.IsBoxScaleGizmo) _workGizmo.BoxScaleGizmo.SetTargetHierarchy(_pivotObject);
            else if (selectionGizmo.IsExtrudeGizmo) _workGizmo.ExtrudeGizmo.SetExtrudeTargets(_targetObjectCollection);
        }

        private void OnTargetObjectGroupUpdated()
        {
            foreach (var transformGizmo in _objectTransformGizmos)
            {
                transformGizmo.SetTargetPivotObject(_pivotObject);
            }

            if (_areGizmosVisible && _workGizmo.IsUsable && _workGizmoId != ObjectSelectionGizmoId.None)
            {
                if (_pivotObject != null) _workGizmo.Gizmo.SetEnabled(true);
                else _workGizmo.Gizmo.SetEnabled(false);
            }

            if (_workGizmo.IsBoxScaleGizmo) _workGizmo.BoxScaleGizmo.SetTargetHierarchy(_pivotObject);
            else if (_workGizmo.IsExtrudeGizmo) _workGizmo.ExtrudeGizmo.SetExtrudeTargets(_targetObjectCollection);
        }

        private void OnObjectSelectionManipSessionBegin(ObjectSelectionManipSession manipSession)
        {
            var allGizmos = GetAllGizmos();
            _gizmosEnabledStateSnapshot.Snapshot(allGizmos);

            foreach (var gizmo in allGizmos) gizmo.SetEnabled(false);
        }

        private void OnObjectSelectionManipSessionEnd(ObjectSelectionManipSession manipSession)
        {
            _gizmosEnabledStateSnapshot.Apply();
        }

        private void OnObjectSelectionRotated()
        {
            foreach (var gizmo in _allGizmos)
            {
                if (gizmo.IsTransformGizmo) gizmo.TransformGizmo.RefreshPositionAndRotation();
                else if (gizmo.IsBoxScaleGizmo) gizmo.BoxScaleGizmo.FitBoxToTargetHierarchy();
                else if (gizmo.IsExtrudeGizmo) gizmo.ExtrudeGizmo.FitBoxToTargets();
            }
        }

        private void OnObjectSelectionEnabled()
        {
            _gizmosEnabledStateSnapshot.Apply();
        }

        private void OnObjectSelectionDisabled()
        {
            var allGizmos = GetAllGizmos();
            _gizmosEnabledStateSnapshot.Snapshot(allGizmos);

            foreach (var gizmo in allGizmos) gizmo.SetEnabled(false);
        }

        private ObjectSelectionGizmo GetObjectSelectionGizmo(Gizmo gizmo)
        {
            foreach(var objectSelectionGizmo in _allGizmos)
            {
                if (objectSelectionGizmo.Gizmo == gizmo) return objectSelectionGizmo;
            }

            return null;
        }

        private ObjectSelectionGizmo GetObjectSelectionGizmo(int id)
        {
            foreach (var objectSelectionGizmo in _allGizmos)
            {
                if (objectSelectionGizmo.Id == id) return objectSelectionGizmo;
            }

            return null;
        }

        private bool IsGizmoRegistered(int gizmoId)
        {
            return _allGizmos.FindAll(item => item.Id == gizmoId).Count != 0;
        }

        private bool IsGizmoRegistered(Gizmo gizmo)
        {
            return _allGizmos.FindAll(item => item.Gizmo == gizmo).Count != 0;
        }

        private bool RegisterGizmo(int gizmoId, Gizmo gizmo)
        {
            if (IsGizmoRegistered(gizmoId) || IsGizmoRegistered(gizmo)) return false;
            _allGizmos.Add(new ObjectSelectionGizmo(gizmoId, gizmo));

            ObjectTransformGizmo transformGizmo = gizmo.GetFirstBehaviourOfType<ObjectTransformGizmo>();
            if (transformGizmo != null)
            {
                _objectTransformGizmos.Add(transformGizmo);
                transformGizmo.SetTargetObjects(_targetObjectCollection);
            }

            var moveGizmo = gizmo.GetFirstBehaviourOfType<MoveGizmo>();
            if (moveGizmo != null)
            {
                moveGizmo.SharedSettings2D = MoveGizmoSettings2D;
                moveGizmo.SharedSettings3D = MoveGizmoSettings3D;
                moveGizmo.SharedLookAndFeel2D = MoveGizmoLookAndFeel2D;
                moveGizmo.SharedLookAndFeel3D = MoveGizmoLookAndFeel3D;
                moveGizmo.SharedHotkeys = MoveGizmoHotkeys;
                moveGizmo.SetVertexSnapTargetObjects(_targetObjectCollection);
                if (transformGizmo != null) transformGizmo.SharedSettings = ObjectMoveGizmoSettings;
            }

            var rotationGizmo = gizmo.GetFirstBehaviourOfType<RotationGizmo>();
            if (rotationGizmo != null)
            {
                rotationGizmo.SharedSettings3D = RotationGizmoSettings3D;
                rotationGizmo.SharedLookAndFeel3D = RotationGizmoLookAndFeel3D;
                rotationGizmo.SharedHotkeys = RotationGizmoHotkeys;
                if (transformGizmo != null) transformGizmo.SharedSettings = ObjectRotationGizmoSettings;
            }

            var scaleGizmo = gizmo.GetFirstBehaviourOfType<ScaleGizmo>();
            if (scaleGizmo != null)
            {
                scaleGizmo.SharedSettings3D = ScaleGizmoSettings3D;
                scaleGizmo.SharedLookAndFeel3D = ScaleGizmoLookAndFeel3D;
                scaleGizmo.SharedHotkeys = ScaleGizmoHotkeys;
                scaleGizmo.SetScaleGuideTargetObjects(_targetObjectCollection);
                if (transformGizmo != null) transformGizmo.SharedSettings = ObjectScaleGizmoSettings;
            }

            var boxScaleGizmo = gizmo.GetFirstBehaviourOfType<BoxGizmo>();
            if (boxScaleGizmo != null)
            {
                boxScaleGizmo.SharedSettings3D = BoxScaleGizmoSettings3D;
                boxScaleGizmo.SharedLookAndFeel3D = BoxScaleGizmoLookAndFeel3D;
                boxScaleGizmo.SharedHotkeys = BoxScaleGizmoHotkeys;
            }

            var universalGizmo = gizmo.GetFirstBehaviourOfType<UniversalGizmo>();
            if (universalGizmo != null)
            {
                universalGizmo.SharedSettings2D = UniversalGizmoSettings2D;
                universalGizmo.SharedSettings3D = UniversalGizmoSettings3D;
                universalGizmo.SharedLookAndFeel2D = UniversalGizmoLookAndFeel2D;
                universalGizmo.SharedLookAndFeel3D = UniversalGizmoLookAndFeel3D;
                universalGizmo.SharedHotkeys = UniversalGizmoHotkeys;
                universalGizmo.SetMvVertexSnapTargetObjects(_targetObjectCollection);
                universalGizmo.SetScaleGuideTargetObjects(_targetObjectCollection);
                if (transformGizmo != null) transformGizmo.SharedSettings = ObjectUniversalGizmoSettings;
            }

            var extrudeGizmo = gizmo.GetFirstBehaviourOfType<ObjectExtrudeGizmo>();
            if (extrudeGizmo != null)
            {
                extrudeGizmo.SharedLookAndFeel3D = ExtrudeGizmoLookAndFeel3D;
                extrudeGizmo.SharedHotkeys = ExtrudeGozmoHotkeys;
                extrudeGizmo.SetExtrudeTargets(_targetObjectCollection);
            }

            gizmo.PostEnabled += OnGizmoPostEnabled;
            return true;
        }
    }
}
