using UnityEngine;

public class Capsule : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField]
    private GameObject capsulePartPrefab;

    [Header("Fall")]
    [SerializeField]
    private float fallInterval = 1.0f;

    [SerializeField]
    private float fastFallInterval = 0.1f;

    private float fallTimer;

    private bool isLanded = false;
    // グリッド座標（leftPart の位置）
    private int gridX = 3;
    private int gridY = 0;

    // 回転状態 0=右, 1=上, 2=左, 3=下（rightPart が leftPart から見てどの方向にあるか）
    private int rotation = 0;

    // 回転状態ごとのグリッド上オフセット（Y は下方向がプラス）
    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(1, 0),  // 右
        new Vector2Int(0, -1), // 上
        new Vector2Int(-1, 0), // 左
        new Vector2Int(0, 1)   // 下
    };

    // カプセルのパーツ
    private GameObject leftPart;
    private GameObject rightPart;

    private void Start()
    {
        CreateCapsule();
        UpdatePosition();
    }

    private void Update()
    {
        if (!isLanded)
        {
            Move();
            Fall();
            Rotate();
        }
    }

    //----------------------------------------------------
    // 移動
    //----------------------------------------------------

    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMove(-1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMove(1);
        }
    }

    private void TryMove(int dx)
    {
        Vector2Int dir = Directions[rotation];

        int newX = gridX + dx;

        // 移動先で両パーツが盤面内かつ空いているか
        if (BoardManager.Instance.IsOccupied(newX, gridY))
            return;

        if (BoardManager.Instance.IsOccupied(newX + dir.x, gridY + dir.y))
            return;

        gridX = newX;
        UpdatePosition();
    }

    //----------------------------------------------------
    // 落下
    //----------------------------------------------------

    private void Fall()
    {
        float interval = fallInterval;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            interval = fastFallInterval;
        }

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

        // 各パーツの1マス下を判定（相方パーツのいるマスは空き扱い）
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

    //----------------------------------------------------
    // 回転
    //----------------------------------------------------

    private void Rotate()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        // 右 → 上 → 左 → 下 → 右... の順に一周（反時計回り）
        int newRotation = (rotation + 1) % 4;
        Vector2Int dir = Directions[newRotation];

        // 回転先が盤面外または埋まっていたら回転しない
        if (BoardManager.Instance.IsOccupied(gridX + dir.x, gridY + dir.y))
            return;

        rotation = newRotation;
        UpdatePartPositions();
    }

    //----------------------------------------------------
    // カプセル生成
    //----------------------------------------------------

    private void CreateCapsule()
    {
        leftPart = CreatePart(Vector3.zero);
        rightPart = CreatePart(GetSubPartLocalPosition());
    }

    private GameObject CreatePart(Vector3 localPos)
    {
        GameObject obj = Instantiate(
            capsulePartPrefab,
            transform);

        obj.transform.localPosition = localPos;

        // ★ 追加：パーツの大きさを固定（マスと揃える）
        obj.transform.localScale = Vector3.one;

        CapsulePart part = obj.GetComponent<CapsulePart>();

        KanjiData kanjiData = KanjiDatabase.GetRandomKanji();

        if (kanjiData != null)
        {
            part.SetKanji(kanjiData);
        }

        return obj;
    }


    //----------------------------------------------------
    // 表示位置更新
    //----------------------------------------------------

    private void UpdatePosition()
    {
        transform.position =
            BoardManager.Instance.GridToWorld(gridX, gridY);
        UpdatePartPositions();
    }

    private void UpdatePartPositions()
    {
        leftPart.transform.localPosition = Vector3.zero;
        rightPart.transform.localPosition = GetSubPartLocalPosition();
    }

    private Vector3 GetSubPartLocalPosition()
    {
        Vector2Int dir = Directions[rotation];
        // ★ 距離を 1 → 0.9 にして「つながり感」を出す
        float distance = 0.9f;

        return new Vector3(dir.x * distance, -dir.y * distance, 0f);
    }

    private void Land()
    {
        isLanded = true;

        Vector2Int dir = Directions[rotation];

        BoardManager.Instance.PlaceCapsule(
            gridX,
            gridY,
            gridX + dir.x,
            gridY + dir.y,
            leftPart,
            rightPart);

        Debug.Log("着地完了");

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
        // 連鎖処理が始まるのを待つ
        yield return null;

        // 連鎖が完全に終わるまで待つ
        while (BoardManager.Instance.IsChainProcessing())
        {
            yield return null;
        }

        // 連鎖終了
        SpawnNextCapsule();
    }


    private System.Collections.IEnumerator WaitForGravityAndSpawn()
    {
        // 重力落下が始まるのを待つ
        yield return null;

        // 重力落下中なら、終わるまで待つ
        while (BoardManager.Instance.IsGravityFalling())
        {
            yield return null;
        }

        // 落下が完全に終わった
        SpawnNextCapsule();
    }

    private void SpawnNextCapsule()
    {
        GameManager.Instance.SpawnCapsule();

        Destroy(gameObject);
    }
}
