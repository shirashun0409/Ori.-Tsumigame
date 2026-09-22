using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    public const int Width = 8;
    public const int Height = 12;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float cellSize = 1.0f;

    public float CellSize => cellSize;

    public TextMeshProUGUI ReadingText;
    public TextMeshProUGUI MeaningText;

    private CapsulePart[,] board;
    private Vector2 boardOrigin;

    [SerializeField] private float gravityFallInterval = 0.5f;

    private bool isGravityFalling = false;
    private bool isChainProcessing = false;

    // ==== 出現率調整用 ====
    private HashSet<string> idiomKanji;

    private float elapsedTime = 0f;

    private float idiomWeightStart = 3f;
    private float idiomWeightEnd = 1f;
    private float difficultyTime = 120f;


    private void Awake()
    {
        Instance = this;

        board = new CapsulePart[Width, Height];
    }


    private IEnumerator Start()
    {
        boardOrigin = new Vector2(
            -(Width - 1) * cellSize / 2f,
             (Height - 1) * cellSize / 2f
        );

        CreateBoard();

        yield return null;

        // 熟語に使われる漢字を登録
        idiomKanji = new HashSet<string>();

        foreach (var entry in GameManager.Instance.CurrentRegistry)
        {
            foreach (var partner in entry.IdiomPartners)
            {
                idiomKanji.Add(entry.Kanji);
                idiomKanji.Add(partner);
            }
        }

        if (ReadingText != null)
            ReadingText.text = "";

        if (MeaningText != null)
            MeaningText.text = "";
    }


    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }


    //==================================================
    // 重み付きランダムで漢字を選ぶ
    //==================================================
    public string GetRandomKanji()
    {
        var registry = GameManager.Instance.CurrentRegistry;

        float t =
            Mathf.Clamp01(elapsedTime / difficultyTime);

        float currentIdiomWeight =
            Mathf.Lerp(
                idiomWeightStart,
                idiomWeightEnd,
                t
            );

        float normalWeight = 1f;

        float totalWeight = 0f;

        List<(string kanji, float weight)> weightedList =
            new List<(string kanji, float weight)>();

        foreach (var entry in registry)
        {
            float w =
                idiomKanji.Contains(entry.Kanji)
                    ? currentIdiomWeight
                    : normalWeight;

            weightedList.Add(
                (entry.Kanji, w)
            );

            totalWeight += w;
        }

        float r = Random.value * totalWeight;

        foreach (var item in weightedList)
        {
            r -= item.weight;

            if (r <= 0f)
                return item.kanji;
        }

        return weightedList[weightedList.Count - 1].kanji;
    }


    //==================================================
    // 盤面生成
    //==================================================
    private void CreateBoard()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Instantiate(
                    cellPrefab,
                    GridToWorld(x, y),
                    Quaternion.identity,
                    transform
                );
            }
        }
    }


    //==================================================
    // 座標変換
    //==================================================
    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(
            boardOrigin.x + x * cellSize,
            boardOrigin.y - y * cellSize,
            0f
        );
    }


    public bool IsEmpty(int x, int y)
    {
        return board[x, y] == null;
    }


    public bool IsInsideBoard(int x, int y)
    {
        return
            x >= 0 &&
            x < Width &&
            y >= 0 &&
            y < Height;
    }


    //==================================================
    // カプセル配置
    //==================================================
    public void PlaceCapsule(
        int leftX,
        int leftY,
        int rightX,
        int rightY,
        GameObject left,
        GameObject right)
    {
        CapsulePart leftPart =
            left.GetComponent<CapsulePart>();

        CapsulePart rightPart =
            right.GetComponent<CapsulePart>();

        board[leftX, leftY] = leftPart;
        board[rightX, rightY] = rightPart;

        left.transform.SetParent(transform);
        right.transform.SetParent(transform);

        left.transform.position =
            GridToWorld(leftX, leftY);

        right.transform.position =
            GridToWorld(rightX, rightY);
    }


    public void SetPart(
        int x,
        int y,
        CapsulePart part)
    {
        board[x, y] = part;
    }


    public CapsulePart GetPart(
        int x,
        int y)
    {
        return board[x, y];
    }


    public bool IsOccupied(
        int x,
        int y)
    {
        if (!IsInsideBoard(x, y))
            return true;

        return board[x, y] != null;
    }


    //==================================================
    // 消去処理
    //==================================================
    public void RemovePart(
        int x,
        int y)
    {
        if (!IsInsideBoard(x, y))
            return;

        CapsulePart part =
            board[x, y];

        if (part == null)
            return;

        board[x, y] = null;

        part.transform
            .DOScale(0f, 0.15f)
            .SetEase(Ease.InBack);

        GameManager.Instance.PlaySE(
            GameManager.Instance.eraseSE
        );

        Destroy(
            part.gameObject,
            0.15f
        );
    }


    //==================================================
    // 重力落下
    //==================================================
    public bool IsGravityFalling()
    {
        return isGravityFalling;
    }


    public void ApplyGravity()
    {
        if (!isGravityFalling)
        {
            StartCoroutine(
                GravityFallCoroutine()
            );
        }
    }


    private IEnumerator GravityFallCoroutine()
    {
        isGravityFalling = true;

        while (true)
        {
            bool movedThisStep = false;

            for (int y = Height - 2; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    CapsulePart part =
                        board[x, y];

                    if (part == null)
                        continue;

                    bool belowEmpty =
                        board[x, y + 1] == null;

                    if (!belowEmpty)
                        continue;

                    bool hasLeftSupport =
                        x > 0 &&
                        board[x - 1, y] != null &&
                        board[x - 1, y + 1] != null;

                    bool hasRightSupport =
                        x < Width - 1 &&
                        board[x + 1, y] != null &&
                        board[x + 1, y + 1] != null;

                    if (hasLeftSupport ||
                        hasRightSupport)
                    {
                        continue;
                    }

                    board[x, y + 1] = part;
                    board[x, y] = null;

                    part.transform.position =
                        GridToWorld(
                            x,
                            y + 1
                        );

                    movedThisStep = true;
                }
            }

            if (!movedThisStep)
                break;

            yield return new WaitForSeconds(
                gravityFallInterval
            );
        }

        isGravityFalling = false;
    }


    //==================================================
    // 熟語1つ分の文字列を作る
    //==================================================
    private string BuildIdiomString(
        Vector2Int first,
        Vector2Int second)
    {
        CapsulePart firstPart =
            board[first.x, first.y];

        CapsulePart secondPart =
            board[second.x, second.y];

        if (firstPart == null ||
            secondPart == null)
        {
            return "";
        }

        // 横なら左 → 右
        if (first.y == second.y)
        {
            if (first.x > second.x)
            {
                Vector2Int temp = first;
                first = second;
                second = temp;
            }
        }
        // 縦なら上 → 下
        else if (first.x == second.x)
        {
            if (first.y > second.y)
            {
                Vector2Int temp = first;
                first = second;
                second = temp;
            }
        }

        firstPart = board[first.x, first.y];
        secondPart = board[second.x, second.y];

        if (firstPart == null ||
            secondPart == null)
        {
            return "";
        }

        return firstPart.Kanji + secondPart.Kanji;
    }


    //==================================================
    // 連鎖処理開始
    //==================================================
    public void StartChain()
    {
        if (!isChainProcessing)
        {
            StartCoroutine(
                ChainCoroutine()
            );
        }
    }


    //==================================================
    // 連鎖処理
    //==================================================
    private IEnumerator ChainCoroutine()
    {
        isChainProcessing = true;

        while (true)
        {
            List<IdiomMatch> matches =
                KanjiMatchFinder.FindIdiomMatches();

            if (matches.Count == 0)
                break;


            //==================================================
            // 成立した熟語
            //==================================================

            List<string> idioms =
                new List<string>();


            //==================================================
            // 実際に消すマス
            //==================================================

            HashSet<Vector2Int> uniquePositions =
                new HashSet<Vector2Int>();


            //==================================================
            // 熟語を調べる
            //==================================================

            foreach (IdiomMatch match in matches)
            {
                string idiom =
                    BuildIdiomString(
                        match.First,
                        match.Second
                    );

                if (string.IsNullOrEmpty(idiom))
                    continue;


                // 辞書に存在する熟語だけ有効
                if (!IdiomDictionary.Contains(idiom))
                    continue;


                // 同じ熟語を重複して数えない
                if (!idioms.Contains(idiom))
                {
                    idioms.Add(idiom);
                }


                // 実際に消す漢字の位置を記録
                uniquePositions.Add(match.First);
                uniquePositions.Add(match.Second);
            }


            //==================================================
            // 有効な熟語がなければ終了
            //==================================================

            if (idioms.Count == 0)
                break;


            //==================================================
            // スコア計算
            //==================================================

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(
                    uniquePositions.Count,
                    idioms.Count
                );
            }


            //==================================================
            // 熟語表示
            //==================================================

            GameManager.Instance.OnIdiomsCreated(
                idioms
            );


            //==================================================
            // 読み・意味・構造
            //
            // 熟語と同じ順番で作る
            //==================================================

            List<string> readings =
                new List<string>();

            List<string> meanings =
                new List<string>();


            foreach (string idiom in idioms)
            {
                string reading =
                    IdiomDictionary.GetReading(
                        idiom
                    );

                string meaning =
                    IdiomDictionary.GetMeaning(
                        idiom
                    );

                string structure =
                    IdiomDictionary.GetStructure(
                        idiom
                    );


                //==============================================
                // 読み
                //
                // 熟語1つにつき必ず1行
                //==============================================

                if (string.IsNullOrEmpty(reading))
                {
                    readings.Add("？？？");
                }
                else
                {
                    readings.Add(reading);
                }


                //==============================================
                // 意味 + 構造
                //
                // 熟語1つにつき必ず1行
                //==============================================

                if (string.IsNullOrEmpty(meaning))
                {
                    meanings.Add(
                        idiom + "：？？？"
                    );
                }
                else if (string.IsNullOrEmpty(structure))
                {
                    meanings.Add(
                        idiom + "：" + meaning
                    );
                }
                else
                {
                    meanings.Add(
                        idiom +
                        "：" +
                        meaning +
                        "（構造：" +
                        structure +
                        "）"
                    );
                }
            }


            //==================================================
            // 読み表示
            //==================================================

            if (ReadingText != null)
            {
                ReadingText.text = string.Join(
      "　",
      readings
  );
            }


            //==================================================
            // 意味表示
            //==================================================

            if (MeaningText != null)
            {
                MeaningText.text =
                    string.Join(
                        "\n",
                        meanings
                    );
            }


            //==================================================
            // コンボSE
            //==================================================

            GameManager.Instance.PlaySE(
                GameManager.Instance.comboSE
            );


            //==================================================
            // 漢字を消す
            //==================================================

            foreach (Vector2Int pos in uniquePositions)
            {
                RemovePart(
                    pos.x,
                    pos.y
                );
            }


            //==================================================
            // 重力
            //==================================================

            ApplyGravity();

            yield return null;


            // 重力が終わるまで待つ
            while (isGravityFalling)
            {
                yield return null;
            }


            // 1フレーム待つ
            yield return null;
        }


        isChainProcessing = false;
    }


    public bool IsChainProcessing()
    {
        return isChainProcessing;
    }
}