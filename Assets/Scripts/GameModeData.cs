using UnityEngine;
using System.Collections.Generic;

public static class GameModeData
{
    // 次にGameSceneで使うゲームモード
    public static GameMode SelectedMode = GameMode.Idiom;
    public static bool ShadowEnabled = true;
    public static HashSet<int> SelectedTestRanges = new HashSet<int>
    {
        22, 23, 24, 25, 26, 27, 28, 29, 30
    };

    public static string GetModeSpecificKey(string key)
    {
        return key + "_" + SelectedMode;
    }
}
