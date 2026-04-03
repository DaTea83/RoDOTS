using Unity.Entities;
using Unity.Physics;

namespace EugeneC.Utilities {

    public static partial class HelperCollection {

        public static (Entity, Entity) GetSimulationEntities<T, TU>(this TriggerEvent triggerEvent,
            ComponentLookup<T> aLookup,
            ComponentLookup<TU> bLookup)
            where T : unmanaged, IComponentData
            where TU : unmanaged, IComponentData {
            Entity aEntity, bEntity;

            if (aLookup.HasComponent(triggerEvent.EntityA) &&
                bLookup.HasComponent(triggerEvent.EntityB)) {
                aEntity = triggerEvent.EntityA;
                bEntity = triggerEvent.EntityB;

                return (aEntity, bEntity);
            }

            if (aLookup.HasComponent(triggerEvent.EntityB) &&
                bLookup.HasComponent(triggerEvent.EntityA)) {
                aEntity = triggerEvent.EntityB;
                bEntity = triggerEvent.EntityA;

                return (aEntity, bEntity);
            }

            aEntity = Entity.Null;
            bEntity = Entity.Null;

            return (aEntity, bEntity);
        }

        public static (Entity, Entity, int) GetSimulationEntities<T, U, V>(this TriggerEvent triggerEvent,
            ComponentLookup<T> aLookup,
            ComponentLookup<U> bLookup,
            ComponentLookup<V> cLookup)
            where T : unmanaged, IComponentData
            where U : unmanaged, IComponentData
            where V : unmanaged, IComponentData {
            var (aEntity, bEntity) = triggerEvent.GetSimulationEntities(aLookup, bLookup);

            if (aEntity != Entity.Null && bEntity != Entity.Null) return (aEntity, bEntity, 1);

            (aEntity, bEntity) = triggerEvent.GetSimulationEntities(bLookup, cLookup);

            if (aEntity != Entity.Null && bEntity != Entity.Null) return (aEntity, bEntity, 2);

            (aEntity, bEntity) = triggerEvent.GetSimulationEntities(aLookup, cLookup);

            if (aEntity != Entity.Null && bEntity != Entity.Null) return (aEntity, bEntity, 3);

            return (aEntity, bEntity, -1);
        }

        public static (Entity, Entity) GetSimulationEntities<T, U>(this CollisionEvent collisionEvent,
            ComponentLookup<U> aLookup,
            ComponentLookup<T> bLookup)
            where U : unmanaged, IComponentData
            where T : unmanaged, IComponentData {
            Entity aEntity, bEntity;

            if (aLookup.HasComponent(collisionEvent.EntityA) &&
                bLookup.HasComponent(collisionEvent.EntityB)) {
                aEntity = collisionEvent.EntityA;
                bEntity = collisionEvent.EntityB;

                return (aEntity, bEntity);
            }

            if (aLookup.HasComponent(collisionEvent.EntityB) &&
                bLookup.HasComponent(collisionEvent.EntityA)) {
                aEntity = collisionEvent.EntityB;
                bEntity = collisionEvent.EntityA;

                return (aEntity, bEntity);
            }

            aEntity = Entity.Null;
            bEntity = Entity.Null;

            return (aEntity, bEntity);
        }

        public static (Entity, Entity, int) GetSimulationEntities<T, U, V>(this CollisionEvent collisionEvent,
            ComponentLookup<T> aLookup,
            ComponentLookup<U> bLookup,
            ComponentLookup<V> cLookup)
            where T : unmanaged, IComponentData
            where U : unmanaged, IComponentData
            where V : unmanaged, IComponentData {
            var (aEntity, bEntity) = collisionEvent.GetSimulationEntities(aLookup, bLookup);

            if (aEntity != Entity.Null && bEntity != Entity.Null) return (aEntity, bEntity, 1);

            (aEntity, bEntity) = collisionEvent.GetSimulationEntities(bLookup, cLookup);

            if (aEntity != Entity.Null && bEntity != Entity.Null) return (aEntity, bEntity, 2);

            (aEntity, bEntity) = collisionEvent.GetSimulationEntities(aLookup, cLookup);

            if (aEntity != Entity.Null && bEntity != Entity.Null) return (aEntity, bEntity, 3);

            return (aEntity, bEntity, -1);
        }

    }

}