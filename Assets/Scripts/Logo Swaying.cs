using UnityEngine;

public class LogoFloat3D : MonoBehaviour
{
    public float amplitude = 0.2f;  // 揺れる高さ
    public float speed = 1.5f;      // 揺れる速さ

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}
