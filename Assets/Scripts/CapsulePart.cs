using UnityEngine;

public class CapsulePart : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(CapsuleColor color)
    {
        switch (color)
        {
            case CapsuleColor.Red:
                spriteRenderer.color = Color.red;
                break;

            case CapsuleColor.Blue:
                spriteRenderer.color = Color.blue;
                break;

            case CapsuleColor.Yellow:
                spriteRenderer.color = Color.yellow;
                break;
        }
    }
}