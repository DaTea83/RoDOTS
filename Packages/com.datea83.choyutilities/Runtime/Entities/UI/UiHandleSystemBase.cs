using System;
using Unity.Entities;

namespace EugeneC.ECS {

    //TODO
    [UpdateInGroup(typeof(Eu_EffectSystemGroup))]
    public partial class UiHandleSystemBase : SystemBase {

        public event Action<byte, byte> OnUiEnterHover;
        public event Action<byte, byte> OnUiExitHover;
        public event Action<byte, byte> OnUiClicked;

        protected override void OnCreate() { }

        protected override void OnUpdate() { }

    }

}