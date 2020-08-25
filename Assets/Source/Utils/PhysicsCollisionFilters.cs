using Unity.Physics;

namespace VertexFragment
{
    /// <summary>
    /// Different hard-coded layers that a physics object may exist in and/or collide with.
    /// </summary>
    public enum CollisionFilterLayers
    {
        /// <summary>
        /// Is not part of any layer and/or does not collide with any layer.
        /// </summary>
        None = 0,

        /// <summary>
        /// Exists in all layers and/or collides with all layers.
        /// </summary>
        All = 0xFFFF,

        /// <summary>
        /// Generic layer for static entities which cause collisions but are not afected by any forces or impulses.
        /// </summary>
        Static = 1 << 0,

        /// <summary>
        /// Generic layer for dynamic entities which cause collisions and are affected by forces and/or impulses.
        /// </summary>
        Dynamic = 1 << 1,

        /// <summary>
        /// Layer representing the terrain, which is considered differently from any other generic static.
        /// </summary>
        Terrain = 1 << 2,

        /// <summary>
        /// Generic layer for trigger volumes.
        /// </summary>
        TriggerVolumes = 1 << 3
    };

    /// <summary>
    /// Collection of pre-made collision filters.
    /// </summary>
    public static class PhysicsCollisionFilters
    {
        public static readonly CollisionFilter AllWithAll = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.All,
            CollidesWith = (int)CollisionFilterLayers.All
        };

        public static readonly CollisionFilter AllWithStatic = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.All,
            CollidesWith = (int)CollisionFilterLayers.Static
        };

        public static readonly CollisionFilter AllWithDynamic = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.All,
            CollidesWith = (int)CollisionFilterLayers.Dynamic
        };

        public static readonly CollisionFilter AllWithTerrain = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.All,
            CollidesWith = (int)CollisionFilterLayers.Terrain
        };

        public static readonly CollisionFilter AllWithTriggerVolumes = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.All,
            CollidesWith = (int)CollisionFilterLayers.TriggerVolumes
        };

        public static readonly CollisionFilter StaticWithStatic = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Static,
            CollidesWith = (int)CollisionFilterLayers.Static
        };

        public static readonly CollisionFilter StaticWithDynamic = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Static,
            CollidesWith = (int)CollisionFilterLayers.Dynamic
        };

        public static readonly CollisionFilter StaticWitTerrain = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Static,
            CollidesWith = (int)CollisionFilterLayers.Terrain
        };

        public static readonly CollisionFilter StaticWithTriggerVolumes = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Static,
            CollidesWith = (int)CollisionFilterLayers.TriggerVolumes
        };

        public static readonly CollisionFilter DynamicWithDynamic = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Dynamic,
            CollidesWith = (int)CollisionFilterLayers.Dynamic
        };

        public static readonly CollisionFilter DynamicWithTerrain = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Dynamic,
            CollidesWith = (int)CollisionFilterLayers.Terrain
        };

        public static readonly CollisionFilter DynamicWithPhysical = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Dynamic,
            CollidesWith = (int)CollisionFilterLayers.Terrain | (int)CollisionFilterLayers.Static | (int)CollisionFilterLayers.Dynamic,
            GroupIndex = -1
        };

        public static readonly CollisionFilter DynamicWithTriggerVolumes = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Dynamic,
            CollidesWith = (int)CollisionFilterLayers.TriggerVolumes
        };

        public static readonly CollisionFilter TerrainWithTerrain = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Terrain,
            CollidesWith = (int)CollisionFilterLayers.Terrain
        };

        public static readonly CollisionFilter TerrainWithTriggerVolumes = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.Terrain,
            CollidesWith = (int)CollisionFilterLayers.TriggerVolumes
        };

        public static readonly CollisionFilter TriggerVolumesWithTriggerVolumes = new CollisionFilter()
        {
            BelongsTo = (int)CollisionFilterLayers.TriggerVolumes,
            CollidesWith = (int)CollisionFilterLayers.TriggerVolumes
        };

        /// <summary>
        /// Used to construct more complex filters than the default provided ones. For example:
        /// 
        /// <code>
        ///     // A filter which is dynamic, but collides with static and dynamic (not trigger volumes, etc.).
        ///     var filter = PhysicsCollisionFilters.Build((uint)CollisionFilterLayers.Dynamic, (uint)CollisionFilterLayers.Static, (uint)CollisionFilterLayers.Dynamic);
        /// </code>
        /// </summary>
        /// <param name="belongsTo"></param>
        /// <param name="collidesWith"></param>
        /// <returns></returns>
        public static CollisionFilter Build(uint belongsTo, params uint[] collidesWith)
        {
            CollisionFilter filter = new CollisionFilter()
            {
                BelongsTo = belongsTo
            };

            foreach (var i in collidesWith)
            {
                filter.CollidesWith |= i;
            }

            return filter;
        }
    }
}
