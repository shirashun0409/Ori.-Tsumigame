using UnityEngine;

public class ButtonSE : MonoBehaviour
{
    [Header("Button Sound Type")]
    [SerializeField]
    private int soundGroupIndex = 0;

    [SerializeField] private AudioClip soundOverride;

    /// <summary>
    /// ボタンを押したときに呼び出す
    /// </summary>
    public void Play()
    {
        if (UIAudioManager.Instance == null)
        {
            Debug.LogWarning(
                "ButtonSE: UIAudioManagerが見つかりません。"
            );

            return;
        }

        if (soundOverride != null)
        {
            UIAudioManager.Instance.PlaySound(soundOverride);
            return;
        }

        UIAudioManager.Instance.PlayButtonSE(soundGroupIndex);
    }
}