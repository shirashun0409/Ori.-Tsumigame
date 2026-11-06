using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ModeSelectController : MonoBehaviour
{
    [SerializeField] private GameObject jukugoButton;
    [SerializeField] private GameObject busyuButton;
    [SerializeField] private GameObject yomiButton;
    private GameObject testPreparationButton;

    public void SetTestPreparationButton(GameObject button)
    {
        testPreparationButton = button;
    }

    public void ShowButtons()
    {
        if (jukugoButton == null || busyuButton == null)
        {
            Debug.LogError("ModeSelectController: モードボタンが設定されていません。", this);
            return;
        }

        MenuSceneEffects.AttachButtonEffects(jukugoButton.transform.parent);

        jukugoButton.SetActive(true);
        busyuButton.SetActive(true);
        if (testPreparationButton != null)
            testPreparationButton.SetActive(true);
        if (yomiButton != null)
        {
            yomiButton.transform.DOKill();
            yomiButton.SetActive(false);
        }

        // ボタン背景を不透明度100%にし、子ラベルとクリック判定を維持する。
        Image radicalButtonImage = busyuButton.GetComponent<Image>();
        if (radicalButtonImage != null)
        {
            Color imageColor = radicalButtonImage.color;
            imageColor.a = 1f;
            radicalButtonImage.color = imageColor;
        }

        if (MenuSceneEffects.ReducedMotion)
        {
            jukugoButton.transform.localScale = Vector3.one;
            busyuButton.transform.localScale = Vector3.one;
            if (testPreparationButton != null)
                testPreparationButton.transform.localScale = Vector3.one;
        }
        else
        {
            jukugoButton.transform.localScale = Vector3.zero;
            busyuButton.transform.localScale = Vector3.zero;
            if (testPreparationButton != null)
                testPreparationButton.transform.localScale = Vector3.zero;

            jukugoButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            busyuButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f);
            if (testPreparationButton != null)
            {
                testPreparationButton.transform
                    .DOScale(1f, 0.5f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.4f);
            }
        }
    }
}
