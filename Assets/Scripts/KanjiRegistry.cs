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
        // ============================
        // ★ 既存の3つ（残す）
        // ============================
        new Entry("生", "生", new[] { "せい", "しょう", "い", "なま" }, new[] { "物" }), // 生物
        new Entry("物", "牛", new[] { "ぶつ", "もの" }),
        new Entry("動", "力", new[] { "どう" }, new[] { "物" }), // 動物

        // ============================
        // 人・体・動き
        // ============================
        new Entry("人", "人", new[] { "じん", "にん", "ひと" }, new[] { "口", "名" }), // 人口、人名
        new Entry("口", "口", new[] { "こう", "く", "くち" }),
        new Entry("手", "手", new[] { "しゅ", "て" }, new[] { "紙" }), // 手紙
        new Entry("足", "足", new[] { "そく", "あし" }),
        new Entry("目", "目", new[] { "もく", "め" }, new[] { "安" }), // 目安
        new Entry("心", "心", new[] { "しん", "こころ" }),
        new Entry("気", "气", new[] { "き" }, new[] { "力" }), // 気力
        new Entry("力", "力", new[] { "りょく", "りき", "ちから" }),
        new Entry("名", "夕", new[] { "めい", "みょう", "な" }, new[] { "人", "目" }), // 名人、名目

        // ============================
        // 自然
        // ============================
        new Entry("山", "山", new[] { "さん", "やま" }, new[] { "川", "道", "水" }), // 山川、山道、山水
        new Entry("川", "川", new[] { "せん", "かわ" }, new[] { "上", "下" }), // 川上、川下
        new Entry("海", "水", new[] { "かい", "うみ" }, new[] { "水" }), // 海水
        new Entry("空", "穴", new[] { "くう", "そら" }, new[] { "気" }), // 空気
        new Entry("雨", "雨", new[] { "う", "あめ" }),
        new Entry("木", "木", new[] { "ぼく", "もく", "き" }),
        new Entry("花", "艹", new[] { "か", "はな" }, new[] { "火" }), // 花火
        new Entry("草", "艹", new[] { "そう", "くさ" }, new[] { "木" }), // 草木
        new Entry("火", "火", new[] { "か", "ひ" }, new[] { "山" }), // 火山
        new Entry("水", "水", new[] { "すい", "みず" }, new[] { "道", "海", "川" }), // 水道、海水、川水

        // ============================
        // 学校・学習
        // ============================
        new Entry("文", "文", new[] { "ぶん", "もん" }, new[] { "字" }), // 文字
        new Entry("字", "子", new[] { "じ" }, new[] { "文" }), // 
        new Entry("学", "子", new[] { "がく", "まな" }, new[] { "校", "科" }), // 学校、学科
        new Entry("校", "木", new[] { "こう" }),
        new Entry("図", "囗", new[] { "ず", "と" }, new[] { "書" }), // 図書
        new Entry("書", "曰", new[] { "しょ", "か" }, new[] { "図", "読" }), // 書図、書読
        new Entry("読", "言", new[] { "どく", "よ" }, new[] { "書" }), // 読書
        new Entry("言", "言", new[] { "げん", "ごん", "こと" }),
        new Entry("話", "言", new[] { "わ", "はなし" }),
        new Entry("理", "玉", new[] { "り" }, new[] { "科" }), // 理科

        // ============================
        // 時間・季節
        // ============================
        new Entry("日", "日", new[] { "にち", "じつ", "ひ" }, new[] { "時" }), // 日時
        new Entry("月", "月", new[] { "げつ", "がつ", "つき" }, new[] { "日" }), // 月日
        new Entry("年", "干", new[] { "ねん", "とし" }, new[] { "月" }), // 年月
        new Entry("時", "日", new[] { "じ", "とき" }),
        new Entry("春", "日", new[] { "しゅん", "はる" }, new[] { "風" }), // 春風
        new Entry("夏", "夂", new[] { "か", "なつ" }, new[] { "休" }), // 夏休
        new Entry("秋", "禾", new[] { "しゅう", "あき" }, new[] { "雨" }), // 秋雨
        new Entry("冬", "夂", new[] { "とう", "ふゆ" }, new[] { "空" }), // 冬空

        // ============================
        // 方向・位置（左→右だけ成立）
        // ============================
        new Entry("上", "一", new[] { "じょう", "うえ" }, new[] { "下" }), // 上下
        new Entry("下", "一", new[] { "か", "げ", "した" }),
        new Entry("左", "工", new[] { "さ", "ひだり" }, new[] { "右" }), // 左右
        new Entry("右", "口", new[] { "う", "ゆう", "みぎ" }),
        new Entry("前", "刂", new[] { "ぜん", "まえ" }, new[] { "後" }), // 前後
        new Entry("後", "彳", new[] { "ご", "あと", "うしろ" }),

        // ============================
        // 生活・物
        // ============================
        new Entry("車", "車", new[] { "しゃ", "くるま" }, new[] { "道" }), // 車道
        new Entry("電", "雨", new[] { "でん" }, new[] { "車", "気", "力" }), // 電車、電気、電力
        new Entry("音", "音", new[] { "おん", "ね" }, new[] { "色" }), // 音色
        new Entry("色", "色", new[] { "しょく", "いろ" }),
        new Entry("金", "金", new[] { "きん", "かね" }, new[] { "魚" }), // 金魚
        new Entry("魚", "魚", new[] { "ぎょ", "さかな" }),
        new Entry("鳥", "鳥", new[] { "ちょう", "とり" }),
        new Entry("犬", "犬", new[] { "けん", "いぬ" }),
    };
}
