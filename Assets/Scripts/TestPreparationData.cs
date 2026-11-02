using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Flags]
public enum TestPreparationClearType
{
    None = 0,
    Idiom = 1,
    SameKanji = 2,
    SameStrokeCount = 4,
    Okurigana = 8,
    StandaloneKanji = 16
}

public sealed class TestPreparationWord
{
    public readonly int Range;
    public readonly string Surface;
    public readonly string Reading;
    public readonly string Meaning;
    public readonly string Okurigana;
    public readonly string KanjiSequence;

    public TestPreparationWord(
        int range,
        string surface,
        string reading,
        string meaning,
        string okurigana = "")
    {
        Range = range;
        Surface = surface;
        Reading = reading;
        Meaning = meaning;
        Okurigana = okurigana;
        string prefix = string.IsNullOrEmpty(okurigana)
            ? surface
            : surface.Substring(0, surface.Length - okurigana.Length);
        KanjiSequence = string.Concat(prefix.Where(IsKanjiOrIterationMark));
    }

    private static bool IsKanjiOrIterationMark(char character)
    {
        return (character >= '\u4e00' && character <= '\u9fff') ||
               character == '々';
    }
}

public static class TestPreparationData
{
    private sealed class KanjiFact
    {
        public readonly int Strokes;
        public readonly string Radical;
        public readonly string Reading;
        public readonly string Meaning;

        public KanjiFact(int strokes, string radical, string reading, string meaning)
        {
            Strokes = strokes;
            Radical = radical;
            Reading = reading;
            Meaning = meaning;
        }
    }

