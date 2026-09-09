public static class KanjiRegistryNormal
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
        // ===== 人・家族・社会 =====
        new Entry("兄", "儿", new[] { "けい", "あに" }, new[] { "弟" }), // 兄弟
        new Entry("弟", "弓", new[] { "てい", "おとうと" }),

        new Entry("姉", "女", new[] { "し", "あね" }, new[] { "妹" }), // 姉妹
        new Entry("妹", "女", new[] { "まい", "いもうと" }),

        new Entry("家", "宀", new[] { "か", "いえ" }, new[] { "族", "庭" }), // 家族、家庭
        new Entry("族", "方", new[] { "ぞく" }),
        new Entry("庭", "广", new[] { "てい", "にわ" }),

        new Entry("住", "亻", new[] { "じゅう", "す" }, new[] { "民", "人" }), // 住民、住人
        new Entry("民", "氏", new[] { "みん" }),

        new Entry("市", "巾", new[] { "し", "いち" }, new[] { "民" }), // 市民
        new Entry("国", "囗", new[] { "こく", "くに" }, new[] { "民" }), // 国民

        new Entry("文", "文", new[] { "ぶん" }, new[] { "化", "明" }), // 文化、文明
        new Entry("化", "亻", new[] { "か" }),
        new Entry("明", "日", new[] { "めい" }),

        new Entry("社", "示", new[] { "しゃ" }, new[] { "会" }), // 社会
        new Entry("会", "曰", new[] { "かい" }, new[] { "話", "議" }), // 会話、会議
        new Entry("話", "言", new[] { "わ" }),
        new Entry("議", "言", new[] { "ぎ" }),

        new Entry("世", "一", new[] { "せい", "よ" }, new[] { "界" }), // 世界
        new Entry("界", "田", new[] { "かい" }),

        new Entry("未", "木", new[] { "み" }, new[] { "来" }), // 未来
        new Entry("来", "木", new[] { "らい" }),

        new Entry("現", "王", new[] { "げん" }, new[] { "在" }), // 現在
        new Entry("在", "土", new[] { "ざい" }),

        new Entry("過", "辶", new[] { "か" }, new[] { "去" }), // 過去
        new Entry("去", "厶", new[] { "きょ" }),

        // ===== 自然・地理 =====
        new Entry("地", "土", new[] { "ち" }, new[] { "球", "図", "面", "帯", "形", "質", "中", "上", "下" }),
        new Entry("球", "王", new[] { "きゅう" }),

        new Entry("山", "山", new[] { "さん" }, new[] { "脈", "頂", "道" }),
        new Entry("脈", "月", new[] { "みゃく" }),
        new Entry("頂", "頁", new[] { "ちょう" }),

        new Entry("河", "氵", new[] { "か" }, new[] { "川" }),
        new Entry("川", "川", new[] { "せん" }),

        new Entry("湖", "氵", new[] { "こ" }, new[] { "水" }),
        new Entry("水", "水", new[] { "すい" }),

        new Entry("海", "氵", new[] { "かい" }, new[] { "岸", "底", "面", "流", "風", "水", "路", "辺", "原", "域", "洋" }),
        new Entry("岸", "山", new[] { "がん" }),
        new Entry("底", "广", new[] { "てい" }),
        new Entry("流", "氵", new[] { "りゅう" }),
        new Entry("風", "風", new[] { "ふう" }),
        new Entry("路", "足", new[] { "ろ" }),
        new Entry("辺", "辶", new[] { "へん" }),
        new Entry("原", "厂", new[] { "げん" }),
        new Entry("域", "土", new[] { "いき" }),
        new Entry("洋", "氵", new[] { "よう" }),

        new Entry("森", "木", new[] { "しん" }),
        new Entry("林", "木", new[] { "りん" }),

        new Entry("草", "艹", new[] { "そう" }, new[] { "原" }),

        new Entry("火", "火", new[] { "か" }, new[] { "山" }),

        new Entry("気", "气", new[] { "き" }, new[] { "温", "圧", "候" }),
        new Entry("温", "氵", new[] { "おん" }),
        new Entry("圧", "土", new[] { "あつ" }),
        new Entry("候", "亻", new[] { "こう" }),

        new Entry("天", "大", new[] { "てん" }, new[] { "気" }),

        new Entry("景", "日", new[] { "けい" }),

        // ===== 時間 =====
        new Entry("時", "日", new[] { "じ" }, new[] { "間", "刻", "速" }),
        new Entry("間", "門", new[] { "かん" }),
        new Entry("刻", "刂", new[] { "こく" }),
        new Entry("速", "辶", new[] { "そく" }),

        new Entry("秒", "禾", new[] { "びょう" }),

        new Entry("年", "干", new[] { "ねん" }, new[] { "代", "月", "間" }),
        new Entry("代", "亻", new[] { "だい" }),

        new Entry("日", "日", new[] { "にち" }, new[] { "常" }),
        new Entry("常", "巾", new[] { "じょう" }),

        new Entry("週", "辶", new[] { "しゅう" }, new[] { "間" }),
        new Entry("月", "月", new[] { "げつ" }, new[] { "間" }),

        new Entry("永", "水", new[] { "えい" }, new[] { "遠" }),
        new Entry("遠", "辶", new[] { "えん" }),

        new Entry("近", "辶", new[] { "きん" }, new[] { "日" }),

        new Entry("翌", "羽", new[] { "よく" }, new[] { "日", "年", "月" }),

        new Entry("当", "小", new[] { "とう" }, new[] { "日", "月", "年" }),

        new Entry("平", "干", new[] { "へい" }, new[] { "日" }),
        new Entry("休", "亻", new[] { "きゅう" }, new[] { "日" }),

        new Entry("祝", "示", new[] { "しゅく" }, new[] { "日" }),

        new Entry("連", "辶", new[] { "れん" }, new[] { "休", "日", "夜", "続" }),
        new Entry("夜", "夕", new[] { "や" }),
        new Entry("続", "糸", new[] { "ぞく" }),

        new Entry("期", "月", new[] { "き" }),

        // ===== 学校・学習 =====
        new Entry("学", "子", new[] { "がく" }, new[] { "習", "問", "者", "生" }),
        new Entry("習", "羽", new[] { "しゅう" }),
        new Entry("問", "口", new[] { "もん" }),
        new Entry("者", "老", new[] { "しゃ" }),
        new Entry("生", "生", new[] { "せい" }),

        new Entry("先", "儿", new[] { "せん" }, new[] { "生" }),

        new Entry("教", "攵", new[] { "きょう" }, new[] { "室", "科", "材" }),
        new Entry("室", "宀", new[] { "しつ" }),
        new Entry("科", "禾", new[] { "か" }),
        new Entry("材", "木", new[] { "ざい" }),

        new Entry("図", "囗", new[] { "ず" }, new[] { "書", "鑑" }),
        new Entry("書", "曰", new[] { "しょ" }),
        new Entry("鑑", "金", new[] { "かん" }),

        new Entry("文", "文", new[] { "ぶん" }, new[] { "章", "書", "法" }),
        new Entry("章", "立", new[] { "しょう" }),
        new Entry("法", "氵", new[] { "ほう" }),

        new Entry("語", "言", new[] { "ご" }, new[] { "句", "彙", "学" }),
        new Entry("句", "口", new[] { "く" }),
        new Entry("彙", "彑", new[] { "い" }),

        new Entry("国", "囗", new[] { "こく" }, new[] { "語" }),

        new Entry("算", "竹", new[] { "さん" }, new[] { "数" }),
        new Entry("数", "攵", new[] { "すう" }),

        new Entry("理", "玉", new[] { "り" }, new[] { "科" }),

        new Entry("歴", "止", new[] { "れき" }, new[] { "史" }),
        new Entry("史", "口", new[] { "し" }),

        new Entry("地", "土", new[] { "ち" }, new[] { "理" }),

        new Entry("体", "人", new[] { "たい" }, new[] { "育" }),
        new Entry("育", "月", new[] { "いく" }),

        new Entry("美", "羊", new[] { "び" }, new[] { "術" }),
        new Entry("術", "行", new[] { "じゅつ" }),

        new Entry("音", "音", new[] { "おん" }, new[] { "楽" }),
        new Entry("楽", "木", new[] { "がく" }),

        new Entry("試", "言", new[] { "し" }, new[] { "験" }),
        new Entry("験", "馬", new[] { "けん" }),

        new Entry("宿", "宀", new[] { "しゅく" }, new[] { "題" }),
        new Entry("題", "頁", new[] { "だい" }),

        new Entry("課", "木", new[] { "か" }, new[] { "題" }),

        new Entry("研", "石", new[] { "けん" }, new[] { "究" }),
        new Entry("究", "穴", new[] { "きゅう" }),

        new Entry("発", "癶", new[] { "はつ" }, new[] { "表" }),
        new Entry("表", "衣", new[] { "ひょう" }),

        new Entry("記", "言", new[] { "き" }, new[] { "録", "述" }),
        new Entry("録", "金", new[] { "ろく" }),
        new Entry("述", "辶", new[] { "じゅつ" }),

        new Entry("説", "言", new[] { "せつ" }, new[] { "明" }),
        new Entry("明", "日", new[] { "めい" }),

        new Entry("読", "言", new[] { "どく" }, new[] { "解" }),
        new Entry("解", "角", new[] { "かい" }),

        new Entry("作", "亻", new[] { "さく" }, new[] { "文" }),
   
                // ===== 行動・生活 =====
        new Entry("生", "生", new[] { "せい" }, new[] { "活" }),
        new Entry("活", "氵", new[] { "かつ" }),

        new Entry("行", "彳", new[] { "こう" }, new[] { "動", "為" }),
        new Entry("動", "力", new[] { "どう" }),
        new Entry("為", "爪", new[] { "い" }),

        new Entry("運", "辶", new[] { "うん" }, new[] { "動", "転" }),
        new Entry("転", "車", new[] { "てん" }),

        new Entry("作", "亻", new[] { "さく" }, new[] { "業" }),
        new Entry("業", "木", new[] { "ぎょう" }),

        new Entry("仕", "亻", new[] { "し" }, new[] { "事" }),
        new Entry("事", "亅", new[] { "じ" }),

        new Entry("労", "力", new[] { "ろう" }, new[] { "働" }),
        new Entry("働", "亻", new[] { "どう" }),

        new Entry("料", "斗", new[] { "りょう" }, new[] { "理" }),
        new Entry("理", "玉", new[] { "り" }),

        new Entry("掃", "扌", new[] { "そう" }, new[] { "除" }),
        new Entry("除", "阝", new[] { "じょ" }),

        new Entry("洗", "氵", new[] { "せん" }, new[] { "濯" }),
        new Entry("濯", "氵", new[] { "たく" }),

        new Entry("買", "貝", new[] { "ばい" }, new[] { "物" }),
        new Entry("物", "牛", new[] { "ぶつ" }),

        new Entry("旅", "方", new[] { "りょ" }, new[] { "行" }),

        new Entry("散", "攵", new[] { "さん" }, new[] { "歩" }),
        new Entry("歩", "止", new[] { "ほ" }),

        new Entry("停", "亻", new[] { "てい" }, new[] { "止" }),

        new Entry("開", "門", new[] { "かい" }),
        new Entry("終", "糸", new[] { "しゅう" }),

        new Entry("準", "氵", new[] { "じゅん" }, new[] { "備" }),
        new Entry("備", "亻", new[] { "び" }),

        new Entry("片", "片", new[] { "へん" }, new[] { "付" }),
        new Entry("付", "亻", new[] { "ふ" }),

        new Entry("整", "攵", new[] { "せい" }, new[] { "備" }),

        new Entry("修", "亻", new[] { "しゅう" }, new[] { "理" }),

        new Entry("交", "亠", new[] { "こう" }, new[] { "換" }),
        new Entry("換", "扌", new[] { "かん" }),

        new Entry("使", "亻", new[] { "し" }, new[] { "用" }),
        new Entry("用", "月", new[] { "よう" }),

        new Entry("利", "禾", new[] { "り" }, new[] { "用" }),

        new Entry("保", "亻", new[] { "ほ" }, new[] { "存" }),
        new Entry("存", "子", new[] { "そん" }),

        new Entry("管", "竹", new[] { "かん" }, new[] { "理" }),

        new Entry("計", "言", new[] { "けい" }, new[] { "画" }),
        new Entry("画", "田", new[] { "が" }),

        new Entry("実", "宀", new[] { "じつ" }, new[] { "行" }),

        new Entry("成", "戈", new[] { "せい" }, new[] { "功" }),
        new Entry("功", "力", new[] { "こう" }),

        new Entry("失", "夫", new[] { "しつ" }, new[] { "敗" }),
        new Entry("敗", "攵", new[] { "はい" }),

        new Entry("努", "女", new[] { "ど" }, new[] { "力" }),

        new Entry("挑", "扌", new[] { "ちょう" }, new[] { "戦" }),
        new Entry("戦", "戈", new[] { "せん" }),

        new Entry("習", "羽", new[] { "しゅう" }, new[] { "慣" }),
        new Entry("慣", "心", new[] { "かん" }),

        new Entry("健", "亻", new[] { "けん" }, new[] { "康" }),
        new Entry("康", "广", new[] { "こう" }),

        new Entry("安", "宀", new[] { "あん" }, new[] { "全" }),
        new Entry("全", "人", new[] { "ぜん" }),

        new Entry("注", "氵", new[] { "ちゅう" }, new[] { "意" }),
        new Entry("意", "心", new[] { "い" }),

        new Entry("警", "言", new[] { "けい" }, new[] { "戒" }),
        new Entry("戒", "戈", new[] { "かい" }),

        new Entry("連", "辶", new[] { "れん" }, new[] { "絡" }),
        new Entry("絡", "糸", new[] { "らく" }),

        new Entry("相", "木", new[] { "そう" }, new[] { "談" }),
        new Entry("談", "言", new[] { "だん" }),

        // ===== 感情・思考 =====
        new Entry("感", "心", new[] { "かん" }, new[] { "情", "覚", "動", "謝", "心" }),
        new Entry("情", "心", new[] { "じょう" }),
        new Entry("覚", "見", new[] { "かく" }),
        new Entry("動", "力", new[] { "どう" }),
        new Entry("謝", "言", new[] { "しゃ" }),

        new Entry("思", "心", new[] { "し" }, new[] { "考", "想", "案", "索" }),
        new Entry("考", "老", new[] { "こう" }),
        new Entry("想", "心", new[] { "そう" }),
        new Entry("案", "木", new[] { "あん" }),
        new Entry("索", "糸", new[] { "さく" }),

        new Entry("記", "言", new[] { "き" }, new[] { "憶" }),
        new Entry("憶", "心", new[] { "おく" }),

        new Entry("判", "半", new[] { "はん" }, new[] { "断" }),
        new Entry("断", "斤", new[] { "だん" }),

        new Entry("納", "糸", new[] { "のう" }, new[] { "得" }),
        new Entry("得", "彳", new[] { "とく" }),

        new Entry("疑", "疋", new[] { "ぎ" }, new[] { "問" }),

        new Entry("安", "宀", new[] { "あん" }, new[] { "心" }),
        new Entry("心", "心", new[] { "しん" }),

        new Entry("満", "氵", new[] { "まん" }, new[] { "足" }),
        new Entry("足", "足", new[] { "そく" }),

        new Entry("希", "布", new[] { "き" }, new[] { "望" }),
        new Entry("望", "月", new[] { "ぼう" }),

        new Entry("決", "氵", new[] { "けつ" }, new[] { "意", "断" }),

        new Entry("反", "又", new[] { "はん" }, new[] { "省" }),
        new Entry("省", "目", new[] { "せい" }),

        new Entry("後", "彳", new[] { "ご" }, new[] { "悔" }),
        new Entry("悔", "心", new[] { "かい" }),

        new Entry("興", "臼", new[] { "きょう" }, new[] { "味" }),
        new Entry("味", "口", new[] { "み" }),

        // ===== 抽象語 =====
        new Entry("必", "心", new[] { "ひつ" }, new[] { "要" }),
        new Entry("要", "女", new[] { "よう" }),

        new Entry("不", "一", new[] { "ふ" }, new[] { "要", "可", "満" }),
        new Entry("可", "口", new[] { "か" }),

        new Entry("以", "人", new[] { "い" }, new[] { "上", "下", "内", "外" }),
        new Entry("上", "一", new[] { "じょう" }),
        new Entry("下", "一", new[] { "か" }),
        new Entry("内", "冂", new[] { "ない" }),
        new Entry("外", "夕", new[] { "がい" }),

        new Entry("程", "禾", new[] { "てい" }, new[] { "度" }),
        new Entry("度", "广", new[] { "ど" }),

        new Entry("結", "糸", new[] { "けつ" }, new[] { "果" }),
        new Entry("果", "木", new[] { "か" }),

        new Entry("原", "厂", new[] { "げん" }, new[] { "因" }),
        new Entry("因", "囗", new[] { "いん" }),

        new Entry("理", "玉", new[] { "り" }, new[] { "由" }),
        new Entry("由", "田", new[] { "ゆう" }),

        new Entry("目", "目", new[] { "もく" }, new[] { "的" }),
        new Entry("的", "白", new[] { "てき" }),

        new Entry("方", "方", new[] { "ほう" }, new[] { "法" }),

        new Entry("手", "手", new[] { "しゅ" }, new[] { "段" }),
        new Entry("段", "殳", new[] { "だん" }),

        new Entry("条", "木", new[] { "じょう" }, new[] { "件" }),
        new Entry("件", "亻", new[] { "けん" }),

        new Entry("状", "犬", new[] { "じょう" }, new[] { "態" }),
        new Entry("態", "心", new[] { "たい" }),

        new Entry("現", "王", new[] { "げん" }, new[] { "象" }),
        new Entry("象", "豕", new[] { "しょう" }),

        new Entry("事", "亅", new[] { "じ" }, new[] { "実" }),
        new Entry("実", "宀", new[] { "じつ" }),

        new Entry("証", "言", new[] { "しょう" }, new[] { "拠" }),
        new Entry("拠", "扌", new[] { "きょ" }),

        new Entry("例", "亻", new[] { "れい" }, new[] { "外" }),

        new Entry("通", "辶", new[] { "つう" }, new[] { "常" }),

        new Entry("特", "牛", new[] { "とく" }, new[] { "別" }),
        new Entry("別", "刂", new[] { "べつ" }),

        new Entry("一", "一", new[] { "いち" }, new[] { "般" }),
        new Entry("般", "舟", new[] { "はん" }),

        new Entry("具", "八", new[] { "ぐ" }, new[] { "体" }),

        new Entry("抽", "扌", new[] { "ちゅう" }, new[] { "象" }),

        new Entry("複", "衣", new[] { "ふく" }, new[] { "雑" }),
        new Entry("雑", "隹", new[] { "ざつ" }),

        new Entry("簡", "竹", new[] { "かん" }, new[] { "単" }),
        new Entry("単", "田", new[] { "たん" }),

        new Entry("重", "里", new[] { "じゅう" }, new[] { "要" }),

        new Entry("基", "土", new[] { "き" }, new[] { "本" }),
        new Entry("本", "木", new[] { "ほん" }),

        new Entry("応", "心", new[] { "おう" }, new[] { "用" }),

        new Entry("発", "癶", new[] { "はつ" }, new[] { "展" }),
        new Entry("展", "尸", new[] { "てん" }),

        new Entry("構", "木", new[] { "こう" }, new[] { "造" }),
        new Entry("造", "辶", new[] { "ぞう" }),

        new Entry("機", "木", new[] { "き" }, new[] { "能" }),
        new Entry("能", "月", new[] { "のう" }),

        new Entry("制", "刂", new[] { "せい" }, new[] { "度" }),

        new Entry("規", "見", new[] { "き" }, new[] { "則" }),
        new Entry("則", "貝", new[] { "そく" }),

        new Entry("法", "氵", new[] { "ほう" }, new[] { "律" }),
        new Entry("律", "彳", new[] { "りつ" }),

        new Entry("権", "木", new[] { "けん" }, new[] { "利" }),
        new Entry("利", "禾", new[] { "り" }),

        new Entry("義", "羊", new[] { "ぎ" }, new[] { "務" }),
        new Entry("務", "力", new[] { "む" }),

        // ===== 物・道具・技術 =====
        new Entry("電", "雨", new[] { "でん" }, new[] { "話", "車", "力", "気" }),
        new Entry("話", "言", new[] { "わ" }),
        new Entry("車", "車", new[] { "しゃ" }),
        new Entry("力", "力", new[] { "りょく" }),
        new Entry("気", "气", new[] { "き" }),

        new Entry("家", "宀", new[] { "か" }, new[] { "電" }),

        new Entry("具", "八", new[] { "ぐ" }, new[] { "材" }),

        new Entry("機", "木", new[] { "き" }, new[] { "械" }),
        new Entry("械", "木", new[] { "かい" }),

        new Entry("装", "衣", new[] { "そう" }, new[] { "置", "備" }),
        new Entry("置", "罒", new[] { "ち" }),

        new Entry("部", "阝", new[] { "ぶ" }, new[] { "品" }),
        new Entry("品", "口", new[] { "ひん" }),

        new Entry("材", "木", new[] { "ざい" }, new[] { "料" }),

        new Entry("製", "衣", new[] { "せい" }, new[] { "品" }),

        new Entry("商", "冏", new[] { "しょう" }, new[] { "品" }),

        new Entry("食", "食", new[] { "しょく" }, new[] { "品" }),

        new Entry("衣", "衣", new[] { "い" }, new[] { "類", "服" }),
        new Entry("類", "頁", new[] { "るい" }),
        new Entry("服", "月", new[] { "ふく" }),

        new Entry("文", "文", new[] { "ぶん" }, new[] { "具" }),

        new Entry("工", "工", new[] { "こう" }, new[] { "具" }),

        new Entry("車", "車", new[] { "しゃ" }, new[] { "両" }),
        new Entry("両", "一", new[] { "りょう" }),

        new Entry("乗", "禾", new[] { "じょう" }, new[] { "車", "船", "客", "員" }),
        new Entry("船", "舟", new[] { "せん" }),
        new Entry("客", "宀", new[] { "きゃく" }),
        new Entry("員", "口", new[] { "いん" }),

        new Entry("建", "廴", new[] { "けん" }, new[] { "物", "築" }),
        new Entry("物", "牛", new[] { "ぶつ" }),
        new Entry("築", "木", new[] { "ちく" }),

        new Entry("道", "辶", new[] { "どう" }, new[] { "路", "具" }),
        new Entry("路", "足", new[] { "ろ" }),

        new Entry("通", "辶", new[] { "つう" }, new[] { "路", "行", "信" }),
        new Entry("行", "彳", new[] { "こう" }),
        new Entry("信", "亻", new[] { "しん" }),

        new Entry("放", "攵", new[] { "ほう" }, new[] { "送" }),
        new Entry("送", "辶", new[] { "そう" }),

        new Entry("映", "日", new[] { "えい" }, new[] { "像" }),
        new Entry("像", "豕", new[] { "ぞう" }),

        new Entry("音", "音", new[] { "おん" }, new[] { "声" }),
        new Entry("声", "士", new[] { "せい" }),

        new Entry("録", "金", new[] { "ろく" }, new[] { "音", "画" }),
        new Entry("画", "田", new[] { "が" }),

        new Entry("印", "卩", new[] { "いん" }, new[] { "刷" }),
        new Entry("刷", "刂", new[] { "さつ" }),

        new Entry("加", "力", new[] { "か" }, new[] { "工" }),
        new Entry("工", "工", new[] { "こう" }),
    };
}