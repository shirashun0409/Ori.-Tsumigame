using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        StartCoroutine(CountdownRoutine());
    }


    private IEnumerator CountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);


        //==================================================
        // 三
        //==================================================

        PlayCountdownSE(GameManager.Instance.countThreeSE);

        yield return ShowCount("三");


        //==================================================
        // 二
        //==================================================

        PlayCountdownSE(GameManager.Instance.countTwoSE);

        yield return ShowCount("二");


        //==================================================
        // 一
        //==================================================

        PlayCountdownSE(GameManager.Instance.countOneSE);

        yield return ShowCount("一");


        //==================================================
        // はじめ！
        //==================================================

        countdownText.text = "はじめ！";

        // はじめだけフォントサイズを小さくする
        countdownText.fontSize = 170;

        // 黒色・透明
        countdownText.color =
            new Color(0f, 0f, 0f, 0f);

        // 初期状態
        countdownText.transform.localScale =
            Vector3.zero;


        //==================================================
        // はじめ！SE
        //==================================================

        PlayCountdownSE(GameManager.Instance.startSE);


        //==================================================
        // BGM開始
        //==================================================

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGameBGM();
        }


        //==================================================
        // 「はじめ！」の演出
        //==================================================

        countdownText.transform
            .DOScale(1.0f, 0.5f)
            .SetEase(Ease.OutBack);

        countdownText.DOFade(1f, 0.5f);


        // 「はじめ！」を少し表示
        yield return new WaitForSeconds(1.0f);


        //==================================================
        // カウントダウン終了
        //==================================================

        countdownText.gameObject.SetActive(false);

        GameManager.Instance.StartGame();
    }


    //==================================================
    // 数字の演出
    //==================================================

    private IEnumerator ShowCount(string text)
    {
        countdownText.text = text;

        // 黒色に統一
        countdownText.color =
            new Color(0f, 0f, 0f, 0f);

        // 初期状態
        countdownText.transform.localScale =
            Vector3.zero;

        // ポンッと出る
        countdownText.transform
            .DOScale(1f, 0.4f)
            .SetEase(Ease.OutBack);

        // フェードイン
        countdownText.DOFade(1f, 0.4f);

        // テンポ
        yield return new WaitForSeconds(1.2f);
    }


    //==================================================
    // カウントダウンSE
    //==================================================

    private void PlayCountdownSE(AudioClip clip)
    {
        if (clip == null)
            return;

        if (GameManager.Instance == null)
            return;

        GameManager.Instance.PlaySE(clip);
    }
}