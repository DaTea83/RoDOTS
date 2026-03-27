#if UNITY_2023_1_OR_NEWER
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace EugeneC.Utilities {

    public static partial class UtilityCollection {

        private const string TaskCancellationMessage = "Task was cancelled";

        public static async Awaitable AwaitableUntil(this CancellationToken cancellationToken, Func<bool> condition) {
            while (!condition()) await Awaitable.NextFrameAsync(cancellationToken);
        }

        public static async Awaitable AwaitableUntil(this CancellationToken cancellationToken,
            Func<bool> condition,
            Action onWaitDone) {
            while (!condition()) {
                onWaitDone?.Invoke();
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        public static async Awaitable RotateObjectAsync(this CancellationToken token,
            GameObject obj,
            Vector3 rotateTo,
            float rotateDuration,
            Action onDone = null) {
            try {
                if (obj is null) return;

                var time = 0f;
                var startRot = obj.transform.rotation;
                var endRot = Quaternion.Euler(rotateTo);

                while (time <= rotateDuration) {
                    time += Time.unscaledDeltaTime;
                    obj.transform.rotation = Quaternion.Slerp(startRot, endRot, time / rotateDuration);
                    await Awaitable.EndOfFrameAsync(token);
                }

                obj.transform.rotation = endRot;
                onDone?.Invoke();
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable<bool> RotateObjectAsync(this CancellationToken token,
            Transform obj,
            float3 rotateTo,
            float rotateDuration,
            Action onDone = null) {
            try {
                if (obj is null) return false;

                var time = 0f;
                quaternion startRot = obj.transform.rotation;
                var endRot = quaternion.Euler(rotateTo);

                while (time <= rotateDuration) {
                    time += Time.unscaledDeltaTime;
                    obj.transform.rotation = math.slerp(startRot, endRot, time / rotateDuration);
                    await Awaitable.EndOfFrameAsync(token);
                }

                if (token.IsCancellationRequested) return false;
                obj.transform.rotation = endRot;
                onDone?.Invoke();

                return true;
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable ScaleObjectAsync(this CancellationToken token,
            GameObject obj,
            Vector3 scaleTo,
            float scalingDuration,
            Action onDone = null) {
            try {
                if (obj is null) return;

                var time = 0f;
                var startScale = obj.transform.localScale;

                while (time <= scalingDuration) {
                    time += Time.unscaledDeltaTime;
                    obj.transform.localScale = Vector3.Lerp(startScale, scaleTo, time / scalingDuration);
                    await Awaitable.EndOfFrameAsync(token);
                }

                obj.transform.localScale = scaleTo;
                onDone?.Invoke();
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable DialogueAsync(this CancellationToken token,
            List<string> dialogueList,
            float dialogueDuration,
            Action<string> displayTo,
            float timePerChar,
            Action onDone = null) {
            try {
                if (dialogueList is null || dialogueList.Count == 0) return;

                foreach (var line in dialogueList) {
                    if (line == string.Empty) continue;

                    var timer = 0f;
                    var currentDisplaying = "";

                    while (currentDisplaying != line) {
                        timer += Time.unscaledDeltaTime;
                        var length = (int)math.ceil(timer / timePerChar);
                        length = math.clamp(length, 0, line.Length);
                        currentDisplaying = line[..length];
                        displayTo(currentDisplaying);

                        await Awaitable.NextFrameAsync(token);
                    }

                    await Awaitable.WaitForSecondsAsync(dialogueDuration, token);
                }

                displayTo("");
                onDone?.Invoke();
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable DialogueAsync(this CancellationToken token,
            string[] dialogueList,
            float dialogueDuration,
            Action<string> displayTo,
            float timePerChar = 0.05f,
            Action onDone = null) {
            try {
                if (dialogueList is null || dialogueList.Length == 0) return;

                foreach (var line in dialogueList) {
                    if (line == string.Empty) continue;

                    var timer = 0f;
                    var currentDisplaying = "";

                    while (currentDisplaying != line) {
                        timer += Time.unscaledDeltaTime;
                        var length = (int)math.ceil(timer / timePerChar);
                        length = math.clamp(length, 0, line.Length);
                        currentDisplaying = line[..length];
                        displayTo(currentDisplaying);

                        await Awaitable.NextFrameAsync(token);
                    }

                    await Awaitable.WaitForSecondsAsync(dialogueDuration, token);
                }

                displayTo("");
                onDone?.Invoke();
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable RollRightAngleAsync(this CancellationToken token,
            Transform ob,
            float rollSpeed,
            Vector3 dir,
            float rollCooldown = 0.1f) {
            try {
                var anchor = ob.position + (Vector3.down + dir) * 0.5f;
                var axis = Vector3.Cross(Vector3.up, dir);

                for (var i = 0; i <= 90 / rollSpeed; i++) {
                    ob.RotateAround(anchor, axis, i);
                    await Awaitable.WaitForSecondsAsync(rollCooldown, token);
                }
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable TimeScaleAsync(this CancellationToken token,
            float targetScale,
            float loadDuration = 2f,
            Action onDone = null) {
            try {
                var currentScale = Time.timeScale;
                float unscaledTimer = 0;

                while (unscaledTimer <= loadDuration) {
                    unscaledTimer += Time.unscaledDeltaTime;
                    var t = math.lerp(currentScale, targetScale, unscaledTimer / loadDuration);
                    Time.timeScale = t;
                    await Awaitable.EndOfFrameAsync(token);
                }

                onDone?.Invoke();
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        #region Fade Screen Async

        public enum EFadeType : byte {

            FadeIn = 0,
            FadeOut = 1

        }

        public static async Awaitable FadeScreenAsync(this CancellationToken token,
            Image fadeImage,
            bool isFadein,
            float loadDuration,
            Action onDone = null) {
            try {
                if (fadeImage is null) return;

                var targetAlpha = isFadein ? 1f : 0f;
                var currentAlpha = fadeImage.color.a;
                float time = 0;

                while (time <= loadDuration) {
                    time += Time.unscaledDeltaTime;
                    var alpha = Mathf.Lerp(currentAlpha, targetAlpha, time / loadDuration);
                    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                    await Awaitable.EndOfFrameAsync(token);
                }

                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
                onDone?.Invoke();
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable<bool> FadeScreenAsync(this CancellationToken token,
            Image fadeImage,
            EFadeType fadeType,
            float loadDuration,
            float dt,
            Action onDone = null) {
            try {
                if (fadeImage is null) return false;

                var targetAlpha = fadeType switch {
                    EFadeType.FadeOut => 0,
                    EFadeType.FadeIn => 1,
                    _ => fadeImage.color.a
                };

                var currentAlpha = fadeImage.color.a;
                float time = 0;

                while (time <= loadDuration) {
                    time += dt;
                    var alpha = math.lerp(currentAlpha, targetAlpha, time / loadDuration);
                    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                    await Awaitable.EndOfFrameAsync(token);
                }

                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
                onDone?.Invoke();

                return true;
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable<bool> FadeScreenAsync(this CancellationToken token,
            CanvasGroup cg,
            EFadeType fadeType,
            float loadDuration,
            float dt,
            Action onDone = null) {
            try {
                if (cg is null) return false;

                var targetAlpha = fadeType switch {
                    EFadeType.FadeOut => 0,
                    EFadeType.FadeIn => 1,
                    _ => cg.alpha
                };
                var currentAlpha = cg.alpha;
                var time = 0f;

                while (time <= loadDuration) {
                    time += dt;
                    var alpha = math.lerp(currentAlpha, targetAlpha, time / loadDuration);
                    cg.alpha = alpha;
                    await Awaitable.EndOfFrameAsync(token);
                }

                cg.alpha = targetAlpha;
                onDone?.Invoke();

                return true;
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        #endregion

        #region Change Color Async

        public static async Awaitable<bool> ChangeColorAsync(this CancellationToken token,
            Image start,
            Color target,
            float loadDuration,
            Action onDone = null) {
            try {
                if (start is null) return false;

                var current = start.color;
                float time = 0;

                while (time <= loadDuration) {
                    time += Time.unscaledDeltaTime;
                    var col = Color.Lerp(current, target, time / loadDuration);
                    start.color = col;
                    await Awaitable.EndOfFrameAsync(token);
                }

                if (token.IsCancellationRequested) return false;
                start.color = target;
                onDone?.Invoke();

                return true;
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable<Color> ChangeColorAsync(this CancellationToken token,
            float4 start,
            float4 target,
            float loadDuration,
            Action onDone = null) {
            try {
                float time = 0;

                while (time <= loadDuration) {
                    time += Time.unscaledDeltaTime;
                    start = math.lerp(start, target, time / loadDuration);
                    await Awaitable.EndOfFrameAsync(token);
                }

                onDone?.Invoke();

                return new Color(start.x, start.y, start.z, start.w);
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable<Color> ChangeColorAsync(this CancellationToken token,
            Color start,
            Color target,
            float loadDuration,
            Action onDone = null) {
            try {
                float time = 0;
                var startColor = new float4(start.r, start.g, start.b, start.a);
                var targetColor = new float4(target.r, target.g, target.b, target.a);
                var newColor = new float4();

                while (time <= loadDuration) {
                    time += Time.unscaledDeltaTime;
                    newColor = math.lerp(startColor, targetColor, time / loadDuration);
                    await Awaitable.EndOfFrameAsync(token);
                }

                onDone?.Invoke();

                return new Color(newColor.x, newColor.y, newColor.z, newColor.w);
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable<Color> ChangeColorAsync(this CancellationToken token,
            Color start,
            Color target,
            float loadDuration,
            float dt,
            Action onDone = null) {
            try {
                float time = 0;
                var startColor = new float4(start.r, start.g, start.b, start.a);
                var targetColor = new float4(target.r, target.g, target.b, target.a);
                var newColor = new float4();

                while (time <= loadDuration) {
                    time += dt;
                    newColor = math.lerp(startColor, targetColor, time / loadDuration);
                    await Awaitable.EndOfFrameAsync(token);
                }

                onDone?.Invoke();

                return new Color(newColor.x, newColor.y, newColor.z, newColor.w);
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        #endregion

        #region Move Transform Async

        public static async Awaitable<bool> MoveAsync(this CancellationToken token,
            GameObject obj,
            Transform targetPos,
            float moveDuration,
            Action onDone = null) {
            try {
                if (obj is null || targetPos is null) return false;

                var time = 0f;
                var startPos = obj.transform.position;
                var endPos = targetPos.position;

                while (time <= moveDuration) {
                    time += Time.deltaTime;
                    Vector3 pos = math.lerp(startPos, endPos, time / moveDuration);
                    obj.transform.position = pos;
                    await Awaitable.EndOfFrameAsync(token);
                }

                if (token.IsCancellationRequested) return false;
                obj.transform.position = endPos;
                onDone?.Invoke();

                return true;
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable<bool> MoveAsync(this CancellationToken token,
            RectTransform obj,
            float3 target,
            float duration,
            Action onDone = null) {
            try {
                if (obj is null) return false;
                var time = 0f;
                var start = (float3)obj.anchoredPosition3D;

                while (time <= duration) {
                    time += Time.unscaledDeltaTime;
                    obj.anchoredPosition3D = math.lerp(start, target, time / duration);
                    await Awaitable.NextFrameAsync(token);
                }

                if (token.IsCancellationRequested) return false;
                onDone?.Invoke();

                return true;
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        public static async Awaitable<bool> MoveAsync(this CancellationToken token,
            GameObject obj,
            float3 targetPos,
            float moveDuration,
            Action onDone = null) {
            try {
                if (obj is null) return false;

                float time = 0;
                var startPos = (float3)obj.transform.position;

                while (time <= moveDuration) {
                    time += Time.unscaledDeltaTime;
                    var pos = math.lerp(startPos, targetPos, time / moveDuration);
                    obj.transform.position = pos;
                    await Awaitable.EndOfFrameAsync(token);
                }

                if (token.IsCancellationRequested) return false;
                obj.transform.position = targetPos;
                onDone?.Invoke();

                return true;
            }
            catch {
                throw new Exception(TaskCancellationMessage);
            }
        }

        #endregion

    }

}
#endif