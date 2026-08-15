using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    public const int Width = 8;
    public const int Height = 16;

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private float cellSize = 1.0f;
    // 盤面データ

    private CapsulePart[,] board;
    // 盤面の左上座標
    private Vector2 boardOrigin;

    // 消去後の漢字の落下速度
    [SerializeField]
    private float gravityFallInterval = 0.5f;

    // 現在落下中か
    private bool isGravityFalling = false;
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

        Destroy(part.gameObject);
    }
    public void ApplyGravity()
    {
        if (isGravityFalling)
            return;

        StartCoroutine(GravityFallCoroutine());
    }
    private System.Collections.IEnumerator GravityFallCoroutine()
    {
        isGravityFalling = true;

        bool moved;

        do
        {
            moved = false;

            for (int x = 0; x < Width; x++)
            {
                for (int y = Height - 2; y >= 0; y--)
                {
                    CapsulePart part = board[x, y];

                    if (part == null)
                        continue;

                    // 真下が空いているか
                    if (board[x, y + 1] == null)
                    {
                        board[x, y + 1] = part;
                        board[x, y] = null;

                        // 1マスだけ下へ移動
                        part.transform.position =
                            GridToWorld(x, y + 1);

                        moved = true;
                    }
                }
            }

            // 1マス落ちるたびに待つ
            if (moved)
            {
                yield return new WaitForSeconds(
                    gravityFallInterval);
            }

        } while (moved);

        isGravityFalling = false;
    }
}