    private static readonly TestPreparationWord[] Words =
    {
        new TestPreparationWord(22, "旬", "しゅん", "十日間。また、魚や野菜などが最もよく出回る時期。"),
        new TestPreparationWord(22, "醸造", "じょうぞう", "発酵させて酒やしょうゆなどをつくること。"),
        new TestPreparationWord(22, "蜂蜜", "はちみつ", "ミツバチが花の蜜からつくる甘い食品。"),
        new TestPreparationWord(22, "麺類", "めんるい", "小麦粉などを細長く加工した食品の総称。"),
        new TestPreparationWord(22, "煎茶", "せんちゃ", "茶葉を湯で浸出して飲む日本茶。"),
        new TestPreparationWord(22, "石臼", "いしうす", "石でつくった、穀物などをひく臼。"),
        new TestPreparationWord(22, "熟語", "じゅくご", "二つ以上の漢字を組み合わせてできた語。"),
        new TestPreparationWord(22, "充填", "じゅうてん", "すきまに物を詰めて満たすこと。"),
        new TestPreparationWord(22, "佳作", "かさく", "優れた作品。また、賞の次点となった作品。"),
        new TestPreparationWord(22, "凹凸", "おうとつ", "表面にあるへこみと突き出た部分。"),

        new TestPreparationWord(23, "募金", "ぼきん", "寄付金などを広く集めること。"),
        new TestPreparationWord(23, "膜", "まく", "物の表面をおおう薄い皮。"),
        new TestPreparationWord(23, "過剰", "かじょう", "必要な程度を超えて多すぎること。"),
        new TestPreparationWord(23, "剛直", "ごうちょく", "意志が強く、曲がったことをしないこと。"),
        new TestPreparationWord(23, "洗剤", "せんざい", "汚れを落とすために使う薬剤。"),
        new TestPreparationWord(23, "映える", "はえる", "光を受けて美しく見える。"),
        new TestPreparationWord(23, "著す", "あらわす", "書物や作品を書いて世に出す。", "す"),
        new TestPreparationWord(23, "推す", "おす", "人や物を推薦する。", "す"),
        new TestPreparationWord(23, "生地", "きじ", "加工する前の布や材料。また、衣服の布地。"),
        new TestPreparationWord(23, "裁つ", "たつ", "布を切って衣服などの形にする。", "つ"),

        new TestPreparationWord(24, "巨大", "きょだい", "非常に大きいこと。"),
        new TestPreparationWord(24, "孤島", "ことう", "陸地から遠く離れて孤立した島。"),
        new TestPreparationWord(24, "膨張", "ぼうちょう", "ふくらんで大きくなること。"),
        new TestPreparationWord(24, "謎", "なぞ", "正体や意味が分からず、解き明かす必要のある事柄。"),
        new TestPreparationWord(24, "栽培", "さいばい", "植物を植えて育てること。"),
        new TestPreparationWord(24, "推定", "すいてい", "事情をもとに、おしはかって決めること。"),
        new TestPreparationWord(24, "凝灰岩", "ぎょうかいがん", "火山灰などが固まってできた岩石。"),
        new TestPreparationWord(24, "水溶液", "すいようえき", "物質が水に溶けてできた液体。"),
        new TestPreparationWord(24, "抵触", "ていしょく", "規則や法律などに反すること。"),
        new TestPreparationWord(24, "運搬", "うんぱん", "物をほかの場所へ運ぶこと。"),

        new TestPreparationWord(25, "堆積", "たいせき", "物が積み重なること。また、積み重ねること。"),
        new TestPreparationWord(25, "徐々", "じょじょ", "少しずつ進むさま。"),
        new TestPreparationWord(25, "薪", "まき", "燃料として使う、割った木。"),
        new TestPreparationWord(25, "放棄", "ほうき", "権利や役目などを捨てて行わないこと。"),
        new TestPreparationWord(25, "侵食", "しんしょく", "土地などが水や風で少しずつ削られること。"),
        new TestPreparationWord(25, "抗争", "こうそう", "互いに張り合い、争うこと。"),
        new TestPreparationWord(25, "頻発", "ひんぱつ", "同じことが何度も起こること。"),
        new TestPreparationWord(25, "崩壊", "ほうかい", "くずれて形を保てなくなること。"),
        new TestPreparationWord(25, "割合", "わりあい", "全体に対する部分の比率。また、比較的。"),
        new TestPreparationWord(25, "飢餓", "きが", "食べ物が不足して飢えること。"),

        new TestPreparationWord(26, "恒常化", "こうじょうか", "一時的な状態が、いつも続く状態になること。"),
        new TestPreparationWord(26, "資源", "しげん", "産業や生活に利用できる自然界の材料や財産。"),
        new TestPreparationWord(26, "漆黒", "しっこく", "漆のようにつやのある深い黒色。"),
        new TestPreparationWord(26, "地獄", "じごく", "仏教で、罪を犯した者が苦しみを受ける世界。"),
        new TestPreparationWord(26, "延びる", "のびる", "時間や期間が長くなる。", "びる"),
        new TestPreparationWord(26, "吟味", "ぎんみ", "よく調べて、念入りに確かめること。"),
        new TestPreparationWord(26, "把握", "はあく", "内容や状況をしっかり理解すること。"),
        new TestPreparationWord(26, "一致", "いっち", "二つ以上のものが同じになること。"),
        new TestPreparationWord(26, "昨晩", "さくばん", "きのうの夜。"),
        new TestPreparationWord(26, "忍ぶ", "しのぶ", "人に知られないようにする。また、こらえる。", "ぶ"),

        new TestPreparationWord(27, "扇", "おうぎ", "あおいで風を起こす道具。"),
        new TestPreparationWord(27, "陣", "じん", "軍隊がとどまる場所。また、構えや集団。"),
        new TestPreparationWord(27, "僅か", "わずか", "数量や程度がほんの少しであるさま。", "か"),
        new TestPreparationWord(27, "嵐", "あらし", "激しい風雨。また、激しく荒れること。"),
        new TestPreparationWord(27, "突如", "とつじょ", "何の前触れもなく、突然。"),
        new TestPreparationWord(27, "小舟", "こぶね", "小さな船。"),
        new TestPreparationWord(27, "手綱", "たづな", "馬を操るためにくつわにつける綱。"),
        new TestPreparationWord(27, "漂う", "ただよう", "水面や空中を定まらずに動く。", "う"),
        new TestPreparationWord(27, "面", "めん", "顔。また、物の表面。"),
        new TestPreparationWord(27, "浦", "うら", "海や湖が陸地に入り込んだ所。"),

        new TestPreparationWord(28, "堪える", "たえる", "苦しさや困難を我慢する。", "える"),
        new TestPreparationWord(28, "逸話", "いつわ", "世間にあまり知られていない興味深い話。"),
        new TestPreparationWord(28, "騎馬隊", "きばたい", "馬に乗って戦う兵士の部隊。"),
        new TestPreparationWord(28, "惜しむ", "おしむ", "失うのを残念に思う。また、出ししぶる。", "しむ"),
        new TestPreparationWord(28, "嘲笑", "ちょうしょう", "人をばかにして笑うこと。"),
        new TestPreparationWord(28, "鎌倉時代", "かまくらじだい", "鎌倉幕府が政治を行った時代。"),
        new TestPreparationWord(28, "鋭い", "するどい", "先端がとがっている。また、感覚や判断が優れている。", "い"),
        new TestPreparationWord(28, "勝る", "まさる", "ほかのものより優れている。", "る"),
        new TestPreparationWord(28, "暁", "あかつき", "夜明け。また、あることが実現したとき。"),
        new TestPreparationWord(28, "俗人", "ぞくじん", "世間一般の人。また、出家していない人。"),

        new TestPreparationWord(29, "悠然", "ゆうぜん", "落ち着いていて、ゆったりとしているさま。"),
        new TestPreparationWord(29, "平凡", "へいぼん", "特に優れた点や変わった点がないこと。"),
        new TestPreparationWord(29, "締める", "しめる", "ゆるみのないように結ぶ。", "める"),
        new TestPreparationWord(29, "敷く", "しく", "平らに広げて置く。", "く"),
        new TestPreparationWord(29, "又聞き", "またぎき", "人から聞いた話を、さらに別の人から聞くこと。", "き"),
        new TestPreparationWord(29, "茂", "しげる", "草木がたくさん生える。"),
        new TestPreparationWord(29, "沈む", "しずむ", "水面より下に入る。また、気分が暗くなる。", "む"),
        new TestPreparationWord(29, "楼閣", "ろうかく", "高く建てた立派な建物。"),
        new TestPreparationWord(29, "浪人", "ろうにん", "主家を離れた武士。また、入学先が決まっていない人。"),
        new TestPreparationWord(29, "旧暦", "きゅうれき", "太陰太陽暦に基づく、現在の暦より前の暦。"),

        new TestPreparationWord(30, "解剖学", "かいぼうがく", "生物の体の構造を研究する学問。"),
        new TestPreparationWord(30, "理屈", "りくつ", "物事の筋道。また、道理を並べた説明。"),
        new TestPreparationWord(30, "衝撃", "しょうげき", "強い力が急に加わること。また、心に受ける大きな影響。"),
        new TestPreparationWord(30, "食卓", "しょくたく", "食事をするときに使う机。"),
        new TestPreparationWord(30, "芝居", "しばい", "劇を演じること。また、その劇。"),
        new TestPreparationWord(30, "波紋", "はもん", "水面に広がる輪。また、周囲に及ぶ影響。"),
        new TestPreparationWord(30, "刑", "けい", "罪を犯した人に科す罰。"),
        new TestPreparationWord(30, "容貌", "ようぼう", "顔かたち。"),
        new TestPreparationWord(30, "狭い", "せまい", "幅や広さが小さい。", "い"),
        new TestPreparationWord(30, "緻密", "ちみつ", "細部まで注意が行き届いていること。")
    };

