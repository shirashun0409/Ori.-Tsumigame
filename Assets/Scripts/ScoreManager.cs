using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text comboText;

    int score = 0;
    int combo = 0;
    float comboTimer = 0f;
    float comboLimit = 1.5f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (combo > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboLimit)
            {
                combo = 0;
                comboText.text = "";
            }
        }
    }

    public void AddScore(int kanjiCount, bool isJukugo, bool isRensa)
    {
        combo++;
        comboTimer = 0f;

        int baseScore = kanjiCount * 50;
        int bonus = 0;

        if (isJukugo) bonus += 300;
        if (isRensa) bonus += 200;

        float comboRate = GetComboRate(combo);
        int add = Mathf.RoundToInt((baseScore + bonus) * comboRate);

        score += add;

        scoreText.text = score.ToString();
        comboText.text = combo + " Combo!";

        AnimateScore();
    }

    float GetComboRate(int combo)
    {
        if (combo == 1) return 1.0f;
        if (combo == 2) return 1.2f;
        if (combo == 3) return 1.5f;
        if (combo == 4) return 2.0f;
        if (combo == 5) return 2.5f;
        return 3.0f;
    }

    void AnimateScore()
    {
        scoreText.transform.DOScale(1.3f, 0.1f).SetLoops(2, LoopType.Yoyo);
        comboText.transform.DOScale(1.2f, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
}
