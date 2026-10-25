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
        isPauseAllowed = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }


    private void Update()
    {
        Gamepad gamepad = Gamepad.current;
        bool pausePressed = Input.GetKeyDown(KeyCode.UpArrow) ||
            (gamepad != null &&
             (gamepad.startButton.wasPressedThisFrame ||
              gamepad.buttonEast.wasPressedThisFrame));
        if (pausePressed)
        {
            TogglePause();
        }
    }


    public void TogglePause()
    {
        if (!isPauseAllowed)
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


    private void PauseGame()
    {
        isPaused = true;

        if (GameManager.Instance != null)
            GameManager.Instance.PauseGameBGM();

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


    public void SetPauseAllowed(bool allowed)
    {
        isPauseAllowed = allowed;
    }

    private void ResumeGame()
    {
        isPaused = false;

        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGameBGM();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Debug.Log("ゲームを再開しました。");
    }
}

