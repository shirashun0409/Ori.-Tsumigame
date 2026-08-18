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

        public Entry(
            string kanji,
            string radical,
            string[] readings,
            string[] idiomPartners = null)
        {
            Kanji = kanji;
            Radical = radical;
            Readings = readings;
            IdiomPartners = idiomPartners ?? System.Array.Empty<string>();
        }
    }

    public static readonly Entry[] Entries =
    {
        // --- 熟語モード（例: 生物, 動物） ---
        new Entry("生", "生", new[] { "せい", "しょう", "い", "なま" }, new[] { "物" }),
        new Entry("物", "牛", new[] { "ぶつ", "もの" }),
        new Entry("動", "力", new[] { "どう" }, new[] { "物" }),

        // --- 読みモード用（例: 雨・飴・天 → すべて「あめ/てん」系） ---
        // スプライトを Resources/KanjiSprites/ に置いたらコメントを外してください
        // new Entry("雨", "雨", new[] { "あめ", "う" }),
        // new Entry("飴", "食", new[] { "あめ" }),
        // new Entry("天", "大", new[] { "てん", "あめ" }),

        // --- 部首モード用（例: 松・林・村 → すべて「木」） ---
        // new Entry("松", "木", new[] { "しょう", "まつ" }),
        // new Entry("林", "木", new[] { "りん", "はやし" }),
        // new Entry("村", "木", new[] { "そん", "むら" }),
    };
}
