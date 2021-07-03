using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Basic system which follows the entity with the <see cref="CameraFollowComponent"/>.
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
    public sealed class CameraFollowSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((
                Entity entity,
                ref Translation position,
                ref Rotation rotation,
                ref CameraFollowComponent camera) =>
            {
                ProcessCameraInput(ref camera);

                Vector3 currPos = Camera.main.transform.position;
                Vector3 targetPos = new Vector3(position.Value.x, position.Value.y + 1.0f, position.Value.z);

                targetPos += (Camera.main.transform.forward * -camera.Zoom);
                float posLerp = Mathf.Clamp(Time.DeltaTime * 8.0f, 0.0f, 1.0f);

                Camera.main.transform.rotation = new Quaternion();
                Camera.main.transform.Rotate(new Vector3(camera.Pitch, camera.Yaw, 0.0f));
                Camera.main.transform.position = Vector3.Lerp(currPos, targetPos, posLerp);

                camera.Forward = Camera.main.transform.forward;
                camera.Right = Camera.main.transform.right;
            });
        }

        /// <summary>
        /// Handles all camera related input.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        private bool ProcessCameraInput(ref CameraFollowComponent camera)
        {
            return ProcessCameraZoom(ref camera) ||
                   ProcessCameraYawPitch(ref camera);
        }

        /// <summary>
        /// Handles input for zooming the camera in and out.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        private bool ProcessCameraZoom(ref CameraFollowComponent camera)
        {
            float scroll = Input.GetAxis("Mouse Scroll Wheel");

            if (!MathUtils.IsZero(scroll))
            {
                camera.Zoom -= scroll;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles input for manipulating the camera yaw (rotating around).
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        private bool ProcessCameraYawPitch(ref CameraFollowComponent camera)
        {
            if (MathUtils.IsZero(Input.GetAxis("Mouse 2")))
            {
                return false;
            }

            camera.Yaw += Input.GetAxis("Mouse X");
            camera.Pitch -= Input.GetAxis("Mouse Y");

            return true;
        }
    }
}
