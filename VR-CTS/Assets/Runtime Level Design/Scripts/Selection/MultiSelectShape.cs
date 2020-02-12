using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    public class MultiSelectShape
    {
        private Rect _enclosingRect;
        private bool _isVisible;
        private int _minSize = 3;

        public Rect EnclosingRect { get { return _enclosingRect; } }
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public int MinSize { get { return _minSize; } set { _minSize = Mathf.Max(1, value); } }

        public void SetEnclosingRectTopLeftPoint(Vector2 topLeftPoint)
        {
            _enclosingRect.xMin = topLeftPoint.x;
            _enclosingRect.yMax = topLeftPoint.y;
        }

        public void SetEnclosingRectBottomRightPoint(Vector2 bottomRightPoint)
        {
            _enclosingRect.xMax = bottomRightPoint.x;
            _enclosingRect.yMin = bottomRightPoint.y;
        }

        public List<GameObject> GetOverlappedObjects(List<GameObject> gameObjects, Camera camera, ObjectBounds.QueryConfig boundsQConfig, MultiSelectOverlapMode overlapMode)
        {
            if (gameObjects.Count == 0 || !IsBigEnoughForOverlap()) return new List<GameObject>();

            var overlappedObjects = new List<GameObject>(gameObjects.Count);
            if (overlapMode == MultiSelectOverlapMode.Partial)
            {
                foreach (var gameObject in gameObjects)
                {
                    Rect objectScreenRect = ObjectBounds.CalcScreenRect(gameObject, camera, boundsQConfig);
                    if (_enclosingRect.Overlaps(objectScreenRect, true)) overlappedObjects.Add(gameObject);
                }
            }
            else
            {
                foreach (var gameObject in gameObjects)
                {
                    Rect objectScreenRect = ObjectBounds.CalcScreenRect(gameObject, camera, boundsQConfig);
                    if (_enclosingRect.ContainsAllPoints(objectScreenRect.GetCornerPoints())) overlappedObjects.Add(gameObject);
                }
            }

            return overlappedObjects;
        }

        public bool OverlapsObject(GameObject gameObject, Camera camera, ObjectBounds.QueryConfig boundsQConfig, MultiSelectOverlapMode overlapMode)
        {
            if (!IsBigEnoughForOverlap()) return false;

            if (overlapMode == MultiSelectOverlapMode.Partial)
            {
                Rect objectScreenRect = ObjectBounds.CalcScreenRect(gameObject, camera, boundsQConfig);
                return _enclosingRect.Overlaps(objectScreenRect, true);
            }
            else
            {
                Rect objectScreenRect = ObjectBounds.CalcScreenRect(gameObject, camera, boundsQConfig);
                return _enclosingRect.ContainsAllPoints(objectScreenRect.GetCornerPoints());
            }
        }

        public void Render(Color fillColor, Color borderColor, Camera camera)
        {
            if (!_isVisible) return;

            Material material = MaterialPool.Get.SimpleColor;

            material.SetColor(fillColor);
            material.SetCullModeOff();
            material.SetPass(0);
            GLRenderer.DrawRect2D(_enclosingRect, camera);

            material.SetColor(borderColor);
            material.SetPass(0);
            GLRenderer.DrawRectBorder2D(_enclosingRect, camera);
        }

        private bool IsBigEnoughForOverlap()
        {
            return (Mathf.Abs(_enclosingRect.width) >= _minSize && Mathf.Abs(_enclosingRect.height) >= _minSize);
        }
    }
}
