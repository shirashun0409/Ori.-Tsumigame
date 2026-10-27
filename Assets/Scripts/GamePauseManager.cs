using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }

    [Header("一時停止表示")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private TMP_Text pauseText2;

    private bool isPaused = false;
    private bool isPauseAllowed = false;
    private bool settingsOpen;
    private bool settingsStartedPause;

    public bool IsPaused => isPaused;
    public bool IsPauseAllowed => isPauseAllowed;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
            Instance = null;
        }
    }


    private void Start()
    {
        isPaused = false;
        isPauseAllowed = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }


    private void Update()
    {
        Gamepad gamepad = Gamepad.current;
        bool pausePressed =
            (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
            (gamepad != null && gamepad.startButton.wasPressedThisFrame);
        if (pausePressed)
        {
            TogglePause();
        }
    }


    public void TogglePause()
    {
        if (!isPauseAllowed || settingsOpen)
            return;

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }


    private void PauseGame(bool showPausePanel = true)
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (GameManager.Instance != null)
            GameManager.Instance.PauseGameBGM();

        if (pausePanel != null)
        {
            pausePanel.SetActive(showPausePanel);
        }

        if (pauseText != null)
        {
            pauseText.text = "一時停止中";
        }

        if (pauseText2 != null)
        {
            pauseText2.text = "Esc / STARTボタンで再開";
        }

        Debug.Log("ゲームを一時停止しました。");
    }


    public void SetPauseAllowed(bool allowed)
    {
        isPauseAllowed = allowed;
    }

    public void SetSettingsPaused(bool paused)
    {
        if (settingsOpen == paused)
        {
            return;
        }

        settingsOpen = paused;
        if (paused)
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }

            settingsStartedPause = isPauseAllowed && !isPaused;
            if (settingsStartedPause)
            {
                PauseGame(showPausePanel: false);
            }
        }
        else
        {
            if (settingsStartedPause)
            {
                settingsStartedPause = false;
                if (isPaused)
                {
                    ResumeGame();
                }
            }
            else if (isPaused && pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGameBGM();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Debug.Log("ゲームを再開しました。");
    }
}
