using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Tracks if the attached entity is moving, and how it can be moved.
    /// </summary>
    public struct CharacterControllerComponent : IComponentData
    {
        // -------------------------------------------------------------------------------------
        // Current Movement
        // -------------------------------------------------------------------------------------

        /// <summary>
        /// The current direction that the character is moving.
        /// </summary>
        public float3 CurrentDirection { get; set; }

        /// <summary>
        /// The current magnitude of the character movement.
        /// If <c>0.0</c>, then the character is not being directly moved by the controller but residual forces may still be active.
        /// </summary>
        public float CurrentMagnitude { get; set; }

        /// <summary>
        /// Is the character requesting to jump?
        /// Used in conjunction with <see cref="IsGrounded"/> to determine if the <see cref="JumpStrength"/> should be used to make the entity jump.
        /// </summary>
        public bool Jump { get; set; }

        // -------------------------------------------------------------------------------------
        // Control Properties
        // -------------------------------------------------------------------------------------

        /// <summary>
        /// Gravity force applied to the character.
        /// </summary>
        public float3 Gravity { get; set; }

        /// <summary>
        /// The maximum speed at which this character moves.
        /// </summary>
        public float MaxSpeed { get; set; }

        /// <summary>
        /// The current speed at which the player moves.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// The jump strength which controls how high a jump is, in conjunction with <see cref="Gravity"/>.
        /// </summary>
        public float JumpStrength { get; set; }

        /// <summary>
        /// The maximum height the character can step up, in world units.
        /// </summary>
        public float MaxStep { get; set; }

        /// <summary>
        /// Drag value applied to reduce the <see cref="VerticalVelocity"/>.
        /// </summary>
        public float Drag { get; set; }

        // -------------------------------------------------------------------------------------
        // Control State
        // -------------------------------------------------------------------------------------

        /// <summary>
        /// True if the character is on the ground.
        /// </summary>
        public bool IsGrounded { get; set; }

        /// <summary>
        /// The current horizontal velocity of the character.
        /// </summary>
        public float3 HorizontalVelocity { get; set; }

        /// <summary>
        /// The current jump velocity of the character.
        /// </summary>
        public float3 VerticalVelocity { get; set; }
    }

    /// <summary>
    /// Used to add <see cref="CharacterControllerComponent"/> via the Editor.
    /// </summary>
    [Serializable]
    public sealed class CharacterControllerComponentView : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float3 Gravity = new float3(0.0f, -22.0f, 0.0f);
        public float MaxSpeed = 7.5f;
        public float Speed = 5.0f;
        public float JumpStrength = 9.0f;
        public float MaxStep = 0.35f;
        public float Drag = 0.2f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled)
            {
                return;
            }

            dstManager.AddComponentData(entity, new CharacterControllerComponent()
            {
                Gravity = Gravity,
                MaxSpeed = MaxSpeed,
                Speed = Speed,
                JumpStrength = JumpStrength,
                MaxStep = MaxStep,
                Drag = Drag
            });
        }
    }
}