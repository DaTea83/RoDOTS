using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace EugeneC.Utilities {

    public static partial class HelperCollection {

        /// <summary>
        ///     Bake the spline using Barry–Goldman algorithm
        ///     Or formula of centripetal Catmull–Rom spline
        /// </summary>
        /// <remarks>
        ///     https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline
        /// </remarks>
        public static NativeList<float3> BakePoints(this Allocator allocator,
            NativeArray<float3> points,
            byte subdivisions = 100) {
            var baked = new NativeList<float3>(allocator);
            baked.Add(points[0]);

            for (var i = 0; i < points.Length; i++) {
                var p0 = GetPointAt(i - 1);
                var p1 = GetPointAt(i);
                var p2 = GetPointAt(i + 1);
                var p3 = GetPointAt(i + 2);

                float t0 = 0;
                var t1 = t0 + math.distance(p0, p1);
                var t2 = t1 + math.distance(p1, p2);
                var t3 = t2 + math.distance(p2, p3);

                for (var j = 0; j < subdivisions; j += 1) {
                    var t = math.lerp(t1, t2, (1 + j) / (float)subdivisions);

                    var a1 = ((t1 - t) * p0 + (t - t0) * p1) / (t1 - t0);
                    var a2 = ((t2 - t) * p1 + (t - t1) * p2) / (t2 - t1);
                    var a3 = ((t3 - t) * p2 + (t - t2) * p3) / (t3 - t2);

                    var b1 = ((t2 - t) * a1 + (t - t0) * a2) / (t2 - t0);
                    var b2 = ((t3 - t) * a2 + (t - t1) * a3) / (t3 - t1);

                    var c = ((t2 - t) * b1 + (t - t1) * b2) / (t2 - t1);

                    baked.Add(c);
                }
            }

            return baked;

            float3 GetPointAt(int index) {
                index %= points.Length;
                if (index < 0) index += points.Length;

                return points[index];
            }
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