using UnityEngine;

[CreateAssetMenu(
    fileName = "KanjiData",
    menuName = "Kanji Game/Kanji Data")]
public class KanjiData : ScriptableObject
{
    [Header("基本情報")]
    public string kanji;

    [Header("表示")]
    public Sprite sprite;

    [Header("部首")]
    public string radical;

    [Header("読み")]
    public string[] readings;

    [Header("二字熟語")]
    public string[] idioms;
}