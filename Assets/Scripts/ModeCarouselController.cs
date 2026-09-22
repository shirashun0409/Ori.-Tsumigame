using UnityEngine;
using DG.Tweening;

public class ModeCarouselController : MonoBehaviour
{
    [Header("Carousel Settings")]
    [SerializeField] private float rotationDuration = 0.5f;

    [Header("3D Circle")]
    [SerializeField] private float radius = 3.0f;

    private bool isRotating = false;

    // 現在の中心位置
    private int currentIndex = 0;

    // 子オブジェクトの数
    private int itemCount;


    //==================================================
    // 初期設定
    //==================================================
    private void Start()
    {
        itemCount = transform.childCount;

        ArrangeItems();
    }


    //==================================================
    // 左ボタン
    //==================================================
    public void RotateLeft()
    {
        if (isRotating)
            return;

        if (itemCount <= 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = itemCount - 1;

        ArrangeItemsWithAnimation();
    }


    //==================================================
    // 右ボタン
    //==================================================
    public void RotateRight()
    {
        if (isRotating)
            return;

        if (itemCount <= 0)
            return;

        currentIndex++;

        if (currentIndex >= itemCount)
            currentIndex = 0;

        ArrangeItemsWithAnimation();
    }


    //==================================================
    // 初期配置
    //==================================================
    private void ArrangeItems()
    {
        if (itemCount == 0)
            return;

        for (int i = 0; i < itemCount; i++)
        {
            Transform item = transform.GetChild(i);

            float angle =
                (i - currentIndex) *
                (360f / itemCount);

            SetItemPosition(item, angle);
        }
    }


    //==================================================
    // 回転アニメーション
    //==================================================
    private void ArrangeItemsWithAnimation()
    {
        isRotating = true;

        for (int i = 0; i < itemCount; i++)
        {
            Transform item = transform.GetChild(i);

            float angle =
                (i - currentIndex) *
                (360f / itemCount);

            Vector3 targetPosition =
                GetPosition(angle);

            item.DOLocalMove(
                targetPosition,
                rotationDuration
            )
            .SetEase(Ease.InOutCubic);
        }

        DOVirtual.DelayedCall(
            rotationDuration,
            () =>
            {
                isRotating = false;
            }
        );
    }


    //==================================================
    // 円周上の位置を計算
    //==================================================
    private Vector3 GetPosition(float angle)
    {
        float radian =
            angle * Mathf.Deg2Rad;

        float x =
            Mathf.Sin(radian) * radius;

        float z =
            -Mathf.Cos(radian) * radius;

        return new Vector3(
            x,
            0f,
            z
        );
    }


    //==================================================
    // オブジェクトを配置
    //==================================================
    private void SetItemPosition(
        Transform item,
        float angle)
    {
        item.localPosition =
            GetPosition(angle);
    }
}