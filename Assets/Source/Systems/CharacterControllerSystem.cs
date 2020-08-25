using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace VertexFragment
{
    /// <summary>
    /// Base controller for character movement.
    /// Is not physics-based, but uses physics to check for collisions.
    /// </summary>
    [UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
    public sealed class CharacterControllerSystem : JobComponentSystem
    {
        private const float Epsilon = 0.001f;

        private BuildPhysicsWorld buildPhysicsWorld;
        private ExportPhysicsWorld exportPhysicsWorld;
        private EndFramePhysicsSystem endFramePhysicsSystem;

        private EntityQuery characterControllerGroup;

        protected override void OnCreate()
        {
            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            exportPhysicsWorld = World.GetOrCreateSystem<ExportPhysicsWorld>();
            endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();

            characterControllerGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(CharacterControllerComponent),
                    typeof(Translation),
                    typeof(Rotation),
                    typeof(PhysicsCollider)
                }
            });
        }

        protected override unsafe JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (characterControllerGroup.CalculateChunkCount() == 0)
            {
                return inputDeps;
            }

            var entityTypeHandle = GetEntityTypeHandle();
            var colliderData = GetComponentDataFromEntity<PhysicsCollider>(true);

            var characterControllerTypeHandle = GetComponentTypeHandle<CharacterControllerComponent>();
            var translationTypeHandle = GetComponentTypeHandle<Translation>();
            var rotationTypeHandle = GetComponentTypeHandle<Rotation>();

            var controllerJob = new CharacterControllerJob()
            {
                DeltaTime = Time.DeltaTime,
                PhysicsWorld = buildPhysicsWorld.PhysicsWorld,
                EntityType = entityTypeHandle,
                ColliderData = colliderData,

                CharacterControllerType = characterControllerTypeHandle,
                TranslationType = translationTypeHandle,
                RotationType = rotationTypeHandle
            };

            var dependency = JobHandle.CombineDependencies(inputDeps, exportPhysicsWorld.GetOutputDependency());
            var controllerJobHandle = controllerJob.Schedule(characterControllerGroup, dependency);

            endFramePhysicsSystem.AddInputDependency(controllerJobHandle);

            return controllerJobHandle;
        }

        private struct CharacterControllerJob : IJobChunk
        {
            public float DeltaTime;

            [ReadOnly] public PhysicsWorld PhysicsWorld;
            [ReadOnly] public EntityTypeHandle EntityType;
            [ReadOnly] public ComponentDataFromEntity<PhysicsCollider> ColliderData;

            public ComponentTypeHandle<CharacterControllerComponent> CharacterControllerType;
            public ComponentTypeHandle<Translation> TranslationType;
            public ComponentTypeHandle<Rotation> RotationType;


            public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var collisionWorld = PhysicsWorld.CollisionWorld;

                var chunkEntityData = chunk.GetNativeArray(EntityType);
                var chunkCharacterControllerData = chunk.GetNativeArray(CharacterControllerType);
                var chunkTranslationData = chunk.GetNativeArray(TranslationType);
                var chunkRotationData = chunk.GetNativeArray(RotationType);

                for (int i = 0; i < chunk.Count; ++i)
                {
                    var entity = chunkEntityData[i];
                    var controller = chunkCharacterControllerData[i];
                    var position = chunkTranslationData[i];
                    var rotation = chunkRotationData[i];
                    var collider = ColliderData[entity];

                    HandleChunk(ref entity, ref controller, ref position, ref rotation, ref collider, ref collisionWorld);

                    chunkTranslationData[i] = position;
                    chunkCharacterControllerData[i] = controller;
                }
            }

            private void HandleChunk(ref Entity entity, ref CharacterControllerComponent controller, ref Translation position, ref Rotation rotation, ref PhysicsCollider collider, ref CollisionWorld collisionWorld)
            {
                float3 epsilon = new float3(0.0f, Epsilon, 0.0f) * -math.normalize(controller.Gravity);
                float3 currPos = position.Value + epsilon;
                quaternion currRot = rotation.Value;

                float3 verticalVelocity = new float3();
                float3 gravityVelocity = controller.Gravity * DeltaTime * (controller.IsGrounded ? 0.0f : 1.0f);
                float3 jumpVelocity = controller.JumpVelocity;
                float3 horizontalVelocity = controller.HorizontalVelocity + (controller.CurrentDirection * controller.CurrentMagnitude * controller.Speed * DeltaTime);

                if (controller.IsGrounded && controller.Jump && MathUtils.IsZero(math.lengthsq(controller.JumpVelocity)))
                {
                    jumpVelocity += controller.JumpStrength * -math.normalize(controller.Gravity);
                }

                HandleHorizontalMovement(ref horizontalVelocity, ref entity, ref currPos, ref currRot, ref controller, ref collider, ref collisionWorld);
                //ApplyFriction(ref horizontalVelocity, ref controller);

                currPos += horizontalVelocity;

                HandleVerticalMovement(ref verticalVelocity, ref jumpVelocity, ref gravityVelocity, ref entity, ref currPos, ref currRot, ref controller, ref collider, ref collisionWorld);
                ApplyDrag(ref jumpVelocity, ref controller);

                currPos += verticalVelocity;

                CorrectForCollision(ref entity, position.Value, ref currPos, ref currRot, ref controller, ref collider, ref collisionWorld);
                DetermineIfGrounded(entity, ref currPos, ref epsilon, ref controller, ref collider, ref collisionWorld);

                position.Value = currPos - epsilon;
                controller.HorizontalVelocity = new float3();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="currPos"></param>
            /// <param name="currRot"></param>
            /// <param name="controller"></param>
            /// <param name="collider"></param>
            /// <param name="collisionWorld"></param>
            private void CorrectForCollision(ref Entity entity, float3 prevPos, ref float3 currPos, ref quaternion currRot, ref CharacterControllerComponent controller, ref PhysicsCollider collider, ref CollisionWorld collisionWorld)
            {
                RigidTransform transform = new RigidTransform()
                {
                    pos = currPos,
                    rot = currRot
                };

                if (PhysicsUtils.ColliderDistance(out DistanceHit smallestHit, collider, 0.1f, transform, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, null, ColliderData, Allocator.Temp))
                {
                    if (smallestHit.Distance < -0.075f)
                    {
                        currPos += math.abs(smallestHit.Distance) * smallestHit.SurfaceNormal;
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="horizontalVelocity"></param>
            /// <param name="entity"></param>
            /// <param name="currPos"></param>
            /// <param name="currRot"></param>
            /// <param name="controller"></param>
            /// <param name="collider"></param>
            /// <param name="collisionWorld"></param>
            /// <param name="entityManager"></param>
            private void HandleHorizontalMovement(ref float3 horizontalVelocity, ref Entity entity, ref float3 currPos, ref quaternion currRot, ref CharacterControllerComponent controller, ref PhysicsCollider collider, ref CollisionWorld collisionWorld)
            {
                if (MathUtils.IsZero(horizontalVelocity))
                {
                    return;
                }

                float3 targetPos = currPos + horizontalVelocity;
                float3 step = new float3(0.0f, controller.MaxStep, 0.0f);

                var horizontalCollisions = PhysicsUtils.ColliderCastAll(collider, currPos, targetPos, ref collisionWorld, entity, Allocator.Temp);
                PhysicsUtils.TrimByFilter(ref horizontalCollisions, ColliderData, PhysicsCollisionFilters.DynamicWithPhysical);

                if (horizontalCollisions.Count != 0)
                {
                    // We either have to step or slide as something is in our way.
                    PhysicsUtils.ColliderCast(out ColliderCastHit nearestStepHit, collider, targetPos + step, targetPos, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, null, ColliderData, Allocator.Temp);

                    if (!MathUtils.IsZero(nearestStepHit.Fraction))
                    {
                        targetPos += (step * (1.0f - nearestStepHit.Fraction));
                        horizontalVelocity = targetPos - currPos;
                    }
                    else
                    {
                        var horizontalDistances = PhysicsUtils.ColliderDistanceAll(collider, 1.0f, new RigidTransform() { pos = currPos + horizontalVelocity, rot = currRot }, ref collisionWorld, entity, Allocator.Temp);
                        PhysicsUtils.TrimByFilter(ref horizontalDistances, ColliderData, PhysicsCollisionFilters.DynamicWithPhysical);

                        foreach (var horizontalDistanceHit in horizontalDistances)
                        {
                            if (horizontalDistanceHit.Distance >= 0.0f)
                            {
                                continue;
                            }

                            horizontalVelocity += (horizontalDistanceHit.SurfaceNormal * -horizontalDistanceHit.Distance);
                        }
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="jumpVelocity"></param>
            /// <param name="entity"></param>
            /// <param name="currPos"></param>
            /// <param name="currRot"></param>
            /// <param name="controller"></param>
            /// <param name="collider"></param>
            /// <param name="collisionWorld"></param>
            /// <param name="entityManager"></param>
            private void HandleVerticalMovement(
                ref float3 totalVelocity,
                ref float3 jumpVelocity,
                ref float3 gravityVelocity,
                ref Entity entity,
                ref float3 currPos,
                ref quaternion currRot,
                ref CharacterControllerComponent controller,
                ref PhysicsCollider collider,
                ref CollisionWorld collisionWorld)
            {
                totalVelocity = jumpVelocity + gravityVelocity;

                var verticalCollisions = PhysicsUtils.ColliderCastAll(collider, currPos, currPos + totalVelocity, ref collisionWorld, entity, Allocator.Temp);
                PhysicsUtils.TrimByFilter(ref verticalCollisions, ColliderData, PhysicsCollisionFilters.DynamicWithPhysical);

                if (verticalCollisions.Count != 0)
                {
                    RigidTransform transform = new RigidTransform()
                    {
                        pos = currPos + totalVelocity,
                        rot = currRot
                    };

                    if (PhysicsUtils.ColliderDistance(out DistanceHit verticalPenetration, collider, 1.0f, transform, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, null, ColliderData, Allocator.Temp))
                    {
                        if (verticalPenetration.Distance < 0.0f)
                        {
                            totalVelocity += (verticalPenetration.SurfaceNormal * -verticalPenetration.Distance);

                            if (PhysicsUtils.ColliderCast(out ColliderCastHit adjustedHit, collider, currPos, currPos + totalVelocity, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, null, ColliderData, Allocator.Temp))
                            {
                                totalVelocity *= adjustedHit.Fraction;
                            }

                            controller.JumpVelocity = new float3();
                        }
                    }
                }

                totalVelocity = MathUtils.ZeroOut(totalVelocity, 0.01f);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="jumpVelocity"></param>
            /// <param name="controller"></param>
            /// <param name="deltaTime"></param>
            private void ApplyDrag(ref float3 jumpVelocity, ref CharacterControllerComponent controller)
            {
                float currSpeed = math.length(jumpVelocity);
                float dragDelta = controller.Drag * DeltaTime;

                currSpeed = math.max((currSpeed - dragDelta), 0.0f);

                if (MathUtils.IsZero(currSpeed))
                {
                    jumpVelocity = new float3();
                }
                else
                {
                    jumpVelocity = math.normalize(jumpVelocity) * currSpeed;
                    jumpVelocity = MathUtils.ZeroOut(jumpVelocity, 0.001f);
                }

                controller.JumpVelocity = jumpVelocity;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="currPos"></param>
            /// <param name="epsilon"></param>
            /// <param name="collider"></param>
            /// <param name="collisionWorld"></param>
            /// <returns></returns>
            private unsafe static void DetermineIfGrounded(Entity entity, ref float3 currPos, ref float3 epsilon, ref CharacterControllerComponent controller, ref PhysicsCollider collider, ref CollisionWorld collisionWorld)
            {
                float3 gravity = math.normalize(controller.Gravity);
                float3 offset = (gravity * 0.1f);

                var aabb = collider.ColliderPtr->CalculateAabb();
                float mod = 0.15f;

                float3 posLeft = currPos - new float3(aabb.Extents.x * mod, 0.0f, 0.0f);
                float3 posRight = currPos + new float3(aabb.Extents.x * mod, 0.0f, 0.0f);
                float3 posForward = currPos + new float3(0.0f, 0.0f, aabb.Extents.z * mod);
                float3 posBackward = currPos - new float3(0.0f, 0.0f, aabb.Extents.z * mod);

                controller.IsGrounded = PhysicsUtils.Raycast(out RaycastHit centerHit, currPos, currPos + offset, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, Allocator.Temp) ||
                                        PhysicsUtils.Raycast(out RaycastHit leftHit, posLeft, posLeft + offset, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, Allocator.Temp) ||
                                        PhysicsUtils.Raycast(out RaycastHit rightHit, posRight, posRight + offset, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, Allocator.Temp) ||
                                        PhysicsUtils.Raycast(out RaycastHit forwardHit, posForward, posForward + offset, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, Allocator.Temp) ||
                                        PhysicsUtils.Raycast(out RaycastHit backwardHit, posBackward, posBackward + offset, ref collisionWorld, entity, PhysicsCollisionFilters.DynamicWithPhysical, Allocator.Temp);
            }
        }
    }
}
