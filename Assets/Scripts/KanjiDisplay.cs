using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KanjiDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text idiomText;

    // 通常時の文字サイズ
    [SerializeField] private float normalFontSize = 140f;
    [SerializeField] private float doubleFontSize = 110f;
    [SerializeField] private float multipleFontSize = 70f;
    //==================================================
    // 熟語1つを表示
    //==================================================
    public void SetIdiom(string idiom)
    {
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
            return;
        }

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
}