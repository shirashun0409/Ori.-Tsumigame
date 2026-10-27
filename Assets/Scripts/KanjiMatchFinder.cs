using System.Collections.Generic;
using UnityEngine;

public class IdiomMatch
{
    public Vector2Int First;
    public Vector2Int Second;

    public IdiomMatch(
        Vector2Int first,
        Vector2Int second)
    {
        First = first;
        Second = second;
    }
}


public static class KanjiMatchFinder
{
    /// <summary>
    /// 現在のゲームモードに応じて、
    /// 成立する隣接する2つの漢字を探します。
    ///
    /// 熟語モード：
    /// 実際に存在する二字熟語を探す
    ///
    /// 部首モード：
    /// 同じ部首の漢字2個を探す
    ///
    /// 読みモード：
    /// 現在は未実装
    ///
    /// 横方向：左 → 右
    /// 縦方向：上 → 下
    /// のみ確認します。
    /// </summary>
    public static List<IdiomMatch> FindIdiomMatches()
    {
        List<IdiomMatch> matches =
            new List<IdiomMatch>();

        BoardManager board =
            BoardManager.Instance;

        //======================================
        // 現在のゲームモードを取得
        //======================================

        GameManager gameManager =
            GameManager.Instance;

        if (gameManager == null)
        {
            return matches;
        }


        for (int y = 0;
             y < BoardManager.Height;
             y++)
        {
            for (int x = 0;
                 x < BoardManager.Width;
                 x++)
            {
                CapsulePart currentPart =
                    board.GetPart(x, y);

                if (currentPart == null)
                    continue;


                KanjiData currentKanji =
                    currentPart.GetKanjiData();

                if (currentKanji == null)
                    continue;


                //======================================
                // 右隣
                //======================================

                if (x + 1 < BoardManager.Width)
                {
                    CheckPair(
                        x,
                        y,
                        x + 1,
                        y,
                        matches,
                        gameManager
                    );
                }


                //======================================
                // 下隣
                //======================================

                if (y + 1 < BoardManager.Height)
                {
                    CheckPair(
                        x,
                        y,
                        x,
                        y + 1,
                        matches,
                        gameManager
                    );
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
        List<IdiomMatch> matches,
        GameManager gameManager)
    {
        BoardManager board =
            BoardManager.Instance;


        CapsulePart firstPart =
            board.GetPart(x1, y1);

        CapsulePart secondPart =
            board.GetPart(x2, y2);


        if (firstPart == null ||
            secondPart == null)
        {
            return;
        }


        KanjiData firstKanji =
            firstPart.GetKanjiData();

        KanjiData secondKanji =
            secondPart.GetKanjiData();


        if (firstKanji == null ||
            secondKanji == null)
        {
            return;
        }


        //======================================
        // 熟語モード
        //======================================

        if (gameManager.IsIdiomMode())
        {
            // first → second の順番のみ判定
            if (KanjiMatcher.IsIdiomMatch(
                firstKanji,
                secondKanji))
            {
                matches.Add(
                    new IdiomMatch(
                        new Vector2Int(x1, y1),
                        new Vector2Int(x2, y2)
                    )
                );
            }

            return;
        }


        //======================================
        // 部首モード
        //======================================

        if (gameManager.IsRadicalMode())
        {
            if (KanjiMatcher.IsRadicalMatch(
                firstKanji,
                secondKanji))
            {
                matches.Add(
                    new IdiomMatch(
                        new Vector2Int(x1, y1),
                        new Vector2Int(x2, y2)
                    )
                );
            }

            return;
        }


        //======================================
        // 読みモード
        //======================================

        if (gameManager.IsReadingMode())
        {
            // 読みモードはまだ未実装
            return;
        }
    }
}