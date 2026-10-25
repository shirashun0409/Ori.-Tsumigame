using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreNumber;
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private Button rankingRegisterButton;
    [SerializeField] private TMP_Text yourRankText;

    [Header("Ranking")]
    [SerializeField] private RankingManager rankingManager;

    private bool isRegistered = false;

    private const string RegisteredKey = "LastRankingRegistered";
    private const string NicknameKey = "LastRankingNickname";
    private const string ScoreKey = "LastRankingScore";
    private const string PersonalBestKey = "PersonalBestScore";

    private string ModeKey(string key)
    {
        return GameModeData.GetModeSpecificKey(key);
    }

    private void Start()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>(FindObjectsInactive.Include);
        if (canvas != null)
        {
            MenuSceneEffects.AttachButtonEffects(canvas.transform.root);
            SetNicknameNavigationMode();
            StartCoroutine(ConfigureControllerNavigationAfterLayout(canvas));
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, MenuSceneEffects.ReducedMotion ? 0.12f : 0.45f).SetEase(Ease.OutQuad);
        }

        // スコア表示
        if (scoreNumber == null)
        {
            Debug.LogError("ScoreNumberが設定されていません！");
        }
        else
        {
            int finalScore = GameManager.LastScore;
            string personalBestKey = ModeKey(PersonalBestKey);
            int previousBest = PlayerPrefs.GetInt(personalBestKey, 0);

            // 旧バージョンの共通記録は熟語モードの記録として引き継ぐ。
            if (!PlayerPrefs.HasKey(personalBestKey) &&
                GameModeData.SelectedMode == GameMode.Idiom)
            {
                previousBest = PlayerPrefs.GetInt(PersonalBestKey, 0);
            }

            bool isNewRecord = finalScore > previousBest;
            if (isNewRecord)
            {
                PlayerPrefs.SetInt(personalBestKey, finalScore);
                PlayerPrefs.Save();
            }

            scoreNumber.text = "0";
            if (MenuSceneEffects.ReducedMotion)
            {
                scoreNumber.text = finalScore.ToString("N0");
            }
            else
            {
                scoreNumber.transform.localScale = Vector3.one * 0.88f;
                DOTween.To(
                    () => 0,
                    value => scoreNumber.text = value.ToString("N0"),
                    finalScore,
                    0.9f).SetEase(Ease.OutCubic);
                scoreNumber.transform.DOScale(1f, 0.55f).SetEase(Ease.OutBack).SetDelay(0.3f);
            }

            if (isNewRecord)
                CelebrateNewRecord();
        }

        // 現在のゲームですでに登録済みか確認
        isRegistered =
            GameManager.RankingRegisteredThisGame;

        // ランキング登録ボタン
        if (rankingRegisterButton != null)
        {
            rankingRegisterButton.onClick.AddListener(
                RegisterRanking
            );
        }
        else
        {
            Debug.LogError(
                "RankingRegisterButtonが設定されていません！"
            );
        }

        // GameOverSceneでは順位を表示しない
        if (yourRankText != null)
        {
            yourRankText.gameObject.SetActive(false);
        }

        // すでに登録済みならボタンを無効化
        if (isRegistered)
        {
            if (rankingRegisterButton != null)
            {
                rankingRegisterButton.interactable = false;
            }

            Debug.Log(
                "このゲームではすでにランキング登録済みです。"
            );
        }
    }

    private void SetNicknameNavigationMode()
    {
        if (nicknameInput != null)
        {
            Navigation navigation = nicknameInput.navigation;
            navigation.mode = Navigation.Mode.None;
            nicknameInput.navigation = navigation;
        }

        if (EventSystem.current != null &&
            nicknameInput != null &&
            EventSystem.current.currentSelectedGameObject == nicknameInput.gameObject)
        {
            Selectable fallback = rankingRegisterButton;
            if (fallback == null || !fallback.IsActive() || !fallback.IsInteractable())
            {
                fallback = FindFirstNonNicknameSelectable();
            }

            if (fallback != null)
                EventSystem.current.SetSelectedGameObject(fallback.gameObject);
        }
    }

    private IEnumerator ConfigureControllerNavigationAfterLayout(Canvas canvas)
    {
        yield return null;

        if (canvas == null)
            yield break;

        SetNicknameNavigationMode();
        Selectable[] selectables = canvas.GetComponentsInChildren<Selectable>();
        foreach (Selectable selectable in selectables)
        {
            if (selectable == null ||
                selectable == nicknameInput ||
                !selectable.IsActive() ||
                !selectable.IsInteractable())
            {
                continue;
            }

            Navigation navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = FindNearestSelectable(selectable, selectables, Vector2.up);
            navigation.selectOnDown = FindNearestSelectable(selectable, selectables, Vector2.down);
            navigation.selectOnLeft = FindNearestSelectable(selectable, selectables, Vector2.left);
            navigation.selectOnRight = FindNearestSelectable(selectable, selectables, Vector2.right);
            selectable.navigation = navigation;
        }
    }

    private Selectable FindFirstNonNicknameSelectable()
    {
        if (nicknameInput == null)
            return null;

        Canvas canvas = nicknameInput.GetComponentInParent<Canvas>();
        if (canvas == null)
            return null;

        Selectable[] selectables = canvas.GetComponentsInChildren<Selectable>();
        foreach (Selectable selectable in selectables)
        {
            if (selectable != nicknameInput &&
                selectable.IsActive() &&
                selectable.IsInteractable())
            {
                return selectable;
            }
        }

        return null;
    }

    private Selectable FindNearestSelectable(
        Selectable source,
        Selectable[] candidates,
        Vector2 direction)
    {
        Selectable nearest = null;
        float nearestScore = float.MaxValue;
        Vector2 sourcePosition = source.transform.position;

        foreach (Selectable candidate in candidates)
        {
            if (candidate == null ||
                candidate == source ||
                candidate == nicknameInput ||
                !candidate.IsActive() ||
                !candidate.IsInteractable())
            {
                continue;
            }

            Vector2 offset = (Vector2)candidate.transform.position - sourcePosition;
            float distanceInDirection = Vector2.Dot(offset, direction);
            if (distanceInDirection <= 0f)
                continue;

            float perpendicularDistance = Mathf.Abs(
                offset.x * direction.y - offset.y * direction.x);
            float score = distanceInDirection + perpendicularDistance * 2f;
            if (score < nearestScore)
            {
                nearestScore = score;
                nearest = candidate;
            }
        }

        return nearest;
    }

    public void OpenRanking()
    {
        SceneTransitionEffect.LoadScene("RankingScene");
    }


    public void RetryGame()
    {
        SceneTransitionEffect.LoadScene("GameScene");
    }


    public void BackToModeSelect()
    {
        SceneTransitionEffect.LoadScene("StartScene");
    }


    private void CelebrateNewRecord()
    {
        if (scoreNumber == null)
            return;

        Color originalColor = scoreNumber.color;
        Color recordGold = new Color(0.88f, 0.72f, 0.40f, originalColor.a);
        if (MenuSceneEffects.ReducedMotion)
            scoreNumber.color = recordGold;
        else
        {
            scoreNumber.DOColor(recordGold, 0.2f).SetDelay(0.45f);
            scoreNumber.transform.DOPunchScale(Vector3.one * 0.16f, 0.5f, 4, 0.65f).SetDelay(0.4f);
        }

        RectTransform scoreRect = scoreNumber.rectTransform;
        GameObject recordObject = new GameObject(
            "NewRecordLabel",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        recordObject.transform.SetParent(scoreNumber.transform.parent, false);

        RectTransform recordRect = recordObject.GetComponent<RectTransform>();
        recordRect.anchorMin = scoreRect.anchorMin;
        recordRect.anchorMax = scoreRect.anchorMax;
        recordRect.pivot = scoreRect.pivot;
        recordRect.anchoredPosition = scoreRect.anchoredPosition + Vector2.up * (scoreRect.rect.height * 0.75f);
        recordRect.sizeDelta = new Vector2(scoreRect.sizeDelta.x, 48f);

        TMP_Text recordText = recordObject.GetComponent<TextMeshProUGUI>();
        recordText.font = scoreNumber.font;
        recordText.fontSharedMaterial = scoreNumber.fontSharedMaterial;
        recordText.fontSize = Mathf.Min(scoreNumber.fontSize * 0.55f, 42f);
        recordText.alignment = TextAlignmentOptions.Center;
        recordText.color = new Color(recordGold.r, recordGold.g, recordGold.b, MenuSceneEffects.ReducedMotion ? 1f : 0f);
        recordText.text = "NEW RECORD!";
        recordText.raycastTarget = false;
        if (MenuSceneEffects.ReducedMotion)
            recordText.transform.localScale = Vector3.one;
        else
        {
            recordText.transform.localScale = Vector3.one * 0.7f;
            recordText.DOFade(1f, 0.3f).SetDelay(0.5f);
            recordText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.5f);
        }
    }

    private void RegisterRanking()
    {
        //==================================================
        // ★ すでに登録済みなら絶対に登録しない
        //==================================================

        if (isRegistered ||
            GameManager.RankingRegisteredThisGame)
        {
            Debug.Log(
                "このゲームではすでにランキング登録済みです。"
            );
            return;
        }


        if (rankingManager == null)
        {
            Debug.LogError(
                "RankingManagerが設定されていません！"
            );
            return;
        }


        string nickname =
            nicknameInput.text.Trim();


        // ニックネーム未入力
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogWarning(
                "ニックネームが入力されていません！"
            );
            return;
        }


        int score =
            GameManager.LastScore;


        //==================================================
        // ランキング登録
        //==================================================

        rankingManager.RegisterScore(
            nickname,
            score
        );


        //==================================================
        // 今回の登録情報を保存
        //==================================================

        PlayerPrefs.SetInt(
            ModeKey(RegisteredKey),
            1
        );

        PlayerPrefs.SetString(
            ModeKey(NicknameKey),
            nickname
        );

        PlayerPrefs.SetInt(
            ModeKey(ScoreKey),
            score
        );

        PlayerPrefs.Save();


        //==================================================
        // ★ GameManagerにも登録済みと記録
        //==================================================

        GameManager.SetRankingRegistered();

        isRegistered = true;


        // 登録ボタンを無効化
        if (rankingRegisterButton != null)
        {
            rankingRegisterButton.interactable = false;
        }


        Debug.Log(
            "ランキング登録完了！ "
            + nickname
            + " / "
            + score
        );
    }
}

