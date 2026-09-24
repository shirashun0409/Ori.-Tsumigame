using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text nameText;

    [Header("Ranking")]
    [SerializeField] private RankingManager rankingManager;

    private void Start()
    {
        DisplayRanking();
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
}