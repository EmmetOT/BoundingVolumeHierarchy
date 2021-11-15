using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoundingVolumeHierarchy
{
    public static class BVHUtils
    {
        /// <summary>
        /// Get the perimeter length of the bounds.
        /// </summary>
        public static float GetPerimeter(this Bounds bounds)
        {
            float x_size = bounds.max.x - bounds.min.x;
            float y_size = bounds.max.y - bounds.min.y;

            return 2f * (x_size + y_size);
        }

        /// <summary>
        /// Does this aabb contain the provided AABB.
        /// </summary>
        public static bool Contains(this Bounds thisBounds, Bounds other)
        {
            bool result = true;
            result = result && thisBounds.min.x <= other.min.x;
            result = result && thisBounds.min.y <= other.min.y;
            result = result && thisBounds.min.z <= other.min.z;
            result = result && other.max.x <= thisBounds.max.x;
            result = result && other.max.y <= thisBounds.max.y;
            result = result && other.max.z <= thisBounds.max.z;
            return result;
        }

        /// <summary>
        /// Get the surface area of the entire bounds.
        /// </summary>
        public static float GetSurfaceArea(this Bounds box)
        {
            float x_size = box.max.x - box.min.x;
            float y_size = box.max.y - box.min.y;
            float z_size = box.max.z - box.min.z;

            return 2.0f * ((x_size * y_size) + (x_size * z_size) + (y_size * z_size));

        }
    }
}
