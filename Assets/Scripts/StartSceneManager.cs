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
    private int currentIndex = 0;
    private bool isMoving = false;

    private void Start()
    {
        // StartSceneが開いたら、
        // モード選択ボタンをぽわっと表示
        if (modeSelectController != null)
        {
            modeSelectController.ShowButtons();
        }
        else
        {
            Debug.LogError(
                "StartSceneManagerにModeSelectControllerが設定されていません！"
            );
        }
    }

    // 熟語モードを押したとき
    public void SelectJukugoMode()
    {
        modeSelectGroup.SetActive(false);
        explanationGroup.SetActive(true);

        currentIndex = 0;

        for (int i = 0; i < slides.Length; i++)
        {
            if (i == 0)
            {
                slides[i].anchoredPosition = Vector2.zero;
            }
            else
            {
                slides[i].anchoredPosition =
                    new Vector2(slideDistance, 0f);
            }
        }

        UpdatePageIndicator();

        if (slideGuide != null)
        {
            slideGuide.gameObject.SetActive(true);
        }
    }

    // 右タッチ → 次の画像へ
    public void SlideRight()
    {
        if (isMoving) return;
        if (currentIndex >= slides.Length - 1) return;

        isMoving = true;

        RectTransform current = slides[currentIndex];
        RectTransform next = slides[currentIndex + 1];

        // 現在の画像を左へ流す
        current.DOAnchorPosX(
            -slideDistance,
            duration
        ).SetEase(Ease.InOutCubic);

        // 次の画像を右から中央へ
        next.DOAnchorPosX(
            0f,
            duration
        )
        .SetEase(Ease.InOutCubic)
        .OnComplete(() =>
{
    currentIndex++;

    UpdatePageIndicator();

    if (slideGuide != null)
    {
        slideGuide.gameObject.SetActive(false);
    }

    isMoving = false;
});
    }

    // 左タッチ → 前の画像へ
    public void SlideLeft()
    {
        if (isMoving) return;
        if (currentIndex <= 0) return;

        isMoving = true;

        RectTransform current = slides[currentIndex];
        RectTransform prev = slides[currentIndex - 1];

        // 現在の画像を右へ流す
        current.DOAnchorPosX(
            slideDistance,
            duration
        ).SetEase(Ease.InOutCubic);

        // 前の画像を左から中央へ
        prev.DOAnchorPosX(
            0f,
            duration
        )
        .SetEase(Ease.InOutCubic)
        .OnComplete(() =>
{
    currentIndex--;

    UpdatePageIndicator();

    isMoving = false;
});
    }

    // 最後の画像でゲーム開始
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
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
}