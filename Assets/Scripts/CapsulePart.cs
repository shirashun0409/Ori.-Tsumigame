using UnityEngine;

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

        spriteRenderer.sprite =
            data.sprite;
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