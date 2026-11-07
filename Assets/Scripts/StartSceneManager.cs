using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class StartSceneManager : MonoBehaviour
{
    [Header("画面")]
    [SerializeField] private GameObject modeSelectGroup;
    [SerializeField] private GameObject explanationGroup;
    [SerializeField] private TMP_FontAsset testRangeFont;

    [Header("モード選択")]
    [SerializeField] private ModeSelectController modeSelectController;

    [Header("スライド")]
    [SerializeField] private RectTransform[] slides;
    [SerializeField] private float slideDistance = 1800f;
    [SerializeField] private float duration = 0.6f;

    [Header("ページ表示")]
    [SerializeField] private TMP_Text pageIndicator;
    [SerializeField] private TMP_Text slideGuide;

    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private int modeSelectionSoundGroupIndex = 0;
    [SerializeField] private int slideButtonSoundGroupIndex = 1;

    private Button helpButton;
    private TMP_Text helpButtonLabel;
    private Button modoruButton;
    private RectTransform modoruButtonRectTransform;
    private Vector2 modoruButtonInitialPosition;
    private int currentIndex = 0;
    private bool isMoving = false;
    private int controllerSlideDirection;
    private static bool hasShownSlideHint = false;
    private static bool hasViewedIdiomExplanation = false;
    private GameObject testRangeSelector;
    private TMP_Text testRangeStatusText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSessionState()
    {
        hasViewedIdiomExplanation = false;
    }

    private void Start()
    {
        SetupHelpButton();
        SetupModoruButton();
        SetupTestPreparationButton();

        if (modeSelectController != null)
        {
            modeSelectController.ShowButtons();
            MenuSceneEffects.AttachButtonEffects(modeSelectGroup != null ? modeSelectGroup.transform : null);
            MenuSceneEffects.AttachButtonEffects(explanationGroup != null ? explanationGroup.transform : null);
        }
        else
        {
            Debug.LogError(
                "StartSceneManagerにModeSelectControllerが設定されていません！"
            );
        }
    }

    private void SetupTestPreparationButton()
        {
            if (modeSelectController == null)
                return;

            GameObject testButton = FindGameObjectIncludingInactive("Testbutton");
            if (testButton == null)
                testButton = FindGameObjectIncludingInactive("TestButton");

            if (testButton == null)
            {
                Debug.LogError("StartSceneManager: Testbutton was not found in the StartScene.", this);
                return;
            }

            Button button = testButton.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("StartSceneManager: Testbutton requires a Button.", testButton);
                return;
            }

            TMP_Text label = testButton.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                if (testRangeFont != null)
                    label.font = testRangeFont;
                label.text = "テスト対策";
            }

            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(SelectTestPreparationMode);
            MenuSceneEffects.AttachButtonEffect(testButton);
            modeSelectController.SetTestPreparationButton(testButton);
        }

        public void SelectTestPreparationMode()
        {
            PlayModeSelectionSE();
            GameModeData.SelectedMode = GameMode.TestPreparation;

            if (explanationGroup != null)
                explanationGroup.SetActive(false);

            if (testRangeSelector == null)
            {
                Canvas canvas = modeSelectGroup != null
                    ? modeSelectGroup.GetComponentInParent<Canvas>()
                    : FindAnyObjectByType<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("StartSceneManager: テスト範囲選択を表示するCanvasがありません。", this);
                    return;
                }
                testRangeSelector = CreateTestRangeSelector(canvas.transform);
            }

            if (modeSelectGroup != null)
                modeSelectGroup.SetActive(false);
            if (helpButton != null)
                helpButton.gameObject.SetActive(false);
            testRangeSelector.SetActive(true);
            RefreshTestRangeButtons();
        }

        private GameObject CreateTestRangeSelector(Transform parent)
        {
            GameObject overlay = new GameObject(
                "TestPreparationRangeSelector",
                typeof(RectTransform),
                typeof(Image));
            overlay.transform.SetParent(parent, false);
            overlay.layer = parent.gameObject.layer;

            RectTransform overlayRect = overlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            Image overlayImage = overlay.GetComponent<Image>();
            overlayImage.color = new Color(0f, 0f, 0f, 0.45f);
            overlayImage.raycastTarget = true;

            GameObject panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(overlay.transform, false);
            panel.layer = overlay.layer;
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(1120f, 680f);

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0.08f, 0.07f, 0.06f, 0.96f);

            CreateRangeLabel(panel.transform, "テスト範囲を選択（複数選択可）", 34f, new Vector2(0f, 270f), new Vector2(1020f, 66f));
            testRangeStatusText = CreateRangeLabel(
                panel.transform,
                string.Empty,
                20f,
                new Vector2(0f, -180f),
                new Vector2(1020f, 38f));

            for (int i = 0; i < 9; i++)
            {
                int range = 22 + i;
                int column = i % 3;
                int row = i / 3;
                Button rangeButton = CreateRangeButton(
                    panel.transform,
                    range.ToString(),
                    new Vector2((column - 1) * 280f, 155f - row * 105f),
                    new Vector2(240f, 76f),
                    48f);
                rangeButton.onClick.AddListener(() => ToggleTestRange(range));
            }

            Button selectAll = CreateRangeButton(panel.transform, "全選択", new Vector2(-385f, -280f), new Vector2(180f, 62f));
            selectAll.onClick.AddListener(SelectAllTestRanges);
            Button clearAll = CreateRangeButton(panel.transform, "全解除", new Vector2(-130f, -280f), new Vector2(180f, 62f));
            clearAll.onClick.AddListener(ClearTestRanges);
            Button startButton = CreateRangeButton(panel.transform, "開始", new Vector2(130f, -280f), new Vector2(180f, 62f));
            startButton.onClick.AddListener(StartSelectedTestGame);
            Button cancelButton = CreateRangeButton(panel.transform, "戻る", new Vector2(385f, -280f), new Vector2(170f, 62f));
            cancelButton.onClick.AddListener(CancelTestPreparationSelection);

            return overlay;
        }

        private void CancelTestPreparationSelection()
        {
            if (testRangeSelector != null)
                testRangeSelector.SetActive(false);
            if (modeSelectGroup != null)
                modeSelectGroup.SetActive(true);
            if (helpButton != null)
                helpButton.gameObject.SetActive(true);
        }

        private Button CreateRangeButton(
            Transform parent,
            string label,
            Vector2 position,
            Vector2 size,
            float labelFontSize = 26f)
        {
            GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.layer = parent.gameObject.layer;

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.32f, 0.26f, 0.19f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            button.colors = new ColorBlock
            {
                normalColor = image.color,
                highlightedColor = new Color(0.55f, 0.44f, 0.29f, 1f),
                pressedColor = new Color(0.75f, 0.62f, 0.4f, 1f),
                selectedColor = image.color,
                disabledColor = new Color(0.2f, 0.18f, 0.15f, 0.7f),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
            CreateRangeLabel(buttonObject.transform, label, labelFontSize, Vector2.zero, size);
            return button;
        }

        private TMP_Text CreateRangeLabel(Transform parent, string label, float fontSize, Vector2 position, Vector2 size)
        {
            GameObject textObject = new GameObject(label + "Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);
            textObject.layer = parent.gameObject.layer;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.font = testRangeFont != null ? testRangeFont : TMP_Settings.defaultFontAsset;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            text.text = label;
            return text;
        }

        private void ToggleTestRange(int range)
        {
            if (!GameModeData.SelectedTestRanges.Add(range))
                GameModeData.SelectedTestRanges.Remove(range);
            RefreshTestRangeButtons();
        }

        private void SelectAllTestRanges()
        {
            GameModeData.SelectedTestRanges.Clear();
            for (int range = 22; range <= 30; range++)
                GameModeData.SelectedTestRanges.Add(range);
            RefreshTestRangeButtons();
        }

        private void ClearTestRanges()
        {
            GameModeData.SelectedTestRanges.Clear();
            RefreshTestRangeButtons();
        }

        private void RefreshTestRangeButtons()
        {
            if (testRangeSelector == null)
                return;

            for (int range = 22; range <= 30; range++)
            {
                Transform rangeButton = testRangeSelector.transform.Find("Panel/" + range);
                if (rangeButton == null)
                    continue;

                bool selected = GameModeData.SelectedTestRanges.Contains(range);
                Color buttonColor = selected
                    ? new Color(0.45f, 0.34f, 0.2f, 1f)
                    : new Color(0.2f, 0.18f, 0.15f, 1f);
                rangeButton.GetComponent<Image>().color = buttonColor;
                ColorBlock colors = rangeButton.GetComponent<Button>().colors;
                colors.normalColor = buttonColor;
                colors.selectedColor = buttonColor;
                rangeButton.GetComponent<Button>().colors = colors;
                TMP_Text label = rangeButton.GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = selected ? range + " ✓" : range.ToString();
            }

            if (testRangeStatusText != null)
            {
                testRangeStatusText.text = GameModeData.SelectedTestRanges.Count == 0
                    ? "範囲を1つ以上選択してください"
                    : "選択中：" + string.Join("、", GameModeData.SelectedTestRanges.OrderBy(range => range));
            }
        }

    private void StartSelectedTestGame()
        {
            if (GameModeData.SelectedTestRanges.Count == 0)
            {
                Debug.LogWarning("StartSceneManager: テスト範囲を1つ以上選択してください。", this);
                return;
            }

            if (testRangeSelector != null)
                testRangeSelector.SetActive(false);
            GameModeData.SelectedMode = GameMode.TestPreparation;
            StartGame();
        }

    private void SetupHelpButton()
    {
        GameObject helpButtonObject = GameObject.Find("Helpbuttn");
        if (helpButtonObject == null)
        {
            Debug.LogError("StartSceneManager: Helpbuttn was not found in the StartScene.", this);
            return;
        }

        helpButton = helpButtonObject.GetComponent<Button>();
        helpButtonLabel = helpButtonObject.GetComponentInChildren<TMP_Text>(true);
        if (helpButton == null || helpButtonLabel == null)
        {
            Debug.LogError("StartSceneManager: Helpbuttn requires a Button and a child TMP_Text.", helpButtonObject);
            return;
        }

        helpButton.onClick.AddListener(ToggleHelpMode);
        UpdateHelpButtonLabel();
        MenuSceneEffects.AttachButtonEffect(helpButtonObject);
    }

    private void SetupModoruButton()
    {
        GameObject modoruButtonObject = FindGameObjectIncludingInactive("ModoruButton");
        if (modoruButtonObject == null)
        {
            // UIが未配置の場合は何もしない（UIは別途作成済み前提）
            return;
        }

        modoruButton = modoruButtonObject.GetComponent<Button>();
        if (modoruButton == null)
            modoruButton = modoruButtonObject.GetComponentInChildren<Button>();

        if (modoruButton == null)
        {
            Debug.LogError("StartSceneManager: ModoruButton requires a Button.", modoruButtonObject);
            return;
        }

        modoruButton.onClick.RemoveListener(ReturnToModeSelect);
        modoruButton.onClick.AddListener(ReturnToModeSelect);
        modoruButtonRectTransform = modoruButton.GetComponent<RectTransform>();
        if (modoruButtonRectTransform == null)
        {
            Debug.LogError("StartSceneManager: ModoruButton requires a RectTransform.", modoruButtonObject);
            modoruButton = null;
            return;
        }

        modoruButtonInitialPosition = modoruButtonRectTransform.anchoredPosition;
        modoruButton.interactable = false;
        MenuSceneEffects.AttachButtonEffect(modoruButtonObject);
        modoruButtonObject.SetActive(false);
    }

    private static GameObject FindGameObjectIncludingInactive(string name)
    {
        GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in all)
        {
            if (go.name == name && go.scene.IsValid())
            {
                return go;
            }
        }
        return GameObject.Find(name);
    }

    private void ToggleHelpMode()
    {
        PlayModeSelectionSE();
        GameModeData.ShadowEnabled = !GameModeData.ShadowEnabled;
        UpdateHelpButtonLabel();
    }

    private void PlayModeSelectionSE()
    {
        if (UIAudioManager.Instance != null)
        {
            UIAudioManager.Instance.PlayButtonSE(modeSelectionSoundGroupIndex);
        }
        else
        {
            Debug.LogWarning("StartSceneManager: UIAudioManager was not found; mode selection sound was skipped.", this);
        }
    }

    private void UpdateHelpButtonLabel()
    {
        if (helpButtonLabel != null)
        {
            helpButtonLabel.text = GameModeData.ShadowEnabled
                ? "お助けモード：ON"
                : "お助けモード：OFF";
        }
    }

    private void Update()
    {
        if (explanationGroup == null || !explanationGroup.activeInHierarchy)
        {
            controllerSlideDirection = 0;
            return;
        }

        Gamepad gamepad = Gamepad.current;
        if (gamepad == null)
        {
            return;
        }

        float horizontal = gamepad.dpad.ReadValue().x;
        if (Mathf.Abs(horizontal) < 0.5f)
        {
            horizontal = gamepad.leftStick.ReadValue().x;
        }

        int direction = horizontal > 0.55f ? 1 : horizontal < -0.55f ? -1 : 0;
        if (direction == 0)
        {
            controllerSlideDirection = 0;
        }
        else if (direction != controllerSlideDirection)
        {
            if (direction > 0)
                SlideRight();
            else
                SlideLeft();

            controllerSlideDirection = direction;
        }

        if (gamepad.buttonSouth.wasPressedThisFrame &&
            slides != null &&
            currentIndex >= slides.Length - 1)
        {
            StartGame();
        }
    }

    //==================================================
    // 熟語モードを押したとき
    //==================================================
    public void SelectJukugoMode()
    {
        PlayModeSelectionSE();
        // 熟語モードを保存
        GameModeData.SelectedMode = GameMode.Idiom;

        if (hasViewedIdiomExplanation)
        {
            StartGame();
            return;
        }

        if (TransitionToExplanation())
        {
            hasViewedIdiomExplanation = true;
        }

        currentIndex = 0;
        isMoving = false;

        // すべてのTweenを停止
        for (int i = 0; i < slides.Length; i++)
        {
            slides[i].DOKill();

            if (i == 0)
            {
                slides[i].anchoredPosition = Vector2.zero;
            }
            else
            {
                slides[i].anchoredPosition =
                    new Vector2(slideDistance * i, 0f);
            }
        }

        if (modoruButtonRectTransform != null)
        {
            modoruButtonRectTransform.DOKill();
            modoruButtonRectTransform.anchoredPosition = modoruButtonInitialPosition;
        }
        if (modoruButton != null)
        {
            modoruButton.interactable = true;
            modoruButton.gameObject.SetActive(true);
        }
        UpdatePageIndicator();

        if (slideGuide != null)
        {
            slideGuide.gameObject.SetActive(true);
        }
        PlayFirstSlideHint();
    }
    //==================================================
    // 部首モードを押したとき
    //==================================================
    public void SelectRadicalMode()
    {
        PlayModeSelectionSE();
        GameModeData.SelectedMode = GameMode.Radical;
        StartGame();
    }

    //==================================================
    // 右タッチ → 次の画像へ
    //==================================================
    public void SlideRight()
    {
        if (isMoving)
            return;

        if (currentIndex >= slides.Length - 1)
            return;

        PlaySlideButtonSE();
        isMoving = true;
        if (modoruButton != null)
            modoruButton.interactable = false;

        int nextIndex = currentIndex + 1;
        if (modoruButtonRectTransform != null)
        {
            modoruButtonRectTransform.DOKill();
            modoruButtonRectTransform
                .DOAnchorPosX(modoruButtonInitialPosition.x - nextIndex * slideDistance, duration)
                .SetEase(Ease.InOutCubic);
        }

        // すべてのTweenを停止して、
        // 現在のページ番号を基準に正しい位置へ移動
        for (int i = 0; i < slides.Length; i++)
        {
            slides[i].DOKill();

            float targetX =
                (i - nextIndex) * slideDistance;

            slides[i]
                .DOAnchorPosX(targetX, duration)
                .SetEase(Ease.InOutCubic);
        }

        currentIndex = nextIndex;

        UpdatePageIndicator();
        SetNavigationArrowsVisible(false);

        if (slideGuide != null)
        {
            slideGuide.gameObject.SetActive(false);
        }

        DOVirtual.DelayedCall(duration, () =>
        {
            isMoving = false;
            if (modoruButton != null)
                modoruButton.interactable = currentIndex == 0;
            UpdatePageIndicator();
        });
    }

    //==================================================
    // 左タッチ → 前の画像へ
    //==================================================
    public void SlideLeft()
    {
        if (isMoving)
            return;

        if (currentIndex <= 0)
            return;

        PlaySlideButtonSE();
        isMoving = true;

        int previousIndex = currentIndex - 1;
        if (modoruButton != null)
            modoruButton.interactable = false;

        if (modoruButtonRectTransform != null)
        {
            modoruButtonRectTransform.DOKill();
            modoruButtonRectTransform
                .DOAnchorPosX(modoruButtonInitialPosition.x - previousIndex * slideDistance, duration)
                .SetEase(Ease.InOutCubic);
        }

        // すべてのTweenを停止して、
        // 現在のページ番号を基準に正しい位置へ移動
        for (int i = 0; i < slides.Length; i++)
        {
            slides[i].DOKill();

            float targetX =
                (i - previousIndex) * slideDistance;

            slides[i]
                .DOAnchorPosX(targetX, duration)
                .SetEase(Ease.InOutCubic);
        }

        currentIndex = previousIndex;

        UpdatePageIndicator();
        SetNavigationArrowsVisible(false);

        DOVirtual.DelayedCall(duration, () =>
        {
            isMoving = false;
            if (modoruButton != null)
                modoruButton.interactable = currentIndex == 0;
            UpdatePageIndicator();
        });
    }

    //==================================================
    // 最後の画像でゲーム開始
    //==================================================
    public void StartGame()
    {
        SceneTransitionEffect.LoadScene("GameScene");
    }

    private bool TransitionToExplanation()
    {
        if (modeSelectGroup == null || explanationGroup == null)
            return false;

        if (helpButton != null)
        {
            helpButton.gameObject.SetActive(false);
        }

        CanvasGroup modeGroup = modeSelectGroup.GetComponent<CanvasGroup>();
        if (modeGroup == null)
            modeGroup = modeSelectGroup.AddComponent<CanvasGroup>();

        CanvasGroup explanationCanvasGroup = explanationGroup.GetComponent<CanvasGroup>();
        if (explanationCanvasGroup == null)
            explanationCanvasGroup = explanationGroup.AddComponent<CanvasGroup>();

        explanationGroup.SetActive(true);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
        explanationCanvasGroup.alpha = 0f;
        explanationCanvasGroup.interactable = true;
        explanationCanvasGroup.blocksRaycasts = true;
        float transitionDuration = MenuSceneEffects.ReducedMotion ? 0.12f : 0.28f;
        explanationGroup.transform.localScale = MenuSceneEffects.ReducedMotion ? Vector3.one : Vector3.one * 0.98f;
        explanationGroup.transform.DOScale(1f, transitionDuration).SetEase(Ease.OutBack);
        explanationCanvasGroup.DOFade(1f, transitionDuration);

        modeGroup.interactable = false;
        modeGroup.blocksRaycasts = false;
        modeSelectGroup.transform.DOScale(MenuSceneEffects.ReducedMotion ? 1f : 0.94f, transitionDuration).SetEase(Ease.InBack);
        modeGroup.DOFade(0f, transitionDuration).OnComplete(() => modeSelectGroup.SetActive(false));
        return true;
    }

    private void PlayFirstSlideHint()
    {
        if (hasShownSlideHint || MenuSceneEffects.ReducedMotion)
            return;

        hasShownSlideHint = true;
        if (slideGuide != null)
        {
            slideGuide.transform.DOKill();
            slideGuide.transform.localScale = Vector3.one;
            slideGuide.transform.DOPunchScale(Vector3.one * 0.12f, 0.3f, 3, 0.6f);
        }

        if (rightArrow == null)
            return;

        Graphic arrowGraphic = rightArrow.GetComponent<Graphic>();
        if (arrowGraphic == null)
            return;

        Color original = arrowGraphic.color;
        Sequence hint = DOTween.Sequence();
        hint.Append(arrowGraphic.DOFade(0.25f, 0.16f));
        hint.Append(arrowGraphic.DOFade(original.a, 0.18f));
        hint.Append(arrowGraphic.DOFade(0.25f, 0.16f));
        hint.Append(arrowGraphic.DOFade(original.a, 0.18f));
    }

    //==================================================
    // ページ表示更新
    //==================================================
    private void UpdatePageIndicator()
    {
        if (pageIndicator != null)
        {
            string result = "";

            for (int i = 0; i < slides.Length; i++)
            {
                if (i == currentIndex)
                    result += "●";
                else
                    result += "○";

                if (i < slides.Length - 1)
                    result += " ";
            }

            pageIndicator.text = result;
        }

        // 一番左なら左矢印を消す
        if (leftArrow != null)
        {
            leftArrow.SetActive(currentIndex > 0);
        }

        // 一番右なら右矢印を消す
        if (rightArrow != null)
        {
            rightArrow.SetActive(currentIndex < slides.Length - 1);
        }

    }

    private void SetNavigationArrowsVisible(bool visible)
    {
        if (leftArrow != null)
            leftArrow.SetActive(visible && currentIndex > 0);

        if (rightArrow != null)
            rightArrow.SetActive(visible && currentIndex < slides.Length - 1);
    }

    public void ReturnToModeSelect()
    {
        PlayModeSelectionSE();

        if (modeSelectGroup == null || explanationGroup == null)
            return;

        hasViewedIdiomExplanation = false;

        // スライド状態をリセット
        for (int i = 0; i < slides.Length; i++)
        {
            slides[i].DOKill();
            slides[i].anchoredPosition = (i == 0)
                ? Vector2.zero
                : new Vector2(slideDistance * i, 0f);
        }

        currentIndex = 0;
        isMoving = false;

        if (modoruButton != null)
        {
            modoruButtonRectTransform.DOKill();
            modoruButtonRectTransform.anchoredPosition = modoruButtonInitialPosition;
            modoruButton.interactable = false;
            modoruButton.gameObject.SetActive(false);
        }

        if (helpButton != null)
            helpButton.gameObject.SetActive(true);

        CanvasGroup modeGroup = modeSelectGroup.GetComponent<CanvasGroup>();
        if (modeGroup == null)
            modeGroup = modeSelectGroup.AddComponent<CanvasGroup>();

        CanvasGroup explanationCanvasGroup = explanationGroup.GetComponent<CanvasGroup>();
        if (explanationCanvasGroup == null)
            explanationCanvasGroup = explanationGroup.AddComponent<CanvasGroup>();

        modeSelectGroup.SetActive(true);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        modeGroup.alpha = 0f;
        modeGroup.interactable = true;
        modeGroup.blocksRaycasts = true;
        modeSelectGroup.transform.localScale = MenuSceneEffects.ReducedMotion ? Vector3.one : Vector3.one * 0.94f;

        explanationCanvasGroup.interactable = false;
        explanationCanvasGroup.blocksRaycasts = false;

        float transitionDuration = MenuSceneEffects.ReducedMotion ? 0.12f : 0.28f;
        modeSelectGroup.transform.DOScale(1f, transitionDuration).SetEase(Ease.OutBack);
        modeGroup.DOFade(1f, transitionDuration);

        explanationGroup.transform.DOScale(MenuSceneEffects.ReducedMotion ? 1f : 0.98f, transitionDuration).SetEase(Ease.InBack);
        explanationCanvasGroup.DOFade(0f, transitionDuration).OnComplete(() => explanationGroup.SetActive(false));
    }

    private void PlaySlideButtonSE()
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayButtonSE(slideButtonSoundGroupIndex);
    }
}
