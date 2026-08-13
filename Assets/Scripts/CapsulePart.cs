using UnityEngine;

public class CapsulePart : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private KanjiData kanjiData;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetKanji(KanjiData data)
    {
        if (data == null)
        {
            Debug.LogError("KanjiDataが設定されていません。");
            return;
        }

        kanjiData = data;
        spriteRenderer.sprite = data.sprite;
    }

    public KanjiData GetKanjiData()
    {
        return kanjiData;
    }
}