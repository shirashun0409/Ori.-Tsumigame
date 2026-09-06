using UnityEngine;

public class LogoColorPulse : MonoBehaviour
{
    public Color baseColor = Color.white;   // 普段の色
    public float pulseStrength = 0.5f;      // 明るくなる強さ
    public float speed = 2f;                // 光のゆらぎの速さ

    private Material mat;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f; // 0〜1
        float factor = 1f + t * pulseStrength;              // 1〜1+pulse

        Color c = baseColor * factor;
        mat.color = c;
    }
}
