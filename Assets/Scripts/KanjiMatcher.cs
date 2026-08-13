using UnityEngine;

public static class KanjiMatcher
{
    /// <summary>
    /// 2つの漢字が熟語として成立するか判定します。
    /// 順番は問いません。
    /// </summary>
    public static bool IsIdiomMatch(
    KanjiData first,
    KanjiData second)
    {
        if (first == null || second == null)
        {
            Debug.Log("KanjiDataがnullです");
            return false;
        }

        Debug.Log(
            "熟語判定: " +
            first.kanji + " + " +
            second.kanji);

        if (first.kanji == second.kanji)
        {
            return false;
        }

        if (ContainsPartner(first, second.kanji))
        {
            Debug.Log(
                "熟語成立: " +
                first.kanji + " + " +
                second.kanji);

            return true;
        }

        if (ContainsPartner(second, first.kanji))
        {
            Debug.Log(
                "熟語成立: " +
                second.kanji + " + " +
                first.kanji);

            return true;
        }

        Debug.Log("熟語不成立");

        return false;
    }

    private static bool ContainsPartner(
        KanjiData data,
        string targetKanji)
    {
        if (data.idiomPartners == null)
        {
            return false;
        }

        foreach (string partner in data.idiomPartners)
        {
            if (partner == targetKanji)
            {
                return true;
            }
        }

        return false;
    }
}