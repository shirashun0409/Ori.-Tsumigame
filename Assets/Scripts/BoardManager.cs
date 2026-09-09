using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    public const int Width = 8;
    public const int Height = 12;

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private float cellSize = 1.0f;
    // 盤面のマス間隔（落下中のカプセルも同じ間隔で並べる）
    public float CellSize
    {
        get { return cellSize; }
    }

    // 盤面データ

    private CapsulePart[,] board;

    // 盤面の左上座標
    private Vector2 boardOrigin;

    // 消去後の漢字の落下速度
    [SerializeField]
    private float gravityFallInterval = 0.5f;

    // 現在重力落下中か
    private bool isGravityFalling = false;

    // 現在連鎖処理中か
    private bool isChainProcessing = false;


    private void Awake()
    {
        Instance = this;

        board = new CapsulePart[Width, Height];
    }


    private void Start()
    {
        boardOrigin = new Vector2(
            -(Width - 1) * cellSize / 2f,
             (Height - 1) * cellSize / 2f);

        CreateBoard();
    }


    /// <summary>
    /// マス座標をUnity座標へ変換
    /// </summary>
    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(
            boardOrigin.x + x * cellSize,
            boardOrigin.y - y * cellSize,
            0f);
    }


    public bool IsEmpty(int x, int y)
    {
        return board[x, y] == null;
    }


    public bool IsInsideBoard(int x, int y)
    {
        return x >= 0 &&
               x < Width &&
               y >= 0 &&
               y < Height;
    }


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


    public CapsulePart GetPart(int x, int y)
    {
        return board[x, y];
    }


    public bool IsOccupied(int x, int y)
    {
        // 盤面の外は埋まっているものとして扱う
        if (!IsInsideBoard(x, y))
        {
            return true;
        }

        return board[x, y] != null;
    }


    public void RemovePart(int x, int y)
    {
        if (!IsInsideBoard(x, y))
            return;

        CapsulePart part = board[x, y];

        if (part == null)
            return;

        board[x, y] = null;

        // ★ 消滅エフェクト（キュッと縮む）
        part.transform.DOScale(0f, 0.15f).SetEase(Ease.InBack);

        // ★ 消滅SE（ここが必要！）
        GameManager.Instance.PlaySE(GameManager.Instance.eraseSE);

        // 少し待ってから削除（エフェクトを見せるため）
        Destroy(part.gameObject, 0.15f);
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
        if (isGravityFalling)
            return;

        StartCoroutine(GravityFallCoroutine());
    }


    private IEnumerator GravityFallCoroutine()
    {
        isGravityFalling = true;

        while (true)
        {
            bool movedThisStep = false;

            // 下の行から上へ調べる（必ず1マスずつ）
            for (int y = Height - 2; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    CapsulePart part = board[x, y];

                    if (part == null)
                        continue;

                    // 真下が空いている場合のみ1マス落とす
                    // 真下が空いている場合のみ1マス落とす
                    if (board[x, y + 1] == null)
                    {
                        board[x, y + 1] = part;
                        board[x, y] = null;

                        part.transform.position = GridToWorld(x, y + 1);

                        // ★ ここに追加：落下エフェクト
                        part.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);

                        movedThisStep = true;
                    }

                }
            }

            // 1マスも動かなかったら終了
            if (!movedThisStep)
                break;

            // 1マス落ちるたびに待つ
            yield return new WaitForSeconds(
                gravityFallInterval);
        }

        isGravityFalling = false;
    }


    //==================================================
    // 連鎖処理
    //==================================================

    public void StartChain()
    {
        if (isChainProcessing)
            return;

        StartCoroutine(ChainCoroutine());
    }


    private IEnumerator ChainCoroutine()
    {
        isChainProcessing = true;

        while (true)
        {
            // 現在の盤面から熟語を探す
            List<Vector2Int> matches =
                KanjiMatchFinder.FindIdiomMatches();

            HashSet<Vector2Int> uniqueMatches =
                new HashSet<Vector2Int>(matches);

            // 熟語がなければ連鎖終了

            if (uniqueMatches.Count == 0)
            {
                break;
            }

            Debug.Log(
                "熟語を発見！ " +
                uniqueMatches.Count +
                "個の漢字を消します。");

            // ★ 連鎖SE（ここが必要！）
            GameManager.Instance.PlaySE(GameManager.Instance.comboSE);


            // 熟語を全部消す
            foreach (Vector2Int position in uniqueMatches)
            {
                RemovePart(
                    position.x,
                    position.y);
            }

            // 消えたあとに落下開始
            ApplyGravity();

            // 重力処理が開始されるまで1フレーム待つ
            yield return null;

            // 全部落ち終わるまで待つ
            while (isGravityFalling)
            {
                yield return null;
            }

            // 盤面が完全に安定するまでさらに1フレーム待つ
            yield return null;
        }

        isChainProcessing = false;

        Debug.Log("連鎖終了！");
    }


    public bool IsChainProcessing()
    {
        return isChainProcessing;
    }
}
