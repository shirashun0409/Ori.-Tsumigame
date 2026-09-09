
public static class KanjiRegistryHard
{
    public readonly struct Entry
    {
        public readonly string Kanji;
        public readonly string Radical;
        public readonly string[] Readings;
        public readonly string[] IdiomPartners;

        public Entry(string kanji, string radical, string[] readings, string[] idiomPartners = null)
        {
            Kanji = kanji;
            Radical = radical;
            Readings = readings;
            IdiomPartners = idiomPartners ?? System.Array.Empty<string>();
        }
    }

    public static readonly Entry[] Entries =
    {
        // ===== 抽象・哲学 =====
        new Entry("虚", "虍", new[] { "きょ" }, new[] { "無", "心", "構", "実", "偽", "像", "脱", "勢", "弱" }),
        new Entry("無", "火", new[] { "む" }),
        new Entry("心", "心", new[] { "しん" }),
        new Entry("構", "木", new[] { "こう" }),
        new Entry("実", "宀", new[] { "じつ" }),
        new Entry("偽", "亻", new[] { "ぎ" }),
        new Entry("像", "豕", new[] { "ぞう" }),
        new Entry("脱", "月", new[] { "だつ" }),
        new Entry("勢", "力", new[] { "せい" }),
        new Entry("弱", "弓", new[] { "じゃく" }),

        new Entry("空", "穴", new[] { "くう" }, new[] { "疎", "虚", "想", "論", "洞", "白", "転", "前" }),
        new Entry("疎", "疋", new[] { "そ" }),
        new Entry("想", "心", new[] { "そう" }),
        new Entry("論", "言", new[] { "ろん" }),
        new Entry("洞", "氵", new[] { "どう" }),
        new Entry("白", "白", new[] { "はく" }),
        new Entry("転", "車", new[] { "てん" }),
        new Entry("前", "刂", new[] { "ぜん" }),

        new Entry("抽", "扌", new[] { "ちゅう" }, new[] { "象" }),
        new Entry("象", "豕", new[] { "しょう" }),

        new Entry("概", "木", new[] { "がい" }, new[] { "念", "況", "説", "観" }),
        new Entry("念", "心", new[] { "ねん" }),
        new Entry("況", "氵", new[] { "きょう" }),
        new Entry("説", "言", new[] { "せつ" }),
        new Entry("観", "見", new[] { "かん" }),

        new Entry("理", "玉", new[] { "り" }, new[] { "性", "知", "屈", "論" }),
        new Entry("性", "心", new[] { "せい" }),
        new Entry("知", "矢", new[] { "ち" }),
        new Entry("屈", "尸", new[] { "くつ" }),

        new Entry("思", "心", new[] { "し" }, new[] { "索", "案", "惟", "念" }),
        new Entry("索", "糸", new[] { "さく" }),
        new Entry("案", "木", new[] { "あん" }),
        new Entry("惟", "心", new[] { "い" }),

        new Entry("構", "木", new[] { "こう" }, new[] { "造" }),
        new Entry("造", "辶", new[] { "ぞう" }),

        new Entry("機", "木", new[] { "き" }, new[] { "能" }),
        new Entry("能", "月", new[] { "のう" }),

        new Entry("制", "刂", new[] { "せい" }, new[] { "度" }),
        new Entry("度", "广", new[] { "ど" }),

        new Entry("規", "見", new[] { "き" }, new[] { "範" }),
        new Entry("範", "竹", new[] { "はん" }),

        new Entry("倫", "亻", new[] { "りん" }),

        // ===== 文学・表現 =====
        new Entry("文", "文", new[] { "ぶん" }, new[] { "脈", "意", "語", "選", "献", "庫", "筆", "芸" }),
        new Entry("脈", "月", new[] { "みゃく" }),
        new Entry("意", "心", new[] { "い" }),
        new Entry("語", "言", new[] { "ご" }, new[] { "彙", "感", "調", "勢", "尾", "源", "法", "録", "句" }),
        new Entry("選", "辶", new[] { "せん" }),
        new Entry("献", "犬", new[] { "けん" }),
        new Entry("庫", "广", new[] { "こ" }),
        new Entry("筆", "竹", new[] { "ひつ" }),
        new Entry("芸", "艹", new[] { "げい" }),

        new Entry("彙", "彑", new[] { "い" }),
        new Entry("感", "心", new[] { "かん" }),
        new Entry("調", "言", new[] { "ちょう" }),
        new Entry("勢", "力", new[] { "せい" }),
        new Entry("尾", "尸", new[] { "び" }),
        new Entry("源", "氵", new[] { "げん" }),
        new Entry("法", "氵", new[] { "ほう" }),
        new Entry("録", "金", new[] { "ろく" }),
        new Entry("句", "口", new[] { "く" }),

        new Entry("表", "衣", new[] { "ひょう" }, new[] { "現" }),
        new Entry("現", "王", new[] { "げん" }),

        new Entry("描", "扌", new[] { "びょう" }, new[] { "写" }),
        new Entry("写", "冖", new[] { "しゃ" }),

        new Entry("比", "匕", new[] { "ひ" }, new[] { "喩" }),
        new Entry("喩", "口", new[] { "ゆ" }),

        new Entry("寓", "宀", new[] { "ぐう" }, new[] { "話" }),
        new Entry("話", "言", new[] { "わ" }),

        new Entry("叙", "言", new[] { "じょ" }, new[] { "述", "情" }),
        new Entry("述", "辶", new[] { "じゅつ" }),
        new Entry("情", "心", new[] { "じょう" }),

        new Entry("詩", "言", new[] { "し" }, new[] { "情", "想", "趣", "風" }),
        new Entry("想", "心", new[] { "そう" }),
        new Entry("趣", "走", new[] { "しゅ" }),
        new Entry("風", "風", new[] { "ふう" }),

        new Entry("韻", "音", new[] { "いん" }, new[] { "律", "文", "語", "調", "味", "風", "格", "式" }),
        new Entry("律", "彳", new[] { "りつ" }),
        new Entry("味", "口", new[] { "み" }),
        new Entry("格", "木", new[] { "かく" }),
        new Entry("式", "弋", new[] { "しき" }),

        // ===== 自然・地理（前半） =====
        new Entry("海", "氵", new[] { "かい" }, new[] { "岸", "底", "流", "風" }),
        new Entry("岸", "山", new[] { "がん" }),
        new Entry("底", "广", new[] { "てい" }),
        new Entry("流", "氵", new[] { "りゅう" }),
        new Entry("風", "風", new[] { "ふう" }),

        new Entry("深", "氵", new[] { "しん" }, new[] { "海", "層", "谷", "山" }),
        new Entry("層", "尸", new[] { "そう" }),
        new Entry("谷", "谷", new[] { "こく" }),
        new Entry("山", "山", new[] { "さん" }),

        new Entry("峻", "山", new[] { "しゅん" }, new[] { "険", "烈", "厳" }),
        new Entry("険", "阝", new[] { "けん" }),
        new Entry("烈", "火", new[] { "れつ" }),
        new Entry("厳", "厂", new[] { "げん" }),

                // ===== 心理・感情 =====
        new Entry("寡", "宀", new[] { "か" }, new[] { "黙" }),
        new Entry("黙", "黒", new[] { "もく" }),

        new Entry("沈", "氵", new[] { "ちん" }, new[] { "黙" }),

        new Entry("静", "青", new[] { "せい" }, new[] { "謐", "寂", "穏", "粛" }),
        new Entry("謐", "言", new[] { "ひつ" }),
        new Entry("寂", "宀", new[] { "じゃく" }),
        new Entry("穏", "禾", new[] { "おん" }),
        new Entry("粛", "米", new[] { "しゅく" }),

        new Entry("高", "亠", new[] { "こう" }, new[] { "潔" }),
        new Entry("潔", "氵", new[] { "けつ" }, new[] { "白", "癖" }),
        new Entry("白", "白", new[] { "はく" }),
        new Entry("癖", "疒", new[] { "へき" }),

        new Entry("清", "氵", new[] { "せい" }, new[] { "廉" }),
        new Entry("廉", "广", new[] { "れん" }),

        new Entry("純", "糸", new[] { "じゅん" }, new[] { "潔" }),

        new Entry("端", "立", new[] { "たん" }, new[] { "麗", "正", "整", "然", "厳" }),
        new Entry("麗", "鹿", new[] { "れい" }),
        new Entry("正", "止", new[] { "せい" }),
        new Entry("整", "攵", new[] { "せい" }),
        new Entry("然", "灬", new[] { "ぜん" }),
        new Entry("厳", "厂", new[] { "げん" }),

        new Entry("精", "米", new[] { "せい" }, new[] { "緻", "巧", "密", "義", "論" }),
        new Entry("緻", "糸", new[] { "ち" }),
        new Entry("巧", "工", new[] { "こう" }),
        new Entry("密", "宀", new[] { "みつ" }),
        new Entry("義", "羊", new[] { "ぎ" }),
        new Entry("論", "言", new[] { "ろん" }),

        new Entry("奇", "大", new[] { "き" }, new[] { "妙", "怪" }),
        new Entry("妙", "女", new[] { "みょう" }),
        new Entry("怪", "心", new[] { "かい" }, new[] { "奇", "異", "物", "鳥", "魚", "影", "声", "気" }),
        new Entry("異", "田", new[] { "い" }),
        new Entry("物", "牛", new[] { "ぶつ" }),
        new Entry("鳥", "鳥", new[] { "ちょう" }),
        new Entry("魚", "魚", new[] { "ぎょ" }),
        new Entry("影", "彡", new[] { "えい" }),
        new Entry("声", "士", new[] { "せい" }),
        new Entry("気", "气", new[] { "き" }),

        new Entry("幽", "幺", new[] { "ゆう" }, new[] { "玄", "寂", "遠", "閉", "暗", "冥" }),
        new Entry("玄", "亠", new[] { "げん" }),
        new Entry("遠", "辶", new[] { "えん" }),
        new Entry("閉", "門", new[] { "へい" }),
        new Entry("暗", "日", new[] { "あん" }),
        new Entry("冥", "冖", new[] { "めい" }),

        new Entry("憂", "心", new[] { "ゆう" }, new[] { "慮", "愁", "念", "思" }),
        new Entry("慮", "思", new[] { "りょ" }),
        new Entry("愁", "心", new[] { "しゅう" }),
        new Entry("念", "心", new[] { "ねん" }),
        new Entry("思", "心", new[] { "し" }),

        // ===== 文化・歴史 =====
        new Entry("金", "金", new[] { "きん" }, new[] { "剛" }),
        new Entry("剛", "刂", new[] { "ごう" }),

        new Entry("草", "艹", new[] { "そう" }, new[] { "庵", "堂", "房", "寮" }),
        new Entry("庵", "广", new[] { "あん" }),
        new Entry("堂", "土", new[] { "どう" }),
        new Entry("房", "户", new[] { "ぼう" }),
        new Entry("寮", "宀", new[] { "りょう" }),

        new Entry("鳥", "鳥", new[] { "ちょう" }, new[] { "瞰", "観" }),
        new Entry("瞰", "見", new[] { "かん" }),
        new Entry("観", "見", new[] { "かん" }),

        new Entry("荘", "艹", new[] { "そう" }, new[] { "厳", "重" }),
        new Entry("重", "里", new[] { "じゅう" }),

        new Entry("古", "口", new[] { "こ" }, new[] { "典", "語", "文", "跡", "城", "寺", "塔", "墳", "書", "画", "碑", "像", "記", "史", "伝", "風", "流", "式", "案", "図", "録" }),
        new Entry("典", "八", new[] { "てん" }),
        new Entry("語", "言", new[] { "ご" }),
        new Entry("文", "文", new[] { "ぶん" }),
        new Entry("跡", "足", new[] { "せき" }),
        new Entry("城", "土", new[] { "じょう" }),
        new Entry("寺", "寸", new[] { "じ" }),
        new Entry("塔", "土", new[] { "とう" }),
        new Entry("墳", "土", new[] { "ふん" }),
        new Entry("書", "曰", new[] { "しょ" }),
        new Entry("画", "田", new[] { "が" }),
        new Entry("碑", "石", new[] { "ひ" }),
        new Entry("像", "豕", new[] { "ぞう" }),
        new Entry("記", "言", new[] { "き" }),
        new Entry("史", "口", new[] { "し" }),
        new Entry("伝", "亻", new[] { "でん" }),
        new Entry("風", "風", new[] { "ふう" }),
        new Entry("流", "氵", new[] { "りゅう" }),
        new Entry("式", "弋", new[] { "しき" }),
        new Entry("案", "木", new[] { "あん" }),
        new Entry("図", "囗", new[] { "ず" }),
        new Entry("録", "金", new[] { "ろく" }),
    };
}





