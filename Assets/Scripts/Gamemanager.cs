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
        SpawnCapsule();
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