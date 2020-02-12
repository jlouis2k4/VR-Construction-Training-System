using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    public delegate void GizmoEngineCanDoHoverUpdateHandler(YesNoAnswer answer);

    public enum GizmosEnginePipelineStage
    {
        Update,
        PostUpdate,
        Render,
        PostRender,
        GUI,
        PostGUI
    }

    public class RTGizmosEngine : MonoSingleton<RTGizmosEngine>, IHoverableSceneEntityContainer
    {
        public event GizmoEngineCanDoHoverUpdateHandler CanDoHoverUpdate;

        [SerializeField]
        private EditorToolbar _mainToolbar = new EditorToolbar
        (
            new EditorToolbarTab[]
            {
                new EditorToolbarTab("General", "General gizmo engine settings."),
                new EditorToolbarTab("Scene gizmo", "Scene gizmo specific settings.")
            },
            2, Color.green
        );

        [SerializeField]
        private GizmoEngineSettings _settings = new GizmoEngineSettings();
        private GizmosEnginePipelineStage _pipelineStage = GizmosEnginePipelineStage.Update;
        private Gizmo _draggedGizmo;
        private Gizmo _hoveredGizmo;
        private GizmoHoverInfo _gizmoHoverInfo;
        private List<Gizmo> _gizmos = new List<Gizmo>();
        private List<ISceneGizmo> _sceneGizmos = new List<ISceneGizmo>();
        private List<RTSceneGizmoCamera> _sceneGizmoCameras = new List<RTSceneGizmoCamera>();
        private List<Camera> _renderCameras = new List<Camera>();

        [SerializeField]
        private SceneGizmoLookAndFeel _sharedSceneGizmoLookAndFeel = new SceneGizmoLookAndFeel();

        public GizmoEngineSettings Settings { get { return _settings; } }
        public GizmosEnginePipelineStage PipelineStage { get { return _pipelineStage; } }
        public Camera RenderStageCamera { get { return Camera.current; } }
        public bool HasHoveredSceneEntity { get { return IsAnyGizmoHovered; } }
        public bool IsAnyGizmoHovered { get { return _hoveredGizmo != null; } }
        public Gizmo HoveredGizmo { get { return _hoveredGizmo; } }
        public Gizmo DraggedGizmo { get { return _draggedGizmo; } }
        public int NumRenderCameras { get { return _renderCameras.Count; } }
        public SceneGizmoLookAndFeel SharedSceneGizmoLookAndFeel { get { return _sharedSceneGizmoLookAndFeel; } }

        #if UNITY_EDITOR
        public EditorToolbar MainToolbar { get { return _mainToolbar; } }
        #endif

        public void AddRenderCamera(Camera camera)
        {
            if (!IsRenderCamera(camera) && !IsSceneGizmoCamera(camera)) _renderCameras.Add(camera);
        }

        public bool IsRenderCamera(Camera camera)
        {
            return _renderCameras.Contains(camera);
        }

        public void RemoveRenderCamera(Camera camera)
        {
            _renderCameras.Remove(camera);
        }

        public RTSceneGizmoCamera CreateSceneGizmoCamera(Camera sceneCamera, ISceneGizmoCamViewportUpdater viewportUpdater)
        {
            GameObject sceneGizmoCamObject = new GameObject(typeof(RTSceneGizmoCamera).ToString());
            sceneGizmoCamObject.transform.parent = RTGizmosEngine.Get.transform;

            RTSceneGizmoCamera sgCamera = sceneGizmoCamObject.AddComponent<RTSceneGizmoCamera>();
            sgCamera.ViewportUpdater = viewportUpdater;
            sgCamera.SceneCamera = sceneCamera;

            _sceneGizmoCameras.Add(sgCamera);

            return sgCamera;
        }

        public bool IsSceneGizmoCamera(Camera camera)
        {
            int numCameras = _sceneGizmoCameras.Count;
            for (int camIndex = 0; camIndex < numCameras; ++camIndex)
            {
                if (_sceneGizmoCameras[camIndex].Camera == camera) return true;
            }

            return false;
        }

        public ISceneGizmo GetSceneGizmoByCamera(Camera sceneCamera)
        {
            foreach (var sceneGizmo in _sceneGizmos)
                if (sceneGizmo.SceneCamera == sceneCamera) return sceneGizmo;

            return null;
        }

        public Gizmo CreateGizmo()
        {
            Gizmo gizmo = new Gizmo();

            RegisterGizmo(gizmo);
            return gizmo;
        }

        public SceneGizmo CreateSceneGizmo(Camera sceneCamera)
        {
            if (GetSceneGizmoByCamera(sceneCamera) != null) return null;

            var gizmo = new Gizmo();
            RegisterGizmo(gizmo);

            var sceneGizmo = gizmo.AddBehaviour<SceneGizmo>();
            sceneGizmo.SceneGizmoCamera.SceneCamera = sceneCamera;
            sceneGizmo.SharedLookAndFeel = SharedSceneGizmoLookAndFeel;

            _sceneGizmos.Add(sceneGizmo);

            return sceneGizmo;
        }

        public MoveGizmo CreateMoveGizmo()
        {
            Gizmo gizmo = CreateGizmo();
            MoveGizmo moveGizmo = new MoveGizmo();
            gizmo.AddBehaviour(moveGizmo);

            return moveGizmo;
        }

        public ObjectTransformGizmo CreateObjectMoveGizmo()
        {
            MoveGizmo moveGizmo = CreateMoveGizmo();
            var transformGizmo = moveGizmo.Gizmo.AddBehaviour<ObjectTransformGizmo>();
            transformGizmo.SetTransformChannelFlags(ObjectTransformGizmo.Channels.Position);

            return transformGizmo;
        }

        public RotationGizmo CreateRotationGizmo()
        {
            Gizmo gizmo = CreateGizmo();
            RotationGizmo rotationGizmo = new RotationGizmo();
            gizmo.AddBehaviour(rotationGizmo);

            return rotationGizmo;
        }

        public ObjectTransformGizmo CreateObjectRotationGizmo()
        {
            RotationGizmo rotationGizmo = CreateRotationGizmo();
            var transformGizmo = rotationGizmo.Gizmo.AddBehaviour<ObjectTransformGizmo>();
            transformGizmo.SetTransformChannelFlags(ObjectTransformGizmo.Channels.Rotation);

            return transformGizmo;
        }

        public ScaleGizmo CreateScaleGizmo()
        {
            Gizmo gizmo = CreateGizmo();
            ScaleGizmo scaleGizmo = new ScaleGizmo();
            gizmo.AddBehaviour(scaleGizmo);

            return scaleGizmo;
        }

        public ObjectTransformGizmo CreateObjectScaleGizmo()
        {
            ScaleGizmo scaleGizmo = CreateScaleGizmo();
            var transformGizmo = scaleGizmo.Gizmo.AddBehaviour<ObjectTransformGizmo>();
            transformGizmo.SetTransformChannelFlags(ObjectTransformGizmo.Channels.Scale);
            transformGizmo.SetTransformSpace(GizmoSpace.Local);
            transformGizmo.MakeTransformSpacePermanent();

            return transformGizmo;
        }

        public UniversalGizmo CreateUniversalGizmo()
        {
            Gizmo gizmo = CreateGizmo();
            UniversalGizmo universalGizmo = new UniversalGizmo();
            gizmo.AddBehaviour(universalGizmo);

            return universalGizmo;
        }

        public ObjectTransformGizmo CreateObjectUniversalGizmo()
        {
            UniversalGizmo universalGizmo = CreateUniversalGizmo();
            var transformGizmo = universalGizmo.Gizmo.AddBehaviour<ObjectTransformGizmo>();
            transformGizmo.SetTransformChannelFlags(ObjectTransformGizmo.Channels.Position | ObjectTransformGizmo.Channels.Rotation | ObjectTransformGizmo.Channels.Scale);

            return transformGizmo;
        }

        public BoxGizmo CreateBoxGizmo()
        {
            Gizmo gizmo = CreateGizmo();
            BoxGizmo boxGizmo = new BoxGizmo();
            gizmo.AddBehaviour(boxGizmo);

            return boxGizmo;
        }

        public BoxGizmo CreateObjectBoxScaleGizmo()
        {
            BoxGizmo boxGizmo = CreateBoxGizmo();
            boxGizmo.SetUsage(BoxGizmo.Usage.ObjectScale);
            boxGizmo.MakeUsagePermanent();

            return boxGizmo;
        }

        public ObjectExtrudeGizmo CreateObjectExtrudeGizmo()
        {
            Gizmo gizmo = CreateGizmo();
            ObjectExtrudeGizmo extrudeGizmo = new ObjectExtrudeGizmo();
            gizmo.AddBehaviour(extrudeGizmo);

            return extrudeGizmo;
        }

        public void Update_SystemCall()
        {
            foreach (var sceneGizmoCam in _sceneGizmoCameras)
                sceneGizmoCam.Update_SystemCall();

            _pipelineStage = GizmosEnginePipelineStage.Update;
            IInputDevice inputDevice = RTInputDevice.Get.Device;
            bool deviceHasPointer = inputDevice.HasPointer();
            Vector3 inputDevicePos = inputDevice.GetPositionYAxisUp();

            bool isUIHovered = RTScene.Get.IsAnyUIElementHovered();
            bool canUpdateHoverInfo = _draggedGizmo == null && !isUIHovered;

            if (canUpdateHoverInfo)
            {
                YesNoAnswer answer = new YesNoAnswer();
                if (CanDoHoverUpdate != null) CanDoHoverUpdate(answer);
                if (answer.HasNo) canUpdateHoverInfo = false;
            }

            if (canUpdateHoverInfo) 
            {
                _hoveredGizmo = null;
                _gizmoHoverInfo.Reset();
            }

            bool isDeviceInsideFocusCamera = deviceHasPointer && RTFocusCamera.Get.IsViewportHoveredByDevice(); //RTFocusCamera.Get.TargetCamera.pixelRect.Contains(inputDevicePos);
            bool focusCameraCanRenderGizmos = IsRenderCamera(RTFocusCamera.Get.TargetCamera);
            var hoverDataCollection = new List<GizmoHandleHoverData>(10);
            foreach (var gizmo in _gizmos)
            {
                gizmo.OnUpdateBegin_SystemCall();
                if (canUpdateHoverInfo && gizmo.IsEnabled &&
                    isDeviceInsideFocusCamera && deviceHasPointer && focusCameraCanRenderGizmos)
                {
                    var handleHoverData = GetGizmoHandleHoverData(gizmo);
                    if (handleHoverData != null) hoverDataCollection.Add(handleHoverData);
                }
            }

            GizmoHandleHoverData hoverData = null;
            if (canUpdateHoverInfo && hoverDataCollection.Count != 0)
            {
                SortHandleHoverDataCollection(hoverDataCollection, inputDevicePos);

                hoverData = hoverDataCollection[0];
                _hoveredGizmo = hoverData.Gizmo;
                _gizmoHoverInfo.HandleId = hoverData.HandleId;
                _gizmoHoverInfo.HandleDimension = hoverData.HandleDimension;
                _gizmoHoverInfo.HoverPoint = hoverData.HoverPoint;
                _gizmoHoverInfo.IsHovered = true;
            }

            foreach (var gizmo in _gizmos)
            {
                _gizmoHoverInfo.IsHovered = (gizmo == _hoveredGizmo);
                gizmo.UpdateHandleHoverInfo_SystemCall(_gizmoHoverInfo);

                gizmo.HandleInputDeviceEvents_SystemCall();
                gizmo.OnUpdateEnd_SystemCall();
            }

            _pipelineStage = GizmosEnginePipelineStage.PostUpdate;
        }

        public GizmoHandleHoverData GetGizmoHandleHoverData(Gizmo gizmo)
        {
            Camera focusCamera = gizmo.FocusCamera;
            Ray hoverRay = RTInputDevice.Get.Device.GetRay(focusCamera);
            var hoverDataCollection = gizmo.GetAllHandlesHoverData(hoverRay);

            Vector3 screenRayOrigin = focusCamera.WorldToScreenPoint(hoverRay.origin);
            hoverDataCollection.Sort(delegate(GizmoHandleHoverData h0, GizmoHandleHoverData h1)
            {
                var handle0 = gizmo.GetHandleById_SystemCall(h0.HandleId);
                var handle1 = gizmo.GetHandleById_SystemCall(h1.HandleId);

                // Same dimensions?
                bool sameDims = (h0.HandleDimension == h1.HandleDimension);
                if (sameDims)
                {
                    // 2D dimension?
                    if (h0.HandleDimension == GizmoDimension.Dim2D)
                    {
                        // If the priorities are equal, we sort by distance from screen ray origin. 
                        // Otherwise, we sort by priority.
                        if (handle0.HoverPriority2D == handle1.HoverPriority2D)
                        {
                            float d0 = (screenRayOrigin - h0.HoverPoint).sqrMagnitude;
                            float d1 = (screenRayOrigin - h1.HoverPoint).sqrMagnitude;
                            return d0.CompareTo(d1);
                        }
                        else return handle0.HoverPriority2D.CompareTo(handle1.HoverPriority2D);
                    }
                    // 3D dimension
                    else
                    {
                        // If the priorities are equal, we sort by hover enter. Otherwise, we sort by priority.
                        if (handle0.HoverPriority3D == handle1.HoverPriority3D) return h0.HoverEnter3D.CompareTo(h1.HoverEnter3D);
                        else return handle0.HoverPriority3D.CompareTo(handle1.HoverPriority3D);
                    }
                }
                else
                {
                    // When the dimensions differ, we will sort by the gizmo generic priority. If the priorities are equal,
                    // we will give priority to 2D handles.
                    if (handle0.GenericHoverPriority == handle1.GenericHoverPriority)
                    {
                        if (h0.HandleDimension == GizmoDimension.Dim2D) return -1;
                        return 1;
                    }
                    return handle0.GenericHoverPriority.CompareTo(handle1.GenericHoverPriority);
                }
            });

            return hoverDataCollection.Count != 0 ? hoverDataCollection[0] : null;
        }

        public void Render_SystemCall()
        {
            _pipelineStage = GizmosEnginePipelineStage.Render;
            Camera renderCamera = Camera.current;

            if (!IsSceneGizmoCamera(renderCamera) && !IsRenderCamera(renderCamera))
            {
                _pipelineStage = GizmosEnginePipelineStage.PostRender;
                return;
            }

            if (Settings.EnableGizmoSorting)
            {
                Vector3 camPos = RenderStageCamera.transform.position;
                var sortedGizmos = new List<Gizmo>(_gizmos);
                sortedGizmos.Sort(delegate(Gizmo g0, Gizmo g1)
                {
                    float d0 = (g0.Transform.Position3D - camPos).sqrMagnitude;
                    float d1 = (g1.Transform.Position3D - camPos).sqrMagnitude;

                    return d1.CompareTo(d0);
                });

                var worldFrustumPlanes = CameraViewVolume.GetCameraWorldPlanes(renderCamera);
                foreach (var gizmo in sortedGizmos)
                {
                    gizmo.Render_SystemCall(renderCamera, worldFrustumPlanes);
                }
            }
            else
            {
                var worldFrustumPlanes = CameraViewVolume.GetCameraWorldPlanes(renderCamera);
                foreach (var gizmo in _gizmos)
                {
                    gizmo.Render_SystemCall(renderCamera, worldFrustumPlanes);
                }
            }

            _pipelineStage = GizmosEnginePipelineStage.PostRender;
        }

        private void SortHandleHoverDataCollection(List<GizmoHandleHoverData> hoverDataCollection, Vector3 inputDevicePos)
        {
            if (hoverDataCollection.Count == 0) return;

            Ray hoverRay = hoverDataCollection[0].HoverRay;
            hoverDataCollection.Sort(delegate(GizmoHandleHoverData h0, GizmoHandleHoverData h1)
            {
                // Same dimensions?
                bool sameDims = (h0.HandleDimension == h1.HandleDimension);
                if (sameDims)
                {
                    // 2D dimension?
                    if(h0.HandleDimension == GizmoDimension.Dim2D)
                    {
                        // If the gizmo 2D hover priorities are equal, we sort by distance from input device position. 
                        // Otherwise, we sort by priority.
                        if (h0.Gizmo.HoverPriority2D == h1.Gizmo.HoverPriority2D)
                        {
                            float d0 = (inputDevicePos - h0.HoverPoint).sqrMagnitude;
                            float d1 = (inputDevicePos - h1.HoverPoint).sqrMagnitude;
                            return d0.CompareTo(d1);
                        }
                        else return h0.Gizmo.HoverPriority2D.CompareTo(h1.Gizmo.HoverPriority2D);
                    }
                    // 3D dimension
                    else
                    {
                        // If the gizmo 3D hover priorities are equal, we sort by hover enter. Otherwise, we sort by priority.
                        if (h0.Gizmo.HoverPriority3D == h1.Gizmo.HoverPriority3D) return h0.HoverEnter3D.CompareTo(h1.HoverEnter3D);
                        else return h0.Gizmo.HoverPriority3D.CompareTo(h1.Gizmo.HoverPriority3D);
                    }
                }
                else
                {
                    // When the dimensions differ, we will sort by the generic priority. If the priorities are equal,
                    // we will sort by the distance between the gizmo position and the ray origin.
                    if (h0.Gizmo.GenericHoverPriority == h1.Gizmo.GenericHoverPriority)
                    {
                        float d0 = (h0.Gizmo.Transform.Position3D - hoverRay.origin).sqrMagnitude;
                        float d1 = (h1.Gizmo.Transform.Position3D - hoverRay.origin).sqrMagnitude;
                        return d0.CompareTo(d1);
                    }
                    return h0.Gizmo.GenericHoverPriority.CompareTo(h1.Gizmo.GenericHoverPriority);
                }
            });
        }

        private void RegisterGizmo(Gizmo gizmo)
        {
            _gizmos.Add(gizmo);
            gizmo.PreDragBegin += OnGizmoDragBegin;
            gizmo.PreDragEnd += OnGizmoDragEnd;
        }

        private void OnGUI()
        {
            _pipelineStage = GizmosEnginePipelineStage.GUI;
            foreach (var gizmo in _gizmos)
            {
                gizmo.OnGUI_SystemCall();
            }
            _pipelineStage = GizmosEnginePipelineStage.PostGUI;
        }

        private void OnGizmoDragBegin(Gizmo gizmo, int handleId)
        {
            _draggedGizmo = gizmo;
        }

        private void OnGizmoDragEnd(Gizmo gizmo, int handleId)
        {
            _draggedGizmo = null;
        }
    }
}
