using UnityEngine;
using System.Collections.Generic;

namespace RLD
{
    public interface IObjectCollectionGizmoController
    {
        void SetTargetObjectCollection(IEnumerable<GameObject> targetObjectCollection);
    }
}
