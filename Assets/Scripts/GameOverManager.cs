using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreNumber;
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private Button rankingRegisterButton;
    [SerializeField] private TMP_Text yourRankText;

    [Header("Ranking")]
    [SerializeField] private RankingManager rankingManager;

    private bool isRegistered = false;

    private const string RegisteredKey = "LastRankingRegistered";
    private const string NicknameKey = "LastRankingNickname";
    private const string ScoreKey = "LastRankingScore";

    private void Start()
    {
        // スコア表示
        if (scoreNumber == null)
        {
            Debug.LogError("ScoreNumberが設定されていません！");
        }
        else
        {
            scoreNumber.text =
                GameManager.LastScore.ToString("N0");
        }

        // 現在のゲームですでに登録済みか確認
        isRegistered =
            GameManager.RankingRegisteredThisGame;

        // ランキング登録ボタン
        if (rankingRegisterButton != null)
        {
            rankingRegisterButton.onClick.AddListener(
                RegisterRanking
            );
        }
        else
        {
            Debug.LogError(
                "RankingRegisterButtonが設定されていません！"
            );
        }

        // GameOverSceneでは順位を表示しない
        if (yourRankText != null)
        {
            yourRankText.gameObject.SetActive(false);
        }

        // すでに登録済みならボタンを無効化
        if (isRegistered)
        {
            if (rankingRegisterButton != null)
            {
                rankingRegisterButton.interactable = false;
            }

            Debug.Log(
                "このゲームではすでにランキング登録済みです。"
            );
        }
    }


    public void OpenRanking()
    {
        SceneManager.LoadScene("RankingScene");
    }


    public void RetryGame()
    {
        SceneManager.LoadScene("GameScene");
    }


    public void BackToModeSelect()
    {
        SceneManager.LoadScene("StartScene");
    }


    private void RegisterRanking()
    {
        //==================================================
        // ★ すでに登録済みなら絶対に登録しない
        //==================================================

        if (isRegistered ||
            GameManager.RankingRegisteredThisGame)
        {
            Debug.Log(
                "このゲームではすでにランキング登録済みです。"
            );
            return;
        }


        if (rankingManager == null)
        {
            Debug.LogError(
                "RankingManagerが設定されていません！"
            );
            return;
        }


        string nickname =
            nicknameInput.text.Trim();


        // ニックネーム未入力
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogWarning(
                "ニックネームが入力されていません！"
            );
            return;
        }


        int score =
            GameManager.LastScore;


        //==================================================
        // ランキング登録
        //==================================================

        rankingManager.RegisterScore(
            nickname,
            score
        );


        //==================================================
        // 今回の登録情報を保存
        //==================================================

        PlayerPrefs.SetInt(
            RegisteredKey,
            1
        );

        PlayerPrefs.SetString(
            NicknameKey,
            nickname
        );

        PlayerPrefs.SetInt(
            ScoreKey,
            score
        );

        PlayerPrefs.Save();


        //==================================================
        // ★ GameManagerにも登録済みと記録
        //==================================================

        GameManager.SetRankingRegistered();

        isRegistered = true;


        // 登録ボタンを無効化
        if (rankingRegisterButton != null)
        {
            rankingRegisterButton.interactable = false;
        }


        Debug.Log(
            "ランキング登録完了！ "
            + nickname
            + " / "
            + score
        );
    }
}

