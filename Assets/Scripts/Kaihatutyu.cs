using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DevTMPController : MonoBehaviour
{
    [SerializeField] private Button buttonA;
    [SerializeField] private Button buttonB;
    [SerializeField] private TextMeshProUGUI devTMP;

    private void Start()
    {
        devTMP.gameObject.SetActive(true);
        devTMP.color = new Color(1, 1, 1, 0); // 完全透明

        buttonA.onClick.AddListener(() => OnButtonClick(buttonA));
        buttonB.onClick.AddListener(() => OnButtonClick(buttonB));
    }

    private void OnButtonClick(Button clickedButton)
    {
        // 位置を合わせる
        devTMP.rectTransform.position = clickedButton.GetComponent<RectTransform>().position;

        devTMP.text = "現在開発中です";

        // フェードイン → 2秒待つ → フェードアウト
        devTMP.DOFade(1f, 0.4f)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(2f, () =>
                {
                    devTMP.DOFade(0f, 0.4f);
                });
            });
    }
}