    private static readonly Dictionary<string, KanjiFact> Facts =
        new Dictionary<string, KanjiFact>
        {
            { "旬", new KanjiFact(6, "日", "しゅん", "十日間。季節の最もよい時期。") },
            { "醸", new KanjiFact(20, "酉", "じょう", "発酵させて酒などをつくる。") },
            { "蜜", new KanjiFact(14, "虫", "みつ", "蜂が集める甘い液。") },
            { "麺", new KanjiFact(16, "麦", "めん", "小麦粉などを練って細長くした食品。") },
            { "煎", new KanjiFact(13, "火", "せん", "火にかけて水分をとばす。") },
            { "臼", new KanjiFact(6, "臼", "うす", "穀物などをつく道具。") },
            { "充", new KanjiFact(6, "儿", "じゅう", "いっぱいに満たす。") },
            { "填", new KanjiFact(13, "土", "てん", "すきまを埋める。") },
            { "佳", new KanjiFact(8, "人", "か", "よい。美しい。") },
            { "凹", new KanjiFact(5, "凵", "おう", "へこんでいる。") },
            { "凸", new KanjiFact(5, "凵", "とつ", "突き出ている。") },
            { "募", new KanjiFact(12, "力", "ぼ", "広く呼びかけて集める。") },
            { "膜", new KanjiFact(14, "肉", "まく", "表面をおおう薄い皮。") },
            { "剰", new KanjiFact(11, "刀", "じょう", "必要以上に余る。") },
            { "剛", new KanjiFact(10, "刀", "ごう", "かたく、強い。") },
            { "剤", new KanjiFact(10, "刀", "ざい", "調合した薬品。") },
            { "巨", new KanjiFact(5, "工", "きょ", "非常に大きい。") },
            { "孤", new KanjiFact(9, "子", "こ", "ひとりぼっち。") },
            { "膨", new KanjiFact(16, "肉", "ぼう", "ふくらむ。") },
            { "謎", new KanjiFact(17, "言", "なぞ", "意味や正体が分からないこと。") },
            { "栽", new KanjiFact(10, "木", "さい", "植物を植えて育てる。") },
            { "培", new KanjiFact(11, "土", "ばい", "土をかけて育てる。") },
            { "凝", new KanjiFact(16, "冫", "ぎょう", "こり固まる。") },
            { "溶", new KanjiFact(13, "水", "よう", "液体にとける。") },
            { "抵", new KanjiFact(8, "手", "てい", "さからう。") },
            { "搬", new KanjiFact(13, "手", "はん", "運び移す。") },
            { "堆", new KanjiFact(11, "土", "たい", "積み重なる。") },
            { "徐", new KanjiFact(10, "彳", "じょ", "ゆっくり進む。") },
            { "薪", new KanjiFact(16, "艸", "しん", "燃料にする木。") },
            { "棄", new KanjiFact(13, "木", "き", "捨てる。") },
            { "侵", new KanjiFact(9, "人", "しん", "他の領域に入り込む。") },
            { "抗", new KanjiFact(7, "手", "こう", "さからう。") },
            { "頻", new KanjiFact(17, "頁", "ひん", "たびたび。") },
            { "崩", new KanjiFact(11, "山", "ほう", "くずれる。") },
            { "飢", new KanjiFact(10, "食", "き", "食べ物がなくて苦しむ。") },
            { "餓", new KanjiFact(15, "食", "が", "ひどく飢える。") },
            { "恒", new KanjiFact(9, "心", "こう", "いつも変わらない。") },
            { "漆", new KanjiFact(14, "水", "しつ", "うるし。黒くつやのある塗料。") },
            { "獄", new KanjiFact(14, "犬", "ごく", "罪人を閉じ込める牢屋。") },
            { "吟", new KanjiFact(7, "口", "ぎん", "詩歌を声に出して味わう。") },
            { "把", new KanjiFact(7, "手", "は", "手でにぎる。") },
            { "致", new KanjiFact(10, "至", "ち", "ある状態に至らせる。") },
            { "忍", new KanjiFact(7, "心", "にん", "がまんする。人に知られないようにする。") },
            { "扇", new KanjiFact(10, "戸", "せん", "あおいで風を起こす道具。") },
            { "陣", new KanjiFact(10, "阝", "じん", "軍隊の構えや集団。") },
            { "僅", new KanjiFact(13, "人", "きん", "ほんの少し。") },
            { "嵐", new KanjiFact(12, "山", "あらし", "激しい風雨。") },
            { "如", new KanjiFact(6, "女", "じょ", "…のようである。") },
            { "船", new KanjiFact(11, "舟", "せん", "水上を進む乗り物。") },
            { "綱", new KanjiFact(14, "糸", "こう", "太く丈夫な縄。") },
            { "漂", new KanjiFact(14, "水", "ひょう", "水面をただよう。") },
            { "浦", new KanjiFact(10, "水", "ほ", "海や湖が陸に入り込んだ所。") },
            { "堪", new KanjiFact(12, "土", "たん", "たえる。こらえる。") },
            { "逸", new KanjiFact(11, "辶", "いつ", "それる。逃れる。") },
            { "騎", new KanjiFact(18, "馬", "き", "馬に乗る。") },
            { "惜", new KanjiFact(11, "心", "せき", "おしむ。残念に思う。") },
            { "嘲", new KanjiFact(15, "口", "ちょう", "あざける。ばかにして笑う。") },
            { "鎌", new KanjiFact(18, "金", "かま", "草などを刈る道具。") },
            { "鋭", new KanjiFact(15, "金", "えい", "先がとがっている。") },
            { "暁", new KanjiFact(12, "日", "ぎょう", "夜明け。") },
            { "俗", new KanjiFact(9, "人", "ぞく", "世間一般の。") },
            { "悠", new KanjiFact(11, "心", "ゆう", "ゆったりと落ち着いている。") },
            { "凡", new KanjiFact(3, "几", "ぼん", "普通である。") },
            { "締", new KanjiFact(15, "糸", "てい", "ゆるまないように結ぶ。") },
            { "敷", new KanjiFact(15, "攵", "ふ", "広げて一面に置く。") },
            { "又", new KanjiFact(2, "又", "また", "さらに。もう一度。") },
            { "茂", new KanjiFact(8, "艸", "も", "草木が生いしげる。") },
            { "楼", new KanjiFact(13, "木", "ろう", "高い建物。") },
            { "浪", new KanjiFact(10, "水", "ろう", "大きな波。") },
            { "暦", new KanjiFact(14, "日", "れき", "年月や日を数える仕組み。") },
            { "剖", new KanjiFact(10, "刀", "ぼう", "切り分ける。") },
            { "屈", new KanjiFact(8, "尸", "くつ", "かがむ。折れ曲がる。") },
            { "衝", new KanjiFact(15, "行", "しょう", "突き当たる。") },
            { "卓", new KanjiFact(8, "十", "たく", "机。また、ひときわ優れている。") },
            { "芝", new KanjiFact(6, "艸", "しば", "しば草。") },
            { "紋", new KanjiFact(10, "糸", "もん", "衣服などにつける模様。") },
            { "刑", new KanjiFact(6, "刀", "けい", "罪に対する罰。") },
            { "貌", new KanjiFact(14, "豸", "ぼう", "顔かたち。") },
            { "狭", new KanjiFact(9, "犬", "きょう", "幅がせまい。") },
            { "緻", new KanjiFact(16, "糸", "ち", "細かく行き届いている。") },
            { "握", new KanjiFact(12, "手", "あく", "手でしっかりと持つ。") },
            { "一", new KanjiFact(1, "一", "いち", "ひとつ。") },
            { "運", new KanjiFact(12, "辶", "うん", "物をほかの場所へ移す。") },
            { "映", new KanjiFact(9, "日", "えい", "光を受けてうつる。") },
            { "液", new KanjiFact(11, "水", "えき", "水のように流れるもの。") },
            { "延", new KanjiFact(8, "廴", "えん", "時間や距離が長くなる。") },
            { "化", new KanjiFact(4, "匕", "か", "別のものに変わる。") },
            { "過", new KanjiFact(12, "辶", "か", "通り過ぎる。度をこえる。") },
            { "解", new KanjiFact(13, "角", "かい", "結び目をほどく。意味を明らかにする。") },
            { "壊", new KanjiFact(16, "土", "かい", "こわれる。こわす。") },
            { "灰", new KanjiFact(6, "火", "かい", "物が燃えたあとに残る粉。") },
            { "閣", new KanjiFact(14, "門", "かく", "高く立派な建物。") },
            { "学", new KanjiFact(8, "子", "がく", "知識や技術を身につける。") },
            { "割", new KanjiFact(12, "刀", "わり", "分ける。切り分ける。") },
            { "岩", new KanjiFact(8, "山", "がん", "大きく固い石。") },
            { "旧", new KanjiFact(5, "日", "きゅう", "以前の。古い。") },
            { "居", new KanjiFact(8, "尸", "きょ", "そこにいる。住む。") },
            { "金", new KanjiFact(8, "金", "きん", "金属。かね。") },
            { "撃", new KanjiFact(15, "手", "げき", "強く打つ。攻撃する。") },
            { "源", new KanjiFact(13, "水", "げん", "物事の始まり。みなもと。") },
            { "語", new KanjiFact(14, "言", "ご", "言葉。話す。") },
            { "合", new KanjiFact(6, "口", "ごう", "二つ以上のものが一つになる。") },
            { "黒", new KanjiFact(11, "黒", "こく", "くろい色。") },
            { "裁", new KanjiFact(12, "衣", "さい", "布を切り分ける。判断する。") },
            { "作", new KanjiFact(7, "人", "さく", "ものをつくる。") },
            { "昨", new KanjiFact(9, "日", "さく", "過ぎた日。きのう。") },
            { "資", new KanjiFact(13, "貝", "し", "役立つもととなるもの。") },
            { "時", new KanjiFact(10, "日", "じ", "時刻。時間。") },
            { "手", new KanjiFact(4, "手", "しゅ", "体の一部。道具を扱う部分。") },
            { "舟", new KanjiFact(6, "舟", "しゅう", "水上を進む乗り物。") },
            { "熟", new KanjiFact(15, "火", "じゅく", "十分に煮える。よく慣れる。") },
            { "勝", new KanjiFact(12, "力", "しょう", "相手を負かす。まさる。") },
            { "小", new KanjiFact(3, "小", "しょう", "大きさや量が少ない。") },
            { "笑", new KanjiFact(10, "竹", "しょう", "わらう。") },
            { "常", new KanjiFact(11, "巾", "じょう", "いつも変わらない。") },
            { "触", new KanjiFact(13, "角", "しょく", "手や物が当たる。") },
            { "食", new KanjiFact(9, "食", "しょく", "食べる。食べ物。") },
            { "人", new KanjiFact(2, "人", "じん", "ひと。") },
            { "推", new KanjiFact(11, "手", "すい", "おし進める。推薦する。") },
            { "水", new KanjiFact(4, "水", "すい", "みず。") },
            { "生", new KanjiFact(5, "生", "せい", "生きる。生まれる。") },
            { "石", new KanjiFact(5, "石", "せき", "いし。") },
            { "積", new KanjiFact(16, "禾", "せき", "上に重ねる。") },
            { "洗", new KanjiFact(9, "水", "せん", "水で汚れを落とす。") },
            { "然", new KanjiFact(12, "火", "ぜん", "そのとおりである。") },
            { "倉", new KanjiFact(10, "人", "そう", "穀物や物を蓄える建物。") },
            { "争", new KanjiFact(6, "爪", "そう", "互いに勝とうとする。") },
            { "造", new KanjiFact(10, "辶", "ぞう", "材料から物をつくる。") },
            { "隊", new KanjiFact(12, "阝", "たい", "まとまった人々の集団。") },
            { "代", new KanjiFact(5, "人", "だい", "かわり。世代。") },
            { "大", new KanjiFact(3, "大", "だい", "おおきい。") },
            { "地", new KanjiFact(6, "土", "ち", "土地。地面。") },
            { "茶", new KanjiFact(9, "艸", "ちゃ", "茶の木。また、その葉を煎じた飲み物。") },
            { "張", new KanjiFact(11, "弓", "ちょう", "広げる。ぴんと伸ばす。") },
            { "直", new KanjiFact(8, "目", "ちょく", "曲がらずまっすぐ。") },
            { "沈", new KanjiFact(7, "水", "ちん", "水底にしずむ。") },
            { "定", new KanjiFact(8, "宀", "てい", "決める。落ち着く。") },
            { "島", new KanjiFact(10, "山", "とう", "周囲を水に囲まれた陸地。") },
            { "突", new KanjiFact(8, "穴", "とつ", "勢いよくぶつかる。") },
            { "波", new KanjiFact(8, "水", "は", "水面に起こる起伏。") },
            { "馬", new KanjiFact(10, "馬", "ば", "うま。") },
            { "発", new KanjiFact(9, "癶", "はつ", "出発する。外へ出す。") },
            { "晩", new KanjiFact(10, "日", "ばん", "夕方。夜。") },
            { "聞", new KanjiFact(14, "耳", "ぶん", "耳で音や話を受け取る。") },
            { "平", new KanjiFact(5, "干", "へい", "たいら。かたよりがない。") },
            { "放", new KanjiFact(8, "攵", "ほう", "自由にする。はなす。") },
            { "味", new KanjiFact(8, "口", "み", "食べ物の風味。物事の趣。") },
            { "密", new KanjiFact(11, "宀", "みつ", "すきまがない。こまかい。") },
            { "面", new KanjiFact(9, "面", "めん", "顔。物の表面。") },
            { "容", new KanjiFact(10, "宀", "よう", "中に入れる。姿かたち。") },
            { "理", new KanjiFact(11, "玉", "り", "物事の筋道。") },
            { "類", new KanjiFact(18, "頁", "るい", "似たものの集まり。") },
            { "話", new KanjiFact(13, "言", "わ", "言葉を交わす。") },
            { "著", new KanjiFact(11, "艸", "ちょ", "書物や作品を書き表す。") },
            { "蜂", new KanjiFact(13, "虫", "ほう", "針を持つ昆虫。") },
            { "々", new KanjiFact(3, "ノ", "くりかえし", "直前の漢字を繰り返す記号。") }
        };

