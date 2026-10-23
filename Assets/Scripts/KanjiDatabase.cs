
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームモードに応じた漢字データと画像を提供します。
/// </summary>
public static class KanjiDatabase
{
    private static KanjiData[] kanjiList;
    private static Dictionary<string, KanjiData> byKanji;
    private static GameMode? initializedMode;

    public static KanjiData GetRandomKanji()
    {
        EnsureInitialized();

        if (kanjiList.Length == 0)
        {
            Debug.LogError(
                "使用できる漢字がありません。KanjiRegistryと画像フォルダを確認してください。");
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
        GameMode mode = GameModeData.SelectedMode;

        // モードが変わっていなければ、初期化を繰り返さない
        if (kanjiList != null && initializedMode == mode)
        {
            return;
        }

        var list = new List<KanjiData>();
        byKanji = new Dictionary<string, KanjiData>();

        string spriteFolder = GetSpriteFolder(mode);

        foreach (KanjiRegistry.Entry entry in KanjiRegistry.Entries)
        {
            Sprite sprite = Resources.Load<Sprite>(
                spriteFolder + entry.Kanji);

            // 部首モードでは、Radicalフォルダに画像がある漢字だけ採用
            if (mode == GameMode.Radical && sprite == null)
            {
                continue;
            }

            // 熟語・読みモードは、まだ従来の画像を使用
            if (sprite == null && mode != GameMode.Radical)
            {
                sprite = Resources.Load<Sprite>(
                    "KanjiSprites/" + entry.Kanji);
            }

            if (sprite == null)
            {
                Debug.LogWarning(
                    $"スプライトが見つかりません: {entry.Kanji}");
                continue;
            }

            var data = new KanjiData
            {
                kanji = entry.Kanji,
                radical = entry.Radical,
                readings = entry.Readings,
                idiomPartners = entry.IdiomPartners,
                sprite = sprite
            };

            list.Add(data);
            byKanji[entry.Kanji] = data;
        }

        kanjiList = list.ToArray();
        initializedMode = mode;

        Debug.Log(
            $"{mode}モード：使用できる漢字は{kanjiList.Length}個です。");
    }

    private static string GetSpriteFolder(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.Radical:
                return "KanjiSprites/Radical/";

            case GameMode.Idiom:
                return "KanjiSprites/Idiom/";

            case GameMode.Reading:
            default:
                return "KanjiSprites/Reading/";
        }
    }
}
