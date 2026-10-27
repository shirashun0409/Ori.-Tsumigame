using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MenuSceneEffects
{
    // The menu no longer exposes a reduced-motion setting.
    public static bool ReducedMotion => false;

    public static void AttachButtonEffects(Transform root)
    {
        if (root == null)
            return;

        Button[] buttons = root.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
            Attach(button.gameObject);

        Toggle[] toggles = root.GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles)
            Attach(toggle.gameObject);

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null && eventSystem.currentSelectedGameObject == null)
        {
            Selectable[] selectables = root.GetComponentsInChildren<Selectable>();
            foreach (Selectable selectable in selectables)
            {
                if (selectable.IsActive() && selectable.IsInteractable())
                {
                    eventSystem.SetSelectedGameObject(selectable.gameObject);
                    break;
                }
            }
        }

    }

    public static void AttachButtonEffect(GameObject target)
    {
        if (target != null)
            Attach(target);
    }

    private static void Attach(GameObject target)
    {
        if (target.GetComponent<MenuButtonEffect>() != null)
            return;

        Vector3 initialScale = target.transform.localScale;
        MenuButtonEffect effect = target.AddComponent<MenuButtonEffect>();
        effect.Initialize(initialScale);
    }
}
