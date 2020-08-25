using System;
using Unity.Mathematics;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Collection of utility math operations.
    /// </summary>
    public static class MathUtils
    {
        public const float Sqrt2 = 1.414213f;

        /// <summary>
        /// Performs a "circular" mod which provides positive results on negative values. For example:<para/>
        /// 
        /// mod(-5, 5) = 0,
        /// mod(-4, 5) = 1,
        /// mod(-3, 5) = 2,
        /// mod(-2, 5) = 3,
        /// mod(-1, 5) = 4,
        /// mod( 0, 5) = 0,
        /// mod( 1, 5) = 1,
        /// mod( 2, 5) = 2,
        /// mod( 3, 5) = 3,
        /// mod( 4, 5) = 4,
        /// mod( 5, 5) = 0<para/>
        /// 
        /// It is useful for iteration where we want a safe look ahead and look behind value.<para/>
        /// 
        /// Source: https://stackoverflow.com/a/1082938/735425
        /// </summary>
        /// <param name="x"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static int ModCircular(int x, int mod)
        {
            return (x % mod + mod) % mod;
        }

        /// <summary>
        /// Rounds the floating value along the current direction.
        /// If positive, returns the ceiling.
        /// If negative, returns the floor.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float RoundAlong(float value)
        {
            return (value >= 0) ? Mathf.Ceil(value) : Mathf.Floor(value);
        }

        public static float RoundToNearestQuarter(float value)
        {
            return (float)((int)Mathf.Round(value * 4)) / 4.0f;
        }

        public static Vector3 RoundToNearestQuarter(Vector3 v)
        {
            return new Vector3(RoundToNearestQuarter(v.x), RoundToNearestQuarter(v.y), RoundToNearestQuarter(v.z));
        }

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

        public static bool IsZero(float a, float epsilon = 0.0000001f)
        {
            return FloatEquals(a, 0.0f, epsilon);
        }

        public static bool IsZero(Vector3 v, float epsilon = 0.000001f)
        {
            return (IsZero(v.x, epsilon) && IsZero(v.y, epsilon) && IsZero(v.z, epsilon));
        }

        public static bool IsZero(float3 v, float epsilon = 0.000001f)
        {
            return (IsZero(v.x, epsilon) && IsZero(v.y, epsilon) && IsZero(v.z, epsilon));
        }

        public static bool VectorEquals(Vector2 a, Vector2 b, float epsilon = 0.000001f)
        {
            return FloatEquals(a.x, b.x, epsilon) && FloatEquals(a.y, b.y, epsilon);
        }

        public static bool VectorEquals(Vector3 a, Vector3 b, float epsilon = 0.000001f)
        {
            return FloatEquals(a.x, b.x, epsilon) && FloatEquals(a.y, b.y, epsilon) && FloatEquals(a.z, b.z, epsilon);
        }

        public static Vector2 VectorClamp(Vector2 a, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(a.x, min.x, max.x), Mathf.Clamp(a.y, min.y, max.y));
        }

        public static Vector3 VectorClamp(Vector3 a, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(a.x, min.x, max.x), Mathf.Clamp(a.y, min.y, max.y), Mathf.Clamp(a.z, min.z, max.z));
        }

        /// <summary>
        /// Converts the components of a <see cref="Quaternion"/> to a <see cref="float4"/>.
        /// </summary>
        /// <param name="quat"></param>
        /// <returns></returns>
        public static float4 QuatToFloat4(Quaternion quat)
        {
            return new float4(quat.x, quat.y, quat.z, quat.w);
        }

        /// <summary>
        /// Converts Euler angles to a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Quaternion EulerToQuat(float x, float y, float z)
        {
            return Quaternion.Euler(x, y, z);
        }

        /// <summary>
        /// Returns true if the specified point is in view of the camera.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public static bool PointInCamera(Camera camera, Vector3 worldPoint)
        {
            Vector3 pointInCamera = camera.WorldToViewportPoint(worldPoint);

            // Viewport space is normalized with x/y [0.0, 1.0] in range and z >= 0.0 in front of camera
            return (pointInCamera.x >= 0.0 && pointInCamera.x <= 1.0) &&
                   (pointInCamera.y >= 0.0 && pointInCamera.y <= 1.0) &&
                   (pointInCamera.z >= 0.0);
        }

        /// <summary>
        /// Projects a ray from the specified camera and returns  where on the specified plane it intersects.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="screenPos"></param>
        /// <param name="yPlane"></param>
        /// <returns></returns>
        public static Vector3 ScreenPosToWorldPlane(Camera camera, Vector3 screenPos, Plane plane)
        {
            Ray ray = camera.ScreenPointToRay(screenPos);

            float distance;

            if (!plane.Raycast(ray, out distance))
            {
                distance = 0.0f;
            }

            return (ray.origin + (ray.direction * distance));
        }

        /// <summary>
        /// Performs a floor on all of the components of the provided vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 Floor(Vector3 v)
        {
            return new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
        }

        /// <summary>
        /// Performs a ceil on all of the components of the provided vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 Ceil(Vector3 v)
        {
            return new Vector3(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z));
        }

        /// <summary>
        /// Returns a vector comprised of the min components of the provided vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }

        /// <summary>
        /// Returns a vector comprised of the max components of the provided vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        /// <summary>
        /// Returns a new vector where each component is the absolute value of the provided vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        /// <summary>
        /// Multiplies together two 3-component vectors since Unity does not provide this for some reason.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        /// Multiplies together two 4-component vectors since Unity does not provide this for some reason.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector4 Multiply(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        /// <summary>
        /// Returns the signed distance from the point to the plane.<para/>
        /// 
        /// If the result is positive then the point is "in front" of the plane.
        /// If the result is negative then the point is "behind" the plane.
        /// If the result is zero then the point is on the plane.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planePoint"></param>
        /// <param name="planeNormal"></param>
        /// <returns></returns>
        public static float SignedDistanceToPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
        {
            // Remember that the dot product of an arbitrary vector and an unit vector 
            // is the length of the arbitrary vector projected onto the unit vector.
            return Vector3.Dot((point - planePoint), planeNormal);
        }

        /// <summary>
        /// Projects the arbitrary vector onto the provided normal/unit vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3 ProjectVectorOntoNormal(Vector3 vector, Vector3 normal)
        {
            float length = Vector3.Dot(vector, normal);
            return (normal * length);
        }

        /// <summary>
        /// Rotates the 2D vector clockwise 90 degrees.<para/>
        /// 
        /// <c>(1, 0)</c> to <c>(0, -1)</c> to <c>(-1, 0)</c> to <c>(0, 1)</c> back to <c>(1, 0)</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static (float, float) Rotate90Clockwise(float x, float y)
        {
            return (y, -x);
        }

        /// <summary>
        /// Rotates the 2D vector clockwise 90 degrees.<para/>
        /// 
        /// <c>(1, 0)</c> to <c>(0, -1)</c> to <c>(-1, 0)</c> to <c>(0, 1)</c> back to <c>(1, 0)</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static (int, int) Rotate90Clockwise(int x, int y)
        {
            return (y, -x);
        }

        /// <summary>
        /// Rotates the 2D vector counter-clockwise 90 degrees.<para/>
        /// 
        /// <c>(1, 0)</c> to <c>(0, 1)</c> to <c>(-1, 0)</c> to <c>(0, -1)</c> back to <c>(1, 0)</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static (float, float) Rotate90CounterClockwise(float x, float y)
        {
            return (-y, x);
        }

        /// <summary>
        /// Rotates the 2D vector counter-clockwise 90 degrees.<para/>
        /// 
        /// <c>(1, 0)</c> to <c>(0, 1)</c> to <c>(-1, 0)</c> to <c>(0, -1)</c> back to <c>(1, 0)</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static (int, int) Rotate90CounterClockwise(int x, int y)
        {
            return (-y, x);
        }

        /// <summary>
        /// Rotates the given position around the specified point using the specified rotation.
        /// </summary>
        /// <param name="point">The position that is being rotated.</param>
        /// <param name="pivot">The point we are rotating around.</param>
        /// <param name="rotation">The rotation to perform.</param>
        /// <returns></returns>
        public static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return (rotation * (point - pivot) + pivot);
        }

        /// <summary>
        /// Rotates the given position around the specified point using the specified rotation.
        /// </summary>
        /// <param name="point">The position that is being rotated.</param>
        /// <param name="pivot">The point we are rotating around.</param>
        /// <param name="euler">The euler angles to rotate around.</param>
        /// <returns></returns>
        public static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Vector3 euler)
        {
            return RotateAroundPivot(point, pivot, Quaternion.Euler(euler));
        }

        /// <summary>
        /// Returns <c>true</c> if <c>x</c> is in the range <c>[min, max]</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Between(int x, int min, int max)
        {
            return (x >= min) && (x <= max);
        }

        /// <summary>
        /// Returns <c>true</c> if <c>x</c> is in the range <c>[min, max]</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Between(float x, float min, float max)
        {
            return (x >= min) && (x <= max);
        }

        /// <summary>
        /// Returns <c>true</c> for <c>(x, y)</c> if <c>x</c> is in the range <c>[min.x, max.x]</c> and <c>y</c> is in the range <c>[min.y, max.y]</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Between(Vector2 x, Vector2 min, Vector2 max)
        {
            return Between(x.x, min.x, max.x) &&
                   Between(x.y, min.y, max.y);
        }

        /// <summary>
        /// Returns <c>true</c> for <c>(x, y, z)</c> if <c>x</c> is in the range <c>[min.x, max.x]</c> and <c>y</c> is in the range <c>[min.y, max.y]</c> and <c>z</c> is in the range <c>[min.z, max.z]</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Between(Vector3 x, Vector3 min, Vector3 max)
        {
            return Between(x.x, min.x, max.x) &&
                   Between(x.y, min.y, max.y) &&
                   Between(x.z, min.z, max.z);
        }

        /// <summary>
        /// Takes a normalized direction vector and turns it into a cardinal direction.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Vector3 ToCardinalDirection(Vector3 dir)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            {
                if (dir.x < 0)
                {
                    return Vector3.left;
                }
                else
                {
                    return Vector3.right;
                }
            }
            else
            {
                if (dir.z < 0)
                {
                    return Vector3.back;
                }
                else
                {
                    return Vector3.forward;
                }
            }
        }

        /// <summary>
        /// Returns the fractional part of a value.<para/>
        /// 
        /// Example: For <c>12.345</c> returns <c>0.345</c>.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float GetFractionalPart(float x)
        {
            float xint = (float)Math.Truncate(x);
            return Math.Abs(x - xint);
        }

        /// <summary>
        /// Calculates the distance between two vectors.<para/>
        /// 
        /// Yes, Unity provides this functionality already but it involves making copies of the data.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float VectorDistanceSq(ref Vector3 a, ref Vector3 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            float z = a.z - b.z;

            return ((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the distance between two vectors.<para/>
        /// 
        /// Yes, Unity provides this functionality already but it involves making copies of the data.
        /// </summary>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="az"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        /// <param name="bz"></param>
        /// <returns></returns>
        public static float VectorDistanceSq(float ax, float ay, float az, float bx, float by, float bz)
        {
            float x = ax - bx;
            float y = ay - by;
            float z = az - bz;

            return ((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Extension for <see cref="float3"/> to calculate the squared magnitude.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float MagnitudeSquared(this float3 vector)
        {
            return (vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z);
        }

        /// <summary>
        /// Extension for <see cref="float3"/> to calculate the magnitude.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float Magnitude(this float3 vector)
        {
            return Mathf.Sqrt(vector.MagnitudeSquared());
        }

        /// <summary>
        /// Extension for <see cref="float3"/> to return the normalized version of the vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float3 Normalized(this float3 vector)
        {
            float magnitude = vector.Magnitude();

            if (IsZero(magnitude))
            {
                return new float3();
            }

            return new float3(vector.x / magnitude, vector.y / magnitude, vector.z / magnitude);
        }

        /// <summary>
        /// Returns the dot product between the two vectors, the <c>cos(angle)</c>.<para/>
        /// 
        /// Returns,
        /// 
        /// <list type="bullet">
        ///     <item><c>1.0</c> if the vectors are facing the same direction.</item>
        ///     <item><c>0.0</c> if the vectors are perpendicular.</item>
        ///     <item><c>-1.0</c> if the vectors are facing opposite directions.</item>
        ///     <item>Anywhere inbetween accordingly.</item>
        /// </list>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Dot(float3 a, float3 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }

        /// <summary>
        /// Returns the cross product between the two vectors (order dependent).<para/>
        /// 
        /// The result is a new vector that is at a right angle to both vectors.
        /// The magnitude of this resulting vector is equal to the area of a parallelogram with a and b as sides.<para/>
        /// 
        /// Additionally, the resulting vector has a magnitude of zero if the two vectors are equal or opposite direction.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float3 Cross(float3 a, float3 b)
        {
            return new float3(
                (a.y * b.z) - (a.z * b.y),
                (a.z * b.x) - (a.x * b.z),
                (a.x * b.y) - (a.y * b.x));
        }

        public static float3 ZeroOut(float3 vec, float epsilon = 0.001f)
        {
            vec.x = math.abs(vec.x) < epsilon ? 0.0f : vec.x;
            vec.y = math.abs(vec.y) < epsilon ? 0.0f : vec.y;
            vec.z = math.abs(vec.z) < epsilon ? 0.0f : vec.z;

            return vec;
        }
    }
}
