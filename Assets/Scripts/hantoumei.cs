using UnityEngine;
using UnityEngine.UI;

public class TapImageFade : MonoBehaviour
{
    public float speed = 2f;
    private Image img;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        float a = (Mathf.Sin(Time.time * speed) + 1f) / 2f; // 0〜1
        Color c = img.color;
        c.a = 0.5f + a * 0.5f; // 0.5〜1.0でゆっくり光る
        img.color = c;
    }
}
