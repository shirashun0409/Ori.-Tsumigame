using UnityEngine;

[CreateAssetMenu(
    fileName = "KanjiDatabase",
    menuName = "Kanji Game/Kanji Database")]
public class KanjiDatabase : ScriptableObject
{
    [SerializeField]
    private KanjiData[] kanjiList;

    public KanjiData GetRandomKanji()
    {
        if (kanjiList == null || kanjiList.Length == 0)
        {
            Debug.LogError("KanjiDatabaseに漢字が登録されていません。");
            return null;
        }

        int index = Random.Range(0, kanjiList.Length);

        return kanjiList[index];
    }
}