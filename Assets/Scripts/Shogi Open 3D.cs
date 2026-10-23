using UnityEngine;
using DG.Tweening;

public class ShogiOpen3D : MonoBehaviour
{
    public Transform leftShoji;
    public Transform rightShoji;

    public GameObject logoGroup;
    public GameObject pressToStart;

    public Camera mainCamera;

    private bool isOpened = false; // ← 追加：一度だけ実行するためのフラグ

    private void Start()
    {
        if (pressToStart != null && !MenuSceneEffects.ReducedMotion)
        {
            Vector3 promptScale = pressToStart.transform.localScale;
            pressToStart.transform
                .DOScale(promptScale * 1.02f, 0.9f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void CameraZoom()
    {
        mainCamera.transform.DOMoveZ(
            mainCamera.transform.position.z + 3f,
            MenuSceneEffects.ReducedMotion ? 0.2f : 1f
        ).SetEase(Ease.InOutQuad);
    }

    public void OpenShoji()
    {
        if (isOpened) return; // ← 追加：2回目以降は無視
        isOpened = true;      // ← 追加：一度実行したらロック
        float openingDuration = MenuSceneEffects.ReducedMotion ? 0.2f : 1f;

        // ① ロゴを消す
        logoGroup.SetActive(false);

        // ② PressToStartを消す
        pressToStart.SetActive(false);

        // ③ カメラを前にズーム
        mainCamera.transform.DOMoveZ(
            mainCamera.transform.position.z + 3f,
            openingDuration
        ).SetEase(Ease.InOutQuad);

        // ④ 障子を開く
        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            leftShoji.DOMoveX(
                leftShoji.position.x - 12f,
                openingDuration
            ).SetEase(Ease.InOutQuad)
        );

        sequence.Join(
            rightShoji.DOMoveX(
                rightShoji.position.x + 12f,
                openingDuration
            ).SetEase(Ease.InOutQuad)
        );

        // ⑤ 障子が開き終わったらStartSceneへ
        sequence.OnComplete(() =>
        {
            SceneTransitionEffect.LoadScene("StartScene");
        });
    }
}
