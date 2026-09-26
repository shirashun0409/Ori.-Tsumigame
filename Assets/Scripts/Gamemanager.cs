using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


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

        PlayerPrefs.DeleteKey("LastRankingRegistered");
        PlayerPrefs.DeleteKey("LastRankingNickname");
        PlayerPrefs.DeleteKey("LastRankingScore");

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

            nextDisplay.SetNextSprites(
                nextCapsule.GetLeftSprite(),
                nextCapsule.GetRightSprite()
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


        nextDisplay.SetNextSprites(
            nextCapsule.GetLeftSprite(),
            nextCapsule.GetRightSprite()
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


    //==================================================
    // ゲームモード
    //==================================================

    public void SetGameMode(
        GameMode mode)
    {
        currentMode = mode;

        Debug.Log(
            "ゲームモード変更: " +
            currentMode
        );
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

        SceneManager.LoadScene("GameOverScene");
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

        bgmAudioSource.volume = 0f;

        bgmAudioSource.Play();

        StartCoroutine(FadeInBGM());
    }

    private System.Collections.IEnumerator FadeInBGM()
    {
        float timer = 0f;

        while (timer < bgmFadeInDuration)
        {
            timer += Time.deltaTime;

            float progress =
                timer / bgmFadeInDuration;

            bgmAudioSource.volume =
                Mathf.Lerp(
                    0f,
                    bgmVolume,
                    progress
                );

            yield return null;
        }

        bgmAudioSource.volume = bgmVolume;
    }
}

