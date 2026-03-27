using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using static Unity.Physics.Math;
using RaycastHit = Unity.Physics.RaycastHit;

namespace EugeneC.ECS {

    [BurstCompile]
    public struct PhysicsColliderICollector : ICollector<RaycastHit> {

        public PhysicsColliderICollector(int dynamicCount, float maxFraction = 1f) {
            Hit = default;
            IgnoreTriggers = true;
            IgnoreStatic = true;
            _numDynamicBodies = dynamicCount;
            MaxFraction = maxFraction;
            NumHits = 0;
        }

        public RaycastHit Hit;
        public bool IgnoreTriggers;
        public bool IgnoreStatic;
        private readonly int _numDynamicBodies;

        // Below are all ICollector implementations
        public bool EarlyOutOnFirstHit => false;
        public float MaxFraction { get; private set; }
        public int NumHits { get; private set; }

        public bool AddHit(RaycastHit hit) {
            Assert.IsTrue(hit.Fraction <= MaxFraction);

            var passed = true;

            if (IgnoreStatic)
                passed &= hit.RigidBodyIndex >= 0 && hit.RigidBodyIndex < _numDynamicBodies;

            if (IgnoreTriggers)
                passed &= hit.Material.CollisionResponse != CollisionResponsePolicy.RaiseTriggerEvents;

            if (!passed) return false;

            Hit = hit;
            MaxFraction = hit.Fraction;
            NumHits = 1;

            return true;
        }

    }

    [BurstCompile]
    public struct CastIJob : IJob {

        [ReadOnly] public CollisionWorld CollisionWorld;
        [ReadOnly] public bool IgnoreTriggers;
        [ReadOnly] public bool IgnoreStatic;

        public NativeReference<SpringData> SpringDataRef;
        public RaycastInput RayInput;
        public float Near;
        public float3 Forward;
        public float MaxDistance;

        public void Execute() {
            var pickCollector = new PhysicsColliderICollector(CollisionWorld.NumDynamicBodies) {
                IgnoreTriggers = IgnoreTriggers,
                IgnoreStatic = IgnoreStatic
            };

            if (CollisionWorld.CastRay(RayInput, ref pickCollector)) {
                var fraction = pickCollector.Hit.Fraction;
                var hitBody = CollisionWorld.Bodies[pickCollector.Hit.RigidBodyIndex];

                //Grab that specific point on the body instead of the center
                float3 pointOnBody;

                {
                    //Convert world transform to local transform
                    var localTrans = Inverse(new MTransform(hitBody.WorldFromBody));
                    pointOnBody = Mul(localTrans, pickCollector.Hit.Position);
                }

                float rayDot;

                {
                    var rayDir = math.normalize(RayInput.End - RayInput.Start);
                    rayDot = math.dot(rayDir, Forward);
                }

                SpringDataRef.Value = new SpringData {
                    Entity = hitBody.Entity,
                    Picked = true,
                    PointOnBody = pointOnBody,
                    Depth = Near + rayDot * fraction * MaxDistance
                };
            }
            else {
                SpringDataRef.Value = new SpringData {
                    Picked = false
                };
            }
        }

    }

    public struct SpringData {

        public Entity Entity;
        public bool Picked;
        public float3 PointOnBody;
        public float Depth;

    }

}