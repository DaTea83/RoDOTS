using Unity.Collections;
using Unity.Mathematics;

namespace EugeneC.Utilities {

    public static partial class UtilityCollection {

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

    }

}