using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace EugeneC.Utilities {

    public static partial class HelperCollection {
        
        // Time-constant style smoothing
        // -DeltaTime divide timeConstant, math.max just to avoid timeConstant is 0
        // More consistent interpolation with different frame rates
        public static float SmoothFactor(this float deltaTime, float timeConstant = 0.02f) {
            return 1f - math.exp(-deltaTime / math.max(1e-4f, timeConstant));
        }

        public static float3 GetNoiseOffsetPos(this float3 pos,
            float yOffset,
            float time,
            float height,
            float noiseScale,
            float depthOffset) {
            pos.y = height * noise.snoise(new float2(pos.x * noiseScale + time,
                pos.z * noiseScale + time)) + yOffset * depthOffset;

            return pos;
        }

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

    }

}