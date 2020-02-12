using UnityEngine;

namespace RLD
{
    /// <summary>
    /// A simple MonoBheaviour that can be used to capture a spawned prefab
    /// (i.e. the prefab created when you click on a prefab preview in the
    /// prefab lib UI) and snap it onto the surfaces of other objects or onto 
    /// the scene grid.
    /// </summary>
    public class PrefabSpawnSnap : MonoBehaviour
    {
        /// <summary>
        /// This will be set to true when a prefab is spawned. When a prefab is spawned
        /// it will be captured and moving the mouse will move the prefab along with it.
        /// </summary>
        private bool _isSnapSessionActive;
        /// <summary>
        /// The game object that is captured and is being snapped. This is the game object
        /// that results from the prefab instantiation.
        /// </summary>
        private GameObject _targetHierarchy;
        /// <summary>
        /// The snap config instance which controls certain aspects of how the object is
        /// snapped (e.g. axis alignment, offset from surface).
        /// </summary>
        private ObjectSurfaceSnap.SnapConfig _snapConfig = new ObjectSurfaceSnap.SnapConfig();
        /// <summary>
        /// This is a bit mask that indicates what layers can be used as snap surfaces. By default
        /// all bits are set, which means prefabs can be snapped on top of any layer. Feel free to
        /// change this as needed.
        /// </summary>
        private int _objectSurfaceLayers = ~0;

        /// <summary>
        /// Performs any necessary initializations.
        /// </summary>
        private void Awake()
        {
            // Initialize the snap config instance.
            // By default, we want the following:
            //      -align the game object to the surface on which it resides;
            //      -when alignment is turned on, align the object's positive Y axis;
            _snapConfig.AlignAxis = true;
            _snapConfig.AlignmentAxis = TransformAxis.PositiveY;

            // Let the RLD app inform us after it finishes initialization of the modules.
            // We will use the 'OnAppInitialized' handler for this. This handler will 
            // perform any initialization that depends on other RLD modules.
            RLDApp.Get.Initialized += OnAppInitialized;
        }

        /// <summary>
        /// Called every frame update.
        /// </summary>
        private void Update()
        {
            // Is the snap session active?
            if (_isSnapSessionActive)
            {
                // First, check if the condition for session termination is met. If it is,
                // call 'EndSnapSession' to terminate and return from the function.
                if (EvaluateSessionEndCondition())
                {
                    EndSnapSession();
                    return;
                }

                // If the input device (e.g. mouse) was moved, call the 'OnInputDeviceMoved'
                // function. This function will snap the target hierarchy to the scene objects
                // or scene grid depending on settings and what lies under the mouse cursor.
                var inputDevice = RTInputDevice.Get.Device;
                if (inputDevice.WasMoved()) OnInputDeviceMoved(inputDevice);
            }
        }

        /// <summary>
        /// During each frame update, this method is called to check if the snap
        /// session should terminate. By default, the session terminates when the
        /// left mouse button is clicked. Feel free to implement your own criteria
        /// for session termination inside here.
        /// </summary>
        /// <returns>
        /// True if the session must end and false otherwise.
        /// </returns>
        private bool EvaluateSessionEndCondition()
        {
            // If the left mouse button was pressed in the current frame, return true
            // to indicate that the session must terminate.
            var inputDevice = RTInputDevice.Get.Device;
            if (inputDevice.WasButtonPressedInCurrentFrame(0)) return true;

            // Note: Calling 'inputDevice.WasButtonReleasedInCurrentFrame' won't work, so
            //       you can not end the session when the mouse button is released because
            //       the prefab is spawned when the left mouse button is clicked on the 
            //       prefab preview which is followed by a mouse button up. If you attempt
            //       to do that, the session will terminate immediately and it will appear
            //       as if nothing happened (i.e. no snapping is performed).

            // The criteria for session termination was not met, so we return false,
            // indicating that the session must continue.
            return false;
        }

