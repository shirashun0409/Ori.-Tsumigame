using UnityEngine;
using DG.Tweening;

public class ShojiOpen : MonoBehaviour
{
    public RectTransform leftShoji;
    public RectTransform rightShoji;

    public void OpenShoji()
    {
        // 左の障子を左へスライド
        leftShoji.DOAnchorPosX(-800, 1f).SetEase(Ease.InOutQuad);

        // 右の障子を右へスライド
        rightShoji.DOAnchorPosX(800, 1f).SetEase(Ease.InOutQuad);
    }
}
