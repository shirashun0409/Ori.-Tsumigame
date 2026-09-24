using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ShogiOpen3D : MonoBehaviour
{
    public Transform leftShoji;
    public Transform rightShoji;

    public GameObject logoGroup;
    public GameObject pressToStart;

    public Camera mainCamera;

    private bool isOpened = false; // ← 追加：一度だけ実行するためのフラグ

    public void CameraZoom()
    {
        mainCamera.transform.DOMoveZ(
            mainCamera.transform.position.z + 3f,
            1f
        ).SetEase(Ease.InOutQuad);
    }

    public void OpenShoji()
    {
        if (isOpened) return; // ← 追加：2回目以降は無視
        isOpened = true;      // ← 追加：一度実行したらロック

        // ① ロゴを消す
        logoGroup.SetActive(false);

        // ② PressToStartを消す
        pressToStart.SetActive(false);

        // ③ カメラを前にズーム
        mainCamera.transform.DOMoveZ(
            mainCamera.transform.position.z + 3f,
            1f
        ).SetEase(Ease.InOutQuad);

        // ④ 障子を開く
        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            leftShoji.DOMoveX(
                leftShoji.position.x - 9f,
                1f
            ).SetEase(Ease.InOutQuad)
        );

        sequence.Join(
            rightShoji.DOMoveX(
                rightShoji.position.x + 9f,
                1f
            ).SetEase(Ease.InOutQuad)
        );

        // ⑤ 障子が開き終わったらStartSceneへ
        sequence.OnComplete(() =>
        {
            SceneManager.LoadScene("StartScene");
        });
    }
}
