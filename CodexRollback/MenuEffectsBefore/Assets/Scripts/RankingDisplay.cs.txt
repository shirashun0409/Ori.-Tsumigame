
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RankingDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text nameText;

    // ★ あなたの順位を表示するText
    [SerializeField] private TMP_Text yourRankText;

    [Header("Ranking")]
    [SerializeField] private RankingManager rankingManager;

    // GameOverSceneから受け取った情報
    private const string RegisteredKey = "LastRankingRegistered";
    private const string NicknameKey = "LastRankingNickname";
    private const string ScoreKey = "LastRankingScore";

    private void Start()
    {
        DisplayRanking();
        DisplayYourRank();
    }

    public void BackToGameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    public void DisplayRanking()
    {
        if (rankText == null)
        {
            Debug.LogError("RankTextが設定されていません！");
            return;
        }

        if (scoreText == null)
        {
            Debug.LogError("ScoreTextが設定されていません！");
            return;
        }

        if (nameText == null)
        {
            Debug.LogError("NameTextが設定されていません！");
            return;
        }

        if (rankingManager == null)
        {
            Debug.LogError("RankingManagerが設定されていません！");
            return;
        }

        List<RankingData> rankings =
            rankingManager.GetRankings();

        if (rankings.Count == 0)
        {
            rankText.text = "";
            scoreText.text = "";
            nameText.text = "まだランキングがありません";
            return;
        }

        string ranks = "";
        string scores = "";
        string names = "";

        int displayCount = Mathf.Min(10, rankings.Count);

        for (int i = 0; i < displayCount; i++)
        {
            RankingData data = rankings[i];

            ranks += (i + 1) + "位\n";
            scores += data.score.ToString("N0") + "\n";
            names += data.nickname + "\n";
        }

        rankText.text = ranks;
        scoreText.text = scores;
        nameText.text = names;
    }

    // ========================================
    // あなたの順位を表示
    // ========================================
    private void DisplayYourRank()
    {
        // Textが設定されていなければ終了
        if (yourRankText == null)
        {
            Debug.LogWarning(
                "YourRankTextが設定されていません！"
            );
            return;
        }

        // まず非表示にする
        yourRankText.gameObject.SetActive(false);

        // 今回ニックネーム登録したか確認
        int registered =
            PlayerPrefs.GetInt(
                RegisteredKey,
                0
            );

        // 登録していないなら何も表示しない
        if (registered != 1)
        {
            return;
        }

        // ニックネーム取得
        string nickname =
            PlayerPrefs.GetString(
                NicknameKey,
                ""
            );

        // スコア取得
        int score =
            PlayerPrefs.GetInt(
                ScoreKey,
                0
            );

        // ニックネームが空なら表示しない
        if (string.IsNullOrEmpty(nickname))
        {
            return;
        }

        // 順位を取得
        int rank =
            rankingManager.GetRank(
                nickname,
                score
            );

        // 順位が取得できなかった場合
        if (rank <= 0)
        {
            return;
        }

        // ★ ここで表示
        yourRankText.gameObject.SetActive(true);

        yourRankText.text =
            +rank
            + "位";

        Debug.Log(

            +rank
            + "位"
        );
    }
}

