using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MobileGameControls : MonoBehaviour
{
    public enum Control
    {
        Left,
        Right,
        Drop
    }

    private static readonly HashSet<int>[] ActivePointers =
    {
        new HashSet<int>(),
        new HashSet<int>(),
        new HashSet<int>()
    };

    private static int rotatePressedFrame = -1;
    private RectTransform controlsRoot;

    public static bool RotatePressedThisFrame => rotatePressedFrame == Time.frameCount;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneListener()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene" ||
            (!Application.isMobilePlatform && !Input.touchSupported))
        {
            return;
        }

        GameObject controlsObject = new GameObject("MobileGameControls");
        SceneManager.MoveGameObjectToScene(controlsObject, scene);
        controlsObject.AddComponent<MobileGameControls>();
    }

    public static bool IsHeld(Control control)
    {
        return ActivePointers[(int)control].Count > 0;
    }

    private static void AddPointer(Control control, int pointerId)
    {
        ActivePointers[(int)control].Add(pointerId);
    }

    private static void RemovePointer(Control control, int pointerId)
    {
        ActivePointers[(int)control].Remove(pointerId);
    }

    private void Awake()
    {
        Canvas gameCanvas = FindGameCanvas();
        if (gameCanvas == null)
        {
            Debug.LogError("MobileGameControls: Could not find the game Canvas containing SettingsPanel.", this);
            enabled = false;
            return;
        }

        controlsRoot = CreateControlsRoot(gameCanvas.transform);
        CreateTouchButton("LEFT", new Vector2(0.10f, 0.12f), new Vector2(112f, 84f), Control.Left);
        CreateTouchButton("RIGHT", new Vector2(0.24f, 0.12f), new Vector2(112f, 84f), Control.Right);
        CreateTouchButton("TURN", new Vector2(0.72f, 0.12f), new Vector2(112f, 84f), rotate: true);
        CreateTouchButton("DROP", new Vector2(0.87f, 0.12f), new Vector2(112f, 84f), Control.Drop);
        CreateTouchButton("SETTINGS", new Vector2(0.92f, 0.91f), new Vector2(140f, 58f), onClick: ToggleSettings);
        AddMobileCloseButton();
    }

    private Canvas FindGameCanvas()
    {
        GameObject[] roots = gameObject.scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            Transform[] transforms = roots[i].GetComponentsInChildren<Transform>(true);
            for (int j = 0; j < transforms.Length; j++)
            {
                if (transforms[j].name == "SettingsPanel")
                {
                    return transforms[j].GetComponentInParent<Canvas>();
                }
            }
        }

        return null;
    }

    private RectTransform CreateControlsRoot(Transform canvasTransform)
    {
        GameObject root = new GameObject("MobileControls", typeof(RectTransform));
        root.transform.SetParent(canvasTransform, false);

        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Transform settingsPanel = canvasTransform.Find("SettingsPanel");
        if (settingsPanel != null)
        {
            rect.SetSiblingIndex(settingsPanel.GetSiblingIndex());
        }

        return rect;
    }

    private void CreateTouchButton(
        string label,
        Vector2 anchor,
        Vector2 size,
        Control? control = null,
        bool rotate = false,
        UnityEngine.Events.UnityAction onClick = null)
    {
        GameObject buttonObject = new GameObject(
            label + "Button",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button));
        buttonObject.transform.SetParent(controlsRoot, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.12f, 0.14f, 0.16f, 0.68f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        if (onClick != null)
        {
            button.onClick.AddListener(onClick);
        }

        ConfigureTouchEvents(buttonObject, control, rotate);

        GameObject labelObject = new GameObject(
            "Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = labelObject.GetComponent<TextMeshProUGUI>();
        TMP_Text template = FindAnyObjectByType<TMP_Text>(FindObjectsInactive.Include);
        if (template != null)
        {
            text.font = template.font;
            text.fontSharedMaterial = template.fontSharedMaterial;
        }

        text.text = label;
        text.fontSize = 24f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
    }

    private void ConfigureTouchEvents(GameObject buttonObject, Control? control, bool rotate)
    {
        EventTrigger trigger = buttonObject.AddComponent<EventTrigger>();
        AddTouchEvent(trigger, EventTriggerType.PointerDown, eventData =>
        {
            PointerEventData pointerData = eventData as PointerEventData;
            if (pointerData == null)
            {
                return;
            }

            if (control.HasValue)
            {
                AddPointer(control.Value, pointerData.pointerId);
            }
            else if (rotate)
            {
                rotatePressedFrame = Time.frameCount;
            }
        });

        if (control.HasValue)
        {
            UnityEngine.Events.UnityAction<BaseEventData> release = eventData =>
            {
                PointerEventData pointerData = eventData as PointerEventData;
                if (pointerData != null)
                {
                    RemovePointer(control.Value, pointerData.pointerId);
                }
            };
            AddTouchEvent(trigger, EventTriggerType.PointerUp, release);
            AddTouchEvent(trigger, EventTriggerType.PointerExit, release);
        }
    }

    private static void AddTouchEvent(
        EventTrigger trigger,
        EventTriggerType eventType,
        UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private void ToggleSettings()
    {
        GamePauseManager pauseManager = GamePauseManager.Instance;
        if (pauseManager != null && !pauseManager.IsPauseAllowed)
        {
            return;
        }

        SettingsUIController settings = FindAnyObjectByType<SettingsUIController>(FindObjectsInactive.Include);
        if (settings == null)
        {
            Debug.LogError("MobileGameControls: SettingsUIController was not found.", this);
            return;
        }

        settings.Toggle();
    }

    private void AddMobileCloseButton()
    {
        Transform settingsPanel = controlsRoot.parent.Find("SettingsPanel");
        if (settingsPanel == null)
        {
            return;
        }

        SettingsUIController settings = settingsPanel.GetComponent<SettingsUIController>();
        if (settings == null)
        {
            settings = settingsPanel.gameObject.AddComponent<SettingsUIController>();
        }

        GameObject closeObject = new GameObject(
            "MobileCloseButton",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button));
        closeObject.transform.SetParent(settingsPanel, false);

        RectTransform rect = closeObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-18f, -18f);
        rect.sizeDelta = new Vector2(140f, 54f);

        Image image = closeObject.GetComponent<Image>();
        image.color = new Color(0.24f, 0.27f, 0.30f, 0.96f);

        Button button = closeObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        button.onClick.AddListener(settings.Close);

        GameObject labelObject = new GameObject(
            "Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(closeObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        TMP_Text template = settingsPanel.GetComponentInChildren<TMP_Text>(true);
        if (template != null)
        {
            label.font = template.font;
            label.fontSharedMaterial = template.fontSharedMaterial;
        }

        label.text = "閉じる";
        label.fontSize = 24f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < ActivePointers.Length; i++)
        {
            ActivePointers[i].Clear();
        }

        rotatePressedFrame = -1;
    }
}