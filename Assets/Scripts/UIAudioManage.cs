using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    [Header("Button Sound")]
    [SerializeField] private AudioClip buttonSE;

    private AudioSource audioSource;

    private void Awake()
    {
        // すでに存在していたら新しい方を削除
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // シーンを移動しても残す
        DontDestroyOnLoad(gameObject);

        // AudioSourceを取得
        audioSource = GetComponent<AudioSource>();

        // AudioSourceがなければ自動で追加
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // BGMではなく効果音として使う
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// ボタンを押したときの効果音
    /// </summary>
    public void PlayButtonSE()
    {
        if (buttonSE == null)
        {
            Debug.LogWarning("UIAudioManager: Button SEが設定されていません。");
            return;
        }

        audioSource.PlayOneShot(buttonSE);
    }
}