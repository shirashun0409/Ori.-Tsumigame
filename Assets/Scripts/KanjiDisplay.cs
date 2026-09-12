using UnityEngine;
using TMPro;

public class KanjiDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text idiomText; // 生物 とか表示する TextMeshPro

    // 熟語が成立したときに呼ばれる関数
    public void SetIdiom(string idiom)
    {
        idiomText.text = idiom;
    }
}
