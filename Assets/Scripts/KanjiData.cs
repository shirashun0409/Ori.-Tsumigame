using UnityEngine;

/// <summary>
/// 1文字分の漢字データ。定義は <see cref="KanjiRegistry"/>、取得は <see cref="KanjiDatabase"/> から行います。
/// </summary>
public class KanjiData
{
    public string kanji;
    public Sprite sprite;
    public string radical;
    public string[] readings;
    public string[] idiomPartners;
}
