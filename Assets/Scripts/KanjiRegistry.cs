/// <summary>
/// 漢字マスターデータ。ここに1行追加するだけでゲームに漢字を登録できます。
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
            string meaning = ""
        )
        {
            Kanji = kanji;
            Radical = radical;
            Readings = readings;
            IdiomPartners = idiomPartners ?? System.Array.Empty<string>();
            Meaning = meaning;
        }
    }
    public static readonly Entry[] Entries =
    {
        // ============================
        // ★ 既存の3つ（意味を追加）
        // ============================
        new Entry("生", "生", new[] { "せい", "しょう", "い", "なま" }, new[] { "物" }, "いきているもの"), // 生物
        new Entry("物", "牛", new[] { "ぶつ", "もの" }, null, "もの"),
        new Entry("動", "力", new[] { "どう" }, new[] { "物" }, "うごくもの"), // 動物

        // ============================
        // 人・体・動き（意味を追加）
        // ============================
        new Entry("人", "人", new[] { "じん", "にん", "ひと" }, new[] { "口", "名" }, "ひと"),
        new Entry("口", "口", new[] { "こう", "く", "くち" }, null, "くち"),
        new Entry("手", "手", new[] { "しゅ", "て" }, new[] { "紙" }, "て"),
        new Entry("足", "足", new[] { "そく", "あし" }, null, "あし"),
        new Entry("目", "目", new[] { "もく", "め" }, new[] { "安" }, "め"),
        new Entry("心", "心", new[] { "しん", "こころ" }, null, "こころ"),
        new Entry("気", "气", new[] { "き" }, new[] { "力" }, "きもち"),
        new Entry("力", "力", new[] { "りょく", "りき", "ちから" }, null, "ちから"),
        new Entry("名", "夕", new[] { "めい", "みょう", "な" }, new[] { "人", "目" }, "なまえ"),

        // ============================
        // 自然（意味を追加）
        // ============================
        new Entry("山", "山", new[] { "さん", "やま" }, new[] { "川", "道", "水" }, "やま"),
        new Entry("川", "川", new[] { "せん", "かわ" }, new[] { "上", "下" }, "かわ"),
        new Entry("海", "水", new[] { "かい", "うみ" }, new[] { "水" }, "うみ"),
        new Entry("空", "穴", new[] { "くう", "そら" }, new[] { "気" }, "そら"),
        new Entry("雨", "雨", new[] { "う", "あめ" }, null, "あめ"),
        new Entry("木", "木", new[] { "ぼく", "もく", "き" }, null, "き"),
        new Entry("花", "艹", new[] { "か", "はな" }, new[] { "火" }, "はな"),
        new Entry("草", "艹", new[] { "そう", "くさ" }, new[] { "木" }, "くさ"),
        new Entry("火", "火", new[] { "か", "ひ" }, new[] { "山" }, "ひ"),
        new Entry("水", "水", new[] { "すい", "みず" }, new[] { "道", "海", "川" }, "みず"),

        // ============================
        // 学校・学習（意味を追加）
        // ============================
        new Entry("文", "文", new[] { "ぶん", "もん" }, new[] { "字" }, "ふみ"),
        new Entry("字", "子", new[] { "じ" }, new[] { "文" }, "もじ"),
        new Entry("学", "子", new[] { "がく", "まな" }, new[] { "校", "科" }, "まなぶ"),
        new Entry("校", "木", new[] { "こう" }, null, "学校"),
        new Entry("図", "囗", new[] { "ず", "と" }, new[] { "書" }, "ず"),
        new Entry("書", "曰", new[] { "しょ", "か" }, new[] { "図", "読" }, "かく"),
        new Entry("読", "言", new[] { "どく", "よ" }, new[] { "書" }, "よむ"),
        new Entry("言", "言", new[] { "げん", "ごん", "こと" }, null, "ことば"),
        new Entry("話", "言", new[] { "わ", "はなし" }, null, "はなし"),
        new Entry("理", "玉", new[] { "り" }, new[] { "科" }, "ことわり"),

        // ============================
        // 時間・季節（意味を追加）
        // ============================
        new Entry("日", "日", new[] { "にち", "じつ", "ひ" }, new[] { "時" }, "ひ"),
        new Entry("月", "月", new[] { "げつ", "がつ", "つき" }, new[] { "日" }, "つき"),
        new Entry("年", "干", new[] { "ねん", "とし" }, new[] { "月" }, "とし"),
        new Entry("時", "日", new[] { "じ", "とき" }, null, "とき"),
        new Entry("春", "日", new[] { "しゅん", "はる" }, new[] { "風" }, "はる"),
        new Entry("夏", "夂", new[] { "か", "なつ" }, new[] { "休" }, "なつ"),
        new Entry("秋", "禾", new[] { "しゅう", "あき" }, new[] { "雨" }, "あき"),
        new Entry("冬", "夂", new[] { "とう", "ふゆ" }, new[] { "空" }, "ふゆ"),

        // ============================
        // 方向・位置（意味を追加）
        // ============================
        new Entry("上", "一", new[] { "じょう", "うえ" }, new[] { "下" }, "うえ"),
        new Entry("下", "一", new[] { "か", "げ", "した" }, null, "した"),
        new Entry("左", "工", new[] { "さ", "ひだり" }, new[] { "右" }, "ひだり"),
        new Entry("右", "口", new[] { "う", "ゆう", "みぎ" }, null, "みぎ"),
        new Entry("前", "刂", new[] { "ぜん", "まえ" }, new[] { "後" }, "まえ"),
        new Entry("後", "彳", new[] { "ご", "あと", "うしろ" }, null, "うしろ"),

        // ============================
        // 生活・物（意味を追加）
        // ============================
        new Entry("車", "車", new[] { "しゃ", "くるま" }, new[] { "道" }, "くるま"),
        new Entry("電", "雨", new[] { "でん" }, new[] { "車", "気", "力" }, "でんき"),
        new Entry("音", "音", new[] { "おん", "ね" }, new[] { "色" }, "おと"),
        new Entry("色", "色", new[] { "しょく", "いろ" }, null, "いろ"),
        new Entry("金", "金", new[] { "きん", "かね" }, new[] { "魚" }, "かね"),
        new Entry("魚", "魚", new[] { "ぎょ", "さかな" }, null, "さかな"),
        new Entry("鳥", "鳥", new[] { "ちょう", "とり" }, null, "とり"),
        new Entry("犬", "犬", new[] { "けん", "いぬ" }, null, "いぬ"),
    };
    public static string GetMeaning(string idiom)
    {
        foreach (var entry in Entries)
        {
            foreach (var partner in entry.IdiomPartners)
            {
                // 熟語を作る（例：生 + 物 = 生物）
                string formed = entry.Kanji + partner;

                if (formed == idiom)
                {
                    return entry.Meaning;   // ★ Entry に追加した Meaning を返す
                }
            }
        }

        return ""; // 見つからなかった場合
    }

}
