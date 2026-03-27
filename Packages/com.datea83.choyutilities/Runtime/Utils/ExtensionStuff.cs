using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.Utilities {

    public static partial class UtilityCollection {

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

        public static Quaternion RotateTowards(this Transform ob, float3 target, float speed) {
            var dir = math.normalize(target - (float3)ob.position);
            var lookTowards = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

            return Quaternion.Slerp(ob.rotation, lookTowards, Time.deltaTime * speed);
        }

        public static bool FinishRotate(this Transform ob, Vector3 target, float threshold = 5f) {
            var dir = (target - ob.position).normalized;
            var angle = Vector3.Angle(ob.forward, dir);

            return angle < threshold;
        }

        public static Transform FindNearestWaypoint(this List<Transform> posList, Transform currentPosition) {
            if (posList is null || currentPosition is null) return null;

            Transform nearest = null;
            var disToNearest = 0f;

            foreach (var pos in posList) {
                if (pos is null) continue;
                var distance = (currentPosition.position - pos.position).magnitude;

                if (nearest is not null && !(distance < disToNearest))
                    continue; //&& CurrentPosition.CanMoveThere(nearest))) 
                nearest = pos;
                disToNearest = distance;
            }

            return nearest;
        }

        public static Transform FindNearestWaypoint(this List<Transform> posList,
            Transform currentPosition,
            List<Transform> prevPos) {
            if (posList is null || currentPosition is null) return null;

            Transform nearest = null;
            var disToNearest = 0f;

            foreach (var pos in posList) {
                if (pos is null || prevPos is null) continue;
                if (prevPos.Contains(pos)) continue;
                var distance = (currentPosition.position - pos.position).magnitude;

                if (nearest is not null && !(distance < disToNearest))
                    continue; //&& CurrentPosition.CanMoveThere(nearest))) 
                nearest = pos;
                disToNearest = distance;
            }

            return nearest;
        }

        public static bool CanMoveThere(this Transform pos, float3 target, string tag) {
            var dir = math.normalize(target - (float3)pos.position);
            var ray = new Ray(pos.position, dir);

            if (!Physics.Raycast(ray, out var hitInfo)) return true;

            return !hitInfo.collider.CompareTag(tag);
        }

        public static Transform FindNearestEnemy(this GameObject bot, List<GameObject> objectList) {
            Transform target = null;
            var disToNearest = 0f;

            foreach (var potentialTarget in objectList) {
                if (potentialTarget == bot) continue;
                var distance = (bot.transform.position - potentialTarget.transform.position).magnitude;

                if (target is not null && !(distance < disToNearest)) continue;
                target = potentialTarget.transform;
                disToNearest = distance;
            }

            return target;
        }

        public static GameObject FindNearestObjectInRange(this Transform ob, List<GameObject> obList, float maxRange) {
            GameObject nearest = null;
            float distanceToNearest = 0;

            foreach (var spawned in obList) {
                if (spawned.transform == ob) continue;
                var distance = math.distance(ob.position, spawned.transform.position);

                if (!(distance <= maxRange)) continue;
                if (nearest is not null && !(distance < distanceToNearest)) continue;

                nearest = spawned;
                distanceToNearest = distance;
            }

            return nearest;
        }

    }

}