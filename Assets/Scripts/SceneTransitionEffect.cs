using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionEffect
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
