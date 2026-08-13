using UnityEngine;

public class CapsulePart : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetKanji(Sprite kanjiSprite)
    {
        spriteRenderer.sprite = kanjiSprite;
    }
}