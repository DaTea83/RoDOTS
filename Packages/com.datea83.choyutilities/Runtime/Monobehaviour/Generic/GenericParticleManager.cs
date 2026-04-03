using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace EugeneC.Singleton {

    public abstract class GenericParticleManager<TEnum, TMono> : GenericPoolingManager<TEnum, ParticleSystem, TMono>
        where TEnum : struct, Enum
        where TMono : MonoBehaviour {

        protected override void Awake() {
            base.Awake();

            foreach (var p in poolPrefabs) {
                var particle = p.prefab;
                var particleMain = particle.main;
                particleMain.playOnAwake = false;
            }
        }

        public virtual void PlayEffectAtPosition(TEnum id, float3 position) {
            var index = GetPoolIndex(id);

            if (index == -1) return;

            var particle = RuntimePools[index].spawn[RuntimePools[index].currentIndex];
            particle.transform.position = position;
            particle.Play();

            RuntimePools[index].previousIndex = RuntimePools[index].currentIndex;
            RuntimePools[index].currentIndex++;
            RuntimePools[index].currentIndex %= RuntimePools[index].spawn.Length;
        }

        public virtual void PauseAllEffects() {
            PauseIndexes = ListPool<int>.Get();

            for (var i = 0; i < RuntimePools.Length; i++) {
                var system = RuntimePools[i];

                foreach (var p in system.spawn)
                    if (p.isPlaying)
                        p.Pause();
                PauseIndexes.Add(i);
            }
        }

        public virtual void ResumeAllEffects() {
            if (PauseIndexes is null) return;

            foreach (var i in PauseIndexes) {
                var system = RuntimePools[i];

                foreach (var p in system.spawn)
                    if (p.isPaused)
                        p.Play();
            }

            ListPool<int>.Release(PauseIndexes);
        }

    }

}