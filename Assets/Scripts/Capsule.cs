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
    // グリッド座標
    private int gridX = 3;
    private int gridY = 0;

    // false=横　true=縦
    private bool isVertical = false;

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
            if (gridX > 0)
            {
                gridX--;
                UpdatePosition();
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int maxX = isVertical ? BoardManager.Width - 1 : BoardManager.Width - 2;

            if (gridX < maxX)
            {
                gridX++;
                UpdatePosition();
            }
        }
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
            if (gridY < BoardManager.Height - 1)
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


    //----------------------------------------------------
    // 回転
    //----------------------------------------------------

    private void Rotate()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (!isVertical)
        {
            // 横 → 縦
            if (gridY > 0)
            {
                isVertical = true;
                rightPart.transform.localPosition = Vector3.up;
            }
        }
        else
        {
            // 縦 → 横
            if (gridX < BoardManager.Width - 1)
            {
                isVertical = false;
                rightPart.transform.localPosition = Vector3.right;
            }
        }
    }

    //----------------------------------------------------
    // カプセル生成
    //----------------------------------------------------

    private void CreateCapsule()
    {
        leftPart = CreatePart(Vector3.zero);
        rightPart = CreatePart(Vector3.right);
    }

    private GameObject CreatePart(Vector3 localPos)
    {
        GameObject obj = Instantiate(
            capsulePartPrefab,
            transform);

        obj.transform.localPosition = localPos;

        CapsulePart part = obj.GetComponent<CapsulePart>();

        CapsuleColor color =
            (CapsuleColor)Random.Range(0, 3);

        part.SetColor(color);

        return obj;
    }

    //----------------------------------------------------
    // 表示位置更新
    //----------------------------------------------------

    private void UpdatePosition()
    {
        transform.position =
            BoardManager.Instance.GridToWorld(gridX, gridY);
    }
    private void Land()
    {
        isLanded = true;

        Debug.Log("着地完了");

        Invoke(nameof(SpawnNextCapsule), 0.5f);
    }

    private void SpawnNextCapsule()
    {
        GameManager.Instance.SpawnCapsule();

        Destroy(gameObject);
    }
}