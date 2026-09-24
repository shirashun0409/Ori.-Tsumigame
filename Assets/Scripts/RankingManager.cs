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
    private const string RankingKey = "KanjiRanking";

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
            RankingKey,
            json
        );

        PlayerPrefs.Save();

        Debug.Log(
            "ランキングを保存しました！"
        );
    }

    private RankingDataList LoadRanking()
    {
        if (!PlayerPrefs.HasKey(RankingKey))
        {
            return new RankingDataList();
        }

        string json =
            PlayerPrefs.GetString(RankingKey);

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