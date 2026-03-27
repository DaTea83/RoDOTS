using System;
using EugeneC.ECS;
using EugeneC.Utilities;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ObsoleteV2 {

    [Obsolete]
    [DisallowMultipleComponent]
    public abstract class GenericPathwayController<T, TEnum> : MonoBehaviour
        where T : GenericPathwayController<T, TEnum>
        where TEnum : Enum {

        [SerializeField] [Range(1, byte.MaxValue)]
        protected byte subdivision = 48;

        [SerializeField] protected AgentMovementBase[] prefabs;
        [SerializeField] protected PathSerializable[] pathIds;

        protected BlobAssetReference<BlobArray<PathwayBlob>> BakePathways() {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var pathwaysBlob = ref builder.ConstructRoot<BlobArray<PathwayBlob>>();

            var lengthIds = pathIds.Length;
            var pathwayBuilder = builder.Allocate(ref pathwaysBlob, lengthIds);

            for (var p = 0; p < lengthIds; p++) {
                var pathCount = pathIds[p].paths != null ? pathIds[p].paths.Length : 0;

                if (!Enum.IsDefined(typeof(TEnum), pathIds[p].type)) continue;
                var idToByte = Convert.ToByte(pathIds[p].type);

                ref var idArr = ref pathwayBuilder[p].Id;
                var idBuilder = builder.Allocate(ref idArr, 1);
                idBuilder[0] = idToByte;

                ref var pathArr = ref pathwayBuilder[p].Path;
                var splineBuilder = builder.Allocate(ref pathArr, pathCount);

                // For each path available in the pathway
                for (var i = 0; i < pathCount; i++) {
                    ref var originArr = ref pathwayBuilder[p].Origins;
                    var originBuilder = builder.Allocate(ref originArr, pathCount);
                    var pointSerializable = pathIds[p].paths;

                    if (pointSerializable is null || pointSerializable[i].agentPoints.Length < 1) continue;

                    foreach (var t in pointSerializable[i].agentPoints) originBuilder[i] = t.Position;

                    using var bakedPoints = CreateSpline(pathIds[p], i);
                    var bakedLength = bakedPoints.Length;

                    ref var splineBlob = ref splineBuilder[i];
                    ref var posArr = ref splineBlob.Position;
                    ref var rotArr = ref splineBlob.Rotation;
                    ref var disArr = ref splineBlob.Distance;
                    ref var tanArr = ref splineBlob.Tangent;

                    var posBuilder = builder.Allocate(ref posArr, bakedLength);
                    var rotBuilder = builder.Allocate(ref rotArr, bakedLength);
                    var disBuilder = builder.Allocate(ref disArr, bakedLength);
                    var tanBuilder = builder.Allocate(ref tanArr, bakedLength);

                    var distance = 0f;

                    for (var j = 0; j < bakedLength; j++) {
                        var p0 = posBuilder[j] = bakedPoints[j];
                        var p1 = j < bakedLength - 1 ? posBuilder[j + 1] : p0;

                        var fwd = math.normalizesafe(p1 - p0, new float3(0, 0, 1));
                        var up = new float3(0, 1, 0);
                        tanBuilder[j] = fwd;
                        rotBuilder[j] = quaternion.LookRotationSafe(fwd, up);

                        distance += math.distance(p0, p1);
                        disBuilder[j] = distance;
                    }
                }
            }

            return builder.CreateBlobAssetReference<BlobArray<PathwayBlob>>(Allocator.Persistent);
        }

        private NativeList<float3> CreateSpline(PathSerializable p, int i) {
            var bakedPoints = new NativeList<float3>(Allocator.Temp);

            for (var j = 0; j < p.paths[i].agentPoints.Length; j++) {
                var n = p.paths[i].agentPoints.Length;
                var nativePoints = new NativeArray<float3>(n, Allocator.Temp);

                for (var k = 0; k < n; k++) nativePoints[k] = p.paths[i].agentPoints[k].Position;

                switch (p.paths[i].bakingLineType) {
                    case EBakingLineType.Curved: {
                        using var bakedSection = Allocator.Temp.BakePoints(nativePoints, subdivision);
                        foreach (var point in bakedSection) bakedPoints.Add(point);

                        break;
                    }
                    case EBakingLineType.Straight:

                        if (j >= p.paths[i].agentPoints.Length - 1)
                            break;

                        for (var k = 0; k < p.paths[i].agentPoints.Length - 1; k += 2) {
                            var a = p.paths[i].agentPoints[k].Position;
                            var b = p.paths[i].agentPoints[k + 1].Position;

                            if (j == 0) bakedPoints.Add(a);

                            for (var l = 1; l <= subdivision; l++) {
                                var t = l / (float)subdivision;
                                bakedPoints.Add(math.lerp(a, b, t));
                            }
                        }

                        break;
                    default:

                        throw new ArgumentOutOfRangeException();
                }

                nativePoints.Dispose();
            }

            return bakedPoints;
        }

        [Serializable]
        public struct PathSerializable {

            public TEnum type;
            public PointSerializable[] paths;

        }

        [DisallowMultipleComponent]
        [RequireComponent(typeof(RandomAuthoring))]
        [RequireComponent(typeof(DestroyAuthoring))]
        public abstract class AgentMovementBase : MonoBehaviour {

            [SerializeField] protected AgentAttributes stats;

            private readonly T _pathwaysSingleton;
            public abstract TEnum AgentEnum { get; }

        }

    }

    public struct PathwayBlob {

        public BlobArray<byte> Id;
        public BlobArray<Color> Color;
        public BlobArray<float3> Origins;
        public BlobArray<SplineVectorBlob> Path;

    }

    public struct AgentPathwaysIData : IComponentData {

        public BlobAssetReference<BlobArray<PathwayBlob>> Pathways;

    }

    public struct AgentMovementIData : IComponentData {

        public byte PathwayId;
        public float Speed;

    }

}