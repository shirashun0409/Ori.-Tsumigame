using UnityEngine;
using UnityEngine.UI;

public class NextDisplay : MonoBehaviour
{
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;

    public void SetNextSprites(Sprite left, Sprite right)
    {
        if (leftImage != null) leftImage.sprite = left;
        if (rightImage != null) rightImage.sprite = right;
    }
}
