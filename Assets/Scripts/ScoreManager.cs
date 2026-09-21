using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text comboText;

    [Header("Score Popup")]
    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private Transform scorePopupParent;

    [Header("Score Settings")]
    [SerializeField] private int kanjiScore = 50;
    [SerializeField] private int jukugoBonus = 300;
    [SerializeField] private int simultaneousBonus = 200;

    [Header("Combo Settings")]
    [SerializeField] private float comboLimit = 2.5f;

    private int score = 0;
    private int combo = 0;
    private float comboTimer = 0f;


    //==================================================
    // Unity
    //==================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 初期表示
        if (scoreText != null)
        {
            scoreText.text = "0";
        }

        if (comboText != null)
        {
            comboText.text = "";
        }
    }


    private void Update()
    {
        // コンボ中
        if (combo > 0)
        {
            comboTimer += Time.deltaTime;

            // 一定時間消去がなければコンボ終了
            if (comboTimer > comboLimit)
            {
                ResetCombo();
            }
        }
    }


    //==================================================
    // スコア追加
    //==================================================

    /// <summary>
    /// スコアを追加する。
    /// 
    /// kanjiCount
    ///     今回消した漢字の数
    ///
    /// jukugoCount
    ///     今回同時に成立した熟語の数
    /// </summary>
    public void AddScore(
        int kanjiCount,
        int jukugoCount)
    {
        // コンボを1つ増やす
        combo++;
        comboTimer = 0f;


        //==================================================
        // ① 基本点
        //==================================================

        int baseScore =
            kanjiCount * kanjiScore;


        //==================================================
        // ② 熟語ボーナス
        //==================================================

        int jukugoScore =
            jukugoCount * jukugoBonus;


        //==================================================
        // ③ 同時消去ボーナス
        //
        // 1組だけならボーナスなし
        //
        // 2組 → +200
        // 3組 → +400
        // 4組 → +600
        //==================================================

        int rensaScore = 0;

        if (jukugoCount >= 2)
        {
            rensaScore =
                (jukugoCount - 1) * simultaneousBonus;
        }


        //==================================================
        // ④ 小計
        //==================================================

        int subtotal =
            baseScore
            + jukugoScore
            + rensaScore;


        //==================================================
        // ⑤ コンボ倍率
        //==================================================

        float comboRate =
            GetComboRate(combo);


        //==================================================
        // ⑥ 最終獲得点
        //==================================================

        int addScore =
            Mathf.RoundToInt(
                subtotal * comboRate
            );


        //==================================================
        // ⑦ スコア加算
        //==================================================

        score += addScore;

        UpdateScoreUI();
        UpdateComboUI();

        AnimateScore();

        ShowScorePopup(addScore);

        // デバッグ用
        Debug.Log(
            "スコア追加: "
            + addScore
            + " / "
            + "基本:"
            + baseScore
            + " / "
            + "熟語:"
            + jukugoScore
            + " / "
            + "同時消去:"
            + rensaScore
            + " / "
            + "倍率:"
            + comboRate
            + " / "
            + "合計:"
            + score
        );
    }


    //==================================================
    // 旧形式のAddScore
    //
    // 既存コードとの互換性を残すために保管
    //==================================================

    public void AddScore(
        int kanjiCount,
        bool isJukugo,
        bool isRensa)
    {
        int jukugoCount =
            isJukugo ? 1 : 0;

        AddScore(
            kanjiCount,
            jukugoCount
        );

        // isRensaは旧仕様との互換用。
        // 新しいスコア計算では、
        // 同時成立した熟語数からボーナスを計算する。
    }


    //==================================================
    // コンボ倍率
    //==================================================

    private float GetComboRate(int combo)
    {
        if (combo == 1)
            return 1.0f;

        if (combo == 2)
            return 1.2f;

        if (combo == 3)
            return 1.5f;

        if (combo == 4)
            return 2.0f;

        if (combo == 5)
            return 2.5f;

        // 6コンボ以上
        return 3.0f;
    }


    //==================================================
    // スコアUI
    //==================================================

    private void UpdateScoreUI()
    {
        if (scoreText == null)
            return;

        scoreText.text =
            score.ToString("N0");
    }


    //==================================================
    // コンボUI
    //==================================================

    private void UpdateComboUI()
    {
        if (comboText == null)
            return;

        comboText.text =
            combo + " Combo!";
    }


    //==================================================
    // スコア・コンボ演出
    //==================================================

    private void AnimateScore()
    {
        if (scoreText != null)
        {
            scoreText.transform
                .DOKill();

            scoreText.transform
                .DOScale(
                    1.3f,
                    0.1f
                )
                .SetLoops(
                    2,
                    LoopType.Yoyo
                );
        }


        if (comboText != null)
        {
            comboText.transform
                .DOKill();

            comboText.transform
                .DOScale(
                    1.2f,
                    0.1f
                )
                .SetLoops(
                    2,
                    LoopType.Yoyo
                );
        }
    }


    //==================================================
    // コンボリセット
    //==================================================

    private void ResetCombo()
    {
        combo = 0;
        comboTimer = 0f;

        if (comboText != null)
        {
            comboText.text = "";
        }
    }


    //==================================================
    // 外部から取得するためのメソッド
    //==================================================

    public int GetScore()
    {
        return score;
    }


    public int GetCombo()
    {
        return combo;
    }


    public float GetCurrentComboRate()
    {
        return GetComboRate(combo);
    }


    //==================================================
    // ゲーム開始時などに使用
    //==================================================

    public void ResetScore()
    {
        score = 0;
        combo = 0;
        comboTimer = 0f;

        if (scoreText != null)
        {
            scoreText.text = "0";
        }

        if (comboText != null)
        {
            comboText.text = "";
        }
    }
    private void ShowScorePopup(int addScore)
    {
        if (scorePopupPrefab == null)
            return;

        if (scorePopupParent == null)
            return;

        GameObject popup =
            Instantiate(
                scorePopupPrefab,
                scorePopupParent
            );

        TMP_Text popupText =
            popup.GetComponent<TMP_Text>();

        if (popupText == null)
        {
            Destroy(popup);
            return;
        }

        popupText.text =
            "+" + addScore.ToString("N0");

        // ScoreTextの少し下を開始位置にする
        popup.transform.position =
            scoreText.transform.position
            + new Vector3(0f, -40f, 0f);

        popup.transform.localScale =
     Vector3.zero;

        float popupScale = 1.0f;

        if (combo == 2)
        {
            popupScale = 1.15f;
        }
        else if (combo == 3)
        {
            popupScale = 1.3f;
        }
        else if (combo == 4)
        {
            popupScale = 1.5f;
        }
        else if (combo == 5)
        {
            popupScale = 1.7f;
        }
        else if (combo >= 6)
        {
            popupScale = 1.9f;
        }

        popup.transform
     .DOScale(
         popupScale,
         0.2f
     )
     .SetEase(Ease.OutBack);

        popup.transform
            .DOPunchScale(
                Vector3.one * 0.15f,
                0.25f,
                1,
                0.5f
            )
            .SetDelay(0.2f);

        popup.transform
            .DOMoveY(
                popup.transform.position.y + 50f,
                1.2f
            )
            .SetEase(Ease.OutQuad);

        popupText
            .DOFade(
                0f,
                0.8f
            )
            .SetDelay(0.6f)
            .OnComplete(() =>
            {
                Destroy(popup);
            });
    }
}