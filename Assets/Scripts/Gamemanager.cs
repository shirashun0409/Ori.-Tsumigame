using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


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


    private AudioSource audioSource;


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
    }


    private void Start()
    {
        audioSource =
            GetComponent<AudioSource>();


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


        SpawnCapsule();
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

    //==================================================
    // ゲームオーバー判定
    //==================================================

    private bool CanSpawnCapsule()
    {
        int spawnX = 3;
        int spawnY = 0;

        // カプセルの最初の向きでは
        // 左右2マスを使用する
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
        // ゲームオーバー後は新しいカプセルを生成しない
        if (isGameOver)
            return;

        // 出現位置が埋まっていたらゲームオーバー
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


        //==============================================
        // 熟語表示
        //==============================================

        if (kanjiDisplay != null)
        {
            kanjiDisplay.SetIdioms(
                idioms
            );
        }


        //==============================================
        // 意味表示
        //==============================================

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
                    // 熟語名は表示せず、
                    // 意味だけを追加する
                    meanings.Add(
                        meaning
                    );
                }
            }


            // 複数の意味は改行して表示
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

        Debug.Log("GAME OVER");
    }
}