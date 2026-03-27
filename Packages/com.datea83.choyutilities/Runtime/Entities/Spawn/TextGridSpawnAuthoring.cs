using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public sealed class TextGridSpawnAuthoring : MonoBehaviour {

        [SerializeField] private GameObject[] prefabs;
        [SerializeField] private char[] identifiers;
        [SerializeField] private TextAsset textAsset;
        [SerializeField] private int2 total = new(10, 10);
        [SerializeField] private float2 spacing = new(1f, 1f);
        [SerializeField] private float scale = 1f;

        private class TextGridSpawnAuthoringBaker : Baker<TextGridSpawnAuthoring> {

            public override void Bake(TextGridSpawnAuthoring authoring) {
                DependsOn(authoring.textAsset);
                var e = GetEntity(TransformUsageFlags.Dynamic);

                var pattern = MakeBlob(authoring);
                AddBlobAsset(ref pattern, out _);

                AddComponent(e, new TextGridSpawnIData {
                    Pattern = pattern,
                    Scale = authoring.scale,
                    Spacing = authoring.spacing,
                    Total = authoring.total
                });

                var buffer = AddBuffer<TextGridIBuffer>(e);

                for (ushort i = 0; i < authoring.prefabs.Length; i++) {
                    if (authoring.prefabs[i] is null)
                        throw new ArgumentException();

                    var p = GetEntity(authoring.prefabs[i], TransformUsageFlags.Dynamic);

                    buffer.Add(new TextGridIBuffer {
                        Prefab = p
                    });
                }
            }

            private static BlobAssetReference<BlobArray<FixedList512Bytes<byte>>> MakeBlob(
                TextGridSpawnAuthoring authoring) {
                using var builder = new BlobBuilder(Allocator.Temp);
                ref var patternBlob = ref builder.ConstructRoot<BlobArray<FixedList512Bytes<byte>>>();

                var arrayBuilder = builder.Allocate(ref patternBlob, 1);
                var patternString = authoring.textAsset.text;
                var patternList = new FixedList512Bytes<byte>();
                var patternId = authoring.identifiers;

                foreach (var c in patternString) {
                    if (char.IsControl(c)) continue;
                    if (c == '\n') continue;

                    for (byte i = 0; i < patternId.Length; i++) {
                        if (patternId[i] != c) continue;
                        patternList.Add(i);
                    }
                }

                arrayBuilder[0] = patternList;

                return builder.CreateBlobAssetReference<BlobArray<FixedList512Bytes<byte>>>(Allocator.Persistent);
            }

        }

    }

    public struct TextGridSpawnIData : IComponentData {

        public BlobAssetReference<BlobArray<FixedList512Bytes<byte>>> Pattern;
        public float2 Spacing;
        public float Scale;
        public int2 Total;

    }

    public struct TextGridIBuffer : IBufferElementData {

        public Entity Prefab;

    }

    [BurstCompile]
    [UpdateInGroup(typeof(Eu_InitializationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(InitializeRandomISystem))]
    public partial struct TextGridSpawnISystem : ISystem {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (spawn, buffer, ltw, entity)
                     in SystemAPI
                         .Query<RefRO<TextGridSpawnIData>, DynamicBuffer<TextGridIBuffer>, RefRO<LocalToWorld>>()
                         .WithAll<TextGridSpawnIData>().WithEntityAccess()) {
                var rot = ltw.ValueRO.Rotation;

                var right = math.mul(rot, new float3(1f, 0f, 0f));
                var forward = math.mul(rot, new float3(0f, 0f, 1f));

                var halfWidth = spawn.ValueRO.Total.x * spawn.ValueRO.Spacing.x * 0.5f;
                var halfHeight = spawn.ValueRO.Total.y * spawn.ValueRO.Spacing.y * 0.5f;

                var startPos = ltw.ValueRO.Position - right * halfWidth - forward * halfHeight;
                var pattern = spawn.ValueRO.Pattern.Value[0];

                var spawnIndex = 0;

                for (var x = 0; x < spawn.ValueRO.Total.x; x++)
                for (var y = 0; y < spawn.ValueRO.Total.y; y++, spawnIndex++) {
                    var spawnType = pattern[spawnIndex];
                    var spawnEntity = buffer[spawnType].Prefab;

                    if (spawnEntity == Entity.Null) continue;

                    var newEntity = ecb.Instantiate(spawnEntity);

                    var pos =
                        startPos +
                        right * (x * spawn.ValueRO.Spacing.x) +
                        forward * (y * spawn.ValueRO.Spacing.y);

                    var eLt = LocalTransform.FromPositionRotationScale(
                        pos,
                        rot,
                        spawn.ValueRO.Scale);

                    ecb.SetComponent(newEntity, eLt);
                }

                ecb.RemoveComponent<TextGridSpawnIData>(entity);
            }

            ecb.Playback(state.EntityManager);
        }

    }

}