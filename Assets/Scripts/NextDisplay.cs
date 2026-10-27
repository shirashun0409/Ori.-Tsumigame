using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NextDisplay : MonoBehaviour
{
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;

    public void SetNextSprites(Sprite left, Sprite right)
    {
        if (leftImage != null) leftImage.sprite = left;
        if (rightImage != null) rightImage.sprite = right;

        PunchPreview(leftImage);
        PunchPreview(rightImage);
    }

    private void PunchPreview(Image image)
    {
        if (image == null)
            return;

        RectTransform rect = image.rectTransform;
        rect.DOKill();
        rect.localScale = Vector3.one;
        rect.DOPunchScale(new Vector3(0.18f, 0.18f, 0f), 0.22f, 5, 0.65f);
    }
}
