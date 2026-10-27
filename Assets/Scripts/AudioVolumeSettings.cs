using System.Collections.Generic;
using UnityEngine;

public static class AudioVolumeSettings
{
    private const string BgmPreferenceKey = "AudioVolumeSettings.BGM";
    private const string SePreferenceKey = "AudioVolumeSettings.SE";
    private const float DefaultVolume = 0.75f;

    private sealed class RegisteredSource
    {
        public AudioSource source;
        public float baseVolume;
        public float fade = 1f;
        public bool isBgm;
    }

    private static readonly List<RegisteredSource> Sources = new List<RegisteredSource>();

    public static float BgmVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(BgmPreferenceKey, DefaultVolume));
    public static float SeVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(SePreferenceKey, DefaultVolume));

    public static void SetBgmVolume(float volume)
    {
        PlayerPrefs.SetFloat(BgmPreferenceKey, Mathf.Clamp01(volume));
        ApplyRegisteredSources(true);
    }

    public static void SetSeVolume(float volume)
    {
        PlayerPrefs.SetFloat(SePreferenceKey, Mathf.Clamp01(volume));
        ApplyRegisteredSources(false);
    }

    public static void RegisterBgmSource(AudioSource source, float baseVolume)
    {
        RegisterSource(source, baseVolume, true);
    }

    public static void RegisterSeSource(AudioSource source)
    {
        RegisterSource(source, source != null ? source.volume : 1f, false);
    }

    public static void UnregisterSource(AudioSource source)
    {
        Sources.RemoveAll(entry => entry.source == null || entry.source == source);
    }

    public static void SetBgmFade(AudioSource source, float baseVolume, float fade)
    {
        if (source != null)
        {
            float clampedFade = Mathf.Clamp01(fade);
            for (int i = 0; i < Sources.Count; i++)
            {
                RegisteredSource entry = Sources[i];
                if (entry.source == source)
                {
                    entry.baseVolume = Mathf.Clamp01(baseVolume);
                    entry.fade = clampedFade;
                    ApplyVolume(entry);
                    return;
                }
            }

            source.volume = baseVolume * BgmVolume * clampedFade;
        }
    }

    private static void RegisterSource(AudioSource source, float baseVolume, bool isBgm)
    {
        if (source == null)
        {
            return;
        }

        UnregisterSource(source);
        Sources.Add(new RegisteredSource
        {
            source = source,
            baseVolume = Mathf.Clamp01(baseVolume),
            isBgm = isBgm
        });
        ApplyVolume(Sources[Sources.Count - 1]);
    }

    private static void ApplyRegisteredSources(bool isBgm)
    {
        for (int i = Sources.Count - 1; i >= 0; i--)
        {
            RegisteredSource entry = Sources[i];
            if (entry.source == null)
            {
                Sources.RemoveAt(i);
                continue;
            }

            if (entry.isBgm == isBgm)
            {
                ApplyVolume(entry);
            }
        }
    }

    private static void ApplyVolume(RegisteredSource entry)
    {
        float volume = entry.isBgm ? BgmVolume : SeVolume;
        entry.source.volume = entry.baseVolume * volume * entry.fade;
    }
}
