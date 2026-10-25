
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームモードに応じた漢字データと画像を提供します。
/// </summary>
public static class KanjiDatabase
{
    private const int RadicalsPerGame = 20;

    private readonly struct RadicalInfo
    {
        public readonly string Meaning;
        public readonly string Formation;

        public RadicalInfo(string meaning, string formation)
        {
            Meaning = meaning;
            Formation = formation;
        }
    }

    private static KanjiData[] kanjiList;
    private static KanjiData[] radicalKanjiList;
    private static Dictionary<string, KanjiData> byKanji;
    private static GameMode? initializedMode;
    private static string[] selectedRadicals = Array.Empty<string>();
    private static string[] previousRadicals = Array.Empty<string>();

    private static readonly Dictionary<string, string> RadicalLabels =
        new Dictionary<string, string>
        {
            { "一", "いち" }, { "丨", "ぼう" }, { "大", "だい" },
            { "人", "人" }, { "力", "ちから" },
            { "子", "こ" }, { "干", "かん" }, { "文", "ぶん" },
            { "面", "めん" }, { "穴", "あなかんむり" }, { "土", "つちへん" },
            { "水", "さんずい" }, { "辶", "しんにょう" }, { "車", "くるまへん" },
            { "山", "やまへん" }, { "木", "きへん" }, { "玉", "おうへん" },
            { "方", "ほうへん" }, { "夕", "た" }, { "日", "にちへん" },
            { "月", "つきへん" }, { "食", "しょくへん" }, { "風", "かぜ" },
            { "金", "かねへん" }, { "攵", "のぶん" }, { "用", "よう" },
            { "口", "くちへん" }, { "宀", "うかんむり" }, { "囗", "くにがまえ" },
            { "凵", "うけばこ" }, { "雨", "あめかんむり" }, { "心", "こころ" },
            { "火", "ひへん" }, { "手", "てへん" }, { "行", "ぎょうがまえ" },
            { "曰", "ひらび" }, { "阝", "おおざと" }, { "牛", "うしへん" },
            { "色", "いろ" }, { "戈", "ほこづくり" }, { "長", "ながい" },
            { "糸", "いとへん" }, { "言", "ごんべん" }, { "艸", "くさかんむり" },
            { "竹", "たけかんむり" }, { "女", "おんなへん" }, { "田", "たへん" },
            { "石", "いしへん" }, { "門", "もんがまえ" }, { "耳", "みみへん" },
            { "示", "しめすへん" }, { "衣", "ころもへん" }, { "匸", "かくしがまえ" },
            { "匚", "はこがまえ" }, { "冂", "けいがまえ" }, { "犬", "けものへん" },
            { "灬", "れっか" }
        };

