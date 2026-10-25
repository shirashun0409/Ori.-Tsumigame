using UnityEngine;
using UnityEngine.InputSystem;
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
    private bool currentCapsuleInitialized = false;
    private int horizontalInputDirection;
    private float horizontalRepeatTimer;

    private const float HorizontalInitialRepeatDelay = 0.24f;
    private const float HorizontalRepeatInterval = 0.12f;
    private const float StickDeadzone = 0.55f;

    private int gridX = 3;
    private int gridY = 0;
    private int rotation = 0;

    public bool isNextPreview = false;

    private GameObject leftPart;
    private GameObject rightPart;
    private GameObject leftLandingGhost;
    private GameObject rightLandingGhost;

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
        CreateLandingGhosts();
        UpdateLandingGhosts();
        currentCapsuleInitialized = true;
    }

    private void Update()
    {
        if (GamePauseManager.Instance != null &&
            GamePauseManager.Instance.IsPaused)
        {
            return;
        }

        if (!isLanded && !isNextPreview)
        {
            if (!currentCapsuleInitialized)
                InitializePromotedCapsule();

            Move();
            Fall();
            Rotate();
            UpdateLandingGhosts();
        }
    }

    private void InitializePromotedCapsule()
    {
        if (leftPart == null || rightPart == null)
            CreateCapsule();

        if (nextLeftKanji != null && nextRightKanji != null)
        {
            leftPart.GetComponent<CapsulePart>().SetKanji(nextLeftKanji);
            rightPart.GetComponent<CapsulePart>().SetKanji(nextRightKanji);
        }

        UpdatePosition();
        CreateLandingGhosts();
        UpdateLandingGhosts();
        currentCapsuleInitialized = true;
    }

    private void Move()
    {
        Gamepad gamepad = Gamepad.current;
        float stickX = gamepad != null
            ? gamepad.leftStick.ReadValue().x
            : 0f;

        bool leftHeld = Input.GetKey(KeyCode.LeftArrow) ||
            (gamepad != null && (gamepad.dpad.left.isPressed || stickX < -StickDeadzone));
        bool rightHeld = Input.GetKey(KeyCode.RightArrow) ||
            (gamepad != null && (gamepad.dpad.right.isPressed || stickX > StickDeadzone));

        int direction = leftHeld == rightHeld ? 0 : leftHeld ? -1 : 1;
        if (direction == 0)
        {
            horizontalInputDirection = 0;
            horizontalRepeatTimer = 0f;
            return;
        }

        if (direction != horizontalInputDirection)
        {
            TryMove(direction);
            horizontalRepeatTimer = HorizontalInitialRepeatDelay;
        }
        else
        {
            horizontalRepeatTimer -= Time.deltaTime;
            if (horizontalRepeatTimer <= 0f)
            {
                TryMove(direction);
                horizontalRepeatTimer = HorizontalRepeatInterval;
            }
        }

        horizontalInputDirection = direction;
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
        Gamepad gamepad = Gamepad.current;
        bool fastFall = Input.GetKey(KeyCode.DownArrow) ||
            (gamepad != null &&
             (gamepad.dpad.down.isPressed ||
              gamepad.leftStick.ReadValue().y < -StickDeadzone));
        float interval = fastFall ? fastFallInterval : fallInterval;

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
        Gamepad gamepad = Gamepad.current;
        bool rotatePressed = Input.GetKeyDown(KeyCode.Space) ||
            (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame);
        if (!rotatePressed)
            return;

        int newRotation = (rotation + 1) % Directions.Length;
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
        ClearLandingGhosts();

        Vector2Int dir = Directions[rotation];

        BoardManager.Instance.PlaceCapsule(
            gridX,
            gridY,
            gridX + dir.x,
            gridY + dir.y,
            leftPart,
            rightPart);

        GameManager.Instance.PlaySE(GameManager.Instance.dropSE);

        if (GameManager.Instance.IsIdiomMode() ||
            GameManager.Instance.IsRadicalMode())
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

    private void CreateLandingGhosts()
    {
        leftLandingGhost = CreateLandingGhost(leftPart);
        rightLandingGhost = CreateLandingGhost(rightPart);
    }

    private GameObject CreateLandingGhost(GameObject sourceObject)
    {
        if (!GameModeData.ShadowEnabled)
            return null;

        if (sourceObject == null)
            return null;

        SpriteRenderer source = sourceObject.GetComponent<SpriteRenderer>();
        if (source == null || source.sprite == null)
            return null;

        GameObject ghost = new GameObject("LandingGhost");
        SpriteRenderer renderer = ghost.AddComponent<SpriteRenderer>();
        renderer.sprite = source.sprite;
        renderer.sharedMaterial = source.sharedMaterial;
        renderer.sortingLayerID = source.sortingLayerID;
        renderer.sortingOrder = source.sortingOrder + 1;
        Color ghostColor = CapsulePart.GetModeAccentColor(GameModeData.SelectedMode);
        ghostColor.a = 0.58f;
        renderer.color = ghostColor;
        return ghost;
    }

    private void UpdateLandingGhosts()
    {
        if (isLanded || leftLandingGhost == null || rightLandingGhost == null)
            return;

        Vector2Int direction = Directions[rotation];
        int ghostY = gridY;
        int subX = gridX + direction.x;

        while (true)
        {
            int subY = ghostY + direction.y;
            bool baseCanFall = CanOccupy(gridX, ghostY + 1, subX, subY);
            bool subCanFall = CanOccupy(subX, subY + 1, gridX, ghostY);
            if (!baseCanFall || !subCanFall)
                break;

            ghostY++;
        }

        leftLandingGhost.transform.position = BoardManager.Instance.GridToWorld(gridX, ghostY);
        rightLandingGhost.transform.position = BoardManager.Instance.GridToWorld(
            subX,
            ghostY + direction.y
        );
    }

    private void ClearLandingGhosts()
    {
        if (leftLandingGhost != null)
            Destroy(leftLandingGhost);
        if (rightLandingGhost != null)
            Destroy(rightLandingGhost);

        leftLandingGhost = null;
        rightLandingGhost = null;
    }

    private void OnDestroy()
    {
        ClearLandingGhosts();
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
