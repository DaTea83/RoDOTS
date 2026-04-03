using EugeneC.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace EugeneC.ECS {

    [BurstCompile]
    [UpdateInGroup(typeof(EuCPreTransformSystemGroup))]
    public partial struct WaveMoveISystem : ISystem {

        private const float NoiseScale = .2F;
        private const float DepthOffset = 1f;

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            new Job {
                Time = (float)SystemAPI.Time.ElapsedTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct Job : IJobEntity {

            [ReadOnly] public float Time;

            [BurstCompile]
            private void Execute(WaveMoveIData data, ref LocalTransform lt) {
                var pos = lt.Position.GetNoiseOffsetPos(data.YOffset, Time * data.Speed,
                    data.Height, NoiseScale, DepthOffset);
                lt.Position = pos;
            }

        }

    }

}