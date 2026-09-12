using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefab")]
    [SerializeField] private GameObject capsulePrefab;

    [Header("Game Mode")]
    [SerializeField] private GameMode currentMode = GameMode.Idiom;

    private Capsule currentCapsule;
    private Capsule nextCapsule;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        CurrentRegistry = KanjiRegistry.Entries;

        if (kanjiDisplay != null)
            kanjiDisplay.SetIdiom("");

        if (meaningDisplay != null)
            meaningDisplay.SetMeaning("");

        SpawnCapsule();
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    // ★ カプセル生成（NEXT 完全対応）
    public void SpawnCapsule()
    {
        if (nextCapsule == null)
        {
            GameObject firstObj = Instantiate(capsulePrefab);
            nextCapsule = firstObj.GetComponent<Capsule>();

            nextCapsule.isNextPreview = true;

            KanjiData left = KanjiDatabase.GetRandomKanji();
            KanjiData right = KanjiDatabase.GetRandomKanji();
            nextCapsule.SetKanjiForNext(left, right);

            firstObj.transform.position = new Vector3(999, 999, 0);

            // ★ 左右両方の漢字を表示
            nextDisplay.SetNextSprites(
                nextCapsule.GetLeftSprite(),
                nextCapsule.GetRightSprite()
            );
        }

        currentCapsule = nextCapsule;

        currentCapsule.transform.position = BoardManager.Instance.GridToWorld(3, 0);

        currentCapsule.isNextPreview = false;

        GameObject obj = Instantiate(capsulePrefab);
        nextCapsule = obj.GetComponent<Capsule>();

        nextCapsule.isNextPreview = true;

        KanjiData nextLeft = KanjiDatabase.GetRandomKanji();
        KanjiData nextRight = KanjiDatabase.GetRandomKanji();
        nextCapsule.SetKanjiForNext(nextLeft, nextRight);

        obj.transform.position = new Vector3(999, 999, 0);

        // ★ 左右両方の漢字を表示
        nextDisplay.SetNextSprites(
            nextCapsule.GetLeftSprite(),
            nextCapsule.GetRightSprite()
        );
    }

    public void OnIdiomCreated(string idiom)
    {
        if (kanjiDisplay != null)
            kanjiDisplay.SetIdiom(idiom);

        string meaning = IdiomDictionary.GetMeaning(idiom);

        if (meaningDisplay != null)
            meaningDisplay.SetMeaning(meaning);
    }

    public void SetGameMode(GameMode mode)
    {
        currentMode = mode;
        Debug.Log("ゲームモード変更: " + currentMode);
    }

    public GameMode GetGameMode() => currentMode;
    public bool IsIdiomMode() => currentMode == GameMode.Idiom;
    public bool IsRadicalMode() => currentMode == GameMode.Radical;
    public bool IsReadingMode() => currentMode == GameMode.Reading;
}
