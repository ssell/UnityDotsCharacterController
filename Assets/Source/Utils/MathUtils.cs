using Unity.Mathematics;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Collection of utility math operations.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Determines if the two float values are equal to each other within the range of the epsilon.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool FloatEquals(float a, float b, float epsilon = 0.000001f)
        {
            return Mathf.Abs(a - b) <= epsilon;
        }

        /// <summary>
        /// Returns true if the specified float value is <c>0.0</c> (or within the given epsilon).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool IsZero(float a, float epsilon = 0.0000001f)
        {
            return FloatEquals(a, 0.0f, epsilon);
        }

        /// <summary>
        /// Returns true if all components of the specified vector are <c>0.0</c> (or within the given epsilon).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool IsZero(float3 v, float epsilon = 0.000001f)
        {
            return (IsZero(v.x, epsilon) && IsZero(v.y, epsilon) && IsZero(v.z, epsilon));
        }

        /// <summary>
        /// Sets the components of the specified vector to <c>0.0</c> if they are less than the given epsilon.
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static float3 ZeroOut(float3 vec, float epsilon = 0.001f)
        {
            vec.x = math.abs(vec.x) < epsilon ? 0.0f : vec.x;
            vec.y = math.abs(vec.y) < epsilon ? 0.0f : vec.y;
            vec.z = math.abs(vec.z) < epsilon ? 0.0f : vec.z;

            return vec;
        }
    }
}
