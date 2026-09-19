using System;
using System.Collections.Generic;

/// <summary>
/// 漢字マスターデータ。
/// 採用する漢字は、現在の熟語リストに登場する漢字のみ。
/// スプライトは Assets/Resources/KanjiSprites/{漢字}.png を自動で読み込みます。
/// </summary>
public static class KanjiRegistry
{
    public readonly struct Entry
    {
        public readonly string Kanji;
        public readonly string Radical;
        public readonly string[] Readings;
        public readonly string[] IdiomPartners;
        public readonly string Meaning;

        public Entry(
            string kanji,
            string radical,
            string[] readings,
            string[] idiomPartners = null,
            string meaning = "")
        {
            Kanji = kanji;
            Radical = radical;
            Readings = readings;
            IdiomPartners = idiomPartners ?? Array.Empty<string>();
            Meaning = meaning;
        }
    }

    /// <summary>
    /// 現在ゲームで使用する漢字。
    /// 古い漢字は含めていません。
    /// </summary>
    public static readonly Entry[] Entries =
    {
        new Entry("学", "子", new[] { "がく", "まな" }, new[] { "年", "力", "会", "中", "大", "文" }, "まなぶ"),
        new Entry("年", "干", new[] { "ねん", "とし" }, new[] { "学", "長", "月", "金", "中" }, "とし"),
        new Entry("力", "力", new[] { "りょく", "りき", "ちから" }, new[] { "学", "水", "風", "人", "全", "戦", "電", "体" }, "ちから"),
        new Entry("会", "人", new[] { "かい", "あ" }, new[] { "学", "員", "国", "長", "大", "面", "意" }, "あう"),
        new Entry("中", "丨", new[] { "ちゅう", "なか" }, new[] { "学", "空", "水", "年", "心", "道", "部", "手" }, "なか"),
        new Entry("大", "大", new[] { "だい", "たい", "おお" }, new[] { "学", "空", "海", "木", "地", "人", "会", "戦", "工" }, "おおきい"),
        new Entry("文", "文", new[] { "ぶん", "もん", "ふみ" }, new[] { "学", "体", "面", "全", "長" }, "ぶん"),
        new Entry("体", "人", new[] { "たい", "てい", "からだ" }, new[] { "力", "一", "全", "物", "車" }, "からだ"),
        new Entry("面", "面", new[] { "めん", "おもて" }, new[] { "文", "水", "海", "会", "一", "全", "内", "外", "方" }, "おもて"),
        new Entry("全", "入", new[] { "ぜん", "まった" }, new[] { "員", "文", "体", "部", "力", "面", "長", "国" }, "すべて"),
        new Entry("空", "穴", new[] { "くう", "そら", "あ" }, new[] { "大", "中", "地", "色" }, "そら"),
        new Entry("地", "土", new[] { "ち", "じ" }, new[] { "面", "下", "上", "中", "理", "方", "名", "道", "大" }, "つち"),
        new Entry("水", "水", new[] { "すい", "みず" }, new[] { "道", "中", "面", "力", "車", "海", "色" }, "みず"),
        new Entry("道", "辶", new[] { "どう", "みち" }, new[] { "水", "中", "理", "車", "山", "地" }, "みち"),
        new Entry("車", "車", new[] { "しゃ", "くるま" }, new[] { "道", "体", "内", "外", "電", "下", "戦" }, "くるま"),
        new Entry("海", "水", new[] { "かい", "うみ" }, new[] { "水", "上", "中", "面", "外", "風", "大" }, "うみ"),
        new Entry("上", "一", new[] { "じょう", "うえ", "あ" }, new[] { "下", "手", "部", "品", "中", "地", "最" }, "うえ"),
        new Entry("下", "一", new[] { "か", "げ", "した", "さ" }, new[] { "山", "手", "上", "品", "部", "車", "最" }, "した"),
        new Entry("山", "山", new[] { "さん", "やま" }, new[] { "道", "中", "地", "水", "下" }, "やま"),
        new Entry("木", "木", new[] { "ぼく", "もく", "き" }, new[] { "工", "大" }, "き"),
        new Entry("理", "玉", new[] { "り" }, new[] { "地", "方", "物" }, "ことわり"),
        new Entry("方", "方", new[] { "ほう", "かた" }, new[] { "面", "地", "名", "行", "一" }, "ほう"),
        new Entry("名", "夕", new[] { "めい", "みょう", "な" }, new[] { "人", "品", "地" }, "なまえ"),
        new Entry("日", "日", new[] { "にち", "じつ", "ひ", "か" }, new[] { "月", "食", "数", "用" }, "ひ"),
        new Entry("月", "月", new[] { "げつ", "がつ", "つき" }, new[] { "日", "面", "年" }, "つき"),
        new Entry("食", "食", new[] { "しょく", "た", "く" }, new[] { "物", "用", "定", "外" }, "たべる"),
        new Entry("風", "風", new[] { "ふう", "かぜ" }, new[] { "力", "車", "海", "無" }, "かぜ"),
        new Entry("金", "金", new[] { "きん", "かね" }, new[] { "品", "色", "出" }, "かね"),
        new Entry("数", "攵", new[] { "すう", "かず" }, new[] { "定", "日" }, "かず"),
        new Entry("用", "用", new[] { "よう", "もち" }, new[] { "日", "食", "心", "無" }, "つかう"),
        new Entry("人", "人", new[] { "じん", "にん", "ひと" }, new[] { "工", "体", "物", "力", "名", "員", "大", "一", "無" }, "ひと"),
        new Entry("工", "工", new[] { "こう", "く" }, new[] { "員", "学", "大", "木" }, "こう"),
        new Entry("員", "口", new[] { "いん" }, new[] { "全", "会", "定", "人" }, "かず"),
        new Entry("定", "宀", new[] { "てい", "じょう", "さだ" }, new[] { "員", "食", "数", "一" }, "さだめる"),
        new Entry("国", "囗", new[] { "こく", "くに" }, new[] { "会", "外", "内", "全", "戦", "出" }, "くに"),
        new Entry("品", "口", new[] { "ひん", "しな" }, new[] { "金", "食", "物", "名", "上", "下", "手", "出", "部" }, "しなもの"),
        new Entry("出", "凵", new[] { "しゅつ", "で", "だ" }, new[] { "金", "国", "品" }, "でる"),
        new Entry("外", "夕", new[] { "がい", "げ", "そと" }, new[] { "食", "国", "部", "内", "面", "車" }, "そと"),
        new Entry("内", "入", new[] { "ない", "うち" }, new[] { "外", "国", "車", "面", "部" }, "うち"),
        new Entry("電", "雨", new[] { "でん" }, new[] { "車", "力" }, "でんき"),
        new Entry("心", "心", new[] { "しん", "こころ" }, new[] { "中", "用", "理", "無" }, "こころ"),
        new Entry("意", "心", new[] { "い" }, new[] { "外", "地", "会" }, "こころ"),
        new Entry("無", "火", new[] { "む", "ぶ", "な" }, new[] { "理", "用", "人", "名", "力", "数", "地", "風", "心", "色" }, "ない"),
        new Entry("手", "手", new[] { "しゅ", "て" }, new[] { "中", "上", "下", "品" }, "て"),
        new Entry("行", "行", new[] { "こう", "ぎょう", "い", "ゆ" }, new[] { "方", "一" }, "いく"),
        new Entry("最", "曰", new[] { "さい", "もっと" }, new[] { "上", "下", "大", "長" }, "もっとも"),
        new Entry("一", "一", new[] { "いち", "いつ", "ひと" }, new[] { "体", "定", "部", "面", "方", "人", "行" }, "ひとつ"),
        new Entry("部", "阝", new[] { "ぶ" }, new[] { "長", "下", "品", "外", "内", "上", "中", "一" }, "ぶぶん"),
        new Entry("物", "牛", new[] { "ぶつ", "もの" }, new[] { "体", "理", "品", "食" }, "もの"),
        new Entry("色", "色", new[] { "しょく", "いろ" }, new[] { "金", "水", "空", "無" }, "いろ"),
        new Entry("不", "一", new[] { "ふ", "ぶ" }, new[] { "意", "用" }, "～でない"),
        new Entry("戦", "戈", new[] { "せん", "たたか" }, new[] { "国", "車", "力", "大" }, "たたかう"),
        new Entry("長", "長", new[] { "ちょう", "なが" }, new[] { "年", "文", "最", "全", "部" }, "ながい"),
    };

    public static string GetMeaning(string idiom)
    {
        foreach (var entry in Entries)
        {
            foreach (var partner in entry.IdiomPartners)
            {
                if (entry.Kanji + partner == idiom)
                    return entry.Meaning;
            }
        }

        return "";
    }
}