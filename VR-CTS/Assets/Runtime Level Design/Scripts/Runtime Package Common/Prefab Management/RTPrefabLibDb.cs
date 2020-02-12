using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;

namespace RLD
{
    public delegate void PrefabLibDbPrefabSpawnedHander(RTPrefab prefab, GameObject spawnedObject);

    [Serializable]
    [ExecuteInEditMode]
    public class RTPrefabLibDb : MonoSingleton<RTPrefabLibDb>
    {
        #region Inspector variables
        [SerializeField]
        private string _newLibName = string.Empty;
        [SerializeField]
        private Vector2 _prefabScrollPos;
        [SerializeField]
        private int _numPrefabsPerRow = 5;
        #endregion

        public event PrefabLibDbPrefabSpawnedHander PrefabSpawned;

        [NonSerialized]
        private EditorPrefabPreviewGen _editorPrefabPreviewGen = new EditorPrefabPreviewGen();

        [SerializeField]
        private PrefabLibDbSettings _settings = new PrefabLibDbSettings();
        [SerializeField]
        private RTPrefabLibDbUI _runtimeUI;
        [SerializeField]
        private PrefabPreviewLookAndFeel _prefabPreviewLookAndFeel = new PrefabPreviewLookAndFeel();
        [SerializeField]
        private int _activeLibIndex = -1;
        [SerializeField]
        private List<RTPrefabLib> _libs = new List<RTPrefabLib>();

        public int NumLibs { get { return _libs.Count; } }
        public int ActiveLibIndex { get { return _activeLibIndex; } }
        public RTPrefabLib ActiveLib { get { return _activeLibIndex >= 0 ? _libs[_activeLibIndex] : null; } }
        public PrefabPreviewLookAndFeel PrefabPreviewLookAndFeel { get { return _prefabPreviewLookAndFeel; } }
        public RTPrefabLibDbUI RuntimeUI { get { return _runtimeUI; } }
        public bool HasRuntimeUI { get { return _runtimeUI != null && _runtimeUI.gameObject.activeInHierarchy && _runtimeUI.enabled; } }
        public PrefabLibDbSettings Settings { get { return _settings; } }
        public EditorPrefabPreviewGen EditorPrefabPreviewGen { get { return _editorPrefabPreviewGen; } }

        #if UNITY_EDITOR
        public string NewLibName { get { return _newLibName; } set { if (value != null) _newLibName = value; } }
        public Vector2 PrefabScrollPos { get { return _prefabScrollPos; } set { _prefabScrollPos = value; } }
        public int NumPrefabsPerRow { get { return _numPrefabsPerRow; } set { _numPrefabsPerRow = Mathf.Max(1, value); } }
        #endif

        #if UNITY_EDITOR
        public void RefreshEditorPrefabPreviews()
        {
            EditorPrefabPreviewGen.BeginGenSession(PrefabPreviewLookAndFeel);
            for (int libIndex = 0; libIndex < NumLibs; ++libIndex)
            {
                RTPrefabLib lib = _libs[libIndex];
                EditorUtility.DisplayProgressBar("Refreshing previews...", lib.Name, (float)(libIndex + 1) / NumLibs);
                for (int prefabIndex = 0; prefabIndex < lib.NumPrefabs; ++prefabIndex)
                {
                    RTPrefab prefab = lib.GetPrefab(prefabIndex);
                    if (prefab.PreviewTexture != null) Texture2D.DestroyImmediate(prefab.PreviewTexture);
                    prefab.PreviewTexture = EditorPrefabPreviewGen.Generate(prefab.UnityPrefab);
                }
            }
            EditorUtility.ClearProgressBar();
            EditorPrefabPreviewGen.EndGenSession();
        }

