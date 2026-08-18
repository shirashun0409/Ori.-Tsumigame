using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="KanjiRegistry"/> の定義を読み込み、ゲーム中の漢字データを提供します。
/// </summary>
public static class KanjiDatabase
{
    private const string SpriteResourcePath = "KanjiSprites/";

    private static KanjiData[] kanjiList;
    private static Dictionary<string, KanjiData> byKanji;

    public static KanjiData GetRandomKanji()
    {
        EnsureInitialized();

        if (kanjiList.Length == 0)
        {
            Debug.LogError("KanjiRegistry に漢字が登録されていません。");
            return null;
        }

        int index = Random.Range(0, kanjiList.Length);
        return kanjiList[index];
    }

    public static KanjiData GetByKanji(string kanji)
    {
        EnsureInitialized();

        if (byKanji.TryGetValue(kanji, out KanjiData data))
        {
            return data;
        }

        return null;
    }

    public static IReadOnlyList<KanjiData> GetAll()
    {
        EnsureInitialized();
        return kanjiList;
    }

    private static void EnsureInitialized()
    {
        if (kanjiList != null)
        {
            return;
        }

        var list = new List<KanjiData>();
        byKanji = new Dictionary<string, KanjiData>();

        foreach (KanjiRegistry.Entry entry in KanjiRegistry.Entries)
        {
            var data = new KanjiData
            {
                kanji = entry.Kanji,
                radical = entry.Radical,
                readings = entry.Readings,
                idiomPartners = entry.IdiomPartners,
                sprite = Resources.Load<Sprite>(SpriteResourcePath + entry.Kanji)
            };

            if (data.sprite == null)
            {
                Debug.LogWarning(
                    $"スプライトが見つかりません: {entry.Kanji} " +
                    $"(Resources/{SpriteResourcePath}{entry.Kanji})");
            }

            list.Add(data);
            byKanji[entry.Kanji] = data;
        }

        kanjiList = list.ToArray();
    }
}
