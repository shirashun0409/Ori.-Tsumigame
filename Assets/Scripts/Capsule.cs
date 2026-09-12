using UnityEngine;
using DG.Tweening;

public class Capsule : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject capsulePartPrefab;

    [Header("Fall")]
    [SerializeField] private float fallInterval = 1.0f;
    [SerializeField] private float fastFallInterval = 0.1f;

    private float fallTimer;
    private bool isLanded = false;

    private int gridX = 3;
    private int gridY = 0;
    private int rotation = 0;

    public bool isNextPreview = false;

    private GameObject leftPart;
    private GameObject rightPart;

    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1)
    };
    public KanjiData nextLeftKanji;
    public KanjiData nextRightKanji;

    // ★ NEXT 用の漢字をセットする
    public void SetKanjiForNext(KanjiData left, KanjiData right)
    {
        nextLeftKanji = left;
        nextRightKanji = right;

        leftPart = CreatePart(Vector3.zero);
        rightPart = CreatePart(GetSubPartLocalPosition());

        leftPart.GetComponent<CapsulePart>().SetKanji(left);
        rightPart.GetComponent<CapsulePart>().SetKanji(right);
    }

    public Sprite GetSprite()
    {
        if (leftPart != null)
        {
            var sr = leftPart.GetComponent<SpriteRenderer>();
            if (sr != null)
                return sr.sprite;
        }
        return null;
    }

    private void Start()
    {
        if (isNextPreview)
        {
            // NEXT 用はすでに漢字がセット済み
            return;
        }

        // ★ currentCapsule は NEXT の漢字を使う
        if (nextLeftKanji != null && nextRightKanji != null)
        {
            leftPart.GetComponent<CapsulePart>().SetKanji(nextLeftKanji);
            rightPart.GetComponent<CapsulePart>().SetKanji(nextRightKanji);
        }
        else
        {
            // NEXT 以外（初回など）はランダム
            CreateCapsule();
        }

        UpdatePosition();
    }

    private void Update()
    {
        if (!isLanded && !isNextPreview)
        {
            Move();
            Fall();
            Rotate();
        }
    }

    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            TryMove(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            TryMove(1);
    }

    private void TryMove(int dx)
    {
        Vector2Int dir = Directions[rotation];
        int newX = gridX + dx;

        if (BoardManager.Instance.IsOccupied(newX, gridY))
            return;

        if (BoardManager.Instance.IsOccupied(newX + dir.x, gridY + dir.y))
            return;

        gridX = newX;
        UpdatePosition();
    }

    private void Fall()
    {
        float interval = Input.GetKey(KeyCode.DownArrow) ? fastFallInterval : fallInterval;

        fallTimer += Time.deltaTime;

        if (fallTimer >= interval)
        {
            if (CanFall())
            {
                gridY++;
                UpdatePosition();
            }
            else
            {
                Land();
            }
            fallTimer = 0f;
        }
    }

    private bool CanFall()
    {
        Vector2Int dir = Directions[rotation];
        int subX = gridX + dir.x;
        int subY = gridY + dir.y;

        return
            CanOccupy(gridX, gridY + 1, subX, subY) &&
            CanOccupy(subX, subY + 1, gridX, gridY);
    }

    private bool CanOccupy(int x, int y, int otherPartX, int otherPartY)
    {
        if (x == otherPartX && y == otherPartY)
            return true;

        return !BoardManager.Instance.IsOccupied(x, y);
    }

    private void Rotate()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        int newRotation = (rotation + 1) % 4;
        Vector2Int dir = Directions[newRotation];

        if (BoardManager.Instance.IsOccupied(gridX + dir.x, gridY + dir.y))
            return;

        rotation = newRotation;
        UpdatePartPositions();

        transform.DOScale(1.15f, 0.07f).SetLoops(2, LoopType.Yoyo);
        GameManager.Instance.PlaySE(GameManager.Instance.rotateSE);
    }

    private void CreateCapsule()
    {
        leftPart = CreatePart(Vector3.zero);
        rightPart = CreatePart(GetSubPartLocalPosition());
    }

    private GameObject CreatePart(Vector3 localPos)
    {
        GameObject obj = Instantiate(capsulePartPrefab, transform);
        obj.transform.localPosition = localPos;
        obj.transform.localScale = Vector3.one;

        CapsulePart part = obj.GetComponent<CapsulePart>();
        KanjiData kanjiData = KanjiDatabase.GetRandomKanji();

        if (kanjiData != null)
            part.SetKanji(kanjiData);

        return obj;
    }

    private void UpdatePosition()
    {
        if (isNextPreview)
            return;

        transform.position = BoardManager.Instance.GridToWorld(gridX, gridY);
        UpdatePartPositions();
    }

    private void UpdatePartPositions()
    {
        if (isNextPreview)
            return;

        leftPart.transform.localPosition = Vector3.zero;
        rightPart.transform.localPosition = GetSubPartLocalPosition();
    }

    private Vector3 GetSubPartLocalPosition()
    {
        float distance = BoardManager.Instance.CellSize;
        Vector2Int dir = Directions[rotation];
        return new Vector3(dir.x * distance, -dir.y * distance, 0f);
    }

    private void Land()
    {
        transform.localScale = Vector3.one;
        isLanded = true;

        Vector2Int dir = Directions[rotation];

        BoardManager.Instance.PlaceCapsule(
            gridX,
            gridY,
            gridX + dir.x,
            gridY + dir.y,
            leftPart,
            rightPart);

        GameManager.Instance.PlaySE(GameManager.Instance.dropSE);

        if (GameManager.Instance.IsIdiomMode())
        {
            BoardManager.Instance.StartChain();
            StartCoroutine(WaitForChainAndSpawn());
        }
        else
        {
            SpawnNextCapsule();
        }
    }

    private System.Collections.IEnumerator WaitForChainAndSpawn()
    {
        yield return null;

        while (BoardManager.Instance.IsChainProcessing())
            yield return null;

        SpawnNextCapsule();
    }

    private void SpawnNextCapsule()
    {
        GameManager.Instance.SpawnCapsule();
        Destroy(gameObject);
    }

    public Sprite GetLeftSprite()
    {
        if (leftPart == null) return null;
        return leftPart.GetComponent<SpriteRenderer>().sprite;
    }

    public Sprite GetRightSprite()
    {
        if (rightPart == null) return null;
        return rightPart.GetComponent<SpriteRenderer>().sprite;
    }

}
