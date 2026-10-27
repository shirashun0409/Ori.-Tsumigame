using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MenuButtonEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    private RectTransform rectTransform;
    private Vector3 baseScale = Vector3.one;
    private bool isHovered;
    private bool isSelected;

    public void Initialize(Vector3 originalScale)
    {
        baseScale = originalScale;
        rectTransform = transform as RectTransform;
    }

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        if (rectTransform != null && rectTransform.localScale.sqrMagnitude > 0.01f)
            baseScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        AnimateTo(HighlightedScale(), 0.12f, Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        AnimateTo(isSelected ? HighlightedScale() : baseScale, 0.14f, Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateTo(baseScale * (MenuSceneEffects.ReducedMotion ? 0.985f : 0.94f), 0.08f, Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float scale = isHovered || isSelected ? 1.045f : 1f;
        AnimateTo(baseScale * scale, 0.18f, Ease.OutBack);
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        AnimateTo(HighlightedScale(), 0.12f, Ease.OutQuad);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        AnimateTo(isHovered ? HighlightedScale() : baseScale, 0.14f, Ease.OutQuad);
    }

    private Vector3 HighlightedScale()
    {
        return baseScale * (MenuSceneEffects.ReducedMotion ? 1f : 1.035f);
    }

    private void AnimateTo(Vector3 target, float duration, Ease ease)
    {
        if (rectTransform == null)
            return;

        rectTransform.DOKill();
        rectTransform.DOScale(target, duration).SetEase(ease);
    }
}