    private static readonly Dictionary<string, RadicalInfo> RadicalInfoByRoot =
        new Dictionary<string, RadicalInfo>
        {
            { "一", new RadicalInfo("数のひとつ", "指事") },
            { "丨", new RadicalInfo("縦の線", "指事") },
            { "大", new RadicalInfo("大きい・人", "象形") },
            { "人", new RadicalInfo("人", "象形") },
            { "力", new RadicalInfo("力・腕の力", "象形") },
            { "子", new RadicalInfo("子ども", "象形") },
            { "干", new RadicalInfo("盾・さえぎる", "象形") },
            { "文", new RadicalInfo("模様・文字", "象形") },
            { "面", new RadicalInfo("顔・表面", "象形") },
            { "穴", new RadicalInfo("穴・洞穴", "象形") },
            { "土", new RadicalInfo("土・大地", "象形") },
            { "水", new RadicalInfo("水・流れる水", "象形") },
            { "辶", new RadicalInfo("道・歩く", "象形") },
            { "車", new RadicalInfo("車", "象形") },
            { "山", new RadicalInfo("山", "象形") },
            { "木", new RadicalInfo("木・植物", "象形") },
            { "玉", new RadicalInfo("玉・宝石", "象形") },
            { "方", new RadicalInfo("方向・方角", "象形") },
            { "夕", new RadicalInfo("夕方・月", "象形") },
            { "日", new RadicalInfo("太陽・日", "象形") },
            { "月", new RadicalInfo("月", "象形") },
            { "食", new RadicalInfo("食べ物・食べる", "会意") },
            { "風", new RadicalInfo("風", "形声") },
            { "金", new RadicalInfo("金属・金", "象形") },
            { "攵", new RadicalInfo("手に持った棒・動作", "象形") },
            { "用", new RadicalInfo("使う・器", "象形") },
            { "口", new RadicalInfo("口・開いた形", "象形") },
            { "宀", new RadicalInfo("屋根・家", "象形") },
            { "囗", new RadicalInfo("囲い・囲む", "象形") },
            { "凵", new RadicalInfo("口が開いた器", "象形") },
            { "雨", new RadicalInfo("雨・天候", "象形") },
            { "心", new RadicalInfo("心・気持ち", "象形") },
            { "火", new RadicalInfo("火", "象形") },
            { "手", new RadicalInfo("手", "象形") },
            { "行", new RadicalInfo("道・行き来", "象形") },
            { "曰", new RadicalInfo("言う", "象形") },
            { "阝", new RadicalInfo("村・集落", "象形") },
            { "牛", new RadicalInfo("牛", "象形") },
            { "色", new RadicalInfo("色・顔色", "会意") },
            { "戈", new RadicalInfo("ほこ・武器", "象形") },
            { "長", new RadicalInfo("長い・年長", "象形") },
            { "糸", new RadicalInfo("糸・繊維", "象形") },
            { "言", new RadicalInfo("言葉・話す", "象形") },
            { "艸", new RadicalInfo("草・植物", "象形") },
            { "竹", new RadicalInfo("竹", "象形") },
            { "女", new RadicalInfo("女性", "象形") },
            { "田", new RadicalInfo("田・畑", "象形") },
            { "石", new RadicalInfo("石", "象形") },
            { "門", new RadicalInfo("門", "象形") },
            { "耳", new RadicalInfo("耳", "象形") },
            { "示", new RadicalInfo("祭壇・神への供え物", "象形") },
            { "衣", new RadicalInfo("衣服", "象形") },
            { "匸", new RadicalInfo("隠す・覆う", "象形") },
            { "匚", new RadicalInfo("箱・入れ物", "象形") },
            { "冂", new RadicalInfo("外枠・境界", "象形") },
            { "犬", new RadicalInfo("犬", "象形") },
            { "灬", new RadicalInfo("火・燃える", "象形") }
        };

    public static void PrepareRadicalGame()
    {
        if (GameModeData.SelectedMode != GameMode.Radical)
        {
            return;
        }

        EnsureInitialized();

        var radicals = new List<string>();
        foreach (KanjiData data in radicalKanjiList)
        {
            if (!string.IsNullOrEmpty(data.radical) &&
                !radicals.Contains(data.radical))
            {
                radicals.Add(data.radical);
            }
        }

        for (int i = radicals.Count - 1; i > 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);
            string current = radicals[i];
            radicals[i] = radicals[swapIndex];
            radicals[swapIndex] = current;
        }

        int selectedCount = Math.Min(RadicalsPerGame, radicals.Count);
        selectedRadicals = radicals.GetRange(0, selectedCount).ToArray();

        var selectedSet = new HashSet<string>(selectedRadicals);
        if (previousRadicals.Length == selectedRadicals.Length &&
            selectedRadicals.Length > 0 &&
            new HashSet<string>(previousRadicals).SetEquals(selectedSet) &&
            radicals.Count > selectedRadicals.Length)
        {
            string replacement = null;
            foreach (string radical in radicals)
            {
                if (!selectedSet.Contains(radical))
                {
                    replacement = radical;
                    break;
                }
            }

            selectedSet.Remove(selectedRadicals[selectedRadicals.Length - 1]);
            selectedRadicals[selectedRadicals.Length - 1] = replacement;
            selectedSet.Add(replacement);
        }

        previousRadicals = (string[])selectedRadicals.Clone();

        var selectedKanji = new List<KanjiData>();
        byKanji = new Dictionary<string, KanjiData>();
        foreach (KanjiData data in radicalKanjiList)
        {
            if (selectedSet.Contains(data.radical))
            {
                selectedKanji.Add(data);
                byKanji[data.kanji] = data;
            }
        }

