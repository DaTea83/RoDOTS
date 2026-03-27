using System;
using System.Collections.Generic;
using EugeneC.Singleton;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ObsoleteV2 {

    [Obsolete]
    public abstract class GenericParticleManager<TEnum, TMono> : GenericSingleton<TMono>
        where TEnum : Enum
        where TMono : MonoBehaviour {

        [SerializeField] protected ParticleSerialize[] particleEffects;

        [Tooltip("The pool count meant each particle serialize, meaning total = poolCount * particleEffects.Length")]
        [SerializeField]
        protected byte poolCount = 16;

        protected SystemSerialize[] ParticleSystems;
        protected List<int> PauseIndexes;

        protected virtual async void Start() {
            try {
                await Awaitable.WaitForSecondsAsync(.1f, Token);

                ParticleSystems = new SystemSerialize[particleEffects.Length];
                var currentSystem = 0;

                foreach (var particle in particleEffects) {
                    ParticleSystems[currentSystem].particles = new ParticleSystem[poolCount];

                    for (var i = 0; i < poolCount; i++) {
                        if (particle.particle is null) continue;
                        var spawn = Instantiate(particle.particle, transform);
                        ParticleSystems[currentSystem].particles[i] = spawn;
                    }

                    currentSystem++;
                }
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public virtual void PlayEffectAtPosition(TEnum id, float3 position) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return;
            var index = Array.FindIndex(particleEffects, i => EqualityComparer<TEnum>.Default.Equals(i.id, id));

            var particle = ParticleSystems[index].particles[ParticleSystems[index].currentIndex];
            particle.transform.position = position;
            particle.Play();

            ParticleSystems[index].previousIndex = ParticleSystems[index].currentIndex;
            ParticleSystems[index].currentIndex++;
            ParticleSystems[index].currentIndex %= ParticleSystems[index].particles.Length;
        }

        public virtual void PauseAllEffects() {
            PauseIndexes = new List<int>();

            for (var i = 0; i < ParticleSystems.Length; i++) {
                var system = ParticleSystems[i];

                foreach (var p in system.particles)
                    if (p.isPlaying)
                        p.Pause();

                PauseIndexes.Add(i);
            }
        }

        public virtual void ResumeAllEffects() {
            if (PauseIndexes is null) return;

            foreach (var i in PauseIndexes) {
                var system = ParticleSystems[i];

                foreach (var p in system.particles)
                    if (p.isPaused)
                        p.Play();
            }

            PauseIndexes.Clear();
        }

        [Serializable]
        public struct ParticleSerialize {

            public ParticleSystem particle;
            public TEnum id;

        }

        [Serializable]
        protected struct SystemSerialize {

            public ParticleSystem[] particles;
            public int currentIndex;
            public int previousIndex;

        }

    }

}