using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameResultDisplay : MonoBehaviour
{
    [Header("熟語リスト")]
    [SerializeField] private Transform idiomListParent;

    [SerializeField] private GameObject idiomButtonPrefab;

    [Header("説明パネル")]
    [SerializeField] private GameObject infoPanel;

    [SerializeField] private TMP_Text infoTitle;

    [SerializeField] private TMP_Text infoText;

    private void Start()
    {
        DisplayCreatedIdioms();
    }

    private void DisplayCreatedIdioms()
    {
        if (idiomListParent == null)
        {
            Debug.LogError(
                "IdiomListが設定されていません！"
            );
            return;
        }

        if (idiomButtonPrefab == null)
        {
            Debug.LogError(
                "IdiomButton Prefabが設定されていません！"
            );
            return;
        }

        if (infoPanel == null)
        {
            Debug.LogError(
                "IdiomInfoPanelが設定されていません！"
            );
            return;
        }

        if (infoTitle == null)
        {
            Debug.LogError(
                "IdiomInfoTitleが設定されていません！"
            );
            return;
        }

        if (infoText == null)
        {
            Debug.LogError(
                "IdiomInfoTextが設定されていません！"
            );
            return;
        }

        if (GameResultManager.Instance == null)
        {
            Debug.LogError(
                "GameResultManagerが見つかりません！"
            );
            return;
        }

        List<string> idioms =
            GameResultManager.Instance.GetCreatedIdioms();

        if (idioms.Count == 0)
        {
            Debug.Log(
                "今回作った熟語はありません。"
            );
            return;
        }

        foreach (string idiom in idioms)
        {
            GameObject item =
                Instantiate(
                    idiomButtonPrefab,
                    idiomListParent
                );

            IdiomResultItem resultItem =
                item.GetComponent<IdiomResultItem>();

            if (resultItem == null)
            {
                Debug.LogError(
                    "IdiomButtonにIdiomResultItemがありません！"
                );

                Destroy(item);
                continue;
            }

            // 説明パネルを設定
            resultItem.SetInfoPanel(
                infoPanel,
                infoTitle,
                infoText
            );

            // 熟語を設定
            resultItem.SetIdiom(idiom);

            item.SetActive(true);
        }

        // 最初は説明パネルを非表示
        infoPanel.SetActive(false);

        Debug.Log(
            "熟語ボタンを "
            + idioms.Count
            + " 個生成しました！"
        );
    }
}