        kanjiList = selectedKanji.ToArray();
        Debug.Log(
            $"部首モード：{selectedRadicals.Length}種類の部首、使用できる漢字は{kanjiList.Length}個です。");
    }

    public static string GetRadicalGameSummary()
    {
        var labels = new List<string>(selectedRadicals.Length);
        foreach (string radical in selectedRadicals)
        {
            labels.Add(GetRadicalLabel(radical));
        }

        return string.Join("　・　", labels);
    }

    public static int GetSelectedRadicalCount()
    {
        return selectedRadicals.Length;
    }

    public static string GetRadicalDisplayKey(KanjiData data)
    {
        if (data == null || string.IsNullOrEmpty(data.radical))
        {
            return string.Empty;
        }

        return data.radical + ":" + GetRadicalSymbol(data);
    }

    public static string GetRadicalName(KanjiData data)
    {
        if (data == null || string.IsNullOrEmpty(data.radical))
        {
            return string.Empty;
        }

        if (data.radical == "心" && GetRadicalSymbol(data) == "忄")
        {
            return "りっしんべん";
        }

        if (GetRadicalSymbol(data) == "𠆢")
        {
            return "ひとやね";
        }

        if (data.radical == "水" && data.kanji == "水")
        {
            return "みず";
        }

        if (data.radical == "手" && GetRadicalSymbol(data) == "手")
        {
            return "て";
        }

        if (data.radical == "人" && data.kanji == "人")
        {
            return "ひと";
        }

        if (GetRadicalSymbol(data) == "亻")
        {
            return "にんべん";
        }

        return GetRadicalLabel(data.radical);
    }

    public static string GetRadicalSymbol(KanjiData data)
    {
        if (data == null)
        {
            return string.Empty;
        }

        switch (data.radical)
        {
            case "水": return data.kanji == "水" ? "水" : "氵";
            case "人":
                if (data.kanji == "人") return "人";
                if (data.kanji == "会" || data.kanji == "全" || data.kanji == "内") return "𠆢";
                return "亻";
            case "手":
                return data.kanji == "手" || data.kanji == "拳" || data.kanji == "挙"
                    ? "手"
                    : "扌";
            case "心":
                return data.kanji == "性" || data.kanji == "情" ||
                    data.kanji == "怖" || data.kanji == "憧" || data.kanji == "快"
                    ? "忄"
                    : "心";
            case "糸": return "糹";
            case "言": return "訁";
            case "艸": return "艹";
            case "竹": return "⺮";
            case "示": return "礻";
            case "衣": return "衤";
            case "犬": return "犭";
            case "牛": return "牜";
            default: return data.radical;
        }
    }

    public static string GetRadicalExplanation(
        KanjiData data,
        ICollection<string> excludedKanji)
    {
        if (data == null || string.IsNullOrEmpty(data.radical))
        {
            return string.Empty;
        }

        string name = GetRadicalName(data);
        string symbol = GetRadicalSymbol(data);
        string explanation;
        if (!RadicalInfoByRoot.TryGetValue(data.radical, out RadicalInfo info))
        {
            explanation = $"{name}（{symbol}）\n意味：未登録\n成り立ち：未登録";
        }
        else
        {
            explanation = $"{name}（{symbol}）\n意味：{info.Meaning}\n成り立ち：{info.Formation}";
        }

        var examples = new List<string>(3);
        string displayKey = GetRadicalDisplayKey(data);
        foreach (KanjiData candidate in radicalKanjiList)
        {
            if (GetRadicalDisplayKey(candidate) != displayKey ||
                candidate.kanji == data.kanji ||
                (excludedKanji != null && excludedKanji.Contains(candidate.kanji)))
            {
                continue;
            }

            examples.Add(candidate.kanji);
            if (examples.Count == 3)
            {
                break;
            }
        }

        if (examples.Count > 0)
        {
            explanation += "\nほかの漢字：" + string.Join("、", examples);
        }

        return explanation;
    }

    public static string GetRadicalKanji(KanjiData data)
    {
        if (data == null)
        {
            return string.Empty;
        }

        string displayKey = GetRadicalDisplayKey(data);
        var kanji = new List<string>();
        foreach (KanjiRegistry.Entry entry in KanjiRegistry.Entries)
        {
            var candidate = new KanjiData
            {
                kanji = entry.Kanji,
                radical = entry.Radical
            };

            if (GetRadicalDisplayKey(candidate) == displayKey)
            {
                kanji.Add(entry.Kanji);
            }
        }

        return string.Join("、", kanji);
    }

    private static string GetRadicalLabel(string radical)
    {
        if (RadicalLabels.TryGetValue(radical, out string label))
        {
            return label;
        }

        return radical + "（部首）";
    }

    public static KanjiData GetRandomKanji()
    {
        EnsureInitialized();

        if (kanjiList.Length == 0)
        {
            Debug.LogError(
                "使用できる漢字がありません。KanjiRegistryと画像フォルダを確認してください。");
            return null;
        }

        int index = UnityEngine.Random.Range(0, kanjiList.Length);
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
        radicalKanjiList = mode == GameMode.Radical
            ? kanjiList
            : null;
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
