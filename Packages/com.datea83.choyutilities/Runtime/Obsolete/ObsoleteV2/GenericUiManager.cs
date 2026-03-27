using System;
using System.Collections.Generic;
using EugeneC.Mono;
using EugeneC.Singleton;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ObsoleteV2 {

    [Obsolete]
    public abstract class GenericUiManager<TEnum, TMono> : GenericSingleton<TMono>
        where TEnum : Enum
        where TMono : MonoBehaviour {

        [SerializeField] protected Canvas canvasRef;
        [SerializeField] protected UiSerialize[] uiElements;
        protected readonly List<UiHelper> OpenedUi = new();
        protected RectTransform CanvasPos;
        protected bool IsTransitioning;

        protected UiHelper[] UiObjects;

        protected virtual async void Start() {
            try {
                await Awaitable.NextFrameAsync(Token);

                if (canvasRef is null) return;
                CanvasPos = (RectTransform)canvasRef.transform;

                UiObjects = new UiHelper[uiElements.Length];

                for (var i = 0; i < uiElements.Length; i++) {
                    if (uiElements[i].prefab is null) continue;
                    var spawn = Instantiate(uiElements[i].prefab, CanvasPos);
                    spawn.OnSpawn();
                    UiObjects[i] = spawn;
                    spawn.gameObject.SetActive(false);
                }
            }
            catch (Exception e) {
                print(e);
            }
        }

        protected override async void OnDisable() {
            try {
                await CloseAll();
                base.OnDisable();
            }
            catch (Exception e) {
                print(e);
            }
        }

        public event Action OnOpenUi;
        public event Action OnCloseUi;

        public virtual async Awaitable<(UiHelper, bool)> Open(TEnum id) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return (null, false);
            var index = Array.FindIndex(uiElements, i => EqualityComparer<TEnum>.Default.Equals(i.id, id));

            IsTransitioning = true;
            var newUi = UiObjects[index];
            newUi.gameObject.SetActive(true);
            OpenedUi.Add(newUi);

            OnOpenUi?.Invoke();
            var t = newUi.OnStartOpen();
            await Awaitable.WaitForSecondsAsync(math.abs(t), Token);
            newUi.OnEndOpen();
            IsTransitioning = false;

            return (newUi, true);
        }

        public virtual async Awaitable<(UiHelper, bool)> Close(TEnum id, float time) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return (null, false);
            var index = Array.FindIndex(uiElements, i => EqualityComparer<TEnum>.Default.Equals(i.id, id));

            IsTransitioning = true;
            var newUi = UiObjects[index];

            OnCloseUi?.Invoke();
            var t = newUi.OnStartClose();
            await Awaitable.WaitForSecondsAsync(math.abs(t), Token);
            newUi.OnEndClose();

            await Awaitable.NextFrameAsync();
            newUi.gameObject.SetActive(false);
            OpenedUi.Remove(newUi);
            IsTransitioning = false;

            return (newUi, true);
        }

        public virtual async Awaitable<bool> CloseAll() {
            IsTransitioning = true;
            var i = 0f;

            foreach (var ui in OpenedUi) {
                OnCloseUi?.Invoke();
                var t = ui.OnStartClose();
                //Get the highest value and delay the said value
                i = i < t ? t : i;
            }

            await Awaitable.WaitForSecondsAsync(i, Token);

            foreach (var ui in OpenedUi) {
                ui.OnEndClose();
                await Awaitable.NextFrameAsync(Token);
                ui.gameObject.SetActive(false);
            }

            IsTransitioning = false;
            OpenedUi.Clear();

            return true;
        }

        public virtual async Awaitable<bool> Replace(TEnum id) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return false;
            var c = await CloseAll();

            if (!c) return false;
            await Open(id);

            return true;
        }

        [Serializable]
        public struct UiSerialize {

            public UiHelper prefab;
            public TEnum id;

        }

    }

}