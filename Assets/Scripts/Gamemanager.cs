using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefab")]
    [SerializeField] private GameObject capsulePrefab;

    [Header("Game Mode")]
    [SerializeField] private GameMode currentMode = GameMode.Idiom;

    private Capsule currentCapsule;

    // ★ 現在使う辞書（Entry の配列）
    public KanjiRegistry.Entry[] CurrentRegistry;

    [Header("Sound Effects")]
    public AudioClip dropSE;
    public AudioClip eraseSE;
    public AudioClip comboSE;
    public AudioClip rotateSE;

    private AudioSource audioSource;

    // ★ UI（熟語表示 & 意味表示）
    [Header("UI")]
    [SerializeField] private KanjiDisplay kanjiDisplay;
    [SerializeField] private MeaningDisplay meaningDisplay;

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

        // ★ 辞書セット
        CurrentRegistry = KanjiRegistry.Entries;

        // ★ 初期表示（空欄）
        if (kanjiDisplay != null)
            kanjiDisplay.SetIdiom("");

        if (meaningDisplay != null)
            meaningDisplay.SetMeaning("");

        SpawnCapsule();
    }

    // ★ SE 再生
    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    // ★ カプセル生成
    public void SpawnCapsule()
    {
        GameObject obj = Instantiate(capsulePrefab);
        currentCapsule = obj.GetComponent<Capsule>();
    }

    // ★ BoardManager から熟語が成立したときに呼ばれる
    public void OnIdiomCreated(string idiom)
    {
        if (kanjiDisplay != null)
            kanjiDisplay.SetIdiom(idiom);

        // ★ 熟語辞書から意味を取得
        string meaning = IdiomDictionary.GetMeaning(idiom);

        if (meaningDisplay != null)
            meaningDisplay.SetMeaning(meaning);
    }

    // ★ ゲームモード
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