        /// <summary>
        /// Called form the 'Update' function whenever the input device is moved. This function 
        /// will snap the target object to the surface hovered by the mouse cursor. 
        /// </summary>
        private void OnInputDeviceMoved(IInputDevice inputDevice)
        {
            // We will need to perform a raycast to check what lies underneath the mouse cursor.
            // In order to do this, we will first create a raycast filter instance to specify
            // additional information for the raycast...
            SceneRaycastFilter raycastFilter = new SceneRaycastFilter();
            raycastFilter.LayerMask = _objectSurfaceLayers;                                 // The ray can hit only objects belonging to these layers
            raycastFilter.AllowedObjectTypes.Add(GameObjectType.Mesh);                      // Allow the ray to hit mesh objects
            raycastFilter.AllowedObjectTypes.Add(GameObjectType.Terrain);                   // Allow the ray to hit terrain objects
            raycastFilter.IgnoreObjects.AddRange(_targetHierarchy.GetAllChildrenAndSelf()); // The ray can not hit the target object (i.e. the object we are snapping)

            // Perform the raycast. If nothing is hit, just return.
            SceneRaycastHit raycastHit = RTScene.Get.Raycast(inputDevice.GetRay(RTFocusCamera.Get.TargetCamera), SceneRaycastPrecision.BestFit, raycastFilter);
            if (!raycastHit.WasAnythingHit) return;

            // If we reach this point we know that something was hit. We just need to find out what and based
            // on what was hit, we need to fill the necessary information in the snap config instance. Basically,
            // we need to specify surface inf such as the surface normal, hit point, the object that acts as the
            // snap surface etc. This info is extracted from the raycast hit instance differently depending on
            // whether we hit a game object or the scene grid.
            if (raycastHit.WasAnObjectHit)
            {
                // We hit an object. Get its type and return if it's not a mesh or terrain.
                GameObjectType objectType = raycastHit.ObjectHit.HitObject.GetGameObjectType();
                if (objectType != GameObjectType.Mesh && objectType != GameObjectType.Terrain) return;

                // We are dealing with a mesh or terrain object. Extract the surface info from the 'ObjectHit'
                // field of the raycast hit instance.
                _snapConfig.SurfaceHitNormal = raycastHit.ObjectHit.HitNormal;
                _snapConfig.SurfaceHitPlane = raycastHit.ObjectHit.HitPlane;
                _snapConfig.SurfaceHitPoint = raycastHit.ObjectHit.HitPoint;
                _snapConfig.SurfaceObject = raycastHit.ObjectHit.HitObject;
                _snapConfig.SurfaceType = objectType == GameObjectType.Mesh ? ObjectSurfaceSnap.Type.Mesh : ObjectSurfaceSnap.Type.UnityTerrain;
            }
            else
            {
                // The scene grid was hit. Extract the surface info from the 'GridHit'
                // field of the raycast hit instance.
                _snapConfig.SurfaceHitNormal = raycastHit.GridHit.HitNormal;
                _snapConfig.SurfaceHitPlane = raycastHit.GridHit.HitPlane;
                _snapConfig.SurfaceHitPoint = raycastHit.GridHit.HitPoint;
                _snapConfig.SurfaceType = ObjectSurfaceSnap.Type.SceneGrid;
            }

            // Call 'ObjectSurfaceSnap.SnapHierarchy' on the target hierarchy and pass the snap config data along.
            // Note: The function requires that we first set the position of the hierarchy to the surface hit point.
            _targetHierarchy.transform.position = _snapConfig.SurfaceHitPoint;
            ObjectSurfaceSnap.SnapHierarchy(_targetHierarchy, _snapConfig);
        }

        /// <summary>
        /// Begins a new snap session that snaps the specified object. 
        /// </summary>
        /// <param name="targetHierarchy">
        /// The parent object of the hierarchy that must be snapped.
        /// </param>
        private void BeginSnapSession(GameObject targetHierarchy)
        {
            // Start a new session and store a reference to the spawned object.
            // This is the target object that will be moved with the mouse.
            _isSnapSessionActive = true;
            _targetHierarchy = targetHierarchy;
        }

        /// <summary>
        /// Ends the active snap session.
        /// </summary>
        private void EndSnapSession()
        {
            _targetHierarchy = null;
            _isSnapSessionActive = false;
        }

        /// <summary>
        /// Called after the RLD app is initialized. Any initialization that depends
        /// on other RLD modules, must be done in here.
        /// </summary>
        private void OnAppInitialized()
        {
            // We want to know when a prefab is spawned so that we can capture it
            // and snap it accordingly in the 'Update' function. We accomplish this
            // by implementing a handler for the 'PrefabSpawned' event.
            RTPrefabLibDb.Get.PrefabSpawned += OnPrefabSpawned;

            // We don't want to be able to select/deselect game objects while the snap
            // session is active so we need to register a handler for the following 2
            // events.
            RTObjectSelection.Get.CanClickSelectDeselect += OnCanChangeObjectSelection;
            RTObjectSelection.Get.CanMultiSelectDeselect += OnCanChangeObjectSelection;
        }

        /// <summary>
        /// Called when a prefab is spawned in the scene after clicking on one of
        /// the prefab previews in the prefab lib UI.
        /// </summary>
        /// <param name="prefab">
        /// The prefab that was spawned.
        /// </param>
        /// <param name="spawnedObject">
        /// The game object that was spawned from the prefab.
        /// </param>
        private void OnPrefabSpawned(RTPrefab prefab, GameObject spawnedObject)
        {
            // Begin the snap session
            BeginSnapSession(spawnedObject);
        }

        /// <summary>
        /// Event handler for the 'CanClickSelectDeselect' and 'CanMultiSelectDeselect'
        /// events thrown by the object selection module. It allows us to stop objects
        /// from being selected as long as we are snapping.
        /// </summary>
        private void OnCanChangeObjectSelection(YesNoAnswer answer)
        {
            if (_isSnapSessionActive) answer.No();
            else answer.Yes();
        }
    }
}
