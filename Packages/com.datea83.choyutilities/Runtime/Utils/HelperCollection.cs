using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace EugeneC.Utilities {

    public static partial class HelperCollection {

        /// <summary>
        ///     Different with normal remainder (x % y), this one will always return positive value
        /// </summary>
        public static float Modulo(float x, float y) {
            return (x % y + y) % y;
        }

        public static int Modulo(int x, int y) {
            return (x % y + y) % y;
        }

        public static float RandomValue(this Entity entity, double et) {
            var ran = Random.CreateFromIndex(((uint)entity.Index + (uint)et) << (4 + 1));

            return ran.NextFloat();
        }
        
        public static float RandomValue(this Entity entity, double et, float min, float max) {
            var ran = Random.CreateFromIndex(((uint)entity.Index + (uint)et) << (4 + 1));

            return ran.NextFloat(min, max);
        }
        
        public static int RandomValue(this Entity entity, double et, int min, int max) {
            var ran = Random.CreateFromIndex(((uint)entity.Index + (uint)et) << (4 + 1));

            return ran.NextInt(min, max);
        }

        public static float RandomValue(this Component obj) {
            var ran = Random.CreateFromIndex((uint)obj.GetInstanceID() + (uint)Environment.TickCount +
                                             (uint)Time.deltaTime);

            return ran.NextFloat();
        }

        public static float RandomValue(this Component obj, float min, float max) {
            var ran = Random.CreateFromIndex((uint)obj.GetInstanceID() + (uint)Environment.TickCount +
                                             (uint)Time.deltaTime);

            return ran.NextFloat(min, max);
        }

        public static int RandomValue(this Component obj, int min, int max) {
            var ran = Random.CreateFromIndex((uint)obj.GetInstanceID() + (uint)Environment.TickCount +
                                             (uint)Time.deltaTime);

            return ran.NextInt(min, max);
        }

        public static float2 RandomValue2(this Component obj) {
            var ran = Random.CreateFromIndex((uint)obj.GetInstanceID() + (uint)Environment.TickCount +
                                             (uint)Time.deltaTime);

            return ran.NextFloat2();
        }

        public static float3 RandomValue3(this Component obj) {
            var ran = Random.CreateFromIndex((uint)obj.GetInstanceID() + (uint)Environment.TickCount +
                                             (uint)Time.deltaTime);

            return ran.NextFloat3();
        }
        
        public static bool GetDistanceAndDot(this LocalTransform player,
            LocalTransform target,
            out float distanceSqr,
            out float dot) {
            var dir = player.Position - target.Position;
            distanceSqr = math.lengthsq(dir);
            dot = math.dot(player.Forward(), math.normalize(dir));

            return dot >= 0f;
        }

        public static bool GetDistanceAndDot(this LocalToWorld player,
            LocalToWorld target,
            out float distanceSqr,
            out float dot) {
            var dir = player.Position - target.Position;
            distanceSqr = math.lengthsq(dir);
            dot = math.dot(player.Forward, math.normalize(dir));

            return dot >= 0f;
        }
        
        public static bool GetDistanceAndDot(this LocalTransform player,
            LocalToWorld target,
            out float distanceSqr,
            out float dot) {
            var dir = player.Position - target.Position;
            distanceSqr = math.lengthsq(dir);
            dot = math.dot(player.Forward(), math.normalize(dir));

            return dot >= 0f;
        }
        
        public static bool GetDistanceAndDot(this LocalToWorld player,
            LocalTransform target,
            out float distanceSqr,
            out float dot) {
            var dir = player.Position - target.Position;
            distanceSqr = math.lengthsq(dir);
            dot = math.dot(player.Forward, math.normalize(dir));

            return dot >= 0f;
        }

        public static float3 GetClosestPointInSplineSegment(float3 lineStart, float3 lineEnd,
            float3 point, out float t) {
            t = 0f;
            var vec = lineEnd - lineStart;
            var lenSq = math.lengthsq(vec);

            if (lenSq <= 0f) return lineStart;

            t = math.clamp(math.dot(point - lineStart, vec) / lenSq, 0f, 1f);

            return lineStart + vec * t;
        }

        public static void SampleAtDistance(ref SplineVectorBlob spline, float targetDist,
            out float3 position, out quaternion rotation) {
            ref var posArr = ref spline.Position;
            ref var dstArr = ref spline.Distance;
            ref var rotArr = ref spline.Rotation;

            var count = posArr.Length;

            switch (count) {
                case 0:

                    position = default;
                    rotation = quaternion.identity;

                    return;
                case 1:

                    position = posArr[0];
                    rotation = rotArr[0];

                    return;
            }

            var idx = dstArr.LowerBound(targetDist);
            idx = math.clamp(idx, 0, count - 2);

            var d0 = dstArr[idx];
            var d1 = dstArr[idx + 1];
            var segLen = math.max(1e-6f, d1 - d0);
            var t = math.saturate((targetDist - d0) / segLen);

            var p0 = posArr[idx];
            var p1 = posArr[idx + 1];
            position = math.lerp(p0, p1, t);

            var r0 = rotArr[idx];
            var r1 = rotArr[idx + 1];
            rotation = math.slerp(r0, r1, t);
        }

    }

    public struct SplineVectorBlob {

        public BlobArray<float3> Position;
        public BlobArray<float> Distance;
        public BlobArray<quaternion> Rotation;
        public BlobArray<float3> Tangent;

    }

}