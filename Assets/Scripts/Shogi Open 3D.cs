using UnityEngine;
using DG.Tweening;

public class ShogiOpen3D : MonoBehaviour
{
    public Transform leftShoji;
    public Transform rightShoji;

    public GameObject logoGroup;
    public GameObject pressToStart;

    public Camera mainCamera;   // ← Step2 追加

    public void CameraZoom()
    {
        mainCamera.transform.DOMoveZ(mainCamera.transform.position.z + 3f, 1f)
            .SetEase(Ease.InOutQuad);
    }

    public void OpenShoji()
    {
        // ① ロゴを消す
        logoGroup.SetActive(false);

        // ② PressToStart を消す
        pressToStart.SetActive(false);

        // ③ カメラを前にズーム（Step2）
        CameraZoom();

        // ④ 障子を開く
        leftShoji.DOMoveX(leftShoji.position.x - 9f, 1f).SetEase(Ease.InOutQuad);
        rightShoji.DOMoveX(rightShoji.position.x + 9f, 1f).SetEase(Ease.InOutQuad);
    }
}
