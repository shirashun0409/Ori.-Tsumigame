using UnityEngine;

public class LogoSoftRotate : MonoBehaviour
{
    public float angle = 1f;   // ほんの少しだけ角度が変わる
    public float speed = 0.5f; // ゆっくり

    private Quaternion startRot;

    void Start()
    {
        startRot = transform.localRotation;
    }

    void Update()
    {
        float rot = Mathf.Sin(Time.time * speed) * angle;
        transform.localRotation = startRot * Quaternion.Euler(0, rot, 0);
    }
}
