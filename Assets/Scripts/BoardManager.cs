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
}

