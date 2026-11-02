using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class NextDisplay : MonoBehaviour
{
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    private TMP_Text leftFallback;
    private TMP_Text rightFallback;

    public void SetNextSprites(Sprite left, Sprite right)
    {
        if (leftImage != null) leftImage.sprite = left;
        if (rightImage != null) rightImage.sprite = right;

        PunchPreview(leftImage);
        PunchPreview(rightImage);
    }

    public void SetNextKanji(KanjiData left, KanjiData right)
    {
        SetNextSpriteAndFallback(leftImage, ref leftFallback, left);
        SetNextSpriteAndFallback(rightImage, ref rightFallback, right);
        PunchPreview(leftImage);
        PunchPreview(rightImage);
    }

    private static void SetNextSpriteAndFallback(
        Image image,
        ref TMP_Text fallback,
        KanjiData data)
    {
        if (image == null || data == null)
            return;

        image.sprite = data.sprite;
        image.enabled = data.sprite != null;

        if (data.sprite != null)
        {
            if (fallback != null)
                fallback.gameObject.SetActive(false);
            return;
        }

        if (fallback == null)
        {
            GameObject textObject = new GameObject(
                "KanjiFallback",
                typeof(RectTransform),
                typeof(TextMeshProUGUI));
            textObject.transform.SetParent(image.transform, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            fallback = textObject.GetComponent<TextMeshProUGUI>();
            TMP_Text existingText = FindAnyObjectByType<TMP_Text>();
            if (existingText != null)
                fallback.font = existingText.font;
            fallback.fontSize = 42f;
            fallback.alignment = TextAlignmentOptions.Center;
            fallback.color = Color.white;
            fallback.raycastTarget = false;
        }

        fallback.gameObject.SetActive(true);
        fallback.text = data.kanji;
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