    public static IReadOnlyList<TestPreparationWord> GetSelectedWords()
    {
        return Words.Where(word =>
            GameModeData.SelectedTestRanges.Contains(word.Range)).ToArray();
    }

    public static KanjiData[] BuildSelectedKanji()
    {
        var characters = new HashSet<string>();
        foreach (int range in GameModeData.SelectedTestRanges)
        {
            foreach (string character in GetRangeKanji(range))
                characters.Add(character);
        }

        foreach (TestPreparationWord word in GetSelectedWords())
        {
            foreach (char character in word.KanjiSequence)
            {
                if (IsKanjiOrIterationMark(character))
                    characters.Add(character.ToString());
            }
        }

        var result = new List<KanjiData>(characters.Count);
        foreach (string character in characters)
        {
            KanjiData data = CreateKanjiData(character);
            if (data != null)
                result.Add(data);
        }
        return result.ToArray();
    }

    public static string GetOkuriganaGroupKey(TestPreparationWord word)
    {
        return word.Okurigana;
    }

    private static string[] GetRangeKanji(int range)
    {
        string characters;
        switch (range)
        {
            case 22: characters = "旬醸蜜麺煎臼充填佳凹凸"; break;
            case 23: characters = "募膜剰剛剤"; break;
            case 24: characters = "巨孤膨謎栽培凝溶抵搬"; break;
            case 25: characters = "堆徐薪棄侵抗頻崩飢餓"; break;
            case 26: characters = "恒漆獄吟把致忍"; break;
            case 27: characters = "扇陣僅嵐如船綱漂浦"; break;
            case 28: characters = "堪逸騎惜嘲鎌鋭暁俗"; break;
            case 29: characters = "悠凡締敷又茂楼浪暦"; break;
            case 30: characters = "剖屈衝卓芝紋刑貌狭緻"; break;
            default: return Array.Empty<string>();
        }

        var result = new List<string>();
        foreach (char character in characters)
            result.Add(character.ToString());
        return result.ToArray();
    }

