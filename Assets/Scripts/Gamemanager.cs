using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefab")]
    [SerializeField]
    private GameObject capsulePrefab;

    [Header("Game Mode")]

    [SerializeField]
    private GameMode currentMode = GameMode.Idiom;

    private Capsule currentCapsule;

    // ========================================
    // ★ SE（効果音）追加部分
    // ========================================

    [Header("Sound Effects")]
    public AudioClip dropSE;     // ← public に変更
    public AudioClip eraseSE;    // ← public に変更
    public AudioClip comboSE;    // ← public に変更
    public AudioClip rotateSE;   // ← public に変更

    private AudioSource audioSource;

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
        SpawnCapsule();
    }

    // ★ SE 再生関数
    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    //========================================
    // カプセル生成
    //========================================

    public void SpawnCapsule()
    {
        GameObject obj = Instantiate(capsulePrefab);
        currentCapsule = obj.GetComponent<Capsule>();
    }

    //========================================
    // ゲームモード
    //========================================

    public void SetGameMode(GameMode mode)
    {
        currentMode = mode;
        Debug.Log("ゲームモード変更: " + currentMode);
    }

    public GameMode GetGameMode()
    {
        return currentMode;
    }

    public bool IsIdiomMode()
    {
        return currentMode == GameMode.Idiom;
    }

    public bool IsRadicalMode()
    {
        return currentMode == GameMode.Radical;
    }

    public bool IsReadingMode()
    {
        return currentMode == GameMode.Reading;
    }
}