using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ModeSelectController : MonoBehaviour
{
    [SerializeField] private GameObject jukugoButton;
    [SerializeField] private GameObject busyuButton;
    [SerializeField] private GameObject yomiButton;

    private Toggle shadowToggle;
    private TMP_Text shadowToggleLabel;
    private Image shadowToggleBackground;

    public void ShowButtons()
    {
        if (jukugoButton == null || busyuButton == null || yomiButton == null)
        {
            Debug.LogError("ModeSelectController: モードボタンが設定されていません。", this);
            return;
        }

        CreateShadowToggle();
        if (shadowToggle == null)
            return;

        MenuSceneEffects.AttachButtonEffects(jukugoButton.transform.parent);

        jukugoButton.SetActive(true);
        busyuButton.SetActive(true);
        yomiButton.SetActive(true);

        if (MenuSceneEffects.ReducedMotion)
        {
            jukugoButton.transform.localScale = Vector3.one;
            busyuButton.transform.localScale = Vector3.one;
            yomiButton.transform.localScale = Vector3.one;
            shadowToggle.transform.localScale = Vector3.one;
        }
        else
        {
            jukugoButton.transform.localScale = Vector3.zero;
            busyuButton.transform.localScale = Vector3.zero;
            yomiButton.transform.localScale = Vector3.zero;

            jukugoButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            busyuButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f);
            yomiButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.4f);
            shadowToggle.transform.localScale = Vector3.zero;
            shadowToggle.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.6f);
        }
    }

    private void CreateShadowToggle()
    {
        if (shadowToggle != null)
            return;

        GameObject toggleObject = new GameObject(
            "ShadowModeToggle",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Toggle));
        toggleObject.transform.SetParent(jukugoButton.transform.parent, false);

        RectTransform toggleRect = toggleObject.GetComponent<RectTransform>();
        toggleRect.anchorMin = new Vector2(0.5f, 0.5f);
        toggleRect.anchorMax = new Vector2(0.5f, 0.5f);
        toggleRect.anchoredPosition = new Vector2(0f, -410f);
        toggleRect.sizeDelta = new Vector2(460f, 74f);

        Image background = toggleObject.GetComponent<Image>();
        shadowToggleBackground = background;
        Image buttonImage = jukugoButton.GetComponent<Image>();
        if (buttonImage == null)
            buttonImage = busyuButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            background.sprite = buttonImage.sprite;
            background.type = buttonImage.type;
            background.preserveAspect = buttonImage.preserveAspect;
        }
        background.color = new Color(0.22f, 0.25f, 0.17f, 0.95f);

        shadowToggle = toggleObject.GetComponent<Toggle>();
        shadowToggle.targetGraphic = background;

        GameObject labelObject = new GameObject(
            "Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(toggleObject.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(24f, 0f);
        labelRect.offsetMax = Vector2.zero;

        shadowToggleLabel = labelObject.GetComponent<TextMeshProUGUI>();
        TMP_Text labelTemplate = jukugoButton.GetComponentInChildren<TMP_Text>();
        if (labelTemplate != null)
        {
            shadowToggleLabel.font = labelTemplate.font;
            shadowToggleLabel.fontSharedMaterial = labelTemplate.fontSharedMaterial;
        }
        shadowToggleLabel.fontSize = 30f;
        shadowToggleLabel.color = Color.white;
        shadowToggleLabel.alignment = TextAlignmentOptions.MidlineLeft;
        shadowToggleLabel.raycastTarget = false;

        shadowToggle.SetIsOnWithoutNotify(GameModeData.ShadowEnabled);
        UpdateShadowToggleLabel(GameModeData.ShadowEnabled);
        UpdateShadowToggleVisual(GameModeData.ShadowEnabled);
        shadowToggle.onValueChanged.AddListener(OnShadowToggleChanged);
    }

    private void OnShadowToggleChanged(bool isOn)
    {
        GameModeData.ShadowEnabled = isOn;
        UpdateShadowToggleLabel(isOn);
        UpdateShadowToggleVisual(isOn);
        if (!MenuSceneEffects.ReducedMotion)
        {
            shadowToggle.transform.DOKill();
            shadowToggle.transform.DOPunchScale(Vector3.one * 0.06f, 0.22f, 3, 0.6f);
        }
    }

    private void UpdateShadowToggleVisual(bool isOn)
    {
        if (shadowToggleBackground == null)
            return;

        shadowToggleBackground.color = isOn
            ? new Color(0.42f, 0.37f, 0.24f, 0.98f)
            : new Color(0.20f, 0.22f, 0.16f, 0.88f);
    }

    private void UpdateShadowToggleLabel(bool isOn)
    {
        if (shadowToggleLabel != null)
            shadowToggleLabel.text = isOn ? "シャドウモード：ON" : "シャドウモード：OFF";
    }
}
