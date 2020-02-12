using UnityEngine;
using System;
using System.Collections.Generic;

namespace RLD
{
    public class RTObjectGroupDb : MonoSingleton<RTObjectGroupDb>
    {
        [SerializeField]
        private List<GameObject> _objectGroups = new List<GameObject>();

        public int NumGroups { get { return _objectGroups.Count; } }

        public void Add(GameObject gameObject)
        {
            if (!IsGroup(gameObject)) _objectGroups.Add(gameObject);
        }

        public void Remove(GameObject gameObject)
        {
            _objectGroups.Remove(gameObject);
        }

        public void RemoveAt(int index)
        {
            _objectGroups.RemoveAt(index);
        }

        public void Clear()
        {
            _objectGroups.Clear();
        }

        public bool IsGroup(GameObject gameObject)
        {
            return _objectGroups.Contains(gameObject);
        }

        public GameObject GetGroupByIndex(int index)
        {
            return _objectGroups[index];
        }

        public GameObject GetGroupByName(string name)
        {
            var gameObjects = _objectGroups.FindAll(item => item.name == name);
            return gameObjects.Count != 0 ? gameObjects[0] : null;
        }

        public List<GameObject> GetAll()
        {
            return new List<GameObject>(_objectGroups);
        }

        public void RemoveNullRefs()
        {
            _objectGroups.RemoveAll(item => item == null);
        }
    }
}
