using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Assigned to the Entity which the Camera should focus on.
    /// </summary>
    [Serializable]
    public struct CameraFollowComponent : IComponentData
    {
        private float _pitch;
        private float _zoom;

        /// <summary>
        /// The Yaw angle of the camera. For the standard camera this determines how it is rotated around the followed entity.
        /// </summary>
        public float Yaw { get; set; }

        /// <summary>
        /// The Pitch angle of the camera. For the standard camera this determines how high it is above the followed entity.
        /// </summary>
        public float Pitch
        {
            get { return _pitch; }
            set { _pitch = math.clamp(value, MinPitch, MaxPitch); }
        }

        /// <summary>
        /// Minimum allowed Pitch angle. Used to clamp the camera and prevents it from being able to see straight horizontally or even up.
        /// </summary>
        public float MinPitch { get; set; }

        /// <summary>
        /// Maximum allowed Pitch angle. Used to clamp the camera and prevent it from being able to look straight down which ruins certain effects (such as grass billboards).
        /// </summary>
        public float MaxPitch { get; set; }

        /// <summary>
        /// The Zoom of the camera, which determines how far away from the followed entity it is.
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = math.clamp(value, MinZoom, MaxZoom); }
        }

        /// <summary>
        /// The minimum Zoom of the camera. Used to clamp the camera and prevent it from being moved too near/into the followed entity.
        /// </summary>
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum Zoom of the camera. Used to clamp the camera and prevent it from being moved too far from the entity which may reveal too much of the scene.
        /// </summary>
        public float MaxZoom { get; set; }

        /// <summary>
        /// The normalized camera forward vector.
        /// </summary>
        public Vector3 Forward { get; set; }

        /// <summary>
        /// The normalize camera right vector.
        /// </summary>
        public Vector3 Right { get; set; }
    }

    /// <summary>
    /// Used to add <see cref="CameraFollowComponent"/> via the Editor.
    /// </summary>
    [Serializable]
    public sealed class CameraFollowComponentView : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float Yaw = 0.0f;
        public float Pitch = 53.0f;
        public float Zoom = 8.0f;

        public float MinPitch = 53.0f;
        public float MaxPitch = 70.0f;
        public float MinZoom = 5.0f;
        public float MaxZoom = 10.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled)
            {
                return;
            }

            dstManager.AddComponentData(entity, new CameraFollowComponent()
            {
                MinPitch = MinPitch,
                MaxPitch = MaxPitch,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                Yaw = Yaw,
                Pitch = Pitch,
                Zoom = Zoom
            });
        }
    }
}
