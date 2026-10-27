using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

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
    private int clearChainLevel = 1;

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
                    if (board[x, y + 1] != null)
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
