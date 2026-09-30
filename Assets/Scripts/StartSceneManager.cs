using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

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

    //==================================================
    // 熟語モードを押したとき
    //==================================================
    public void SelectJukugoMode()
    {
        modeSelectGroup.SetActive(false);
        explanationGroup.SetActive(true);

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

        if (slideGuide != null)
        {
            slideGuide.gameObject.SetActive(false);
        }

        DOVirtual.DelayedCall(duration, () =>
        {
            isMoving = false;
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

        DOVirtual.DelayedCall(duration, () =>
        {
            isMoving = false;
        });
    }

    //==================================================
    // 最後の画像でゲーム開始
    //==================================================
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
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
}