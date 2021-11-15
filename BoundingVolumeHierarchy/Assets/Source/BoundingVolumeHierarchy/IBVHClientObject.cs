using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoundingVolumeHierarchy
{
    public interface IBVHClientObject
    {
        Bounds GetBounds();
        Vector3 Position { get; }
        Vector3 PreviousPosition { get; }
    }
}