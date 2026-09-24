using System.Collections.Generic;
using UnityEngine;


public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance;

    // 今回のゲームで作った熟語
    private List<string> createdIdioms = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // GameScene → GameOverSceneでも残す
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 熟語が完成したときに呼ぶ
    /// </summary>
    public void AddIdiom(string idiom)
    {
        if (string.IsNullOrEmpty(idiom))
            return;

        // 同じ熟語は一度だけ記録
        if (createdIdioms.Contains(idiom))
            return;

        createdIdioms.Add(idiom);

        Debug.Log("今回作った熟語: " + idiom);
    }

    /// <summary>
    /// 今回作った熟語一覧を取得
    /// </summary>
    public List<string> GetCreatedIdioms()
    {
        return new List<string>(createdIdioms);
    }

    /// <summary>
    /// ゲーム開始時に熟語記録をリセット
    /// </summary>
    public void ResetResult()
    {
        createdIdioms.Clear();
    }
}