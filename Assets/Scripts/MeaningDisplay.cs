using UnityEngine;
using TMPro;

public class MeaningDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text meaningText;

    public void SetMeaning(string meaning)
    {
        meaningText.text = meaning;
    }
}
