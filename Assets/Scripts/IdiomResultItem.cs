using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class IdiomResultItem : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("熟語")]
    [SerializeField] private TMP_Text idiomText;

    [Header("説明")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMP_Text infoTitle;
    [SerializeField] private TMP_Text infoText;

    [Header("説明パネル位置")]
    [SerializeField] private Vector2 popupOffset = new Vector2(20f, 0f);

    private string idiom;

    public void SetInfoPanel(
        GameObject panel,
        TMP_Text title,
        TMP_Text text)
    {
        infoPanel = panel;
        infoTitle = title;
        infoText = text;
    }

    public void SetIdiom(string value)
    {
        idiom = value;

        if (idiomText != null)
        {
            idiomText.text = idiom;
        }
    }

    // マウスが熟語ボタンに入ったとき
    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (infoPanel == null)
            return;

        //========================================
        // 読み・意味を取得
        //========================================

        string reading =
            IdiomDictionary.GetReading(idiom);

        string meaning =
            IdiomDictionary.GetMeaning(idiom);

        //========================================
        // 説明内容を設定
        //========================================

        if (infoTitle != null)
        {
            infoTitle.text = idiom;
        }

        if (infoText != null)
        {
            infoText.text =
                "読み：" + reading
                + "\n"
                + "意味：" + meaning;
        }

        //========================================
        // 説明パネルを熟語の近くへ移動
        //========================================

        RectTransform panelRect =
            infoPanel.GetComponent<RectTransform>();

        Canvas canvas =
            infoPanel.GetComponentInParent<Canvas>();

        if (panelRect != null && canvas != null)
        {
            RectTransform canvasRect =
                canvas.GetComponent<RectTransform>();

            Camera uiCamera = null;

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = canvas.worldCamera;
            }

            // 熟語ボタンの画面上の位置
            Vector2 screenPosition =
                RectTransformUtility.WorldToScreenPoint(
                    uiCamera,
                    transform.position
                );

            // Canvas上のローカル座標へ変換
            Vector2 localPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPosition,
                uiCamera,
                out localPosition
            );

            // パネルを熟語の右側へ
            panelRect.anchoredPosition =
                localPosition + popupOffset;
        }

        // パネルを表示
        infoPanel.SetActive(true);
    }

    // マウスが熟語ボタンから出たとき
    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}