    private static KanjiData CreateKanjiData(string character)
    {
        Sprite sprite = LoadTestSprite(character);
        if (sprite == null && character != "舟")
        {
            Debug.LogError("テスト対策モードの漢字画像が見つかりません: " + character);
            return null;
        }
        if (sprite == null)
            Debug.LogWarning("テスト対策モードの舟画像がないため、文字表示を使用します。");

        if (Facts.TryGetValue(character, out KanjiFact fact))
        {
            return new KanjiData
            {
                kanji = character,
                sprite = sprite,
                radical = fact.Radical,
                readings = new[] { fact.Reading },
                strokeCount = fact.Strokes,
                meaning = fact.Meaning
            };
        }

        foreach (KanjiRegistry.Entry entry in KanjiRegistry.Entries)
        {
            if (entry.Kanji == character)
            {
                return new KanjiData
                {
                    kanji = character,
                    sprite = sprite,
                    radical = entry.Radical,
                    readings = entry.Readings,
                    meaning = entry.Meaning,
                    strokeCount = GetKnownStrokeCount(character)
                };
            }
        }

        TestPreparationWord example = GetSelectedWords()
            .FirstOrDefault(word => word.KanjiSequence.Contains(character));
        Debug.LogError("テスト漢字の読み・意味・部首・画数データが未登録です: " + character);
        return new KanjiData
        {
            kanji = character,
            sprite = sprite,
            readings = new[] { example != null ? example.Reading : "" },
            meaning = example != null ? example.Meaning : "",
            strokeCount = GetKnownStrokeCount(character)
        };
    }

