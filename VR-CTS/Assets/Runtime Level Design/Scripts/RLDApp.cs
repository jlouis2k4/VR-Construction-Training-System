using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLD
{
    public delegate void RLDAppInitializedHandler();

    public class RLDApp : MonoSingleton<RLDApp>, IRLDApplication
    {
        public event RLDAppInitializedHandler Initialized;

        [SerializeField] [HideInInspector]
        private DynamicConvertSettings _dynamicConvertSettings = new DynamicConvertSettings();

        public DynamicConvertSettings DynamicConvertSettings { get { return _dynamicConvertSettings; } }

        // Note: Currently not used. Causes performance issues.
        private void OnSceneCanRenderCameraIcon(Camera camera, YesNoAnswer answer)
        {
            if (camera == RTFocusCamera.Get.TargetCamera ||
                RTGizmosEngine.Get.IsSceneGizmoCamera(camera)) answer.No();
        }

        private void OnCanCameraUseScrollWheel(YesNoAnswer answer)
        {
            if (RTScene.Get.IsAnyUIElementHovered()) answer.No();
            else answer.Yes();
        }

        private void OnCanCameraProcessInput(YesNoAnswer answer)
        {
            if (RTGizmosEngine.Get.DraggedGizmo != null) answer.No();
            else answer.Yes();
        }

        private void OnCanUndoRedo(UndoRedoOpType undoRedoOpType, YesNoAnswer answer)
        {
            if (RTGizmosEngine.Get.DraggedGizmo == null && !RTObjectSelection.Get.IsMultiSelectShapeVisible) answer.Yes();
            else answer.No();

            if (!RTObjectSelection.Get.IsManipSessionActive) answer.Yes();
            else answer.No();
        }

        private void OnCanDoGizmoHoverUpdate(YesNoAnswer answer)
        {
            if (RTObjectSelection.Get != null && 
                RTObjectSelection.Get.IsMultiSelectShapeVisible) answer.No();
            else answer.Yes();
        }

        private void OnCanObjectSelectionClickAndMultiSelectDeselect(YesNoAnswer answer)
        {
            if (RTSceneGrid.Get.Hotkeys.SnapToCursorPickPoint.IsActive()) answer.No();
            else answer.Yes();
        }

        private void OnViewportsCameraAdded(Camera camera)
        {
            RTGizmosEngine.Get.AddRenderCamera(camera);
        }

        private void OnViewportCameraRemoved(Camera camera)
        {
            RTGizmosEngine.Get.RemoveRenderCamera(camera);
        }

        private void Start()
        {
            // Undo/Redo
            RTUndoRedo.Get.CanUndoRedo += OnCanUndoRedo;

            // Camera
            RTFocusCamera.Get.CanProcessInput += OnCanCameraProcessInput;
            RTFocusCamera.Get.CanUseScrollWheel += OnCanCameraUseScrollWheel;
            RTCameraViewports.Get.CameraAdded += OnViewportsCameraAdded;
            RTCameraViewports.Get.CameraRemoved += OnViewportCameraRemoved;

            // Scene
            RTScene.Get.RegisterHoverableSceneEntityContainer(RTGizmosEngine.Get);
            RTSceneGrid.Get.Initialize_SystemCall();

            // Gizmo engine
            RTGizmosEngine.Get.CanDoHoverUpdate += OnCanDoGizmoHoverUpdate;
            RTGizmosEngine.Get.CreateSceneGizmo(RTFocusCamera.Get.TargetCamera);
            RTGizmosEngine.Get.AddRenderCamera(RTFocusCamera.Get.TargetCamera);

            // Object selection
            if (RTObjectSelection.Get != null)
            {
                ObjectSelectEntireHierarchy.Get.SetActive(true);
                RTObjectSelection.Get.CanClickSelectDeselect += OnCanObjectSelectionClickAndMultiSelectDeselect;
                RTObjectSelection.Get.CanMultiSelectDeselect += OnCanObjectSelectionClickAndMultiSelectDeselect;
                RTObjectSelection.Get.Initialize_SystemCall();

                if (RTObjectSelectionGizmos.Get != null)
                {
                    RTObjectSelection.Get.AttachGizmoController(RTObjectSelectionGizmos.Get);
                    RTObjectSelectionGizmos.Get.Initialize_SystemCall();
                }
            }

            RTMeshCompiler.CompileEntireScene();
            if (Initialized != null) Initialized();
        }

        private void Update()
        {
            // Note: Don't change the order :)
            RTInputDevice.Get.Update_SystemCall();
            RTFocusCamera.Get.Update_SystemCall();
            RTScene.Get.Update_SystemCall();
            RTSceneGrid.Get.Update_SystemCall();
            RTGizmosEngine.Get.Update_SystemCall();
            if (RTObjectSelection.Get != null) RTObjectSelection.Get.Update_SystemCall();
            if (RTObjectSelectionGizmos.Get != null) RTObjectSelectionGizmos.Get.Update_SystemCall();
            RTUndoRedo.Get.Update_SystemCall();
        }

        private void OnRenderObject()
        {
            if (RTGizmosEngine.Get.IsSceneGizmoCamera(Camera.current))
            {
                RTGizmosEngine.Get.Render_SystemCall();
            }
            else
            {
                // Note: Don't change the order :)
                if (RTCameraBackground.Get != null) RTCameraBackground.Get.Render_SystemCall();
                RTSceneGrid.Get.Render_SystemCall();
                RTScene.Get.Render_SystemCall();
                if (RTObjectSelection.Get != null) RTObjectSelection.Get.Render_SystemCall();
                RTGizmosEngine.Get.Render_SystemCall();
            }
        }

        #if UNITY_EDITOR
        [MenuItem("Tools/Runtime Level Design/Initialize")]
        public static void Initialize()
        {
            DestroyAppAndModules();
            RLDApp gizmosApp = CreateAppModuleObject<RLDApp>(null);
            Transform appTransform = gizmosApp.transform;

            CreateAppModuleObject<RTPrefabLibDb>(appTransform);
            CreateAppModuleObject<RTGizmosEngine>(appTransform);

            var scene = CreateAppModuleObject<RTScene>(appTransform);
            scene.LookAndFeel.LightIcon = TexturePool.Get.MainLightIcon;
            scene.LookAndFeel.ParticleSystemIcon = TexturePool.Get.MainParticleSystemIcon;
            scene.LookAndFeel.CameraIcon = TexturePool.Get.CameraIcon;
            CreateAppModuleObject<RTSceneGrid>(appTransform);

            CreateAppModuleObject<RTObjectSelection>(appTransform);
            CreateAppModuleObject<RTObjectSelectionGizmos>(appTransform);

            CreateAppModuleObject<RTObjectGroupDb>(appTransform);

            var focusCamera = CreateAppModuleObject<RTFocusCamera>(appTransform);
            focusCamera.SetTargetCamera(Camera.main);
            CreateAppModuleObject<RTCameraBackground>(appTransform);

            CreateAppModuleObject<RTInputDevice>(appTransform);
            CreateAppModuleObject<RTUndoRedo>(appTransform);
        }

        [MenuItem("Tools/Runtime Level Design/Utilities/Dynamic Convert")]
        public static void ShowDynamicConvertWindow()
        {
            RLDEditorWindow.ShowRLDWindow<DynamicConvertWindow>("Dynamic Convert", new Vector2(400.0f, 60.0f));
        }

        private static DataType CreateAppModuleObject<DataType>(Transform parentTransform) where DataType : MonoBehaviour
        {
            string objectName = typeof(DataType).ToString();
            int dotIndex = objectName.IndexOf(".");
            if (dotIndex >= 0) objectName = objectName.Remove(0, dotIndex + 1);

            GameObject moduleObject = new GameObject(objectName);
            moduleObject.transform.parent = parentTransform;

            return moduleObject.AddComponent<DataType>();
        }

        private static void DestroyAppAndModules()
        {
            Type[] allModuleTypes = GetAppModuleTypes();
            foreach (var moduleType in allModuleTypes)
            {
                var allModulesInScene = MonoBehaviour.FindObjectsOfType(moduleType);
                foreach(var module in allModulesInScene)
                {
                    MonoBehaviour moduleMono = module as MonoBehaviour;
                    if (moduleMono != null) DestroyImmediate(moduleMono.gameObject);
                }
            }
        }

        private static Type[] GetAppModuleTypes()
        {
            return new Type[]
            {
                typeof(RLDApp), typeof(RTFocusCamera), typeof(RTCameraBackground), 
                typeof(RTObjectSelection), typeof(RTObjectSelection), typeof(RTScene), typeof(RTSceneGrid), 
                typeof(RTInputDevice), typeof(RTUndoRedo), typeof(RTGizmosEngine), typeof(RTObjectGroupDb),
                typeof(RTPrefabLibDb),
            };
        }
        #endif
    }
}
