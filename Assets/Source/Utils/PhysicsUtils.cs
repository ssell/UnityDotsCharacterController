using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace VertexFragment
{
    /// <summary>
    /// Collection of utility physics operations.
    /// </summary>
    public static class PhysicsUtils
    {
        /// <summary>
        /// Returns a list of all colliders within the specified distance of the provided collider, as a list of <see cref="DistanceHit"/>.<para/>
        /// 
        /// Can be used in conjunction with <see cref="ColliderCastAll"/> to get the penetration depths of any colliders (using the distance of collisions).
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="maxDistance"></param>
        /// <param name="transform"></param>
        /// <param name="collisionWorld"></param>
        /// <returns></returns>
        public unsafe static List<DistanceHit> ColliderDistanceAll(PhysicsCollider collider, float maxDistance, RigidTransform transform, ref CollisionWorld collisionWorld, Entity ignore, Allocator allocator = Allocator.TempJob)
        {
            ColliderDistanceInput input = new ColliderDistanceInput()
            {
                Collider = collider.ColliderPtr,
                MaxDistance = maxDistance,
                Transform = transform
            };

            List<DistanceHit> output = new List<DistanceHit>();
            NativeList<DistanceHit> allDistances = new NativeList<DistanceHit>(allocator);

            if (collisionWorld.CalculateDistance(input, ref allDistances))
            {
                foreach (var hit in allDistances)
                {
                    if (hit.Entity == ignore)
                    {
                        continue;
                    }

                    output.Add(hit);
                }
            }

            allDistances.Dispose();

            return output;
        }

        /// <summary>
        /// Performs a collider cast using the specified collider.
        /// </summary>
        /// <param name="smallestDistanceHit"></param>
        /// <param name="collider"></param>
        /// <param name="maxDistance"></param>
        /// <param name="transform"></param>
        /// <param name="collisionWorld"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public static bool ColliderDistance(
            out DistanceHit smallestDistanceHit,
            PhysicsCollider collider,
            float maxDistance,
            RigidTransform transform,
            ref CollisionWorld collisionWorld,
            Entity ignore,
            CollisionFilter? filter = null,
            EntityManager? manager = null,
            ComponentDataFromEntity<PhysicsCollider>? colliderData = null,
            Allocator allocator = Allocator.TempJob)
        {
            smallestDistanceHit = new DistanceHit();
            var allDistances = ColliderDistanceAll(collider, maxDistance, transform, ref collisionWorld, ignore, allocator);

            if (filter.HasValue)
            {
                if (manager.HasValue)
                {
                    TrimByFilter(ref allDistances, manager.Value, filter.Value);
                }
                else if (colliderData.HasValue)
                {
                    TrimByFilter(ref allDistances, colliderData.Value, filter.Value);
                }
            }

            if (allDistances.Count == 0)
            {
                return false;
            }

            float smallest = float.MaxValue;

            foreach (var distanceHit in allDistances)
            {
                if (distanceHit.Distance < smallest)
                {
                    smallest = distanceHit.Distance;
                    smallestDistanceHit = distanceHit;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs a collider cast along the specified ray and returns all resulting <see cref="ColliderCastHit"/>s.
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="collisionWorld"></param>
        /// <param name="ignore">Will ignore this entity if it was hit. Useful to prevent returning hits from the caster.</param>
        /// <returns></returns>
        public unsafe static List<ColliderCastHit> ColliderCastAll(PhysicsCollider collider, float3 from, float3 to, ref CollisionWorld collisionWorld, Entity ignore, Allocator allocator = Allocator.TempJob)
        {
            ColliderCastInput input = new ColliderCastInput()
            {
                Collider = collider.ColliderPtr,
                Start = from,
                End = to
            };

            NativeList<ColliderCastHit> allHits = new NativeList<ColliderCastHit>(allocator);

            collisionWorld.CastCollider(input, ref allHits);

            List<ColliderCastHit> output = new List<ColliderCastHit>(allHits.Length);

            foreach (var hit in allHits)
            {
                if (hit.Entity == ignore)
                {
                    continue;
                }

                output.Add(hit);
            }

            allHits.Dispose();

            return output;
        }

        /// <summary>
        /// Performs a collider cast along the specified ray.<para/>
        /// 
        /// Will return true if there was a collision and populate the provided <see cref="ColliderCastHit"/>.
        /// </summary>
        /// <param name="nearestHit"></param>
        /// <param name="collider"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="collisionWorld"></param>
        /// <param name="ignore"></param>
        /// <param name="filter">Used to exclude collisions that do not match the filter.</param>
        /// <param name="manager">Required if specifying a collision filter. Otherwise is unused.</param>
        /// <param name="colliderData">Alternative to the EntityManager if used in a job.</param>
        /// <returns></returns>
        public unsafe static bool ColliderCast(
            out ColliderCastHit nearestHit,
            PhysicsCollider collider,
            float3 from,
            float3 to,
            ref CollisionWorld collisionWorld,
            Entity ignore,
            CollisionFilter? filter = null,
            EntityManager? manager = null,
            ComponentDataFromEntity<PhysicsCollider>? colliderData = null,
            Allocator allocator = Allocator.TempJob)
        {
            nearestHit = new ColliderCastHit();
            List<ColliderCastHit> allHits = ColliderCastAll(collider, from, to, ref collisionWorld, ignore, allocator);

            if (filter.HasValue)
            {
                if (manager.HasValue)
                {
                    TrimByFilter(ref allHits, manager.Value, filter.Value);
                }
                else if (colliderData.HasValue)
                {
                    TrimByFilter(ref allHits, colliderData.Value, filter.Value);
                }
            }

            if (allHits.Count == 0)
            {
                return false;
            }

            float nearestFrac = float.MaxValue;

            foreach (var hit in allHits)
            {
                if (hit.Fraction < nearestFrac)
                {
                    nearestFrac = hit.Fraction;
                    nearestHit = hit;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs a raycast along the specified ray and returns all resulting <see cref="RaycastHit"/>s.
        /// </summary>
        /// <param name="collisionWorld"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ignore">Will ignore this entity if it was hit. Useful to prevent returning hits from the caster.</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public unsafe static List<RaycastHit> RaycastAll(float3 from, float3 to, ref CollisionWorld collisionWorld, Entity ignore, CollisionFilter? filter = null, Allocator allocator = Allocator.TempJob)
        {
            RaycastInput input = new RaycastInput()
            {
                Start = from,
                End = to,
                Filter = (filter.HasValue ? filter.Value : CollisionFilter.Default)
            };

            NativeList<RaycastHit> allHits = new NativeList<RaycastHit>(allocator);

            collisionWorld.CastRay(input, ref allHits);

            List<RaycastHit> output = new List<RaycastHit>(allHits.Length);

            foreach (var hit in allHits)
            {
                if (hit.Entity == ignore)
                {
                    continue;
                }

                output.Add(hit);
            }

            allHits.Dispose();

            return output;
        }

        /// <summary>
        /// Performs a raycast along the specified ray.<para/>
        /// 
        /// Will return true if there was a collision and populate the provided <see cref="RaycastHit"/>.
        /// </summary>
        /// <param name="nearestHit"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="collisionWorld"></param>
        /// <param name="ignore">Will ignore this entity if it was hit. Useful to prevent returning hits from the caster.</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public unsafe static bool Raycast(out RaycastHit nearestHit, float3 from, float3 to, ref CollisionWorld collisionWorld, Entity ignore, CollisionFilter? filter = null, Allocator allocator = Allocator.TempJob)
        {
            nearestHit = new RaycastHit();
            List<RaycastHit> allHits = RaycastAll(from, to, ref collisionWorld, ignore, filter, allocator);

            if (allHits.Count == 0)
            {
                return false;
            }

            float nearestHitDist = float.MaxValue;

            foreach (var hit in allHits)
            {
                float3 toHit = hit.Position - from;
                float hitDist = new UnityEngine.Vector3(toHit.x, toHit.y, toHit.z).sqrMagnitude;

                if (hitDist < nearestHitDist)
                {
                    nearestHitDist = hitDist;
                    nearestHit = hit;
                }
            }

            return true;
        }

        /// <summary>
        /// Given a list of <see cref="IQueryResult"/> objects (ie from <see cref="ColliderCastAll"/> or <see cref="ColliderDistanceAll"/>),
        /// removes any entities that:
        /// 
        /// <list type="bullet">
        ///     <item>Do not have a <see cref="PhysicsCollider"/> (in which case, how are they in the list?) or</item>
        ///     <item>Can not collide with the specified filter.</item>
        /// </list>
        /// </summary>
        /// <param name="castResults"></param>
        /// <param name="entityManager"></param>
        /// <param name="filter"></param>
        public unsafe static void TrimByFilter<T>(ref List<T> castResults, EntityManager entityManager, CollisionFilter filter) where T : IQueryResult
        {
            List<int> toRemove = new List<int>(castResults.Count);

            for (int i = 0; i < castResults.Count; ++i)
            {
                if (entityManager.HasComponent<PhysicsCollider>(castResults[i].Entity))
                {
                    PhysicsCollider collider = entityManager.GetComponentData<PhysicsCollider>(castResults[i].Entity);

                    if (CollisionFilter.IsCollisionEnabled(filter, collider.ColliderPtr->Filter))
                    {
                        continue;
                    }
                }

                toRemove.Add(i);
            }

            for (int i = (toRemove.Count - 1); i >= 0; --i)
            {
                castResults.RemoveAt(toRemove[i]);
            }
        }

        /// <summary>
        /// Variant of <see cref="TrimByFilter{T}(ref List{T}, EntityManager, CollisionFilter)"/> to be used within a Job which does not have access to an <see cref="EntityManager"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="castResults"></param>
        /// <param name="colliderData">Retrieved via <see cref="ComponentSystemBase.GetComponentDataFromEntity{T}(bool)"/></param>
        /// <param name="filter"></param>
        public unsafe static void TrimByFilter<T>(ref List<T> castResults, ComponentDataFromEntity<PhysicsCollider> colliderData, CollisionFilter filter) where T : IQueryResult
        {
            List<int> toRemove = new List<int>(castResults.Count);

            for (int i = 0; i < castResults.Count; ++i)
            {
                if (colliderData.HasComponent(castResults[i].Entity))
                {
                    PhysicsCollider collider = colliderData[castResults[i].Entity];

                    if (CollisionFilter.IsCollisionEnabled(filter, collider.ColliderPtr->Filter))
                    {
                        continue;
                    }
                }

                toRemove.Add(i);
            }

            for (int i = (toRemove.Count - 1); i >= 0; --i)
            {
                castResults.RemoveAt(toRemove[i]);
            }
        }
    }
}
