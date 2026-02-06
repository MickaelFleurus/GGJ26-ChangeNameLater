using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface INavigation
{
    VisualElement LastSelectedElement { get; set; }
    List<List<VisualElement>> Navigation { get; set; }

    (int row, int col, bool found) GetFocusedElementPosition();
    void SetFocusAt(VisualElement element);
    void SetFocusAt(int row, int col);
    void MoveFocus(NavigationMoveEvent evt);

}

public static class NavigationExtensions
{
    public static (int row, int col, bool found) GetFocusedElementPosition(this INavigation nav)
    {
        if (nav.Navigation == null || nav.Navigation.Count == 0)
            return (-1, -1, false);

        for (int row = 0; row < nav.Navigation.Count; row++)
        {
            for (int col = 0; col < nav.Navigation[row].Count; col++)
            {
                if (nav.Navigation[row][col] != null && nav.Navigation[row][col].hasFocusPseudoState)
                {
                    return (row, col, true);
                }
            }
        }

        return (-1, -1, false);
    }

    public static void SetFocusAt(VisualElement element)
    {
        if (element != null)
        {
            element.schedule.Execute(() =>
            {
                element.Focus();
            });
        }
    }

    public static void SetFocusAt(this INavigation nav, int row, int col)
    {
        if (nav.Navigation != null &&
            row >= 0 && row < nav.Navigation.Count &&
            col >= 0 && col < nav.Navigation[row].Count &&
            nav.Navigation[row][col] != null)
        {
            nav.Navigation[row][col].Focus();
        }
    }

    public static void MoveFocus(this INavigation nav, NavigationMoveEvent evt)
    {
        var (row, col, found) = nav.GetFocusedElementPosition();
        Vector2 direction = evt.move;
        if (!found)
        {
            SetFocusAt(nav.LastSelectedElement);
            return;
        }

        Func<int, int, int, int> nextIndex = (current, direction, limit) =>
        {
            current += direction;
            if (current < 0) { return limit - 1; } else if (current >= limit) { return 0; } else { return current; }
        };



        int j = nextIndex(row, (int)direction.y, nav.Navigation.Count);
        while (j != row)
        {
            if (IsElementFocusable(nav.Navigation[j][0]))
                break;
            j = nextIndex(j, (int)direction.y, nav.Navigation.Count);
        }

        int i = nextIndex(col, (int)direction.x, nav.Navigation[j].Count);
        while (i != col)
        {
            if (IsElementFocusable(nav.Navigation[j][i]))
                break;
            i = nextIndex(i, (int)direction.x, nav.Navigation[j].Count);
        }

        SetFocusAt(nav.Navigation[j][i]);

    }

    private static bool IsElementFocusable(VisualElement element)
    {
        if (element == null)
            return false;

        // Check if visible
        if (element.style.display == DisplayStyle.None)
            return false;

        // Check if enabled (for buttons and other interactive elements)
        if (element is Button button && !button.enabledInHierarchy)
            return false;

        // Check if focusable
        if (!element.focusable)
            return false;

        return true;
    }

    public static void SetupFocusGuard(this INavigation nav, VisualElement root)
    {
        var catcher = root.Q<VisualElement>("FocusFallback");
        root.RegisterCallback<FocusInEvent>(evt =>
        {
            if (evt.target is VisualElement ve && ve.focusable && evt.target != catcher)
                nav.LastSelectedElement = ve;
        });


        catcher.RegisterCallback<PointerDownEvent>(_ =>
        {
            if (nav.LastSelectedElement != null)
            {
                nav.LastSelectedElement.schedule.Execute(() =>
                    nav.LastSelectedElement.Focus());
            }
        });
    }
}
