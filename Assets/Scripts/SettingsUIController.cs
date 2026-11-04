using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    private const float AnimationDuration = 0.24f;
    private const float DimmerAlpha = 0.52f;
    private const float RightPanelX = 1590f;
    private const string SidePreferenceKey = "GameSettings.SettingsPanelSide";
    private static readonly Color TrackColor = new Color32(57, 64, 72, 255);
    private static readonly Color FillColor = new Color32(167, 167, 122, 255);
    private static readonly Color SelectedLineColor = new Color32(184, 184, 145, 255);
    private static readonly Color OffSelectedColor = new Color32(89, 143, 191, 255);
    private static readonly Color OnSelectedColor = new Color32(226, 205, 112, 255);
    private static readonly Color HandleColor = new Color32(248, 249, 250, 255);
    private static Sprite roundedTrackSprite;
    private static Sprite roundHandleSprite;

    private RectTransform panel;
    private CanvasGroup panelCanvasGroup;
    private Image dimmer;
    private Slider bgmSlider;
    private Slider seSlider;
    private Toggle vibrationToggle;
    private Toggle sideToggle;
    private TMP_Text sideValueText;
    private TMP_Text bgmValueText;
    private TMP_Text seValueText;
    private Vector2 leftOpenPosition;
    private Vector2 openPosition;
    private Coroutine animationRoutine;
    private GameObject previouslySelected;
    private bool isOpen;
    private bool isRightSide;

    private void Awake()
    {
        panel = transform as RectTransform;
        if (panel == null)
        {
            Debug.LogError("SettingsUIController must be attached to a UI panel.", this);
            enabled = false;
            return;
        }

        panelCanvasGroup = GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        dimmer = CreateOrFindDimmer();
        if (dimmer == null)
        {
            return;
        }

        isRightSide = PlayerPrefs.GetInt(SidePreferenceKey, 0) != 0;
        SetupVolumeControl("BGM", AudioVolumeSettings.BgmVolume, out bgmSlider, out bgmValueText);
        SetupVolumeControl("効果音", AudioVolumeSettings.SeVolume, out seSlider, out seValueText);
        SetupSideControl();
        SetupVibrationControl();

        leftOpenPosition = panel.anchoredPosition;
        openPosition = GetOpenPosition();
        panel.anchoredPosition = GetClosedPosition();
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        dimmer.color = new Color(0f, 0f, 0f, 0f);
        dimmer.raycastTarget = false;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            PlayerPrefs.Save();
        }
    }

    private static void PlaySwitchSound()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlaySE(GameManager.Instance.dropSE);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    private void Update()
    {
        GamePauseManager pauseManager = GamePauseManager.Instance;
        if (pauseManager != null && !pauseManager.IsPauseAllowed)
        {
            return;
        }

        bool togglePressed =
            (Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame);

        if (togglePressed)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;
        previouslySelected = EventSystem.current != null
            ? EventSystem.current.currentSelectedGameObject
            : null;

        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.SetSettingsPaused(true);
        }

        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;
        dimmer.raycastTarget = true;
        transform.SetAsLastSibling();

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(bgmSlider.gameObject);
        }

        Animate(true);
    }

    public void Close()
    {
        if (!isOpen)
        {
            return;
        }

        isOpen = false;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        dimmer.raycastTarget = false;

        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.SetSettingsPaused(false);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(previouslySelected);
        }

        Animate(false);
    }

    private Image CreateOrFindDimmer()
    {
        Transform canvas = transform.parent;
        if (canvas == null)
        {
            Debug.LogError("SettingsUIController: SettingsPanel has no Canvas parent.", this);
            enabled = false;
            return null;
        }

        Transform existing = canvas.Find("SettingsDimmer");
        Image image;
        if (existing != null)
        {
            image = existing.GetComponent<Image>();
            if (image == null)
            {
                Debug.LogError("SettingsUIController: SettingsDimmer has no Image component.", existing);
                enabled = false;
                return null;
            }
        }
        else
        {
            GameObject dimmerObject = new GameObject(
                "SettingsDimmer",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            dimmerObject.transform.SetParent(canvas, false);
            image = dimmerObject.GetComponent<Image>();
            image.color = Color.black;
        }

        RectTransform rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.SetAsFirstSibling();
        return image;
    }

    private void SetupVolumeControl(string labelText, float initialValue, out Slider slider, out TMP_Text valueText)
    {
        TMP_Text label = FindLabel(labelText);
        if (label == null)
        {
            Debug.LogError("SettingsUIController: Could not find the " + labelText + " volume label in SettingsPanel.", this);
            enabled = false;
            slider = null;
            valueText = null;
            return;
        }

        valueText = FindValueText(label.transform);
        if (valueText == null)
        {
            valueText = CreateValueText(label);
        }

        Transform existingSlider = transform.Find(labelText + "Slider");
        slider = existingSlider != null ? existingSlider.GetComponent<Slider>() : CreateSlider(label, labelText);
        if (slider == null)
        {
            Debug.LogError("SettingsUIController: Could not create a slider for " + labelText + ".", this);
            enabled = false;
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(initialValue);
        slider.onValueChanged.RemoveListener(labelText == "BGM" ? SetBgmVolume : SetSeVolume);
        slider.onValueChanged.AddListener(labelText == "BGM" ? SetBgmVolume : SetSeVolume);
        ApplySliderStyle(slider);
        UpdateValueText(slider, valueText);
    }

    private void SetupVibrationControl()
    {
        TMP_Text label = FindVibrationLabel();
        if (label == null)
        {
            Debug.LogWarning("SettingsUIController: Could not find the existing vibration label in SettingsPanel.", this);
            return;
        }

        Transform existingToggle = transform.Find("VibrationToggle");
        vibrationToggle = existingToggle != null
            ? existingToggle.GetComponent<Toggle>()
            : CreateVibrationToggle(label);
        if (vibrationToggle == null)
        {
            Debug.LogError("SettingsUIController: Could not create the vibration toggle.", this);
            enabled = false;
            return;
        }

        vibrationToggle.SetIsOnWithoutNotify(VibrationSettings.IsEnabled);
        vibrationToggle.onValueChanged.RemoveListener(SetVibrationEnabled);
        vibrationToggle.onValueChanged.AddListener(SetVibrationEnabled);
        UpdateVibrationToggleVisual(VibrationSettings.IsEnabled);
    }

    private void SetupSideControl()
    {
        TMP_Text label = FindSideLabel();
        if (label == null)
        {
            Debug.LogError("SettingsUIController: Could not find the existing left/right label in SettingsPanel.", this);
            enabled = false;
            return;
        }

        sideValueText = FindSideValueText(label);
        Transform existingToggle = transform.Find("SettingsSideToggle");
        sideToggle = existingToggle != null
            ? existingToggle.GetComponent<Toggle>()
            : CreateSwitchToggle("SettingsSideToggle", label);
        if (sideToggle == null)
        {
            Debug.LogError("SettingsUIController: Could not create the settings side toggle.", this);
            enabled = false;
            return;
        }

        sideToggle.SetIsOnWithoutNotify(isRightSide);
        sideToggle.onValueChanged.RemoveListener(SetSettingsPanelOnRight);
        sideToggle.onValueChanged.AddListener(SetSettingsPanelOnRight);
        UpdateSideValueText(isRightSide);
        UpdateSwitchToggleVisual(sideToggle, isRightSide);
    }

    private TMP_Text FindSideValueText(TMP_Text label)
    {
        TMP_Text[] texts = label.transform.parent.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != label &&
                (texts[i].gameObject.name == "moji" ||
                 texts[i].text == "左" ||
                 texts[i].text == "右"))
            {
                return texts[i];
            }
        }

        return null;
    }

    private void UpdateSideValueText(bool onRight)
    {
        if (sideValueText != null)
        {
            sideValueText.text = onRight ? "右" : "左";
        }
    }

    private TMP_Text FindSideLabel()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].transform != transform &&
                (texts[i].text.Contains("左右") || texts[i].gameObject.name == "leftright"))
            {
                return texts[i];
            }
        }

        return null;
    }

    private TMP_Text FindVibrationLabel()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i] != transform &&
                (transforms[i].name.Contains("sindou") || transforms[i].name.Contains("振動")))
            {
                TMP_Text groupLabel = transforms[i].GetComponentInChildren<TMP_Text>(true);
                if (groupLabel != null)
                {
                    return groupLabel;
                }
            }
        }

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].transform != transform &&
                (texts[i].text.Contains("振") || texts[i].gameObject.name.Contains("振動")))
            {
                return texts[i];
            }
        }

        return null;
    }

    private Toggle CreateVibrationToggle(TMP_Text label)
    {
        return CreateSwitchToggle("VibrationToggle", label);
    }

    private Toggle CreateSwitchToggle(string toggleName, TMP_Text label)
    {
        GameObject toggleObject = new GameObject(
            toggleName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Toggle));
        toggleObject.transform.SetParent(transform, false);

        RectTransform rect = toggleObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(78f, 38f);
        Vector3 labelCenter = panel.InverseTransformPoint(
            label.rectTransform.TransformPoint(label.rectTransform.rect.center));
        float columnX = sideToggle != null
            ? sideToggle.GetComponent<RectTransform>().anchoredPosition.x
            : labelCenter.x + 175f;
        rect.anchoredPosition = new Vector2(columnX, labelCenter.y);

        Image track = toggleObject.GetComponent<Image>();
        track.sprite = GetRoundedTrackSprite();
        track.type = Image.Type.Sliced;
        track.raycastTarget = true;

        GameObject handleObject = new GameObject(
            "Handle",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));
        handleObject.transform.SetParent(toggleObject.transform, false);
        Image handle = handleObject.GetComponent<Image>();
        handle.sprite = GetRoundHandleSprite();
        handle.type = Image.Type.Simple;
        handle.raycastTarget = false;

        RectTransform handleRect = handle.rectTransform;
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.sizeDelta = new Vector2(30f, 30f);

        Toggle toggle = toggleObject.GetComponent<Toggle>();
        toggle.targetGraphic = track;
        toggle.graphic = handle;
        toggle.transition = Selectable.Transition.ColorTint;
        toggle.navigation = new Navigation { mode = Navigation.Mode.Automatic };
        return toggle;
    }

    private void SetVibrationEnabled(bool isEnabled)
    {
        VibrationSettings.SetEnabled(isEnabled);
        UpdateVibrationToggleVisual(isEnabled);
        PlaySwitchSound();
    }

    private void UpdateVibrationToggleVisual(bool isEnabled)
    {
        UpdateSwitchToggleVisual(vibrationToggle, isEnabled);
    }

    private void UpdateSwitchToggleVisual(Toggle toggle, bool isOn)
    {
        if (toggle == null)
            return;

        Image handle = toggle.graphic as Image;
        if (handle != null)
        {
            handle.color = HandleColor;
            handle.rectTransform.anchoredPosition = new Vector2(isOn ? 18f : -18f, 0f);
        }

        ColorBlock colors = toggle.colors;
        Color stateColor = isOn ? FillColor : TrackColor;
        Color selectedColor = isOn ? OnSelectedColor : OffSelectedColor;
        colors.normalColor = stateColor;
        colors.highlightedColor = selectedColor;
        colors.selectedColor = selectedColor;
        colors.pressedColor = Color.Lerp(selectedColor, Color.white, 0.25f);
        colors.disabledColor = TrackColor;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        toggle.colors = colors;
        toggle.transition = Selectable.Transition.ColorTint;
    }

    private static Sprite GetRoundedTrackSprite()
    {
        if (roundedTrackSprite == null)
        {
            roundedTrackSprite = CreateRoundedSprite(32, 16, 8);
        }

        return roundedTrackSprite;
    }

    private static Sprite GetRoundHandleSprite()
    {
        if (roundHandleSprite == null)
        {
            roundHandleSprite = CreateRoundedSprite(16, 16, 8);
        }

        return roundHandleSprite;
    }

    private static Sprite CreateRoundedSprite(int width, int height, int radius)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            name = "SettingsSwitchSprite",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };

        Color32[] pixels = new Color32[width * height];
        float radiusSquared = radius * radius;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nearestX = Mathf.Clamp(x + 0.5f, radius, width - radius);
                float nearestY = Mathf.Clamp(y + 0.5f, radius, height - radius);
                float dx = x + 0.5f - nearestX;
                float dy = y + 0.5f - nearestY;
                pixels[y * width + x] = dx * dx + dy * dy <= radiusSquared
                    ? Color.white
                    : Color.clear;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
        return Sprite.Create(
            texture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 0.5f),
            1f,
            0,
            SpriteMeshType.FullRect,
            new Vector4(radius, radius, radius, radius));
    }

    private void SetSettingsPanelOnRight(bool onRight)
    {
        isRightSide = onRight;
        PlayerPrefs.SetInt(SidePreferenceKey, onRight ? 1 : 0);
        PlayerPrefs.Save();

        openPosition = GetOpenPosition();
        UpdateSideValueText(onRight);
        UpdateSwitchToggleVisual(sideToggle, onRight);
        PlaySwitchSound();
        if (isOpen)
        {
            Animate(true);
        }
        else
        {
            panel.anchoredPosition = GetClosedPosition();
        }
    }

    private TMP_Text FindLabel(string labelText)
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].transform == transform)
            {
                continue;
            }

            if (texts[i].text.Contains(labelText))
            {
                return texts[i];
            }
        }

        return null;
    }

    private TMP_Text FindValueText(Transform label)
    {
        TMP_Text[] texts = label.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != label.GetComponent<TMP_Text>() &&
                (texts[i].text.Contains("%") || texts[i].text.Contains("％")))
            {
                return texts[i];
            }
        }

        return null;
    }

    private TMP_Text CreateValueText(TMP_Text label)
    {
        GameObject valueObject = new GameObject(
            label.name + "Value",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        valueObject.transform.SetParent(label.transform, false);

        TMP_Text value = valueObject.GetComponent<TMP_Text>();
        value.font = label.font;
        value.fontSize = label.fontSize;
        value.color = label.color;
        value.alignment = TextAlignmentOptions.MidlineRight;

        RectTransform rect = value.rectTransform;
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.anchoredPosition = new Vector2(260f, -94f);
        rect.sizeDelta = new Vector2(130f, 50f);
        return value;
    }

    private Slider CreateSlider(TMP_Text label, string labelText)
    {
        GameObject sliderObject = new GameObject(labelText + "Slider", typeof(RectTransform), typeof(Slider));
        sliderObject.transform.SetParent(transform, false);

        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.sizeDelta = new Vector2(Mathf.Min(360f, panel.rect.width - 300f), 32f);
        sliderRect.anchoredPosition = new Vector2(-80f, label.rectTransform.anchoredPosition.y - 94f);

        Image background = CreateImage("Background", sliderObject.transform, TrackColor);
        background.rectTransform.anchorMin = new Vector2(0f, 0.5f);
        background.rectTransform.anchorMax = new Vector2(1f, 0.5f);
        background.rectTransform.sizeDelta = new Vector2(0f, 4f);

        Image fill = CreateImage("Fill", sliderObject.transform, FillColor);
        fill.rectTransform.anchorMin = new Vector2(0f, 0.5f);
        fill.rectTransform.anchorMax = new Vector2(1f, 0.5f);
        fill.rectTransform.offsetMin = new Vector2(0f, -2f);
        fill.rectTransform.offsetMax = new Vector2(-16f, 2f);

        Image handle = CreateImage("Handle", sliderObject.transform, HandleColor);
        handle.rectTransform.sizeDelta = new Vector2(14f, 20f);

        Slider slider = sliderObject.GetComponent<Slider>();
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.navigation = new Navigation { mode = Navigation.Mode.Automatic };
        return slider;
    }

    private static void ApplySliderStyle(Slider slider)
    {
        Transform backgroundTransform = slider.transform.Find("Background");
        if (backgroundTransform != null)
        {
            Image background = backgroundTransform.GetComponent<Image>();
            if (background != null)
            {
                background.color = TrackColor;
                RectTransform trackRect = background.rectTransform;
                trackRect.anchorMin = new Vector2(0f, 0.5f);
                trackRect.anchorMax = new Vector2(1f, 0.5f);
                trackRect.sizeDelta = new Vector2(trackRect.sizeDelta.x, 4f);
            }
        }

        Transform fillTransform = slider.transform.Find("Fill");
        if (fillTransform != null)
        {
            Image fill = fillTransform.GetComponent<Image>();
            if (fill != null)
            {
                fill.color = FillColor;
                RectTransform fillRect = fill.rectTransform;
                fillRect.anchorMin = new Vector2(0f, 0.5f);
                fillRect.anchorMax = new Vector2(1f, 0.5f);
                fillRect.offsetMin = new Vector2(fillRect.offsetMin.x, -2f);
                fillRect.offsetMax = new Vector2(fillRect.offsetMax.x, 2f);
            }
        }

        if (slider.handleRect != null)
        {
            slider.handleRect.sizeDelta = new Vector2(14f, 20f);
        }

        Image handle = slider.handleRect != null ? slider.handleRect.GetComponent<Image>() : null;
        if (handle != null)
        {
            handle.color = HandleColor;
        }

        Image fillGraphic = slider.fillRect != null ? slider.fillRect.GetComponent<Image>() : null;
        if (fillGraphic != null)
        {
            slider.targetGraphic = fillGraphic;
        }

        ColorBlock colors = slider.colors;
        colors.normalColor = FillColor;
        colors.highlightedColor = SelectedLineColor;
        colors.pressedColor = SelectedLineColor;
        colors.disabledColor = TrackColor;
        colors.fadeDuration = 0.08f;
        slider.colors = colors;
        slider.transition = Selectable.Transition.ColorTint;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject imageObject = new GameObject(
            name,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void Animate(bool opening)
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }

        animationRoutine = StartCoroutine(AnimateRoutine(opening));
    }

    private IEnumerator AnimateRoutine(bool opening)
    {
        Vector2 startPosition = panel.anchoredPosition;
        Vector2 endPosition = opening ? openPosition : GetClosedPosition();
        float startAlpha = dimmer.color.a;
        float endAlpha = opening ? DimmerAlpha : 0f;
        float elapsed = 0f;

        while (elapsed < AnimationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / AnimationDuration);
            float eased = t * t * (3f - 2f * t);
            panel.anchoredPosition = Vector2.Lerp(startPosition, endPosition, eased);
            dimmer.color = new Color(0f, 0f, 0f, Mathf.Lerp(startAlpha, endAlpha, eased));
            yield return null;
        }

        panel.anchoredPosition = endPosition;
        dimmer.color = new Color(0f, 0f, 0f, endAlpha);
        animationRoutine = null;
    }

    private Vector2 GetClosedPosition()
    {
        return openPosition + (isRightSide ? Vector2.right : Vector2.left) * panel.rect.width;
    }

    private Vector2 GetOpenPosition()
    {
        return isRightSide
            ? new Vector2(RightPanelX, leftOpenPosition.y)
            : leftOpenPosition;
    }

    private void SetBgmVolume(float volume)
    {
        AudioVolumeSettings.SetBgmVolume(volume);
        UpdateValueText(bgmSlider, bgmValueText);
    }

    private void SetSeVolume(float volume)
    {
        AudioVolumeSettings.SetSeVolume(volume);
        UpdateValueText(seSlider, seValueText);
    }

    private static void UpdateValueText(Slider slider, TMP_Text valueText)
    {
        if (slider != null && valueText != null)
        {
            valueText.text = Mathf.RoundToInt(slider.value * 100f) + "%";
        }
    }
}