        public void CreateRuntimeUI()
        {
            _runtimeUI = FindObjectOfType<RTPrefabLibDbUI>();
            if (_runtimeUI == null)
            {
                GameObject uiPrefab = Resources.Load("Prefabs/RTPrefabLibDbUI") as GameObject;
                GameObject uiObject = GameObject.Instantiate(uiPrefab) as GameObject;
                uiObject.name = uiPrefab.name;
                _runtimeUI = uiObject.GetComponent<RTPrefabLibDbUI>();
            }

            var eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject esObject = new GameObject("Event System");
                esObject.AddComponent<EventSystem>();
                esObject.AddComponent<StandaloneInputModule>();
            }
        }
        #endif

        public RTPrefabLib CreateLib(string libName)
        {
            if (string.IsNullOrEmpty(libName)) return null;

            var finalName = UniqueNameGen.Generate(libName, GetAllLibNames());
            var lib = new RTPrefabLib();
            lib.Name = finalName;
            _libs.Add(lib);

            if (HasRuntimeUI && Application.isPlaying) _runtimeUI.ActiveLibDropDown.SyncWithLibDb();
            if (_activeLibIndex == -1) SetActiveLib(0);

            lib.PrefabCreated += OnPrefabCreatedInLib;
            lib.PrefabRemoved += OnPrefabRemovedFromLib;
            lib.Cleared += OnPrefabLibCleared;
       
            return lib;
        }

        public void SortLibsByName()
        {
            var activeLib = ActiveLib;

            _libs.Sort(delegate(RTPrefabLib l0, RTPrefabLib l1)
            { return l0.Name.CompareTo(l1.Name); });

            if (activeLib != null) SetActiveLib(activeLib);
        }

        public bool SetLibName(RTPrefabLib lib, string newLibName)
        {
            if (lib.Name == newLibName || !Contains(lib)) return false;
            if (string.IsNullOrEmpty(newLibName)) return false;

            var finalName = UniqueNameGen.Generate(newLibName, GetAllLibNames());
            lib.Name = finalName;

            if (HasRuntimeUI && Application.isPlaying) _runtimeUI.ActiveLibDropDown.SyncWithLibDb();

            return true;
        }

        public void SetActiveLib(int libIndex)
        {
            if (NumLibs == 0) return;

            _activeLibIndex = libIndex;
            _activeLibIndex = Mathf.Clamp(_activeLibIndex, 0, NumLibs - 1);

            if (HasRuntimeUI && Application.isPlaying)
            {
                _runtimeUI.ActiveLibDropDown.SetActiveLibIndex(_activeLibIndex);
                _runtimeUI.PrefabScrollView.SyncWithLib(ActiveLib);
            }
        }

        public void SetActiveLib(RTPrefabLib lib)
        {
            int libIndex = GetLibIndex(lib);
            SetActiveLib(libIndex);
        }

        public void SetActiveLib(string libName)
        {
            int libIndex = GetLibIndex(libName);
            SetActiveLib(libIndex);
        }

        public void Clear()
        {
            _libs.Clear();
            _activeLibIndex = -1;

            if (HasRuntimeUI && Application.isPlaying)
            {
                _runtimeUI.ActiveLibDropDown.SyncWithLibDb();
                _runtimeUI.PrefabScrollView.ClearPreviews();
            }
        }

        public void Remove(int libIndex)
        {
            if (libIndex >= 0 && libIndex < NumLibs)
            {
                var removedLib = _libs[libIndex];
                var activeLib = ActiveLib;
         
                _libs.RemoveAt(libIndex);
                _activeLibIndex = _libs.IndexOf(activeLib);
                if (_activeLibIndex < 0) _activeLibIndex = 0;

                if (HasRuntimeUI && Application.isPlaying) _runtimeUI.ActiveLibDropDown.SyncWithLibDb();
            }
        }

        public void Remove(string libName)
        {
            int libIndex = GetLibIndex(libName);
            if (libIndex >= 0) Remove(libIndex);
        }

        public void Remove(RTPrefabLib lib)
        {
            int libIndex = GetLibIndex(lib);
            if (libIndex >= 0) Remove(libIndex);
        }

        public void Remove(List<RTPrefabLib> libs)
        {
            foreach (var lib in libs)
                Remove(lib);
        }

