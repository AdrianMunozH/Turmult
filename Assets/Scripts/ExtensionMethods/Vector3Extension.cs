using UnityEngine;

namespace ExtensionMethods
{

    public static class Vector3Extension
    {
        /// <summary>
        /// Returns the Vector3 with modified values.
        /// </summary>
        /// <param name="original">The base Vector3 to modify.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
        }

        /// <summary>
        /// Returns the Vector3 with a y value of zero.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Vector3 Flat(this Vector3 original)
        {
            return new Vector3(original.x, 0, original.z);
        }

        /// <summary>
        /// Returns a Vector3 with the normalized direction of the source to the destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Vector3 DirectionTo(this Vector3 source, Vector3 destination)
        {
            return Vector3.Normalize(destination - source);
        }
    }
}