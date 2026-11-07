using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public static int LastScore;
    public static GameManager Instance { get; private set; }

    //==================================================
    // 今回のゲームでランキング登録したか
    //==================================================

    public static bool RankingRegisteredThisGame = false;


    [Header("Prefab")]
    [SerializeField] private GameObject capsulePrefab;


    [Header("Game Mode")]
    [SerializeField]
    private GameMode currentMode =
        GameMode.Idiom;


    private Capsule currentCapsule;
    private Capsule nextCapsule;

    private bool isGameOver = false;

    public KanjiRegistry.Entry[] CurrentRegistry;

    [Header("Sound Effects")]
    public AudioClip dropSE;
    public AudioClip eraseSE;
    public AudioClip comboSE;
    public AudioClip rotateSE;
    public AudioClip buttonSE;

    // カウントダウンSE
    public AudioClip countThreeSE;
    public AudioClip countTwoSE;
    public AudioClip countOneSE;
    public AudioClip startSE;
    private AudioSource audioSource;


    //==================================================
    // BGM
    //==================================================

    [Header("Game BGM")]
    [SerializeField] private AudioClip gameBGM;

    [Tooltip("BGMが最大音量になるまでの時間")]
    [SerializeField] private float bgmFadeInDuration = 2.0f;

    [SerializeField] private float bgmVolume = 0.5f;

    private AudioSource bgmAudioSource;


    [Header("UI")]
    [SerializeField] private KanjiDisplay kanjiDisplay;
    [SerializeField] private MeaningDisplay meaningDisplay;
    [SerializeField] private NextDisplay nextDisplay;
    private TMP_Text modeNameText;




    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;


        //==================================================
        // SE用AudioSource
        //==================================================

        audioSource =
            GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource =
                gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;


        //==================================================
        // BGM用AudioSource
        //==================================================

        bgmAudioSource =
            gameObject.AddComponent<AudioSource>();

        bgmAudioSource.playOnAwake = false;
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = 0f;
        AudioVolumeSettings.RegisterBgmSource(bgmAudioSource, bgmVolume);
        AudioVolumeSettings.SetBgmFade(bgmAudioSource, bgmVolume, 0f);
        AudioVolumeSettings.RegisterSeSource(audioSource);
    }

    private void OnDestroy()
    {
        AudioVolumeSettings.UnregisterSource(audioSource);
        AudioVolumeSettings.UnregisterSource(bgmAudioSource);
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.ResetResult();
        }

        //==================================================
        // SE用AudioSource
        //==================================================

        audioSource =
            GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource =
                gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        //==================================================
        // StartSceneで選択したゲームモードを受け取る
        //==================================================

        currentMode = GameModeData.SelectedMode;
        InitializeModeNameDisplay();
        UpdateModeNameDisplay();

        Debug.Log(
            "GameSceneで受け取ったゲームモード: " +
            currentMode
        );

        CurrentRegistry =
            KanjiRegistry.Entries;

        if (kanjiDisplay != null)
        {
            kanjiDisplay.SetIdiom("");
        }

        if (meaningDisplay != null)
        {
            meaningDisplay.SetMeaning("");
        }

        // ★ ここではまだゲーム開始しない
    }
    public void StartGame()
    {
        //==================================================
        // ★ 新しいゲーム開始時にランキング登録状態をリセット
        //==================================================

        RankingRegisteredThisGame = false;

        PlayerPrefs.DeleteKey(
            GameModeData.GetModeSpecificKey("LastRankingRegistered"));
        PlayerPrefs.DeleteKey(
            GameModeData.GetModeSpecificKey("LastRankingNickname"));
        PlayerPrefs.DeleteKey(
            GameModeData.GetModeSpecificKey("LastRankingScore"));

        PlayerPrefs.Save();

        Debug.Log(
            "新しいゲーム開始：ランキング登録状態をリセットしました。"
        );


        //==================================================
        // ゲーム開始
        //==================================================

        SpawnCapsule();
    }


    //==================================================
    // ランキング登録済みにする
    //==================================================

    public static void SetRankingRegistered()
    {
        RankingRegisteredThisGame = true;
    }


    //==================================================
    // SE
    //==================================================

    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonSE()
    {
        if (buttonSE != null)
        {
            audioSource.PlayOneShot(buttonSE);
        }
    }
    //==================================================
    // ゲームオーバー判定
    //==================================================

    private bool CanSpawnCapsule()
    {
        int spawnX = 3;
        int spawnY = 0;

        int secondX = spawnX + 1;
        int secondY = spawnY;

        if (BoardManager.Instance.IsOccupied(spawnX, spawnY))
            return false;

        if (BoardManager.Instance.IsOccupied(secondX, secondY))
            return false;

        return true;
    }


    //==================================================
    // カプセル生成
    //==================================================

    public void SpawnCapsule()
    {
        if (isGameOver)
            return;

        if (!CanSpawnCapsule())
        {
            GameOver();
            return;
        }

        if (nextCapsule == null)
        {
            GameObject firstObj =
                Instantiate(capsulePrefab);

            nextCapsule =
                firstObj.GetComponent<Capsule>();

            nextCapsule.isNextPreview =
                true;

            KanjiData left =
                KanjiDatabase.GetRandomKanji();

            KanjiData right =
                KanjiDatabase.GetRandomKanji();

            nextCapsule.SetKanjiForNext(
                left,
                right
            );

            firstObj.transform.position =
                new Vector3(
                    999,
                    999,
                    0
                );

            nextDisplay.SetNextKanji(
                nextCapsule.nextLeftKanji,
                nextCapsule.nextRightKanji
            );
        }


        currentCapsule =
            nextCapsule;

        currentCapsule.transform.position =
            BoardManager.Instance.GridToWorld(
                3,
                0
            );

        currentCapsule.isNextPreview =
            false;


        GameObject obj =
            Instantiate(capsulePrefab);

        nextCapsule =
            obj.GetComponent<Capsule>();

        nextCapsule.isNextPreview =
            true;


        KanjiData nextLeft =
            KanjiDatabase.GetRandomKanji();

        KanjiData nextRight =
            KanjiDatabase.GetRandomKanji();


        nextCapsule.SetKanjiForNext(
            nextLeft,
            nextRight
        );


        obj.transform.position =
            new Vector3(
                999,
                999,
                0
            );


        nextDisplay.SetNextKanji(
            nextCapsule.nextLeftKanji,
            nextCapsule.nextRightKanji
        );
    }


    //==================================================
    // 熟語1個表示
    //==================================================

    public void OnIdiomCreated(string idiom)
    {
        List<string> idioms =
            new List<string>();

        if (!string.IsNullOrEmpty(idiom))
        {
            idioms.Add(idiom);
        }

        OnIdiomsCreated(idioms);
    }


    //==================================================
    // 複数の熟語を表示
    //==================================================

    public void OnIdiomsCreated(
        List<string> idioms)
    {
        if (idioms == null ||
            idioms.Count == 0)
        {
            return;
        }


        if (kanjiDisplay != null)
        {
            kanjiDisplay.SetIdioms(
                idioms
            );
        }


        if (meaningDisplay != null)
        {
            List<string> meanings =
                new List<string>();


            foreach (string idiom in idioms)
            {
                string meaning =
                    IdiomDictionary.GetMeaning(
                        idiom
                    );


                if (!string.IsNullOrEmpty(meaning))
                {
                    meanings.Add(
                        meaning
                    );
                }
            }


            meaningDisplay.SetMeaning(
                string.Join(
                    "\n",
                    meanings
                )
            );
        }
    }

    public void OnRadicalsCleared(
        List<KanjiData> radicals,
        List<KanjiData> clearedKanji)
    {
        if (radicals == null || radicals.Count == 0)
        {
            return;
        }

        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.AddRadicals(radicals);
        }

        if (kanjiDisplay != null)
        {
            kanjiDisplay.SetRadicals(radicals);
        }

        if (meaningDisplay != null)
        {
            List<string> explanations = new List<string>();
            var excludedKanji = new HashSet<string>();
            foreach (KanjiData data in clearedKanji)
            {
                excludedKanji.Add(data.kanji);
            }

            foreach (KanjiData radical in radicals)
            {
                explanations.Add(
                    KanjiDatabase.GetRadicalExplanation(radical, excludedKanji));
            }

            meaningDisplay.SetMeaning(string.Join("\n\n", explanations));
        }
    }

    public void OnTestPreparationCleared(
        List<KanjiData> clearedKanji,
        List<TestPreparationWord> clearedWords)
    {
        var readings = new List<string>();
        var displayTerms = new List<string>();
        var studyDetails = new List<string>();
        var seenKanji = new HashSet<string>();
        var representedKanji = new HashSet<string>();
        var seenWords = new HashSet<string>();
        foreach (TestPreparationWord word in clearedWords)
        {
            if (word == null || !seenWords.Add(word.Surface))
                continue;

            readings.Add(word.Reading);
            displayTerms.Add(word.Surface);
            foreach (char character in word.KanjiSequence)
                representedKanji.Add(character.ToString());

            if (!string.IsNullOrEmpty(word.Okurigana))
            {
                studyDetails.Add(
                    $"{word.Surface}（読み：{word.Reading}）\n送り仮名：{word.Okurigana}");
            }
        }

        IReadOnlyList<TestPreparationWord> selectedWords =
            TestPreparationData.GetSelectedWords();
        foreach (KanjiData data in clearedKanji)
        {
            if (data == null || !seenKanji.Add(data.kanji))
                continue;

            string reading = data.readings != null && data.readings.Length > 0
                ? string.Join("・", data.readings)
                : "未登録";
            if (!representedKanji.Contains(data.kanji))
                readings.Add(reading);

            List<string> examples = selectedWords
                .Where(word => word.KanjiSequence.Contains(data.kanji))
                .Select(word => word.Surface + "（" + word.Reading + "）")
                .Distinct()
                .Take(2)
                .ToList();
            List<string> okurigana = selectedWords
                .Where(word => word.KanjiSequence.Contains(data.kanji) &&
                               !string.IsNullOrEmpty(word.Okurigana))
                .Select(word => word.Okurigana)
                .Distinct()
                .ToList();
            if (TestPreparationData.IsTestPreparationKanji(data.kanji))
            {
                studyDetails.Add(
                    $"{data.kanji}（読み：{reading}）\n" +
                    $"部首：{data.radical}　総画数：{data.strokeCount}画\n" +
                    "熟語・例文：" + (examples.Count > 0 ? string.Join("、", examples) : "該当語なし") +
                    (okurigana.Count > 0 ? "\n送り仮名：" + string.Join("、", okurigana) : string.Empty));
            }
            if (!representedKanji.Contains(data.kanji))
                displayTerms.Add(data.kanji);
        }

        if (kanjiDisplay != null)
            kanjiDisplay.SetIdioms(displayTerms.Distinct().ToList());

        string readingText = string.Join("　", readings.Distinct());
        string studyText = string.Join("\n\n", studyDetails.Distinct());
        if (meaningDisplay != null)
            meaningDisplay.SetMeaning(studyText);

        if (BoardManager.Instance != null)
        {
            if (BoardManager.Instance.ReadingText != null)
                BoardManager.Instance.ReadingText.text = readingText;
            if (BoardManager.Instance.MeaningText != null)
            {
                BoardManager.Instance.MeaningText.enableAutoSizing = true;
                BoardManager.Instance.MeaningText.fontSizeMin = 10f;
                BoardManager.Instance.MeaningText.fontSizeMax =
                    BoardManager.Instance.MeaningText.fontSize;
                BoardManager.Instance.MeaningText.text = studyText;
            }
        }

        if (meaningDisplay != null)
        {
            TMP_Text text = meaningDisplay.GetComponentInChildren<TMP_Text>(true);
            if (text != null)
            {
                text.enableAutoSizing = true;
                text.fontSizeMin = 10f;
                text.fontSizeMax = text.fontSize;
            }
        }
    }

    //==================================================
    // ゲームモード
    //==================================================

    public void SetGameMode(
        GameMode mode)
    {
        currentMode = mode;
        UpdateModeNameDisplay();

        Debug.Log(
            "ゲームモード変更: " +
            currentMode
        );
    }

    private void InitializeModeNameDisplay()
    {
        GameObject panel = GameObject.Find("ModenamePanel");
        Transform panelTransform = panel != null ? panel.transform : null;
        if (panelTransform != null)
        {
            Transform labelTransform = panelTransform.Find("Modename");
            if (labelTransform != null)
                modeNameText = labelTransform.GetComponent<TMP_Text>();
        }

        if (modeNameText != null)
            return;

        Canvas canvas = null;
        if (kanjiDisplay != null)
            canvas = kanjiDisplay.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("GameManager: ゲームモード名を表示するCanvasが見つかりません。", this);
            return;
        }

        if (panelTransform == null)
        {
            GameObject panelObject = new GameObject("ModenamePanel", typeof(RectTransform));
            panelTransform = panelObject.transform;
            panelTransform.SetParent(canvas.transform, false);

            RectTransform panelRect = panelObject.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 1f);
            panelRect.anchorMax = new Vector2(0.5f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.anchoredPosition = new Vector2(0f, -80f);
            panelRect.sizeDelta = new Vector2(600f, 80f);
        }

        GameObject labelObject = new GameObject(
            "Modename",
            typeof(RectTransform),
            typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(panelTransform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        modeNameText = labelObject.GetComponent<TextMeshProUGUI>();
        TMP_Text referenceText = kanjiDisplay.GetComponentInChildren<TMP_Text>(true);
        if (referenceText != null)
            modeNameText.font = referenceText.font;
        modeNameText.fontSize = 32f;
        modeNameText.color = Color.white;
        modeNameText.alignment = TextAlignmentOptions.Center;
        modeNameText.raycastTarget = false;
    }

    private void UpdateModeNameDisplay()
    {
        if (modeNameText == null)
            return;

        switch (currentMode)
        {
            case GameMode.Idiom:
                modeNameText.text = "熟語モード";
                break;
            case GameMode.Radical:
                modeNameText.text = "部首モード";
                break;
            case GameMode.TestPreparation:
                modeNameText.text = "テスト対策モード";
                break;
            case GameMode.Reading:
                modeNameText.text = "読みモード";
                break;
            default:
                modeNameText.text = string.Empty;
                break;
        }
    }

    public GameMode GetGameMode()
    {
        return currentMode;
    }


    public bool IsIdiomMode()
    {
        return currentMode ==
               GameMode.Idiom;
    }


    public bool IsRadicalMode()
    {
        return currentMode ==
               GameMode.Radical;
    }


    public bool IsReadingMode()
    {
        return currentMode ==
               GameMode.Reading;
    }

    public bool IsTestPreparationMode()
    {
        return currentMode == GameMode.TestPreparation;
    }


    //==================================================
    // ゲームオーバー
    //==================================================

    private void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        // 現在のスコアを保存
        if (ScoreManager.Instance != null)
        {
            LastScore =
                ScoreManager.Instance.GetScore();
        }
        else
        {
            LastScore = 0;
        }

        Debug.Log("GAME OVER");
        Debug.Log("最終スコア: " + LastScore);

        if (BoardManager.Instance != null)
            BoardManager.Instance.PlayGameOverEffect();

        DOVirtual.DelayedCall(0.7f, () => SceneTransitionEffect.LoadScene("GameOverScene"))
            .SetUpdate(true);
    }
    //==================================================
    // ゲームBGM
    //==================================================

    public void StartGameBGM()
    {
        if (gameBGM == null)
        {
            Debug.LogWarning("GameManager: ゲームBGMが設定されていません。");
            return;
        }

        if (bgmAudioSource == null)
        {
            Debug.LogWarning("GameManager: BGM用AudioSourceがありません。");
            return;
        }

        bgmAudioSource.Stop();

        bgmAudioSource.clip = gameBGM;

        AudioVolumeSettings.SetBgmFade(bgmAudioSource, bgmVolume, 0f);

        bgmAudioSource.Play();

        StartCoroutine(FadeInBGM());
    }

    public void PauseGameBGM()
    {
        if (bgmAudioSource != null)
            bgmAudioSource.Pause();
    }

    public void ResumeGameBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.clip == gameBGM)
            bgmAudioSource.UnPause();
    }

    private System.Collections.IEnumerator FadeInBGM()
    {
        float timer = 0f;

        while (timer < bgmFadeInDuration)
        {
            timer += Time.deltaTime;

            float progress =
                timer / bgmFadeInDuration;

            AudioVolumeSettings.SetBgmFade(bgmAudioSource, bgmVolume, progress);

            yield return null;
        }

        AudioVolumeSettings.SetBgmFade(bgmAudioSource, bgmVolume, 1f);
    }
}