        public List<RTPrefabLib> GetEmptyLibs()
        {
            var emptyLibs = new List<RTPrefabLib>(10);
            foreach (var lib in _libs)
                if (lib.NumPrefabs == 0) emptyLibs.Add(lib);

            return emptyLibs;
        }

        public void RemoveEmptyLibs()
        {
            Remove(GetEmptyLibs());
        }

        public bool Contains(string libName)
        {
            return GetLib(libName) != null;
        }

        public bool Contains(RTPrefabLib lib)
        {
            return _libs.Contains(lib);
        }

        public List<string> GetAllLibNames()
        {
            var libNames = new List<string>(10);
            foreach (var lib in _libs) libNames.Add(lib.Name);

            return libNames;
        }

        public int GetLibIndex(string libName)
        {
            return _libs.FindIndex(item => item.Name == libName);
        }

        public int GetLibIndex(RTPrefabLib lib)
        {
            return _libs.IndexOf(lib);
        }

        public RTPrefabLib GetLib(int libIndex)
        {
            return _libs[libIndex];
        }

        public RTPrefabLib GetLib(string libName)
        {
            var libs = _libs.FindAll(item => item.Name == libName);
            return libs.Count != 0 ? libs[0] : null;
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                foreach (var lib in _libs)
                {
                    lib.PrefabCreated += OnPrefabCreatedInLib;
                    lib.PrefabRemoved += OnPrefabRemovedFromLib;
                    lib.Cleared += OnPrefabLibCleared;
                }

                if (HasRuntimeUI)
                {
                    _runtimeUI.ActiveLibDropDown.SyncWithLibDb();
                    _runtimeUI.ActiveLibDropDown.AddValueChangedListener(delegate { OnActiveLibDropDownChanged(); });

                    _runtimeUI.PrefabScrollView.SyncWithLib(ActiveLib);
                    _runtimeUI.PrefabScrollView.PrefabPreviewClicked += OnPrefabPreviewButtonClicked;

                    // !!--- NOTE ---!!
                    // For some weird reason, we need to create a dummy UI object to make the prefab previews 
                    // appear correctly inside the build. Note: Only occurs in build. 
                    var dummyButton = new GameObject("Dummy");
                    dummyButton.AddComponent<RectTransform>();
                    dummyButton.transform.SetParent(_runtimeUI.PrefabScrollView.transform, false);
                    GameObject.Destroy(dummyButton);
                }
            }
        }

        private void OnActiveLibDropDownChanged()
        {
            if (HasRuntimeUI)
            {
                _activeLibIndex = _runtimeUI.ActiveLibDropDown.ActiveLibIndex;
                _runtimeUI.PrefabScrollView.SyncWithLib(ActiveLib);
            }
        }

        private void OnPrefabCreatedInLib(RTPrefabLib prefabLib, RTPrefab prefab)
        {
            if (HasRuntimeUI && prefabLib == ActiveLib && Application.isPlaying)
                _runtimeUI.PrefabScrollView.AddPrefabPreview(prefab);
        }

        private void OnPrefabRemovedFromLib(RTPrefabLib prefabLib, RTPrefab prefab)
        {
            if (HasRuntimeUI && prefabLib == ActiveLib)
                _runtimeUI.PrefabScrollView.SyncWithLib(prefabLib);
        }

        private void OnPrefabLibCleared(RTPrefabLib prefabLib)
        {
            if (HasRuntimeUI && prefabLib == ActiveLib)
                _runtimeUI.PrefabScrollView.ClearPreviews();
        }

        private void OnPrefabPreviewButtonClicked(RTPrefab prefab)
        {
            if (Settings.SpawnPrefabOnPreviewClick)
            {
                var spawnedObject = ObjectSpawnUtil.SpawnInFrontOfCamera(prefab.UnityPrefab, RTFocusCamera.Get.TargetCamera, ObjectSpawnUtil.DefaultConfig);
                var action = new PostObjectSpawnAction(new List<GameObject>() { spawnedObject });
                action.Execute();

                if (PrefabSpawned != null) PrefabSpawned(prefab, spawnedObject);
            }
        }
    }
}
