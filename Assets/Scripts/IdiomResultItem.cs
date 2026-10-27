using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class IdiomResultItem : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
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
    private KanjiData radical;
    private Vector2 infoTextOriginalPosition;
    private float infoTextOriginalFontSize;
    private bool infoTextLayoutCaptured;

    public void SetInfoPanel(
        GameObject panel,
        TMP_Text title,
        TMP_Text text)
    {
        infoPanel = panel;
        infoTitle = title;
        infoText = text;

        if (infoTitle != null)
            infoTitle.raycastTarget = false;
        if (infoText != null)
            infoText.raycastTarget = false;

        if (infoText != null && !infoTextLayoutCaptured)
        {
            infoTextOriginalPosition = infoText.rectTransform.anchoredPosition;
            infoTextOriginalFontSize = infoText.fontSize;
            infoTextLayoutCaptured = true;
        }
    }

    public void SetIdiom(string value)
    {
        idiom = value;
        radical = null;

        if (idiomText != null)
        {
            idiomText.text = idiom;
        }
    }

    public void SetRadical(KanjiData value)
    {
        radical = value;
        idiom = null;

        if (idiomText != null)
        {
            idiomText.text = KanjiDatabase.GetRadicalSymbol(value);
        }
    }

    // マウスが熟語ボタンに入ったとき
    public void OnPointerEnter(
        PointerEventData eventData)
    {
        ShowInfo();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShowInfo();
    }

    private void ShowInfo()
    {
        if (infoPanel == null)
            return;

        //========================================
        // 読み・意味を取得
        //========================================

        string reading;
        string meaning;
        if (radical != null)
        {
            reading = KanjiDatabase.GetRadicalName(radical);
            meaning = "この部首の漢字：" + KanjiDatabase.GetRadicalKanji(radical);
        }
        else
        {
            reading = IdiomDictionary.GetReading(idiom);
            meaning = IdiomDictionary.GetMeaning(idiom);
        }

        //========================================
        // 説明内容を設定
        //========================================

        if (infoTitle != null)
        {
            infoTitle.gameObject.SetActive(radical == null);
            if (radical == null)
            {
                infoTitle.text = idiom;
            }
        }

        if (infoText != null)
        {
            infoText.fontSize = infoTextOriginalFontSize;
            infoText.rectTransform.anchoredPosition = radical != null
                ? infoTextOriginalPosition + Vector2.up * 90f
                : infoTextOriginalPosition;
            infoText.text = radical != null
                ? "<voffset=-30><size=135%>読み：" + reading + "</size></voffset>\n\n<size=100%>" + meaning + "</size>"
                : "読み：" + reading + "\n意味：" + meaning;
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
        HideInfo();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HideInfo();
    }

    private void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}