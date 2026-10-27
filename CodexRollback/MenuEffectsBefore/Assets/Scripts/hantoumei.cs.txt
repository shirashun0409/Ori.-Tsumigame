using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TapFadeUniversal : MonoBehaviour
{
    public float speed = 2f;

    private Image img;
    private TextMeshProUGUI tmp;

    void Start()
    {
        img = GetComponent<Image>();
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        float a = (Mathf.Sin(Time.time * speed) + 1f) / 2f; // 0〜1
        float alpha = 0.5f + a * 0.5f; // 0.5〜1.0でゆっくり光る

        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }

        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = alpha;
            tmp.color = c;
        }
    }
}
