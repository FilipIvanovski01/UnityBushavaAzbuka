#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// When only the new Input System is active, UI needs <see cref="InputSystemUIInputModule"/>.
/// Adds it if missing and disables <see cref="StandaloneInputModule"/> on the same EventSystem.
/// </summary>
public static class InputSystemUIEnsure
{
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        foreach (EventSystem es in Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
        {
            if (es == null)
                continue;

            if (es.GetComponent<InputSystemUIInputModule>() != null)
                continue;

            StandaloneInputModule standalone = es.GetComponent<StandaloneInputModule>();
            if (standalone != null)
                standalone.enabled = false;

            es.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }
#endif
}
