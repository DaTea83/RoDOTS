using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace EugeneC.Utilities {

    public static partial class HelperCollection {

        public static IEnumerator FadeScreenCoroutine(this Image fadeImage,
            bool isFadein,
            float loadDuration,
            Action onDone = null) {
            if (fadeImage is null) yield break;

            var targetAlpha = isFadein ? 1f : 0f;
            var currentAlpha = fadeImage.color.a;
            float time = 0;

            while (time <= loadDuration) {
                time += Time.unscaledDeltaTime;
                var alpha = math.lerp(currentAlpha, targetAlpha, time / loadDuration);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);

                yield return null;
            }

            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
            onDone?.Invoke();
        }

        public static IEnumerator ChangeColorCoroutine(this Image image,
            Color target,
            float loadDuration,
            Action onDone = null) {
            if (image is null) yield break;

            var currentColor = image.color;
            float time = 0;

            while (time <= loadDuration) {
                time += Time.unscaledDeltaTime;
                var col = Color.Lerp(currentColor, target, time / loadDuration);
                currentColor = col;
            }

            image.color = target;
            onDone?.Invoke();
        }

        public static IEnumerator MoveTransformCoroutine(this GameObject obj,
            Transform targetPos,
            float moveDuration,
            Action onDone = null) {
            if (obj is null || targetPos is null) yield break;

            float time = 0;
            var startPos = obj.transform.position;
            var endPos = targetPos.position;

            while (time < moveDuration) {
                time += Time.unscaledDeltaTime;
                var pos = math.lerp(startPos, endPos, time / moveDuration);
                obj.transform.position = pos;

                yield return null;
            }

            obj.transform.position = endPos;
            onDone?.Invoke();
        }

        public static IEnumerator MoveVectorCoroutine(this GameObject obj,
            Vector3 targetPos,
            float moveDuration,
            Action onDone = null) {
            if (obj is null) yield break;

            float time = 0;
            var startPos = obj.transform.position;
            var endPos = targetPos;

            while (time < moveDuration) {
                time += Time.unscaledDeltaTime;
                var pos = math.lerp(startPos, endPos, time / moveDuration);
                obj.transform.position = pos;

                yield return null;
            }

            obj.transform.position = endPos;
            onDone?.Invoke();
        }

        public static IEnumerator RotateObjectCoroutine(this GameObject obj,
            Vector3 rotateTo,
            float rotateDuration,
            Action onDone = null) {
            if (obj is null) yield break;

            var time = 0f;
            var startRot = obj.transform.rotation;
            var endRot = Quaternion.Euler(rotateTo);

            while (time < rotateDuration) {
                time += Time.unscaledDeltaTime;
                obj.transform.rotation = Quaternion.Slerp(startRot, endRot, time / rotateDuration);

                yield return null;
            }

            obj.transform.rotation = endRot; // Ensure final rotation is set
            onDone?.Invoke();
        }

        public static IEnumerator ScaleObjectCoroutine(this GameObject obj,
            Vector3 scaleTo,
            float scalingDuration,
            Action onDone = null) {
            if (obj is null) yield break;
            var time = 0f;
            var startScale = obj.transform.localScale;
            var endScale = scaleTo;

            while (time < scalingDuration) {
                time += Time.unscaledDeltaTime;
                obj.transform.localScale = Vector3.Lerp(startScale, endScale, time / scalingDuration);

                yield return null;
            }

            obj.transform.localScale = endScale;
            onDone?.Invoke();
        }

        public static IEnumerator DialogueCoroutine(this List<string> dialogueList,
            float dialogueDuration,
            Action<string> displayTo,
            float timePerChar,
            Action onDone = null) {
            if (dialogueList == null || dialogueList.Count == 0) yield break;

            foreach (var line in dialogueList) {
                if (line == string.Empty) continue;

                var timer = 0f;
                var textToDisplay = line;
                var currentDisplaying = "";

                while (currentDisplaying != textToDisplay) {
                    timer += Time.unscaledDeltaTime;
                    var length = Mathf.CeilToInt(timer / timePerChar);
                    length = Mathf.Clamp(length, 0, textToDisplay.Length);
                    currentDisplaying = textToDisplay.Substring(0, length);
                    displayTo(currentDisplaying);

                    yield return null;
                }

                yield return new WaitForSeconds(dialogueDuration);
            }

            displayTo("");
            onDone?.Invoke();
        }

        public static IEnumerator RollRightAngleCoroutine(this Transform ob,
            float rollspeed,
            Vector3 dir,
            float rollCd = .1f) {
            var anchor = ob.position + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);

            for (var i = 0; i <= 90 / rollspeed; i++) {
                ob.RotateAround(anchor, axis, i);

                yield return new WaitForSeconds(rollCd);
            }
        }

        public static IEnumerator TimeScaleCoroutine(this float targetScale,
            float loadDuration = 2f,
            Action onDone = null) {
            var currentScale = Time.timeScale;
            float unscaledTimer = 0;

            yield return new WaitForEndOfFrame();

            while (unscaledTimer <= loadDuration) {
                unscaledTimer += Time.unscaledDeltaTime;
                var t = Mathf.Lerp(currentScale, targetScale, unscaledTimer / loadDuration);
                Time.timeScale = t;

                yield return new WaitForEndOfFrame();
            }

            onDone?.Invoke();
        }

    }

}