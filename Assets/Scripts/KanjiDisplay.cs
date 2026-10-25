using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KanjiDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text idiomText;
    [SerializeField] private TMP_Text radicalNameText;

    // 通常時の文字サイズ
    [SerializeField] private float normalFontSize = 140f;
    [SerializeField] private float doubleFontSize = 110f;
    [SerializeField] private float multipleFontSize = 70f;
    //==================================================
    // 熟語1つを表示
    //==================================================
    public void SetIdiom(string idiom)
    {
        if (radicalNameText != null)
            radicalNameText.text = "";

        idiomText.text = idiom;

        // 1種類なので通常サイズ
        idiomText.fontSize = normalFontSize;
    }


    //==================================================
    // 複数の熟語を表示
    //==================================================
    public void SetIdioms(List<string> idioms)
    {
        if (idioms == null || idioms.Count == 0)
        {
            idiomText.text = "";
            if (radicalNameText != null)
                radicalNameText.text = "";
            return;
        }

        if (radicalNameText != null)
            radicalNameText.text = "";

        // 熟語を「 / 」で横並び表示
        idiomText.text = string.Join("  ", idioms);

        // 熟語の数によって文字サイズ変更
        if (idioms.Count == 1)
        {
            idiomText.fontSize = normalFontSize;
        }
        else if (idioms.Count == 2)
        {
            idiomText.fontSize = doubleFontSize;
        }
        else
        {
            idiomText.fontSize = multipleFontSize;
        }
    }

    public void SetRadicals(List<KanjiData> radicals)
    {
        var uniqueRadicals = new List<KanjiData>();
        var seenRadicals = new HashSet<string>();
        if (radicals != null)
        {
            foreach (KanjiData data in radicals)
            {
                string displayKey = KanjiDatabase.GetRadicalDisplayKey(data);
                if (!string.IsNullOrEmpty(displayKey) && seenRadicals.Add(displayKey))
                {
                    uniqueRadicals.Add(data);
                }
            }
        }

        if (uniqueRadicals.Count == 0)
        {
            idiomText.text = "";
            if (radicalNameText != null)
                radicalNameText.text = "";
            return;
        }

        var names = new List<string>(uniqueRadicals.Count);
        var symbols = new List<string>(uniqueRadicals.Count);
        foreach (KanjiData data in uniqueRadicals)
        {
            names.Add(KanjiDatabase.GetRadicalName(data));
            symbols.Add(KanjiDatabase.GetRadicalSymbol(data));
        }

        if (radicalNameText != null)
        {
            radicalNameText.text = string.Join("　", names);
            radicalNameText.fontSize = uniqueRadicals.Count == 1 ? 54f : 38f;
        }

        idiomText.text = string.Join("　", symbols);

        if (uniqueRadicals.Count == 1)
        {
            idiomText.fontSize = normalFontSize;
        }
        else if (uniqueRadicals.Count == 2)
        {
            idiomText.fontSize = doubleFontSize;
        }
        else
        {
            idiomText.fontSize = multipleFontSize;
        }
    }

}
