using UnityEngine;
using TMPro;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }

    [Header("一時停止表示")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private TMP_Text pauseText2;

    private bool isPaused = false;

    public bool IsPaused => isPaused;


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
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }


    private void Update()
    {
        // 上矢印キーで一時停止・再開
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TogglePause();
        }
    }


    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }


    private void PauseGame()
    {
        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (pauseText != null)
        {
            pauseText.text = "一時停止中";
        }

        if (pauseText2 != null)
        {
            pauseText2.text = "上キーで再開";
        }

        Debug.Log("ゲームを一時停止しました。");
    }


    private void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Debug.Log("ゲームを再開しました。");
    }
}

