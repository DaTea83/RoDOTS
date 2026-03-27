using Unity.Burst;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace EugeneC.Editor {

#if UNITY_EDITOR
    /// <summary>
    ///     Static class to check to see if Burst compilation is enabled in the editor.
    /// </summary>
    /// <remarks>
    ///     If Burst is disabled, a warning will be printed in the editor, giving the user more information on how to enable.
    /// </remarks>
    internal static class BurstCompilationChecker {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            if (!BurstCompiler.Options.EnableBurstCompilation)
                Debug.LogWarning(
                    "Burst compilation is not enabled and performance is expected to be degraded. Enable this at Preference > Jobs > Burst > Enable Compilation.");
        }

    }
#endif

}