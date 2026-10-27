using UnityEngine;
using DG.Tweening;

public class ModeCarousel6 : MonoBehaviour
{
    [Header("六角柱")]
    [SerializeField] private float radius = 3.0f;

    [Header("回転")]
    [SerializeField] private float rotateAngle = 60f;
    [SerializeField] private float rotateDuration = 0.45f;

    private bool isRotating = false;


    //==================================================
    // 初期設定
    //==================================================

    private void Start()
    {
        ArrangePanels();
    }


    //==================================================
    // 左ボタン
    //==================================================

    public void RotateLeft()
    {
        if (isRotating)
            return;

        RotateCylinder(rotateAngle);
    }


    //==================================================
    // 右ボタン
    //==================================================

    public void RotateRight()
    {
        if (isRotating)
            return;

        RotateCylinder(-rotateAngle);
    }


    //==================================================
    // 六角柱を回転
    //==================================================

    private void RotateCylinder(float angle)
    {
        isRotating = true;

        transform.DOLocalRotate(
            transform.localEulerAngles +
            new Vector3(0f, angle, 0f),
            rotateDuration
        )
        .SetEase(Ease.InOutCubic)
        .OnComplete(() =>
        {
            isRotating = false;
        });
    }


    //==================================================
    // 6つの面を六角形に配置
    //==================================================

    private void ArrangePanels()
    {
        int count = transform.childCount;

        if (count == 0)
        {
            Debug.LogWarning(
                "ModeCarousel6 にロゴがありません。"
            );

            return;
        }

        float angleStep =
            360f / count;

        for (int i = 0; i < count; i++)
        {
            Transform panel =
                transform.GetChild(i);

            float angle =
                i * angleStep;

            float rad =
                angle * Mathf.Deg2Rad;

            // 六角柱の側面位置
            float x =
                Mathf.Sin(rad) * radius;

            float z =
                -Mathf.Cos(rad) * radius;

            panel.localPosition =
                new Vector3(
                    x,
                    0f,
                    z
                );

            // それぞれの面を六角柱の外側へ向ける
            panel.localRotation =
                Quaternion.Euler(
                    0f,
                    angle,
                    0f
                );
        }
    }
}