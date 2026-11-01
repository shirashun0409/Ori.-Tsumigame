using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [Header("画面")]
    [SerializeField] private GameObject modeSelectGroup;
    [SerializeField] private GameObject explanationGroup;

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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSessionState()
    {
        hasViewedIdiomExplanation = false;
    }

    private void Start()
    {
        SetupHelpButton();
        SetupModoruButton();

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
