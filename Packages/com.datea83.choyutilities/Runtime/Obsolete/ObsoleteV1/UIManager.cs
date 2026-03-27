using System;
using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;

namespace EugeneC.Obsolete {

    [Obsolete]
    public class UIManager : GenericSingleton<UIManager> {

        public const float TimeOverChar = 0.075f;
        public UIObject[] uiObject;
        public Canvas MainCanvas;
        private readonly List<GameObject> OpenedUI = new();

        private readonly Dictionary<UIType, UIObject> UIDictionary = new();
        private RectTransform mainCanvasPos;

        protected override void Awake() {
            base.Awake();
            GetPrefab();
        }

        private void GetPrefab() {
            mainCanvasPos = (RectTransform)Instantiate(MainCanvas).transform;

            foreach (var prefab in uiObject) UIDictionary[prefab.UI_Id] = prefab;
        }

        private GameObject GeneratePrefab(UIType UIPrefab, Action OnDone) {
            GameObject newUI = null;

            if (UIDictionary.TryGetValue(UIPrefab, out var UItemplate))
                newUI = Instantiate(UItemplate.Prefab, mainCanvasPos);
            else
                throw new Exception(UIPrefab + " Couldn't be found");

            return newUI;
        }

        //Open
        public GameObject Open(UIType UIPrefab) {
            var newUI = GeneratePrefab(UIPrefab, null);
            if (newUI != null) OpenedUI.Add(newUI);

            return newUI;
        }

        //Open Replace
        public GameObject OpenReplace(UIType UIPrefab) {
            CloseAll();

            return Open(UIPrefab);
        }

        //Open Persist
        public GameObject OpenPersist(UIType UIPrefab) {
            return GeneratePrefab(UIPrefab, null);
        }

        //Close
        public void Close(GameObject UIPrefab) {
            OpenedUI.Remove(UIPrefab);
            Destroy(UIPrefab);
        }

        //Close All
        public void CloseAll() {
            foreach (var UIPrefab in OpenedUI.ToArray())
                Close(UIPrefab);
        }

    }

    [Serializable]
    public class UIObject {

        public UIType UI_Id;
        public GameObject Prefab;

    }

    public enum UIType {

        Start,
        BlankDark,
        BlankWhite,
        Bedroom

    }

}