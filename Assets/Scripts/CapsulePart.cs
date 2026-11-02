using UnityEngine;
using TMPro;

public class CapsulePart : MonoBehaviour
{
    public static readonly Color IdiomColor =
        new Color(0.6588f, 0.3294f, 0.2627f, 1f);

    public static Color GetModeAccentColor(GameMode mode)
    {
        return mode == GameMode.Idiom
            ? IdiomColor
            : new Color(0.88f, 0.82f, 0.65f, 1f);
    }

    private SpriteRenderer spriteRenderer;
    private KanjiData kanjiData;
    private Color defaultColor;
    private TextMeshPro fallbackLabel;


    public string Kanji
    {
        get
        {
            return kanjiData != null
                ? kanjiData.kanji
                : "";
        }
    }


    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }


    public void SetKanji(KanjiData data)
    {
        if (data == null)
        {
            Debug.LogError(
                "KanjiDataが設定されていません。"
            );

            return;
        }

        kanjiData = data;

        if (data.sprite == null)
        {
            spriteRenderer.enabled = false;
            if (fallbackLabel == null)
            {
                GameObject labelObject = new GameObject("KanjiFallback", typeof(TextMeshPro));
                labelObject.transform.SetParent(transform, false);
                labelObject.transform.localPosition = Vector3.zero;
                labelObject.transform.localScale = Vector3.one * 0.2f;
                fallbackLabel = labelObject.GetComponent<TextMeshPro>();
                TMP_Text existingText = FindAnyObjectByType<TMP_Text>();
                if (existingText != null)
                    fallbackLabel.font = existingText.font;
                fallbackLabel.fontSize = 4.5f;
                fallbackLabel.alignment = TextAlignmentOptions.Center;
                fallbackLabel.color = Color.white;
                fallbackLabel.rectTransform.sizeDelta = new Vector2(5f, 5f);
                labelObject.GetComponent<MeshRenderer>().sortingOrder = 2;
            }
            fallbackLabel.text = data.kanji;
        }
        else
        {
            if (fallbackLabel != null)
                fallbackLabel.gameObject.SetActive(false);
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = data.sprite;
        }
        if (GameModeData.SelectedMode == GameMode.Idiom)
        {
            spriteRenderer.color = IdiomColor;
        }
        else
        {
            spriteRenderer.color = defaultColor;
        }
    }


    public KanjiData GetKanjiData()
    {
        return kanjiData;
    }
}