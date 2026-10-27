using UnityEngine;
using UnityEngine.SceneManagement;

public static class SettingsUIBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneListener()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AttachToCurrentScene()
    {
        AttachToScene(SceneManager.GetActiveScene());
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachToScene(scene);
    }

    private static void AttachToScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            Transform[] transforms = roots[i].GetComponentsInChildren<Transform>(true);
            for (int j = 0; j < transforms.Length; j++)
            {
                if (transforms[j].name == "SettingsPanel" &&
                    transforms[j] is RectTransform &&
                    transforms[j].GetComponent<SettingsUIController>() == null)
                {
                    transforms[j].gameObject.AddComponent<SettingsUIController>();
                }
            }
        }
    }
}
