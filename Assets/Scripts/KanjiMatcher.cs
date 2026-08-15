using UnityEngine;

public static class KanjiMatcher
{
    /// <summary>
    /// first → second の順番で正しい二字熟語になるか判定します。
    /// 順番は入れ替えません。
    /// </summary>
    public static bool IsIdiomMatch(
        KanjiData first,
        KanjiData second)
    {
        if (first == null || second == null)
        {
            return false;
        }

        // 同じ漢字同士は熟語として扱わない
        if (first.kanji == second.kanji)
        {
            return false;
        }

        // first → second の順番だけを確認
        if (ContainsPartner(first, second.kanji))
        {
            return true;
        }

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