    public static Sprite LoadTestSprite(string character)
    {
        Sprite sprite = Resources.Load<Sprite>("KanjiSprites/Test/" + character);
        if (sprite != null)
            return sprite;

        Sprite[] slicedSprites = Resources.LoadAll<Sprite>("KanjiSprites/Test/" + character);
        return slicedSprites.Length > 0 ? slicedSprites[0] : null;
    }

    private static int GetKnownStrokeCount(string character)
    {
        switch (character)
        {
            case "一": return 1;
            case "人": return 2;
            case "小": return 3;
            case "大": return 3;
            case "山": return 3;
            case "又": return 2;
            case "水": return 4;
            case "手": return 4;
            case "火": return 4;
            case "日": return 4;
            case "生": return 5;
            case "石": return 5;
            case "目": return 5;
            case "田": return 5;
            case "同": return 6;
            case "地": return 6;
            case "作": return 7;
            case "言": return 7;
            case "学": return 8;
            case "定": return 8;
            case "国": return 8;
            case "直": return 8;
            case "食": return 9;
            case "面": return 9;
            case "時": return 10;
            case "馬": return 10;
            case "倉": return 10;
            case "常": return 11;
            case "黒": return 11;
            case "勝": return 12;
            case "割": return 12;
            case "楽": return 13;
            case "類": return 18;
            default: return 0;
        }
    }

    private static bool IsKanjiOrIterationMark(char character)
    {
        return (character >= '\u4e00' && character <= '\u9fff') ||
               character == '々';
    }
}
