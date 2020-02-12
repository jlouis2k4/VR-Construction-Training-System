using UnityEngine;
using UnityEngine.UI;

namespace RLD
{
    public class RTHoveredPrefabNameLabel : MonoBehaviour
    {
        private Text _label;

        public string PrefabName { get { return _label.text; } set { if (value != null) _label.text = value; } }

        private void Awake()
        {
            _label = GetComponent<Text>();
        }
    }
}
