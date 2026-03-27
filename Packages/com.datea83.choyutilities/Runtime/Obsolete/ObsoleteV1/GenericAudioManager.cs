using System;
using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace EugeneC.Obsolete {

    [Obsolete]
    public abstract class GenericAudioManager<T> : GenericSingleton<GenericAudioManager<T>>
        where T : Enum {

        public AudioSource sourcePrefab;
        public AudioSfx<T>[] audiosfx;

        private readonly Dictionary<T, AudioSfx<T>> _audioDictionary = new();

        protected override void Awake() {
            base.Awake();

            foreach (var sfx in audiosfx)
                _audioDictionary[sfx.audioId] = sfx;
        }

        public void PlaySourceClip(T audioId, AudioMixerGroup channel, Vector3 pos) {
            if (_audioDictionary.TryGetValue(audioId, out var sfx)) {
                var source = Instantiate(sourcePrefab, pos, Quaternion.identity);
                var key = Random.Range(0, sfx.audioClip.Length);
                var c = sfx.audioClip[key];
                source.outputAudioMixerGroup = channel;
                source.PlayOneShot(c);
                Destroy(source.gameObject, c.length);
            }
        }

        public void PlayClip(T audioId, AudioSource source) {
            if (_audioDictionary.TryGetValue(audioId, out var sfx)) {
                var key = Random.Range(0, sfx.audioClip.Length);
                var c = sfx.audioClip[key];
                source.PlayOneShot(c);
            }
        }

        public void PlayLoopClip(T audioId, AudioSource source) {
            source.loop = true;
            PlayClip(audioId, source);
        }

        public void PlayIgnorePauseClip(T audioId, AudioSource source) {
            source.ignoreListenerPause = true;
            PlayClip(audioId, source);
        }

        public void PlayOverrideClip(T audioId, AudioSource source) {
            source.loop = false;
            source.Stop();
            PlayIgnorePauseClip(audioId, source);
        }

    }

    [Serializable]
    public struct AudioSfx<T>
        where T : Enum {

        public T audioId;
        public AudioClip[] audioClip;

    }

}