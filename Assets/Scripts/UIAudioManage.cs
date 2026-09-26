using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    [System.Serializable]
    public class ButtonSoundGroup
    {
        public string groupName;

        [Tooltip("この種類のボタンで使用するSE")]
        public AudioClip[] sounds;

        [HideInInspector]
        public int lastPlayedIndex = -1;
    }

    [Header("Button Sound Groups")]
    [SerializeField]
    private ButtonSoundGroup[] buttonSoundGroups;

    private AudioSource audioSource;

    private void Awake()
    {
        // すでにUIAudioManagerが存在する場合
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

        // なければ自動追加
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 自動再生しない
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// 指定した種類のボタンSEを再生する
    /// </summary>
    public void PlayButtonSE(int groupIndex)
    {
        if (buttonSoundGroups == null ||
            buttonSoundGroups.Length == 0)
        {
            Debug.LogWarning(
                "UIAudioManager: ボタンSEグループが設定されていません。"
            );

            return;
        }

        if (groupIndex < 0 ||
            groupIndex >= buttonSoundGroups.Length)
        {
            Debug.LogWarning(
                "UIAudioManager: 存在しないSEグループが指定されています。"
            );

            return;
        }

        ButtonSoundGroup group = buttonSoundGroups[groupIndex];

        if (group.sounds == null ||
            group.sounds.Length == 0)
        {
            Debug.LogWarning(
                "UIAudioManager: "
                + group.groupName
                + " にSEが設定されていません。"
            );

            return;
        }

        // 有効なSEが何個あるか確認
        int validCount = 0;

        foreach (AudioClip clip in group.sounds)
        {
            if (clip != null)
            {
                validCount++;
            }
        }

        if (validCount == 0)
        {
            Debug.LogWarning(
                "UIAudioManager: "
                + group.groupName
                + " に有効なSEがありません。"
            );

            return;
        }

        // SEを選ぶ
        int selectedIndex = GetRandomIndex(group);

        AudioClip selectedClip = group.sounds[selectedIndex];

        // 再生
        audioSource.PlayOneShot(selectedClip);

        // 今回再生したSEを記録
        group.lastPlayedIndex = selectedIndex;
    }

    /// <summary>
    /// 前回と同じSEを避けてランダム選択
    /// </summary>
    private int GetRandomIndex(ButtonSoundGroup group)
    {
        int validCount = 0;

        foreach (AudioClip clip in group.sounds)
        {
            if (clip != null)
            {
                validCount++;
            }
        }

        // SEが1つしかない場合
        // 同じ音しかないので、そのまま再生
        if (validCount == 1)
        {
            for (int i = 0; i < group.sounds.Length; i++)
            {
                if (group.sounds[i] != null)
                {
                    return i;
                }
            }
        }

        // 前回と違うSEを探す
        int selectedIndex;

        do
        {
            selectedIndex =
                Random.Range(0, group.sounds.Length);
        }
        while (
            group.sounds[selectedIndex] == null ||
            selectedIndex == group.lastPlayedIndex
        );

        return selectedIndex;
    }
}