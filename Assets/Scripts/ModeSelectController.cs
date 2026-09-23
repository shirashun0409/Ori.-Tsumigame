using UnityEngine;
using DG.Tweening;

public class ModeSelectController : MonoBehaviour
{
    [SerializeField] private GameObject jukugoButton;
    [SerializeField] private GameObject busyuButton;
    [SerializeField] private GameObject yomiButton; // ★追加！



    public void ShowButtons()
    {
        Debug.Log("ShowButtonsが呼ばれた！");

        jukugoButton.SetActive(true);
        busyuButton.SetActive(true);
        yomiButton.SetActive(true);

        jukugoButton.transform.localScale = Vector3.zero;
        busyuButton.transform.localScale = Vector3.zero;
        yomiButton.transform.localScale = Vector3.zero;

        jukugoButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        busyuButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f);
        yomiButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.4f);
    }

}
