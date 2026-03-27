using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    [BurstCompile(CompileSynchronously = true)]
    [UpdateInGroup(typeof(Eu_PreTransformSystemGroup))]
    public partial struct VoxelTransformISystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<VoxelizerISingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var job = new Job {
                DestroyLookup = SystemAPI.GetBufferLookup<DestroyBufferEntryIBuffer>(),
                Voxelizer = SystemAPI.GetSingleton<VoxelizerISingleton>(),
                ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            job.ScheduleParallel();
        }

        [BurstCompile(CompileSynchronously = true)]
        private partial struct Job : IJobEntity {

            [NativeDisableParallelForRestriction] public BufferLookup<DestroyBufferEntryIBuffer> DestroyLookup;
            public VoxelizerISingleton Voxelizer;
            public float ElapsedTime;
            public float DeltaTime;

            private void Execute(Entity entity,
                ref LocalTransform lt,
                ref BoxIData box,
                ref URPMaterialPropertyBaseColor urp) {
                box.ExistTime += DeltaTime;

                if (box.ExistTime >= Voxelizer.VoxelLife) {
                    var d = DestroyLookup[entity];
                    d.Add(new DestroyBufferEntryIBuffer { Value = 1 });
                }

                box.Velocity -= Voxelizer.Gravity * DeltaTime;
                lt.Position.y += box.Velocity.y * DeltaTime;

                if (lt.Position.y < Voxelizer.GroundLevel) {
                    box.Velocity *= -1;
                    lt.Position.y = -lt.Position.y;
                }

                var lifeTime = box.ExistTime / Voxelizer.VoxelLife;
                var lifeSqr = lifeTime * lifeTime;
                lt.Scale = Voxelizer.VoxelSize * (1 - lifeSqr * lifeSqr);

                var hue = lt.Position.z * Voxelizer.ColorFrequency;
                hue = math.frac(hue + ElapsedTime * Voxelizer.ColorSpeed);
                urp.Value = (Vector4)Color.HSVToRGB(hue, 1, 1);
            }

        }

    }

}