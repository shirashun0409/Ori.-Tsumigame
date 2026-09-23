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

        // 三
        yield return ShowCount("三");

        // 二
        yield return ShowCount("二");

        // 一
        yield return ShowCount("一");

        // はじめ！
        countdownText.text = "はじめ！";

        // ★ はじめだけフォントサイズを小さくする（例：150）
        countdownText.fontSize = 170;

        // 黒色に統一
        countdownText.color = new Color(0f, 0f, 0f, 0f);

        // 初期状態（小さく透明）
        countdownText.transform.localScale = Vector3.zero;

        // ポンッと膨らむ
        countdownText.transform
            .DOScale(1.0f, 0.5f)
            .SetEase(Ease.OutBack);

        // フェードイン
        countdownText.DOFade(1f, 0.5f);

        // ★ はじめのあと少し待つ（テンポ調整）
        yield return new WaitForSeconds(1.0f);

        countdownText.gameObject.SetActive(false);

        GameManager.Instance.StartGame();
    }


    // 数字の演出をまとめた関数
    private IEnumerator ShowCount(string text)
    {
        countdownText.text = text;

        // 黒色に統一
        countdownText.color = new Color(0f, 0f, 0f, 0f);

        // 初期状態
        countdownText.transform.localScale = Vector3.zero;

        // ポンッと出る
        countdownText.transform
            .DOScale(1f, 0.4f)
            .SetEase(Ease.OutBack);

        // フェードイン
        countdownText.DOFade(1f, 0.4f);

        // ★ テンポをゆっくりに（1.2秒）
        yield return new WaitForSeconds(1.2f);
    }
}
