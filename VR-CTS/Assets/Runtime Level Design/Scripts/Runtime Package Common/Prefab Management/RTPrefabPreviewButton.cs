using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RLD
{
    public class RTPrefabPreviewButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void HoverEnterHandler(RTPrefab prefab);
        public delegate void HoverExitHandler(RTPrefab prefab);

        public event HoverEnterHandler HoverEnter;
        public event HoverExitHandler HoverExit;

        private Text _text;
        private RTPrefab _prefab;

        public RTPrefab Prefab { get { return _prefab; } set { if (value != null) _prefab = value; } }
        public string Text { get { return _text != null ? _text.text : string.Empty; } set { if (_text != null && value != null) _text.text = value; } }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (HoverEnter != null) HoverEnter(_prefab);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (HoverExit != null) HoverExit(_prefab);
        }

        private void OnEnable()
        {
            _text = gameObject.GetComponentInChildren<Text>();
        }
    }
}
