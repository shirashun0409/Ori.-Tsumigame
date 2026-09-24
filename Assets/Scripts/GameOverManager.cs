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

        // ランキング登録ボタン
        if (rankingRegisterButton != null)
        {
            rankingRegisterButton.onClick.AddListener(RegisterRanking);
        }
        else
        {
            Debug.LogError(
                "RankingRegisterButtonが設定されていません！"
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
        if (isRegistered)
        {
            Debug.Log("すでにランキング登録済みです。");
            return;
        }

        if (rankingManager == null)
        {
            Debug.LogError(
                "RankingManagerが設定されていません！"
            );
            return;
        }

        string nickname = nicknameInput.text.Trim();

        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogWarning(
                "ニックネームが入力されていません！"
            );
            return;
        }

        int score = GameManager.LastScore;

        // ランキングに登録
        rankingManager.RegisterScore(
            nickname,
            score
        );

        // 今回登録した自分の順位を取得
        int rank = rankingManager.GetRank(
            nickname,
            score
        );

        // 順位を画面に表示
        if (yourRankText != null)
        {
            if (rank > 0)
            {
                yourRankText.text =
                    "あなたの順位\n"
                    + rank
                    + "位";
            }
            else
            {
                yourRankText.text =
                    "あなたの順位\n--位";
            }
        }

        isRegistered = true;

        Debug.Log(
            "ランキング登録完了！ "
            + nickname
            + " / "
            + score
            + " / "
            + rank
            + "位"
        );
    }

}