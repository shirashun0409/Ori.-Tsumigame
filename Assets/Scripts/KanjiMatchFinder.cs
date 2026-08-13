using System.Collections.Generic;
using UnityEngine;

public static class KanjiMatchFinder
{
    /// <summary>
    /// 熟語モードで消去対象になる隣接ペアを探します。
    /// </summary>
    public static List<Vector2Int> FindIdiomMatches()
    {
        List<Vector2Int> matches = new List<Vector2Int>();

        BoardManager board = BoardManager.Instance;

        for (int y = 0; y < BoardManager.Height; y++)
        {
            for (int x = 0; x < BoardManager.Width; x++)
            {
                CapsulePart currentPart = board.GetPart(x, y);

                if (currentPart == null)
                    continue;

                KanjiData currentKanji = currentPart.GetKanjiData();

                if (currentKanji == null)
                    continue;

                // 右隣を確認
                if (x + 1 < BoardManager.Width)
                {
                    CheckPair(
                        x,
                        y,
                        x + 1,
                        y,
                        currentKanji,
                        matches);
                }

                // 下隣を確認
                if (y + 1 < BoardManager.Height)
                {
                    CheckPair(
                        x,
                        y,
                        x,
                        y + 1,
                        currentKanji,
                        matches);
                }
            }
        }

        return matches;
    }

    private static void CheckPair(
        int x1,
        int y1,
        int x2,
        int y2,
        KanjiData firstKanji,
        List<Vector2Int> matches)
    {
        CapsulePart secondPart =
            BoardManager.Instance.GetPart(x2, y2);

        if (secondPart == null)
            return;

        KanjiData secondKanji =
            secondPart.GetKanjiData();

        if (secondKanji == null)
            return;

        if (KanjiMatcher.IsIdiomMatch(
            firstKanji,
            secondKanji))
        {
            matches.Add(new Vector2Int(x1, y1));
            matches.Add(new Vector2Int(x2, y2));
        }
    }
    public static void RemoveIdiomMatches()
    {
        List<Vector2Int> matches = FindIdiomMatches();

        HashSet<Vector2Int> uniqueMatches =
            new HashSet<Vector2Int>(matches);

        Debug.Log("熟語候補の数: " + uniqueMatches.Count);

        foreach (Vector2Int position in uniqueMatches)
        {
            Debug.Log(
                "熟語として消去: (" +
                position.x + ", " +
                position.y + ")");

            BoardManager.Instance.RemovePart(
                position.x,
                position.y);
        }
    }
}
