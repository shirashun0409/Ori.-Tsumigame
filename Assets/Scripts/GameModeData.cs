using UnityEngine;

public static class GameModeData
{
    // 次にGameSceneで使うゲームモード
    public static GameMode SelectedMode = GameMode.Idiom;
    public static bool ShadowEnabled = true;

    public static string GetModeSpecificKey(string key)
    {
        return key + "_" + SelectedMode;
    }
}
