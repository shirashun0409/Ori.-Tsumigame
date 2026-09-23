using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StartSceneManager : MonoBehaviour
{
    [Header("画面")]
    [SerializeField] private GameObject modeSelectGroup;
    [SerializeField] private GameObject explanationGroup;

    [Header("スライド")]
    [SerializeField] private RectTransform[] slides;   // 複数画像
    [SerializeField] private float slideDistance = 1800f;
    [SerializeField] private float duration = 0.6f;

    private int currentIndex = 0;
    private bool isMoving = false;

    // 熟語モードを押したとき
    public void SelectJukugoMode()
    {
        modeSelectGroup.SetActive(false);
        explanationGroup.SetActive(true);

        // 最初のスライドだけ中央に置く
        for (int i = 0; i < slides.Length; i++)
        {
            if (i == 0)
                slides[i].anchoredPosition = Vector2.zero;
            else
                slides[i].anchoredPosition = new Vector2(slideDistance, 0f);
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
        current.DOAnchorPosX(-slideDistance, duration)
            .SetEase(Ease.InOutCubic);

        // 次の画像を右から中央へ
        next.DOAnchorPosX(0f, duration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                currentIndex++;
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
        current.DOAnchorPosX(slideDistance, duration)
            .SetEase(Ease.InOutCubic);

        // 前の画像を左から中央へ
        prev.DOAnchorPosX(0f, duration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                currentIndex--;
                isMoving = false;
            });
    }

    // 最後の画像でゲーム開始
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
