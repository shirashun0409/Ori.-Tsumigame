using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankingData
{
    public string nickname;
    public int score;
}

[Serializable]
public class RankingDataList
{
    public List<RankingData> rankings =
        new List<RankingData>();
}

public class RankingManager : MonoBehaviour
{
    private const string LegacyRankingKey = "KanjiRanking";

    public void RegisterScore(string nickname, int score)
    {
        RankingDataList data = LoadRanking();

        RankingData newData = new RankingData();
        newData.nickname = nickname;
        newData.score = score;

        data.rankings.Add(newData);

        // スコアの高い順に並べる
        data.rankings.Sort(
            (a, b) => b.score.CompareTo(a.score)
        );

        SaveRanking(data);

        Debug.Log(
            "ランキング登録: "
            + nickname
            + " / "
            + score
        );
    }

    public List<RankingData> GetRankings()
    {
        RankingDataList data = LoadRanking();

        // スコアの高い順に並べる
        data.rankings.Sort(
            (a, b) => b.score.CompareTo(a.score)
        );

        return data.rankings;
    }

    public int GetRank(string nickname, int score)
    {
        List<RankingData> rankings =
            GetRankings();

        for (int i = 0; i < rankings.Count; i++)
        {
            if (rankings[i].nickname == nickname &&
                rankings[i].score == score)
            {
                return i + 1;
            }
        }

        return -1;
    }

    private void SaveRanking(RankingDataList data)
    {
        string json =
            JsonUtility.ToJson(data);

        PlayerPrefs.SetString(
            GameModeData.GetModeSpecificKey("KanjiRanking"),
            json
        );

        PlayerPrefs.Save();

        Debug.Log(
            "ランキングを保存しました！"
        );
    }

    private RankingDataList LoadRanking()
    {
        string rankingKey =
            GameModeData.GetModeSpecificKey("KanjiRanking");

        if (!PlayerPrefs.HasKey(rankingKey))
        {
            // 既存ランキングはモード情報を持たないため、旧来の標準である熟語モードへ引き継ぐ。
            if (GameModeData.SelectedMode == GameMode.Idiom &&
                PlayerPrefs.HasKey(LegacyRankingKey))
            {
                string legacyJson = PlayerPrefs.GetString(LegacyRankingKey);
                RankingDataList legacyData =
                    JsonUtility.FromJson<RankingDataList>(legacyJson);

                if (legacyData != null)
                    return legacyData;
            }

            return new RankingDataList();
        }

        string json =
            PlayerPrefs.GetString(rankingKey);

        RankingDataList data =
            JsonUtility.FromJson<RankingDataList>(
                json
            );

        if (data == null)
        {
            return new RankingDataList();
        }

        return data;
    }
}
