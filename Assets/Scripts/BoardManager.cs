using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    public const int Width = 8;
    public const int Height = 12;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float cellSize = 1.0f;
    private ParticleSystem clearBurstEffect;
    private MaterialPropertyBlock clearEffectProperties;
    private CapsulePart lastPlacedLeftPart;
    private CapsulePart lastPlacedRightPart;
    private Coroutine clearVibrationCoroutine;
    private Gamepad vibratingGamepad;
    private int clearChainLevel = 1;
    private readonly Dictionary<Vector2Int, TestOkuriganaAnchor> testOkuriganaAnchors =
        new Dictionary<Vector2Int, TestOkuriganaAnchor>();
    private readonly HashSet<Vector2Int> testAnchorPositions =
        new HashSet<Vector2Int>();
    private Material goldKanaMaterial;

    private sealed class TestOkuriganaAnchor
    {
        public string Okurigana;
        public List<TestPreparationWord> Words;
        public List<Vector2Int> OccupiedPositions;
        public GameObject Visual;
    }

    private sealed class TestClearBatch
    {
        public readonly HashSet<Vector2Int> KanjiPositions = new HashSet<Vector2Int>();
        public readonly HashSet<Vector2Int> AnchorPositions = new HashSet<Vector2Int>();
        public readonly List<TestPreparationWord> Words = new List<TestPreparationWord>();
        public int MatchGroupCount;
        public TestPreparationClearType ClearType;
        public readonly HashSet<Vector2Int> StandaloneKanjiPositions = new HashSet<Vector2Int>();
    }

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
        clearEffectProperties = new MaterialPropertyBlock();
        Shader goldKanaShader = Shader.Find("Tsumigame/GoldKana");
        if (goldKanaShader == null)
        {
            Debug.LogError("送り仮名用シェーダー Tsumigame/GoldKana が見つかりません。", this);
        }
        else
        {
            goldKanaMaterial = new Material(goldKanaShader);
            goldKanaMaterial.SetColor("_GoldColor", new Color(0.72f, 0.48f, 0.18f, 1f));
        }

        board = new CapsulePart[Width, Height];

        // Scene内に置かれている演出用パーティクルを取得（初期状態は非表示）。
        foreach (ParticleSystem candidate in Resources.FindObjectsOfTypeAll<ParticleSystem>())
        {
            if (candidate.gameObject.scene.IsValid() &&
                candidate.gameObject.name == "JukugoBurstEffect")
            {
                clearBurstEffect = candidate;
                ConfigureClearBurstEffect();
                break;
            }
        }
    }

    private void OnDestroy()
    {
        StopClearVibration();

        if (goldKanaMaterial != null)
            Destroy(goldKanaMaterial);
    }

    private void OnEnable()
    {
        VibrationSettings.SettingChanged += OnVibrationSettingChanged;
    }

    private void OnDisable()
    {
        VibrationSettings.SettingChanged -= OnVibrationSettingChanged;
        StopClearVibration();
    }


    private IEnumerator Start()
    {
        boardOrigin = new Vector2(
            -(Width - 1) * cellSize / 2f,
             (Height - 1) * cellSize / 2f
        );

        CreateBoard();

        yield return null;

        idiomKanji = new HashSet<string>();

        if (GameModeData.SelectedMode == GameMode.TestPreparation)
        {
            CreateTestOkuriganaAnchors();
        }
        else
        {
            foreach (var entry in GameManager.Instance.CurrentRegistry)
            {
                foreach (var partner in entry.IdiomPartners)
                {
                    idiomKanji.Add(entry.Kanji);
                    idiomKanji.Add(partner);
                }
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
        if (GameModeData.SelectedMode == GameMode.TestPreparation)
        {
            KanjiData data = KanjiDatabase.GetRandomKanji();
            return data != null ? data.kanji : string.Empty;
        }

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
        return IsInsideBoard(x, y) &&
               board[x, y] == null &&
               !testAnchorPositions.Contains(new Vector2Int(x, y));
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
        lastPlacedLeftPart = leftPart;
        lastPlacedRightPart = rightPart;

        ScoreManager.Instance.ResetCombo();
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCombo();
        }

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

        return board[x, y] != null ||
               testAnchorPositions.Contains(new Vector2Int(x, y));
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

        PlayClearVibration();

        board[x, y] = null;

        PlayClearBurstEffect(part.transform.position);

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

    private void PlayClearVibration()
    {
        if (!VibrationSettings.IsEnabled)
        {
            StopClearVibration();
            return;
        }

        if (clearVibrationCoroutine != null)
            return;

#if UNITY_WEBGL && !UNITY_EDITOR
        PlayBrowserControllerVibration(0.35f, 0.65f, 180);
        clearVibrationCoroutine = StartCoroutine(StopClearVibrationAfterDelay(null));
#else
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null)
            return;

        vibratingGamepad = gamepad;
        gamepad.SetMotorSpeeds(0.35f, 0.65f);
        clearVibrationCoroutine = StartCoroutine(StopClearVibrationAfterDelay(gamepad));
#endif
    }

    private IEnumerator StopClearVibrationAfterDelay(Gamepad gamepad)
    {
        yield return new WaitForSecondsRealtime(0.18f);

        if (gamepad != null && vibratingGamepad == gamepad)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
            vibratingGamepad = null;
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        else if (gamepad == null)
        {
            StopBrowserControllerVibration();
        }
#endif
        clearVibrationCoroutine = null;
    }

    private void StopClearVibration()
    {
        if (clearVibrationCoroutine != null)
        {
            StopCoroutine(clearVibrationCoroutine);
            clearVibrationCoroutine = null;
        }

        if (vibratingGamepad != null)
        {
            vibratingGamepad.SetMotorSpeeds(0f, 0f);
            vibratingGamepad = null;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        StopBrowserControllerVibration();
#endif
    }

    private void OnVibrationSettingChanged(bool isEnabled)
    {
        if (!isEnabled)
            StopClearVibration();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void PlayBrowserControllerVibration(
        float weakMagnitude,
        float strongMagnitude,
        int durationMilliseconds);

    [DllImport("__Internal")]
    private static extern void StopBrowserControllerVibration();
#endif

    private void ConfigureClearBurstEffect()
    {
        if (clearBurstEffect == null)
            return;

        ParticleSystem.MainModule main = clearBurstEffect.main;
        main.duration = 0.55f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.55f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2.8f, 4.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.045f, 0.11f);
        main.startColor = Color.white;
        main.stopAction = ParticleSystemStopAction.Destroy;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = clearBurstEffect.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fadeGradient = new Gradient();
        fadeGradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.9f, 0.55f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fadeGradient);

        ParticleSystem.EmissionModule emission = clearBurstEffect.emission;
        emission.rateOverTime = 0f;

        ParticleSystem.ShapeModule shape = clearBurstEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.035f;

        ParticleSystemRenderer particleRenderer = clearBurstEffect.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer != null)
        {
            particleRenderer.sortingOrder = 5;
            // 粒子シェーダーによっては ParticleSystem の色指定が反映されないため、
            // Renderer のマテリアル側にも基準色を設定する。
            Material effectMaterial = particleRenderer.material;
            Color foilGold = new Color(0.68f, 0.61f, 0.46f, 1f);
            if (effectMaterial.HasProperty("_BaseColor"))
                effectMaterial.SetColor("_BaseColor", foilGold);
            if (effectMaterial.HasProperty("_Color"))
                effectMaterial.SetColor("_Color", foilGold);
        }
    }

    private void PlayClearBurstEffect(Vector3 position)
    {
        if (clearBurstEffect == null)
            return;

        // 同じフレームに複数マスを消す場合も各位置に表示できるよう、
        // 漢字ごとに独立したParticleSystemを生成する。
        ParticleSystem effect = Instantiate(
            clearBurstEffect,
            position,
            Quaternion.identity
        );
        effect.gameObject.SetActive(true);
        ApplyClearEffectColor(effect);
        effect.Play(true);
        ParticleSystem.MainModule main = effect.main;
        float chainBonus = Mathf.Clamp(clearChainLevel - 1, 0, 5);
        main.startSpeed = new ParticleSystem.MinMaxCurve(
            2.8f + chainBonus * 0.3f,
            4.8f + chainBonus * 0.55f
        );
        effect.Emit(22 + Mathf.RoundToInt(chainBonus * 4f));
    }

    private void ApplyClearEffectColor(ParticleSystem effect)
    {
        if (GameModeData.SelectedMode != GameMode.Idiom)
        {
            return;
        }

        Color modeColor = CapsulePart.GetModeAccentColor(GameModeData.SelectedMode);

        ParticleSystem.MainModule main = effect.main;
        main.startColor = modeColor;

        Gradient fadeGradient = new Gradient();
        fadeGradient.SetKeys(
            new[]
            {
                new GradientColorKey(modeColor, 0f),
                new GradientColorKey(modeColor, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.9f, 0.55f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = effect.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fadeGradient);

        ParticleSystemRenderer particleRenderer = effect.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer == null || particleRenderer.sharedMaterial == null)
        {
            return;
        }

        clearEffectProperties.Clear();
        particleRenderer.GetPropertyBlock(clearEffectProperties);
        Material material = particleRenderer.sharedMaterial;
        if (material.HasProperty("_BaseColor"))
            clearEffectProperties.SetColor("_BaseColor", modeColor);
        if (material.HasProperty("_Color"))
            clearEffectProperties.SetColor("_Color", modeColor);
        particleRenderer.SetPropertyBlock(clearEffectProperties);
    }

    private void HighlightMatchedParts(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int position in positions)
        {
            CapsulePart part = GetPart(position.x, position.y);
            if (part == null)
                continue;

            SpriteRenderer spriteRenderer = part.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                continue;

            Color originalColor = spriteRenderer.color;
            Color highlightColor = GameModeData.SelectedMode == GameMode.Idiom
                ? Color.Lerp(CapsulePart.IdiomColor, Color.white, 0.28f)
                : new Color(1f, 0.91f, 0.62f, originalColor.a);
            highlightColor.a = originalColor.a;
            spriteRenderer.DOKill();
            spriteRenderer.DOColor(highlightColor, 0.07f)
                .SetLoops(2, LoopType.Yoyo);
        }
    }

    public void PlayGameOverEffect()
    {
        if (lastPlacedLeftPart != null)
            lastPlacedLeftPart.transform.DOShakePosition(0.45f, new Vector3(0.06f, 0.04f, 0f), 12, 90f);
        if (lastPlacedRightPart != null)
            lastPlacedRightPart.transform.DOShakePosition(0.45f, new Vector3(0.06f, 0.04f, 0f), 12, 90f);

        GameObject overlayObject = new GameObject("GameOverDarkOverlay");
        Canvas canvas = overlayObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;

        RectTransform rect = overlayObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image overlay = overlayObject.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0f);
        overlay.raycastTarget = false;
        overlay.DOFade(0.68f, 0.55f).SetUpdate(true);
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

                    // 真下が空いているなら落下する
                    if (board[x, y + 1] != null ||
                        testAnchorPositions.Contains(new Vector2Int(x, y + 1)))
                        continue;

                    // 1マス下へ移動
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

            // 今回まったく動かなければ終了
            if (!movedThisStep)
                break;

            // 1マスずつ落ちる演出
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

    private void CreateTestOkuriganaAnchors()
    {
        IReadOnlyList<TestPreparationWord> words = TestPreparationData.GetSelectedWords();
        var groups = words
            .Where(word => !string.IsNullOrEmpty(word.Okurigana))
            .GroupBy(word => word.Okurigana)
            .ToDictionary(group => group.Key, group => group.ToList());

        if (groups.Count == 0)
        {
            Debug.Log("選択範囲に送り仮名を含む語句がないため、送り仮名は配置しません。", this);
            return;
        }

        List<string> okurigana = groups.Keys.ToList();
        for (int i = okurigana.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            string value = okurigana[i];
            okurigana[i] = okurigana[swapIndex];
            okurigana[swapIndex] = value;
        }

        int anchorCount = Random.Range(1, Mathf.Min(3, okurigana.Count) + 1);
        var positions = new List<Vector2Int>();
        for (int y = 1; y <= Mathf.Min(4, Height - 1); y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (y <= 4 && x >= 2 && x <= 5)
                    continue;

                positions.Add(new Vector2Int(x, y));
            }
        }

        for (int i = 0; i < anchorCount; i++)
        {
            string token = okurigana[i];

            var kanaSprites = new List<Sprite>(token.Length);
            bool hasAllKanaSprites = true;
            foreach (char kana in token)
            {
                Sprite sprite = TestPreparationData.LoadTestSprite(kana.ToString());
                if (sprite == null)
                {
                    Debug.LogError(
                        "送り仮名の画像が見つかりません: Resources/KanjiSprites/Test/" + kana,
                        this);
                    hasAllKanaSprites = false;
                    break;
                }
                kanaSprites.Add(sprite);
            }

            if (!hasAllKanaSprites)
                continue;

            int requiredKanjiCells = groups[token]
                .Max(word => word.KanjiSequence.Length);
            List<Vector2Int> candidates = positions
                .Where(candidate => candidate.x >= requiredKanjiCells &&
                    candidate.x + token.Length <= Width &&
                    Enumerable.Range(0, token.Length).All(kanaIndex =>
                        !IsInMiddleColumns(candidate.x + kanaIndex)))
                .OrderBy(_ => Random.value)
                .ToList();
            Vector2Int position = default;
            bool foundPosition = false;
            foreach (Vector2Int candidate in candidates)
            {
                bool cellsAvailable = true;
                for (int kanaIndex = 0; kanaIndex < token.Length; kanaIndex++)
                {
                    Vector2Int cell = new Vector2Int(candidate.x + kanaIndex, candidate.y);
                    if (testAnchorPositions.Contains(cell) ||
                        testAnchorPositions.Any(existing => GetGridDistance(existing, cell) < 2))
                    {
                        cellsAvailable = false;
                        break;
                    }
                }

                if (!cellsAvailable)
                    continue;

                position = candidate;
                foundPosition = true;
                break;
            }

            if (!foundPosition)
            {
                Debug.LogWarning("送り仮名を重ならずに配置できるマスがありません: " + token, this);
                continue;
            }

            var visual = new GameObject("Okurigana_" + token);
            visual.transform.SetParent(transform, true);
            visual.transform.position = GridToWorld(position.x, position.y);
            var occupiedPositions = new List<Vector2Int>(token.Length);

            for (int kanaIndex = 0; kanaIndex < kanaSprites.Count; kanaIndex++)
            {
                Vector2Int kanaPosition = new Vector2Int(position.x + kanaIndex, position.y);
                GameObject kanaObject = new GameObject(
                    "Kana_" + token[kanaIndex],
                    typeof(SpriteRenderer));
                kanaObject.transform.SetParent(visual.transform, false);
                kanaObject.transform.localPosition = new Vector3(
                    kanaIndex * cellSize,
                    0f,
                    0f);
                kanaObject.transform.localScale = Vector3.one * cellSize;

                SpriteRenderer spriteRenderer = kanaObject.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = kanaSprites[kanaIndex];
                spriteRenderer.sharedMaterial = goldKanaMaterial;
                spriteRenderer.color = Color.white;
                spriteRenderer.sortingOrder = 8;

                testAnchorPositions.Add(kanaPosition);
                occupiedPositions.Add(kanaPosition);
            }

            testOkuriganaAnchors.Add(position, new TestOkuriganaAnchor
            {
                Okurigana = token,
                Words = groups[token],
                OccupiedPositions = occupiedPositions,
                Visual = visual
            });
        }

        Debug.Log(
            $"送り仮名を{testOkuriganaAnchors.Count}個配置しました（候補{groups.Count}種類）。",
            this);
    }

    private static bool IsInMiddleColumns(int x)
    {
        return x >= Width / 2 - 1 && x <= Width / 2;
    }

    private static int GetGridDistance(Vector2Int first, Vector2Int second)
    {
        return Mathf.Abs(first.x - second.x) + Mathf.Abs(first.y - second.y);
    }

    private TestClearBatch FindTestClearBatch()
    {
        var batch = new TestClearBatch();
        IReadOnlyList<TestPreparationWord> words = TestPreparationData.GetSelectedWords();
        foreach (TestPreparationWord word in words)
        {
            if (word.Okurigana.Length > 0 || word.KanjiSequence.Length < 2)
                continue;

            foreach (Vector2Int direction in new[]
            {
                new Vector2Int(1, 0),
                new Vector2Int(0, 1)
            })
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (MatchesTestWordAt(x, y, direction, word.KanjiSequence))
                        {
                            for (int i = 0; i < word.KanjiSequence.Length; i++)
                                batch.KanjiPositions.Add(new Vector2Int(x + direction.x * i, y + direction.y * i));
                            AddTestWord(batch, word);
                            batch.MatchGroupCount++;
                            batch.ClearType |= TestPreparationClearType.Idiom;
                        }
                    }
                }
            }
        }

        AddTestRuns(batch, true);
        AddTestRuns(batch, false);
        AddOkuriganaMatches(batch);
        AddStandaloneKanji(batch);
        return batch;
    }

    private void AddStandaloneKanji(TestClearBatch batch)
    {
        IReadOnlyList<TestPreparationWord> words = TestPreparationData.GetSelectedWords();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                CapsulePart part = board[x, y];
                if (part == null || testAnchorPositions.Contains(position))
                    continue;

                if (batch.KanjiPositions.Contains(position))
                    continue;

                KanjiData data = part.GetKanjiData();
                if (data == null)
                    continue;

                TestPreparationWord standaloneWord = words.FirstOrDefault(word =>
                    string.IsNullOrEmpty(word.Okurigana) &&
                    word.KanjiSequence.Length == 1 &&
                    word.KanjiSequence == data.kanji);
                if (standaloneWord == null)
                    continue;

                batch.KanjiPositions.Add(position);
                batch.StandaloneKanjiPositions.Add(position);
                AddTestWord(batch, standaloneWord);
                batch.MatchGroupCount++;
                batch.ClearType |= TestPreparationClearType.StandaloneKanji;
            }
        }
    }

    private bool MatchesTestWordAt(int x, int y, Vector2Int direction, string sequence)
    {
        int endX = x + direction.x * (sequence.Length - 1);
        int endY = y + direction.y * (sequence.Length - 1);
        if (!IsInsideBoard(endX, endY))
            return false;

        for (int i = 0; i < sequence.Length; i++)
        {
            int currentX = x + direction.x * i;
            int currentY = y + direction.y * i;
            CapsulePart part = board[currentX, currentY];
            if (part == null || part.Kanji != sequence[i].ToString())
                return false;
        }
        return true;
    }

    private void AddTestRuns(TestClearBatch batch, bool matchKanji)
    {
        Vector2Int[] directions =
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1)
        };

        foreach (Vector2Int direction in directions)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    CapsulePart first = board[x, y];
                    if (first == null || testAnchorPositions.Contains(new Vector2Int(x, y)))
                        continue;

                    int previousX = x - direction.x;
                    int previousY = y - direction.y;
                    if (IsInsideBoard(previousX, previousY) &&
                        IsSameTestGroup(first, board[previousX, previousY], matchKanji))
                    {
                        continue;
                    }

                    var run = new List<Vector2Int>();
                    int currentX = x;
                    int currentY = y;
                    while (IsInsideBoard(currentX, currentY) &&
                           IsSameTestGroup(first, board[currentX, currentY], matchKanji))
                    {
                        run.Add(new Vector2Int(currentX, currentY));
                        currentX += direction.x;
                        currentY += direction.y;
                    }

                    if (run.Count < 3)
                        continue;

                    foreach (Vector2Int position in run)
                        batch.KanjiPositions.Add(position);
                    batch.MatchGroupCount++;
                    batch.ClearType |= matchKanji
                        ? TestPreparationClearType.SameKanji
                        : TestPreparationClearType.SameStrokeCount;
                }
            }
        }
    }

    private static bool IsSameTestGroup(CapsulePart first, CapsulePart second, bool matchKanji)
    {
        if (first == null || second == null)
            return false;

        KanjiData firstData = first.GetKanjiData();
        KanjiData secondData = second.GetKanjiData();
        if (firstData == null || secondData == null)
            return false;

        if (matchKanji)
            return firstData.kanji == secondData.kanji;

        return firstData.strokeCount > 0 &&
               firstData.strokeCount == secondData.strokeCount;
    }

    private void AddOkuriganaMatches(TestClearBatch batch)
    {
        foreach (KeyValuePair<Vector2Int, TestOkuriganaAnchor> pair in testOkuriganaAnchors)
        {
            Vector2Int anchor = pair.Key;
            Vector2Int[] directions =
            {
                new Vector2Int(1, 0),
                new Vector2Int(0, 1)
            };

            foreach (Vector2Int direction in directions)
            {
                foreach (TestPreparationWord word in pair.Value.Words)
                {
                    int startX = anchor.x - direction.x * word.KanjiSequence.Length;
                    int startY = anchor.y - direction.y * word.KanjiSequence.Length;
                    if (!MatchesTestWordAt(startX, startY, direction, word.KanjiSequence))
                        continue;

                    for (int i = 0; i < word.KanjiSequence.Length; i++)
                    {
                        batch.KanjiPositions.Add(new Vector2Int(
                            startX + direction.x * i,
                            startY + direction.y * i));
                    }
                    batch.AnchorPositions.Add(anchor);
                    AddTestWord(batch, word);
                    batch.MatchGroupCount++;
                    batch.ClearType |= TestPreparationClearType.Okurigana;
                    break;
                }

                if (batch.AnchorPositions.Contains(anchor))
                    break;
            }
        }
    }

    private static void AddTestWord(TestClearBatch batch, TestPreparationWord word)
    {
        if (!batch.Words.Any(existing => existing.Surface == word.Surface))
            batch.Words.Add(word);
    }

    private void RemoveTestAnchor(Vector2Int position)
    {
        if (!testOkuriganaAnchors.TryGetValue(position, out TestOkuriganaAnchor anchor))
            return;

        testOkuriganaAnchors.Remove(position);
        foreach (Vector2Int occupiedPosition in anchor.OccupiedPositions)
            testAnchorPositions.Remove(occupiedPosition);
        if (anchor.Visual != null)
            Destroy(anchor.Visual);
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
        int chainCount = 0;

        while (true)
        {
            //==================================================
            // 一時停止中なら、再開するまで待つ
            //==================================================

            while (GamePauseManager.Instance != null &&
                   GamePauseManager.Instance.IsPaused)
            {
                yield return null;
            }


            //==================================================
            // 現在のゲームモードを取得
            //==================================================

            GameManager gameManager =
                GameManager.Instance;

            if (gameManager == null)
            {
                break;
            }

            if (gameManager.IsTestPreparationMode())
            {
                TestClearBatch batch = FindTestClearBatch();
                if (batch.KanjiPositions.Count == 0 &&
                    batch.AnchorPositions.Count == 0)
                {
                    break;
                }

                chainCount++;
                clearChainLevel = chainCount;
                HighlightMatchedParts(new HashSet<Vector2Int>(
                    batch.KanjiPositions.Except(batch.StandaloneKanjiPositions)));
                yield return new WaitForSeconds(0.14f);

                var clearedKanji = new List<KanjiData>();
                foreach (Vector2Int position in batch.KanjiPositions)
                {
                    CapsulePart part = GetPart(position.x, position.y);
                    if (part != null)
                        clearedKanji.Add(part.GetKanjiData());
                }

                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddTestScore(
                        batch.KanjiPositions.Count,
                        batch.Words.Count,
                        batch.MatchGroupCount,
                        batch.AnchorPositions.Count);
                }

                if (GameResultManager.Instance != null)
                {
                    foreach (TestPreparationWord word in batch.Words)
                        GameResultManager.Instance.AddIdiom(word.Surface);
                }

                gameManager.OnTestPreparationCleared(
                    clearedKanji,
                    batch.Words);

                GameManager.Instance.PlaySE(GameManager.Instance.comboSE);
                foreach (Vector2Int position in batch.AnchorPositions)
                    RemoveTestAnchor(position);
                foreach (Vector2Int position in batch.KanjiPositions)
                    RemovePart(position.x, position.y);

                ApplyGravity();
                yield return null;
                while (isGravityFalling)
                    yield return null;
                yield return null;
                continue;
            }


            //==================================================
            // 隣接する成立ペアを探す
            //==================================================

            List<IdiomMatch> matches =
                KanjiMatchFinder.FindIdiomMatches();

            if (matches.Count == 0)
            {
                break;
            }

            chainCount++;
            clearChainLevel = chainCount;


            //==================================================
            // 熟語モード
            //==================================================

            if (gameManager.IsIdiomMode())
            {
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


                    //==================================================
                    // 辞書に存在する熟語だけ有効
                    //==================================================

                    if (!IdiomDictionary.Contains(idiom))
                        continue;


                    //==================================================
                    // 同じ熟語を重複して数えない
                    //==================================================

                    if (!idioms.Contains(idiom))
                    {
                        idioms.Add(idiom);
                    }


                    //==================================================
                    // 実際に消す漢字の位置を記録
                    //==================================================

                    uniquePositions.Add(match.First);
                    uniquePositions.Add(match.Second);
                }


                //==================================================
                // 有効な熟語がなければ終了
                //==================================================

                if (idioms.Count == 0)
                    break;

                HighlightMatchedParts(uniquePositions);
                yield return new WaitForSeconds(0.14f);


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
                // 今回のゲームで作った熟語を記録
                //==================================================

                if (GameResultManager.Instance != null)
                {
                    foreach (string idiom in idioms)
                    {
                        GameResultManager.Instance.AddIdiom(idiom);
                    }
                }


                //==================================================
                // 熟語表示
                //==================================================

                GameManager.Instance.OnIdiomsCreated(
                    idioms
                );


                //==================================================
                // 読み・意味・構造
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


                //==================================================
                // 重力が終わるまで待つ
                //==================================================

                while (isGravityFalling)
                {
                    yield return null;
                }


                //==================================================
                // 1フレーム待つ
                //==================================================

                yield return null;

                continue;
            }


            //==================================================
            // 部首モード
            //==================================================

            if (gameManager.IsRadicalMode())
            {
                //==================================================
                // 実際に消すマス
                //==================================================

                HashSet<Vector2Int> uniquePositions =
                    new HashSet<Vector2Int>();


                //==================================================
                // 同じ部首のペアを記録
                //==================================================

                foreach (IdiomMatch match in matches)
                {
                    uniquePositions.Add(match.First);
                    uniquePositions.Add(match.Second);
                }


                //==================================================
                // 有効なペアがなければ終了
                //==================================================

                if (uniquePositions.Count == 0)
                    break;

                // 熟語モードと同じ消去前の強調演出を行う
                HighlightMatchedParts(uniquePositions);
                yield return new WaitForSeconds(0.14f);


                //==================================================
                // 部首モードでは
                // 熟語の読み・意味・構造は表示しない
                //==================================================

                if (ReadingText != null)
                {
                    ReadingText.text = "";
                }

                if (MeaningText != null)
                {
                    MeaningText.text = "";
                }

                List<KanjiData> radicalDisplays = new List<KanjiData>();
                List<KanjiData> clearedKanji = new List<KanjiData>();
                HashSet<string> seenKanji = new HashSet<string>();
                HashSet<string> seenRadicals = new HashSet<string>();
                foreach (Vector2Int pos in uniquePositions)
                {
                    CapsulePart part = GetPart(pos.x, pos.y);
                    KanjiData data = part != null
                        ? part.GetKanjiData()
                        : null;

                    if (data != null &&
                        !string.IsNullOrEmpty(data.radical))
                    {
                        if (seenKanji.Add(data.kanji))
                        {
                            clearedKanji.Add(data);
                        }

                        if (seenRadicals.Add(KanjiDatabase.GetRadicalDisplayKey(data)))
                        {
                            radicalDisplays.Add(data);
                        }
                    }
                }

                GameManager.Instance.OnRadicalsCleared(
                    radicalDisplays,
                    clearedKanji);


                //==================================================
                // 部首モードのスコア
                //
                // 1字200点に、2組目以降の同時消しボーナスを加算
                //==================================================

                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddRadicalScore(
                        uniquePositions.Count,
                        matches.Count
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


                //==================================================
                // 重力が終わるまで待つ
                //==================================================

                while (isGravityFalling)
                {
                    yield return null;
                }


                //==================================================
                // 1フレーム待つ
                //==================================================

                yield return null;

                continue;
            }


            //==================================================
            // 読みモード
            //==================================================

            if (gameManager.IsReadingMode())
            {
                // 読みモードはまだ未実装
                break;
            }


            //==================================================
            // 不明なモード
            //==================================================

            break;
        }


        isChainProcessing = false;
    }

    public bool IsChainProcessing()
    {
        return isChainProcessing;
    }

}
