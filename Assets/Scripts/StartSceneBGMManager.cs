using UnityEngine;
using System.Collections;

public class StartSceneBGMManager : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] private AudioClip startSceneBGM;

    [Header("Settings")]
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private float fadeInDuration = 2.0f;
    [SerializeField] private bool loop = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource =
                gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = loop;

        // 最初は無音
        audioSource.volume = 0f;
    }


    private void Start()
    {
        if (startSceneBGM == null)
        {
            Debug.LogWarning(
                "StartSceneBGMManager: BGMが設定されていません。"
            );

            return;
        }

        // BGMを設定
        audioSource.clip = startSceneBGM;

        // 再生開始
        audioSource.Play();

        // フェードイン開始
        StartCoroutine(FadeInBGM());
    }


    private IEnumerator FadeInBGM()
    {
        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;

            float progress =
                timer / fadeInDuration;

            audioSource.volume =
                Mathf.Lerp(
                    0f,
                    volume,
                    progress
                );

            yield return null;
        }

        // 最後は確実に設定音量にする
        audioSource.volume = volume;
    }
}