using UnityEngine;

public static class KanjiMatcher
{
    /// <summary>
    /// first → second の順番で
    /// ゲーム内で成立する二字熟語か判定します。
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

        // first → second の順番だけ確認
        if (!ContainsPartner(
            first,
            second.kanji))
        {
            return false;
        }

        // 最終的に辞書に登録されているか確認
        string idiom =
            first.kanji + second.kanji;

        if (!IdiomDictionary.Contains(idiom))
        {
            return false;
        }

        return true;
    }


    private static bool ContainsPartner(
        KanjiData data,
        string targetKanji)
    {
        if (data.idiomPartners == null)
        {
            return false;
        }

        foreach (string partner
                 in data.idiomPartners)
        {
            if (partner == targetKanji)
            {
                return true;
            }
        }

        return false;
    }
}