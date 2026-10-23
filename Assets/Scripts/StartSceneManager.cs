using UnityEngine;
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
    [SerializeField] private int slideButtonSoundGroupIndex = 1;

    private int currentIndex = 0;
    private bool isMoving = false;
    private static bool hasShownSlideHint = false;

    private void Start()
    {
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

    //==================================================
    // 熟語モードを押したとき
    //==================================================
    public void SelectJukugoMode()
    {
        // 熟語モードを保存
        GameModeData.SelectedMode = GameMode.Idiom;

        TransitionToExplanation();

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
        // 部首モードを保存
        GameModeData.SelectedMode = GameMode.Radical;

        TransitionToExplanation();

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

        UpdatePageIndicator();

        if (slideGuide != null)
        {
            slideGuide.gameObject.SetActive(true);
        }
        PlayFirstSlideHint();
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

        int nextIndex = currentIndex + 1;

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

    private void TransitionToExplanation()
    {
        if (modeSelectGroup == null || explanationGroup == null)
            return;

        CanvasGroup modeGroup = modeSelectGroup.GetComponent<CanvasGroup>();
        if (modeGroup == null)
            modeGroup = modeSelectGroup.AddComponent<CanvasGroup>();

        CanvasGroup explanationCanvasGroup = explanationGroup.GetComponent<CanvasGroup>();
        if (explanationCanvasGroup == null)
            explanationCanvasGroup = explanationGroup.AddComponent<CanvasGroup>();

        explanationGroup.SetActive(true);
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

    private void PlaySlideButtonSE()
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayButtonSE(slideButtonSoundGroupIndex);
    }
}
