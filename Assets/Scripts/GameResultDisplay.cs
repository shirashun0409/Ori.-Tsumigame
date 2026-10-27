using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class GameResultDisplay : MonoBehaviour
{
    [Header("熟語リスト")]
    [SerializeField] private Transform idiomListParent;

    [SerializeField] private GameObject idiomButtonPrefab;

    [Header("ページ送り")]
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [Header("説明パネル")]
    [SerializeField] private GameObject infoPanel;

    [SerializeField] private TMP_Text infoTitle;

    [SerializeField] private TMP_Text infoText;


    // 1ページに表示する熟語数
    private const int IdiomsPerPage = 27;

    // 今回作った熟語
    private List<string> idioms = new List<string>();
    private List<KanjiData> radicals = new List<KanjiData>();
    private bool isRadicalMode;

    // 現在のページ
    private int currentPage = 0;


    private void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>(true);
        if (canvas != null)
        {
            MenuSceneEffects.AttachButtonEffects(canvas.transform.root);
        }

        // 熟語を取得
        if (GameResultManager.Instance == null)
        {
            Debug.LogError(
                "GameResultManagerが見つかりません！"
            );

            return;
        }

        isRadicalMode = GameModeData.SelectedMode == GameMode.Radical;
        if (isRadicalMode)
        {
            radicals = GameResultManager.Instance.GetClearedRadicals();
        }
        else
        {
            idioms = GameResultManager.Instance.GetCreatedIdioms();
        }


        // 説明パネルを最初は非表示
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }


        // ボタン設定
        if (prevButton != null)
        {
            prevButton.onClick.AddListener(PreviousPage);
        }
        else
        {
            Debug.LogError(
                "PrevButtonが設定されていません！"
            );
        }


        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextPage);
        }
        else
        {
            Debug.LogError(
                "NextButtonが設定されていません！"
            );
        }


        // 最初のページを表示
        DisplayCurrentPage();
    }


    /// <summary>
    /// 現在のページを表示
    /// </summary>
    private void DisplayCurrentPage()
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


        // --------------------------------------------------
        // 前のページの熟語ボタンを全部削除
        // --------------------------------------------------

        for (int i = idiomListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(
                idiomListParent.GetChild(i).gameObject
            );
        }


        // 熟語がない場合
        if (GetItemCount() == 0)
        {
            Debug.Log(isRadicalMode
                ? "今回消した部首はありません。"
                : "今回作った熟語はありません。");

            UpdatePageButtons();

            return;
        }


        // --------------------------------------------------
        // 今のページで表示する範囲
        // --------------------------------------------------

        int startIndex =
            currentPage * IdiomsPerPage;

        int endIndex = Mathf.Min(
            startIndex + IdiomsPerPage,
            GetItemCount());


        // --------------------------------------------------
        // 熟語ボタンを生成
        // --------------------------------------------------

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject item =
                Instantiate(
                    idiomButtonPrefab,
                    idiomListParent
                );

            MenuSceneEffects.AttachButtonEffect(item);
            CanvasGroup itemCanvasGroup = item.GetComponent<CanvasGroup>();
            if (itemCanvasGroup == null)
                itemCanvasGroup = item.AddComponent<CanvasGroup>();
            itemCanvasGroup.alpha = 0f;
            itemCanvasGroup.blocksRaycasts = true;
            itemCanvasGroup.DOFade(1f, 0.22f)
                .SetDelay(MenuSceneEffects.ReducedMotion ? 0f : (i - startIndex) * 0.025f);

            Vector3 itemScale = item.transform.localScale;
            if (!MenuSceneEffects.ReducedMotion)
            {
                item.transform.localScale = itemScale * 0.92f;
                item.transform.DOScale(itemScale, 0.24f)
                    .SetEase(Ease.OutBack)
                    .SetDelay((i - startIndex) * 0.025f);
            }


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


            if (isRadicalMode)
            {
                resultItem.SetRadical(radicals[i]);
            }
            else
            {
                resultItem.SetIdiom(idioms[i]);
            }


            item.SetActive(true);
            if (i == startIndex && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(item);
            }
        }


        // ページボタンの表示状態を更新
        UpdatePageButtons();


        Debug.Log(
            (isRadicalMode ? "部首" : "熟語") + "ページ表示: "
            + (currentPage + 1)
            + "ページ目 / "
            + GetPageCount()
            + "ページ"
        );
    }


    /// <summary>
    /// 次のページへ
    /// </summary>
    public void NextPage()
    {
        int pageCount = GetPageCount();

        if (currentPage >= pageCount - 1)
        {
            return;
        }


        currentPage++;

        DisplayCurrentPage();
    }


    /// <summary>
    /// 前のページへ
    /// </summary>
    public void PreviousPage()
    {
        if (currentPage <= 0)
        {
            return;
        }


        currentPage--;

        DisplayCurrentPage();
    }


    /// <summary>
    /// ページ数を取得
    /// </summary>
    private int GetPageCount()
    {
        if (GetItemCount() == 0)
        {
            return 0;
        }


        return Mathf.CeilToInt(
            (float)GetItemCount() / IdiomsPerPage
        );
    }

    private int GetItemCount()
    {
        return isRadicalMode
            ? (radicals != null ? radicals.Count : 0)
            : (idioms != null ? idioms.Count : 0);
    }


    /// <summary>
    /// ← → ボタンの表示状態を更新
    /// </summary>
    private void UpdatePageButtons()
    {
        int pageCount = GetPageCount();


        // 前へ
        if (prevButton != null)
        {
            prevButton.gameObject.SetActive(
                currentPage > 0
            );
        }


        // 次へ
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(
                currentPage < pageCount - 1
            );
        }
    }
}

