using UnityEngine;
using System;

namespace RLD
{
    [Serializable]
    public class PrefabLibDbSettings
    {
        [SerializeField]
        private bool _spawnPrefabOnPreviewClick = true;

        public bool SpawnPrefabOnPreviewClick { get { return _spawnPrefabOnPreviewClick; } set { _spawnPrefabOnPreviewClick = value; } }
    }
}
