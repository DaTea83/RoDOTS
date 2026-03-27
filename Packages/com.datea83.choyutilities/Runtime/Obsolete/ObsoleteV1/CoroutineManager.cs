using System;
using System.Collections;
using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace EugeneC.Obsolete {

    [Obsolete]
    public class CoroutineManager : GenericSingleton<CoroutineManager> {

        public static IEnumerator FadeScreenCoroutine(bool isFadein,
            Image fadeImage,
            float LoadDuration,
            Action OnDone = null) {
            if (fadeImage == null) yield break;

            var targetAlpha = isFadein ? 1f : 0f;
            var currentAlpha = fadeImage.color.a;
            float time = 0;

            while (time <= LoadDuration) {
                time += Time.unscaledDeltaTime;
                var alpha_ = Mathf.Lerp(currentAlpha, targetAlpha, time / LoadDuration);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha_);

                yield return null;
            }

            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
            OnDone?.Invoke();
        }

        public static IEnumerator ChangeColorCoroutine(Image _image,
            Color target,
            float LoadDuration,
            Action OnDone = null) {
            if (_image == null) yield break;

            var CurrentColor = _image.color;
            float time = 0;

            while (time <= LoadDuration) {
                time += Time.unscaledDeltaTime;
                var col = Color.Lerp(CurrentColor, target, time / LoadDuration);
                CurrentColor = col;
            }

            _image.color = target;
            OnDone?.Invoke();
        }

        public static IEnumerator MoveTransformCoroutine(GameObject obj,
            Transform targetPos,
            float moveDuration,
            Action onDone = null) {
            if (obj == null || targetPos == null) yield break;

            float time = 0;
            var startPos = obj.transform.position;
            var endPos = targetPos.position;

            while (time < moveDuration) {
                time += Time.unscaledDeltaTime;
                var Pos = Vector3.Lerp(startPos, endPos, time / moveDuration);
                obj.transform.position = Pos;

                yield return null;
            }

            obj.transform.position = endPos;
            onDone?.Invoke();
        }

        public static IEnumerator MoveVectorCoroutine(GameObject obj,
            Vector3 targetPos,
            float moveDuration,
            Action onDone = null) {
            if (obj == null || targetPos == null) yield break;

            float time = 0;
            var startPos = obj.transform.position;
            var endPos = targetPos;

            while (time < moveDuration) {
                time += Time.unscaledDeltaTime;
                var Pos = Vector3.Lerp(startPos, endPos, time / moveDuration);
                obj.transform.position = Pos;

                yield return null;
            }

            obj.transform.position = endPos;
            onDone?.Invoke();
        }

        public static IEnumerator RotateObjectCoroutine(GameObject obj,
            Vector3 rotateTo,
            float rotateDuration,
            Action onDone = null) {
            if (obj == null) yield break;

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

        public static IEnumerator ScaleObjectCoroutine(GameObject obj,
            Vector3 scaleTo,
            float scalingduration,
            Action onDone = null) {
            if (obj == null) yield break;
            var time = 0f;
            var StartScale = obj.transform.localScale;
            var EndScale = scaleTo;

            while (time < scalingduration) {
                time += Time.unscaledDeltaTime;
                obj.transform.localScale = Vector3.Lerp(StartScale, EndScale, time / scalingduration);

                yield return null;
            }

            obj.transform.localScale = EndScale;
            onDone?.Invoke();
        }

        public static IEnumerator DialogueCoroutine(List<string> dialogueList,
            float dialogueDuration,
            Action<string> displayTo,
            float timePerChar,
            Action onDone = null) {
            if (dialogueList == null || displayTo == null) yield break;

            foreach (var line in dialogueList) {
                if (line == null) continue;

                var timer = 0f;
                var textToDisplay = line;
                var currentDisplaying = "";

                while (currentDisplaying != textToDisplay) {
                    timer += Time.deltaTime;
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

        public static IEnumerator RollRightAngleCoroutine(Transform ob,
            float rollspeed,
            Vector3 dir,
            float rollcooldown = .1f) {
            var anchor = ob.position + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);

            for (var i = 0; i <= 90 / rollspeed; i++) {
                ob.RotateAround(anchor, axis, i);

                yield return new WaitForSeconds(rollcooldown);
            }
        }

        public static IEnumerator TimeScaleCoroutine(float TargetScale = 0f,
            float LoadDuration = 2f,
            Action OnDone = null) {
            var CurrentScale = Time.timeScale;
            float unscaledTimer = 0;

            yield return new WaitForEndOfFrame();

            while (unscaledTimer <= LoadDuration) {
                unscaledTimer += Time.unscaledDeltaTime;
                var t = Mathf.Lerp(CurrentScale, TargetScale, unscaledTimer / LoadDuration);
                Time.timeScale = t;

                yield return new WaitForEndOfFrame();
            }

            OnDone?.Invoke();
        }

    }

}