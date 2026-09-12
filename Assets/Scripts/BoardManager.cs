using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    public const int Width = 8;
    public const int Height = 12;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float cellSize = 1.0f;
    public float CellSize => cellSize;

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
             (Height - 1) * cellSize / 2f);

        CreateBoard();

        yield return null;

        idiomKanji = new HashSet<string>();
        foreach (var entry in GameManager.Instance.CurrentRegistry)
        {
            foreach (var partner in entry.IdiomPartners)
            {
                idiomKanji.Add(entry.Kanji);
                idiomKanji.Add(partner);
            }
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    //==================================================
    // ★ 重み付きランダムで漢字を選ぶ
    //==================================================
    public string GetRandomKanji()
    {
        var registry = GameManager.Instance.CurrentRegistry;

        float t = Mathf.Clamp01(elapsedTime / difficultyTime);
        float currentIdiomWeight = Mathf.Lerp(idiomWeightStart, idiomWeightEnd, t);

        float normalWeight = 1f;

        float totalWeight = 0f;
        List<(string kanji, float weight)> weightedList = new();

        foreach (var entry in registry)
        {
            float w = idiomKanji.Contains(entry.Kanji)
                ? currentIdiomWeight
                : normalWeight;

            weightedList.Add((entry.Kanji, w));
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
                    transform);
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
            0f);
    }

    public bool IsEmpty(int x, int y) => board[x, y] == null;
    public bool IsInsideBoard(int x, int y) =>
        x >= 0 && x < Width && y >= 0 && y < Height;

    //==================================================
    // カプセル配置
    //==================================================
    public void PlaceCapsule(int leftX, int leftY, int rightX, int rightY,
                             GameObject left, GameObject right)
    {
        CapsulePart leftPart = left.GetComponent<CapsulePart>();
        CapsulePart rightPart = right.GetComponent<CapsulePart>();

        board[leftX, leftY] = leftPart;
        board[rightX, rightY] = rightPart;

        left.transform.SetParent(transform);
        right.transform.SetParent(transform);

        left.transform.position = GridToWorld(leftX, leftY);
        right.transform.position = GridToWorld(rightX, rightY);
    }

    public void SetPart(int x, int y, CapsulePart part)
    {
        board[x, y] = part;
    }

    public CapsulePart GetPart(int x, int y)
    {
        return board[x, y];
    }

    public bool IsOccupied(int x, int y)
    {
        if (!IsInsideBoard(x, y))
            return true;

        return board[x, y] != null;
    }

    //==================================================
    // 消去処理
    //==================================================
    public void RemovePart(int x, int y)
    {
        if (!IsInsideBoard(x, y))
            return;

        CapsulePart part = board[x, y];
        if (part == null)
            return;

        board[x, y] = null;

        part.transform.DOScale(0f, 0.15f).SetEase(Ease.InBack);
        GameManager.Instance.PlaySE(GameManager.Instance.eraseSE);

        Destroy(part.gameObject, 0.15f);
    }

    //==================================================
    // 重力落下
    //==================================================
    public bool IsGravityFalling() => isGravityFalling;

    public void ApplyGravity()
    {
        if (!isGravityFalling)
            StartCoroutine(GravityFallCoroutine());
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
                    CapsulePart part = board[x, y];
                    if (part == null)
                        continue;

                    bool belowEmpty = board[x, y + 1] == null;
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

                    if (hasLeftSupport || hasRightSupport)
                        continue;

                    board[x, y + 1] = part;
                    board[x, y] = null;

                    part.transform.position = GridToWorld(x, y + 1);

                    movedThisStep = true;
                }
            }

            if (!movedThisStep)
                break;

            yield return new WaitForSeconds(gravityFallInterval);
        }

        isGravityFalling = false;
    }

    //==================================================
    // ★ 熟語文字列を作る（追加）
    //==================================================
    private string BuildIdiomString(List<Vector2Int> positions)
    {
        positions.Sort((a, b) => a.y.CompareTo(b.y)); // 上から読む

        string result = "";
        foreach (var pos in positions)
        {
            CapsulePart part = board[pos.x, pos.y];
            if (part != null)
                result += part.Kanji;   // CapsulePart に Kanji がある前提
        }
        return result;
    }

    //==================================================
    // 連鎖処理
    //==================================================
    public void StartChain()
    {
        if (!isChainProcessing)
            StartCoroutine(ChainCoroutine());
    }

    private IEnumerator ChainCoroutine()
    {
        isChainProcessing = true;

        while (true)
        {
            List<Vector2Int> matches =
                KanjiMatchFinder.FindIdiomMatches();

            HashSet<Vector2Int> uniqueMatches =
                new HashSet<Vector2Int>(matches);

            if (uniqueMatches.Count == 0)
                break;

            // ★ 熟語文字列を作って GameManager に渡す（追加）
            string idiom = BuildIdiomString(matches);
            GameManager.Instance.OnIdiomCreated(idiom);

            GameManager.Instance.PlaySE(GameManager.Instance.comboSE);

            foreach (Vector2Int pos in uniqueMatches)
            {
                RemovePart(pos.x, pos.y);
            }

            ApplyGravity();
            yield return null;

            while (isGravityFalling)
                yield return null;

            yield return null;
        }

        isChainProcessing = false;
    }

    public bool IsChainProcessing() => isChainProcessing;
}
