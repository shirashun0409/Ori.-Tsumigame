using UnityEngine;

public class CapsulePart : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private KanjiData kanjiData;


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
    }


    public KanjiData GetKanjiData()
    {
        return kanjiData;
    }
}