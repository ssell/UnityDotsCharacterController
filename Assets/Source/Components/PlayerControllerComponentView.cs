using System;
using Unity.Entities;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Indicates that the entity is controlled directly by player input.
    /// </summary>
    public struct PlayerControllerComponent : IComponentData
    {
        // Intentionally empty.
    }

    /// <summary>
    /// Used to add <see cref="PlayerControllerComponent"/> via the Editor.
    /// </summary>
    [Serializable]
    public sealed class PlayerControllerComponentView : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled)
            {
                return;
            }

            dstManager.AddComponentData(entity, new PlayerControllerComponent());
        }
    }
}
